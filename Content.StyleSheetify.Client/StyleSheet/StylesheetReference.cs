using Robust.Client.UserInterface;


namespace Content.StyleSheetify.Client.StyleSheet;

/// <summary>
/// Представляет собой обёртку над объектом <see cref="Stylesheet"/>, позволяющую отслеживать изменения его значения.
/// Полезен в сценариях, где стили могут обновляться динамически (например, при изменении темы интерфейса),
/// и другие компоненты должны реагировать на такие изменения.
/// </summary>
public sealed class StylesheetReference
{
    /// <summary>
    /// Представляет пустую ссылку на стили (содержит пустой <see cref="Stylesheet"/>).
    /// Может использоваться как значение по умолчанию или "нулевой" объект.
    /// </summary>
    public static StylesheetReference Empty => new StylesheetReference(new([]));

    private Stylesheet _value;

    /// <summary>
    /// Событие, вызываемое при изменении значения <see cref="Value"/>.
    /// Подписчики могут обновлять UI или пересчитывать зависимости при смене стилей.
    /// </summary>
    public Action<Stylesheet>? ValueChanged;

    /// <summary>
    /// Текущее значение стилей. При присвоении нового значения автоматически вызывается событие <see cref="ValueChanged"/>.
    /// Доступ к сеттеру ограничен модификатором <c>internal</c>, чтобы изменение происходило только изнутри сборки.
    /// </summary>
    public Stylesheet Value
    {
        get => _value;
        internal set
        {
            _value = value;
            ValueChanged?.Invoke(value);
        }
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="StylesheetReference"/> с заданным значением стилей.
    /// </summary>
    /// <param name="value">Начальное значение стилей.</param>
    public StylesheetReference(Stylesheet value)
    {
        _value = value;
    }

    /// <summary>
    /// Неявное преобразование из <see cref="StylesheetReference"/> в <see cref="Stylesheet"/>.
    /// Позволяет использовать экземпляр <see cref="StylesheetReference"/> напрямую там, где ожидается <see cref="Stylesheet"/>.
    /// </summary>
    /// <param name="reference">Ссылка на стили.</param>
    /// <returns>Значение <see cref="Stylesheet"/> из этой ссылки.</returns>
    public static implicit operator Stylesheet(StylesheetReference reference) => reference.Value;
}
