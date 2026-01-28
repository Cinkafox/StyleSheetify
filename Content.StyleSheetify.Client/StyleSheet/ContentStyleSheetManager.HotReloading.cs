using System.Diagnostics;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.StyleSheetify.Client.StyleSheet;

internal sealed partial class ContentStyleSheetManager
{
    /// <summary>
    /// Обработчик события перезагрузки прототипов. Вызывается при горячей перезагрузке стилевых прототипов (например, во время разработки).
    /// Автоматически обновляет все кэшированные стили, зависящие от изменённых прототипов, и применяет их к UI-элементам.
    /// </summary>
    private void OnPrototypeReloaded(PrototypesReloadedEventArgs args)
    {
        // Проверяем, были ли перезагружены именно стилевые прототипы
        if (!args.ByType.TryGetValue(typeof(StyleSheetPrototype), out var styleSheetPrototype))
            return;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // Словарь: старый Stylesheet → обновлённый Stylesheet
        // Используется для массовой замены стилей в UI без дублирования пересчётов
        var changedStyles = new Dictionary<Stylesheet, Stylesheet>();

        // Обрабатываем каждый изменённый стилевой прототип
        foreach (var (modifiedPrototype, _) in styleSheetPrototype.Modified)
        {
            // Рекурсивно находим все прототипы, зависящие от изменённого (включая его самого)
            foreach (var alias in EnumerateAliasesAndSelf(modifiedPrototype))
            {
                // Пропускаем, если для этого алиаса нет кэшированного стиля
                if (!_styleCacne.TryGetValue(alias, out var cache))
                    continue;

                // Получаем текущий (устаревший) стиль
                var outdatedStyle = cache.Updated.Value;

                // Пересоздаём обновлённый стиль, объединяя оригинальный с новыми правилами из перезагруженного прототипа
                var updatedStyle = MergeStyles(cache.Original, alias);

                // Сохраняем пару для последующей замены в UI
                changedStyles[outdatedStyle] = updatedStyle;
            }
        }

        // Обновляем глобальный стиль UI, если он был затронут изменениями
        if (_userInterfaceManager.Stylesheet != null &&
            changedStyles.TryGetValue(_userInterfaceManager.Stylesheet, out var userStyleSheet))
        {
            _userInterfaceManager.Stylesheet = userStyleSheet;
        }

        // Рекурсивно обновляем стили всех корневых контролов и их потомков
        foreach (var rootControl in _userInterfaceManager.AllRoots)
        {
            UpdateControlStylesheet(rootControl, changedStyles);
        }

        _logger.Info($"Updated stylesheets in {stopwatch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Рекурсивно обновляет стиль у указанного контрола и всех его дочерних элементов.
    /// Если у контрола установлен стиль, который был обновлён (присутствует в <paramref name="changedStyles"/>),
    /// заменяет его на новую версию и инвалидирует кэш стилей для перерисовки.
    /// </summary>
    /// <param name="control">Корневой контрол, с которого начинается обновление.</param>
    /// <param name="changedStyles">Словарь соответствий старых и новых стилей.</param>
    private void UpdateControlStylesheet(Control control, Dictionary<Stylesheet, Stylesheet> changedStyles)
    {
        // Если у контрола есть стиль, и он был обновлён — заменяем его
        if (control.Stylesheet != null &&
            changedStyles.TryGetValue(control.Stylesheet, out var updatedStyle))
        {
            control.Stylesheet = updatedStyle;
        }

        // Инвалидируем внутренний кэш стилей контрола, чтобы UI пересчитал отображение
        control.InvalidateStyleSheet();

        // Рекурсивно обрабатываем всех потомков
        foreach (var child in control.Children)
        {
            UpdateControlStylesheet(child, changedStyles);
        }
    }
}
