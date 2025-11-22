using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.StyleSheetify.Client.StyleSheet;

/// <summary>
/// Управляет применением и объединением стилевых прототипов (StyleSheetPrototype) в клиентском интерфейсе.
/// Предоставляет методы для применения стилей, получения правил и работы с селекторами элементов.
/// </summary>
public interface IContentStyleSheetManager
{
    /// <summary>
    /// Применяет указанный стилевой прототип к пользовательскому интерфейсу.
    /// Обычно используется для динамической смены темы или активации определённого набора стилей.
    /// </summary>
    /// <param name="stylePrototype">Экземпляр стилевого прототипа, содержащий правила оформления.</param>
    public void ApplyStyleSheet(StyleSheetPrototype stylePrototype);

    /// <summary>
    /// Применяет стилевой прототип по его идентификатору (имени прототипа).
    /// Выполняет разрешение прототипа по имени и применяет полученный <see cref="StyleSheetPrototype"/>.
    /// </summary>
    /// <param name="prototype">Имя (ID) стилевого прототипа.</param>
    public void ApplyStyleSheet(string prototype);

    /// <summary>
    /// Возвращает перечисление правил стиля, связанных с указанным прототипом.
    /// Использует строгий тип <see cref="ProtoId{StyleSheetPrototype}"/> для безопасного доступа к прототипу.
    /// </summary>
    /// <param name="protoId">Идентификатор прототипа стиля.</param>
    /// <returns>Перечисление правил <see cref="StyleRule"/>, определённых в прототипе.</returns>
    public IEnumerable<StyleRule> GetStyleRules(ProtoId<StyleSheetPrototype> protoId);

    /// <summary>
    /// Возвращает список правил стиля из конкретного экземпляра стилевого прототипа.
    /// В отличие от перечисления, возвращает материализованный <see cref="List{T}"/>.
    /// </summary>
    /// <param name="stylePrototype">Экземпляр стилевого прототипа.</param>
    /// <returns>Список правил <see cref="StyleRule"/>.</returns>
    public List<StyleRule> GetStyleRules(StyleSheetPrototype stylePrototype);

    /// <summary>
    /// Создаёт изменяемый селектор (<see cref="MutableSelector"/>) для элемента указанного типа.
    /// Может дополнительно учитывать конкретный стилевой прототип для контекстно-зависимого стилизования.
    /// </summary>
    /// <param name="type">Тип элемента (например, "Button", "Window").</param>
    /// <param name="prototype">Опциональный стилевой прототип, ограничивающий применение селектора.</param>
    /// <returns>Изменяемый селектор, который можно использовать для динамического стилизования элементов.</returns>
    public MutableSelector GetElement(string type, StyleSheetPrototype? prototype = null);

    /// <summary>
    /// Объединяет переданный <see cref="Stylesheet"/> с глобальными или контекстными стилями,
    /// применяя указанный префикс ко всем селекторам для изоляции стилей (например, по пространству имён).
    /// Возвращает обёртку <see cref="StylesheetReference"/>, поддерживающую реактивные обновления.
    /// </summary>
    /// <param name="stylesheet">Базовый набор стилей для объединения.</param>
    /// <param name="prefix">Префикс, добавляемый ко всем селекторам (например, "myMod-").</param>
    /// <returns>Ссылка на объединённый и префиксированный <see cref="Stylesheet"/>.</returns>
    public StylesheetReference MergeStyles(Stylesheet stylesheet, string prefix);
}
