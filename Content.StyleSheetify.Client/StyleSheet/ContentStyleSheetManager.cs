using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;

namespace Content.StyleSheetify.Client.StyleSheet;

/// <summary>
/// Реализация менеджера стилей для клиентского интерфейса.
/// Отвечает за загрузку, применение, объединение и кэширование стилевых прототипов.
/// Поддерживает иерархию стилей через родительские прототипы, псевдоклассы, селекторы и префиксы.
/// </summary>
internal sealed partial class ContentStyleSheetManager : IContentStyleSheetManager, IContentStyleSheetManagerInternal
{
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    /// <summary>
    /// Логгер, используемый для вывода сообщений от модуля StyleSheetify.
    /// </summary>
    private ISawmill _logger = default!;

    /// <summary>
    /// Кэш уже созданных <see cref="StylesheetReference"/> по префиксу/прототипу.
    /// Используется для реактивного обновления стилей при перезагрузке прототипов.
    /// </summary>
    private Dictionary<string, StylesheetDatum> _styleCacne = [];

    /// <summary>
    /// Отображение: ID родительского стилевого прототипа → список дочерних прототипов, которые его наследуют.
    /// Необходимо для рекурсивного обновления стилей при изменении родителя.
    /// </summary>
    private Dictionary<string, List<string>> _aliases = [];

    /// <summary>
    /// Инициализирует менеджер стилей: подписывается на перезагрузку прототипов и создаёт логгер.
    /// </summary>
    public void Initialize()
    {
        _logger = _logManager.GetSawmill("StyleSheetify");
        _prototypeManager.PrototypesReloaded += OnPrototypeReloaded;
        _logger.Info("Initializing stylesheetify...");
    }

    /// <summary>
    /// Заполняет словарь <see cref="_aliases"/>, строя дерево наследования стилевых прототипов.
    /// Это позволяет в будущем быстро находить все прототипы, зависящие от изменённого родителя.
    /// </summary>
    public void PostInitialize()
    {
        EnumerateSheetParentsAliases();
    }

    /// <summary>
    /// Применяет стилевой прототип по его строковому идентификатору.
    /// Если прототип не найден — ничего не делает.
    /// </summary>
    public void ApplyStyleSheet(string prototype)
    {
        if (!_prototypeManager.TryIndex<StyleSheetPrototype>(prototype, out var proto))
            return;

        ApplyStyleSheet(proto);
    }

    /// <summary>
    /// Применяет переданный стилевой прототип к глобальному стилю пользовательского интерфейса.
    /// Собирает все правила стиля (включая унаследованные от родителей) и устанавливает их.
    /// </summary>
    public void ApplyStyleSheet(StyleSheetPrototype stylePrototype)
    {
        _userInterfaceManager.Stylesheet = new(GetStyleRules(stylePrototype));
    }

    /// <summary>
    /// Возвращает правила стиля для указанного прототипа по строгому идентификатору.
    /// Генерирует исключение, если прототип не существует.
    /// </summary>
    public IEnumerable<StyleRule> GetStyleRules(ProtoId<StyleSheetPrototype> protoId)
    {
        if (!_prototypeManager.TryIndex(protoId, out var prototype))
            throw new UnknownPrototypeException(protoId, typeof(StyleSheetPrototype));

        return GetStyleRules(prototype);
    }

    /// <summary>
    /// Рекурсивно собирает все правила стиля из прототипа и его родителей.
    /// Поддерживает наследование стилей: сначала обрабатываются родители, затем собственные правила.
    /// Преобразует описания стилей из прототипа в исполняемые <see cref="StyleRule"/>.
    /// </summary>
    public List<StyleRule> GetStyleRules(StyleSheetPrototype stylePrototype)
    {
        var styleRule = new List<StyleRule>();

        // Рекурсивно добавляем правила из родительских прототипов
        foreach (var parent in stylePrototype.Parents)
        {
            styleRule.AddRange(GetStyleRules(parent));
        }

        // Обрабатываем собственные правила прототипа
        foreach (var (elementPath, value) in stylePrototype.Styles)
        {
            var element = GetElement(elementPath, stylePrototype);
            foreach (var (key, dynamicValue) in value)
            {
                element.Prop(key, dynamicValue.GetValueObject());
            }

            styleRule.Add(element);
        }

        return styleRule;
    }

    /// <summary>
    /// Создаёт селектор элемента по строковому пути (например, "Button.red:hover" или "Window > Title").
    /// Поддерживает:
    /// - вложенные селекторы (через пробел),
    /// - псевдоклассы (после ":"),
    /// - CSS-подобные классы (после ".").
    /// Также разрешает кастомные типы через <see cref="StyleSheetPrototype.TypeDefinition"/>.
    /// </summary>
    public MutableSelector GetElement(string type, StyleSheetPrototype? prototype = null)
    {
        var childHandler = type.Split(' ').ToList();

        // Рекурсивная обработка вложенных селекторов (родитель → потомок)
        if (childHandler.Count > 1)
        {
            var child = new MutableSelectorChild();
            child.Parent(GetElement(childHandler[0]));
            childHandler.RemoveAt(0);
            child.Child(GetElement(string.Join(' ', childHandler)));
            return child;
        }

        // Обработка псевдоклассов и классов
        var pseudoSeparator = type.Split(":");
        var classSeparator = pseudoSeparator[0].Split(".");
        var definedType = classSeparator[0];
        var element = new MutableSelectorElement();

        // Разрешение типа элемента через Reflection или через TypeDefinition в прототипе
        if (definedType != "*" && !string.IsNullOrEmpty(definedType))
        {
            if (prototype != null && prototype.TypeDefinition.TryGetValue(definedType, out var definition))
            {
                definedType = definition;
            }

            element.Type = _reflectionManager.GetType(definedType);
        }

        // Добавление CSS-подобных классов
        for (var i = 1; i < classSeparator.Length; i++)
        {
            element.Class(classSeparator[i]);
        }

        // Добавление псевдоклассов (hover, disabled и т.д.)
        for (var i = 1; i < pseudoSeparator.Length; i++)
        {
            element.Pseudo(pseudoSeparator[i]);
        }

        return element;
    }

    /// <summary>
    /// Объединяет переданный <see cref="Stylesheet"/> с правилами из стилевого прототипа, идентифицируемого по <paramref name="prefix"/>.
    /// Все свойства из прототипа перезаписывают или дополняют существующие в <paramref name="stylesheet"/>.
    /// Возвращает <see cref="StylesheetReference"/>, который поддерживает реактивные обновления.
    /// </summary>
    public StylesheetReference MergeStyles(Stylesheet stylesheet, string prefix)
    {
        if (!_prototypeManager.TryIndex<StyleSheetPrototype>(prefix, out var proto))
        {
            _logger.Warning($"Stylesheet merge failed! Style proto {prefix} not found!");
            return new StylesheetReference(stylesheet);
        }

        // Индексируем существующие правила по селектору для быстрого поиска
        var rules = stylesheet.Rules
            .GroupBy(r => r.Selector)
            .ToDictionary(g => g.Key, g => g.First());

        // Правила из прототипа (тоже индексируются)
        var newRules = GetStyleRules(proto)
            .GroupBy(r => r.Selector)
            .ToDictionary(g => g.Key, g => g.First());

        var mergedPropsCount = 0;
        var mergedStylesCount = 0;
        var addedStylesCount = 0;

        // Объединяем правила
        foreach (var (key, value) in newRules)
        {
            if (rules.TryGetValue(key, out var oriValue))
            {
                // Существующий селектор: объединяем свойства
                var oriProps = oriValue.Properties
                    .GroupBy(p => p.Name)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var prop in value.Properties)
                {
                    oriProps[prop.Name] = prop;
                    mergedPropsCount++;
                }

                var mergedRule = new StyleRule(key, oriProps.Values.ToList());
                rules[key] = mergedRule;
                mergedStylesCount++;
            }
            else
            {
                // Новый селектор: добавляем целиком
                rules[key] = value;
                addedStylesCount++;
            }
        }

        _logger.Info(
            $"Successfully merged style {prefix}: " +
            $"{mergedPropsCount} props merged, " +
            $"{mergedStylesCount} styles merged, " +
            $"{addedStylesCount} styles added."
        );

        var newStylesheet = new Stylesheet(rules.Values.ToList());
        StylesheetReference newStylesheetReference;

        // Используем кэшированную ссылку, если она существует (для реактивности)
        if (_styleCacne.TryGetValue(prefix, out var cache))
        {
            newStylesheetReference = cache.Updated;
            newStylesheetReference.Value = newStylesheet;
        }
        else
        {
            newStylesheetReference = new StylesheetReference(newStylesheet);
        }

        _styleCacne[prefix] = new(stylesheet, newStylesheetReference);

        return newStylesheetReference;
    }

    /// <summary>
    /// Строит карту наследования: для каждого родительского прототипа находит все дочерние прототипы,
    /// которые его используют через поле <see cref="StyleSheetPrototype.Parents"/>.
    /// Это необходимо для корректного обновления кэшированных стилей при перезагрузке прототипов.
    /// </summary>
    private void EnumerateSheetParentsAliases()
    {
        foreach (var styleSheetPrototype in _prototypeManager.EnumeratePrototypes<StyleSheetPrototype>())
        {
            foreach (var parentId in styleSheetPrototype.Parents)
            {
                if (!_aliases.TryGetValue(parentId, out var list))
                {
                    list = [];
                    _aliases.Add(parentId, list);
                }

                list.Add(styleSheetPrototype.ID);
            }
        }
    }

    /// <summary>
    /// Рекурсивно перечисляет все прототипы, которые зависят от указанного <paramref name="prefix"/>,
    /// включая самого себя и всех его потомков по цепочке наследования.
    /// </summary>
    private IEnumerable<string> EnumerateAliasesAndSelf(string prefix)
    {
        yield return prefix;

        if (!_aliases.TryGetValue(prefix, out var directAliases))
            yield break;

        foreach (var alias in directAliases)
        {
            foreach (var descendant in EnumerateAliasesAndSelf(alias))
            {
                yield return descendant;
            }
        }
    }
}

/// <summary>
/// Структура для хранения пары: исходный стиль и реактивная ссылка на обновлённую версию.
/// Используется для кэширования и последующего обновления стилей без пересоздания ссылок.
/// </summary>
internal record struct StylesheetDatum(Stylesheet Original, StylesheetReference Updated);
