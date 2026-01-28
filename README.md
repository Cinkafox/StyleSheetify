# StyleSheetify

**StyleSheetify** — это внешняя библиотека-сабмодуль, которая позволяет задавать и управлять стилями пользовательского интерфейса в проектах на движке Robust Toolbox (например, Space Station 14) **через YAML-прототипы**.
Стили описываются декларативно, поддерживают наследование, псевдоклассы (`:hover`, `:pressed`) и могут объединяться с программно заданными стилями.

---

## 📄 Файл стилей

Стили описываются в YAML-файлах внутри папки `Prototypes/`. Пример:

```yaml
- type: styleSheet
  id: shared
  typeDefinition:
    PanelContainer: "Robust.Client.UserInterface.Controls.PanelContainer"
  styles:
    PanelContainer.ChatMainPanel:
      panel:
        valueType: Robust.Client.Graphics.StyleBoxTexture
        value:
          texture: "/Textures/_White/NovaUI/Chat/panel.png"
          patchMargin: 3,5,3,5
          expandMargin: 3,5,3,5
          textureScale: 2,2
      modulate-self: "#FFFFFF"
```

### Структура правил

```yaml
styles:
  <Type>.<Class>[:<PseudoClass>]:
    <Свойство>:
      valueType: <Тип>
      value: <Значение>
```

- **`typeDefinition`** — сокращения имён UI-классов. Например, `PanelContainer` → полное имя класса.
- **`Type`** — имя из `typeDefinition`.
- **`Class`** — пользовательский CSS-подобный "класс" (например, `ChatMainPanel`).
- **`PseudoClass`** — необязательный, задаёт состояние элемента (`:hover`, `:pressed`, `:normal` и т.д.).
- **`valueType` и `value`** — сериализуемое представление значения свойства (например, цвет, текстура, отступы).

> ✅ Один файл может содержать сколько угодно непересекающихся селекторов.

### Пример с псевдоклассами

```yaml
ContainerButton.APCControlButton:
  stylebox:
    valueType: Robust.Client.Graphics.StyleBoxTexture
    value:
      texture: "/Textures/_White/NovaUI/APC/button-panel-inactive.png"
      patchMargin: 1,3,1,1
      expandMargin: 1,1,1,3
      textureScale: 4,4
  modulate-self: "#FFFFFF"

ContainerButton.APCControlButton:pressed:
  stylebox:
    valueType: Robust.Client.Graphics.StyleBoxTexture
    value:
      texture: "/Textures/_White/NovaUI/APC/button-panel-active.png"
      patchMargin: 1,3,1,1
      expandMargin: 1,3,1,1
      textureScale: 4,4

ContainerButton.APCControlButton:hover:
  modulate-self: "#AAAAAA"
```

> 🔍 Поля (`stylebox`, `modulate-self` и др.) берутся из свойств соответствующих `Control`-классов в `Space Station 14`.

📌 **Примеры стилей**: [WWDP Prototypes/_White/Styles](https://github.com/WWhiteDreamProject/wwdpublic/tree/738af69d63984c93ee45c3a8f636d0abe7c339cc/Resources/Prototypes/_White/Styles)

---

## 🔌 Подключение как сабмодуля

**Что такое сабмодуль?**
Это способ подключить внешний репозиторий (в данном случае `StyleSheetify`) внутрь вашего проекта как зависимость, которую можно обновлять независимо от основного кода.

### 1. Добавьте сабмодуль

Из корня вашего проекта выполните:

#### Windows
```powershell
git submodule add https://github.com/Cinkafox/StyleSheetify
.\StyleSheetify\Tools\AddToSolution.ps1
```

#### Linux / macOS
```bash
git submodule add https://github.com/Cinkafox/StyleSheetify
bash ./StyleSheetify/Tools/AddToSolution.sh
```

> 💡 Это скачает код библиотеки в папку `StyleSheetify/` и добавит проекты в ваше решение.

---

### 2. Обновите сборку (Packaging)

#### В `Content.Packaging/ClientPackaging.cs`
Убедитесь, что клиент включает сборки StyleSheetify:

```csharp
await RobustSharedPackaging.WriteContentAssemblies(
    inputPass,
    contentDir,
    "Content.Client",
    new[] {
        "Content.Client",
        "Content.Shared",
        "Content.Shared.Database",
        "Content.StyleSheetify.Client", // ← добавить
        "Content.StyleSheetify.Shared" // ← добавить
    },
    cancel: cancel);
```

#### В `Content.Packaging/ServerPackaging.cs`
Добавьте сборки на сервер для включения в список игнорирование некоторых прототипов:

```csharp
private static readonly List<string> ServerContentAssemblies = new()
{
    "Content.Server.Database",
    "Content.Server",
    "Content.Shared",
    "Content.Shared.Database",
    "Content.StyleSheetify.Shared",  // ← добавить
    "Content.StyleSheetify.Server"   // ← добавить
};
```

---

### 3. Интеграция в `StylesheetManager`

Замените стандартный `StylesheetManager` на версию, использующую `IContentStyleSheetManager`.

#### `Content.Client/Stylesheets/StylesheetManager.cs`
```csharp
using Content.StyleSheetify.Client.StyleSheet;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client.Stylesheets
{
    public sealed class StylesheetManager : IStylesheetManager
    {
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;
        [Dependency] private readonly IResourceCache _resourceCache = default!;
        [Dependency] private readonly IContentStyleSheetManager _contentStyleSheetManager = default!;

        public StylesheetReference SheetNano { get; private set; } = default!;
        public StylesheetReference SheetSpace { get; private set; } = default!;

        public void Initialize()
        {
            // Объединяем программный стиль с YAML-прототипом "nano"
            SheetNano = _contentStyleSheetManager.MergeStyles(new StyleNano(_resourceCache).Stylesheet, "nano");
            SheetSpace = _contentStyleSheetManager.MergeStyles(new StyleSpace(_resourceCache).Stylesheet, "space");

            _userInterfaceManager.Stylesheet = SheetNano;
        }
    }
}
```

#### Обновите интерфейс
```csharp
using Content.StyleSheetify.Client.StyleSheet;
using Robust.Client.UserInterface;

namespace Content.Client.Stylesheets
{
    public interface IStylesheetManager
    {
        StylesheetReference SheetNano { get; }
        StylesheetReference SheetSpace { get; }
        void Initialize();
    }
}
```

---

## ▶️ Проверка работоспособности

1. Создайте YAML-файл со стилевым прототипом (например, `Resources/Prototypes/styles/nano.yml`).
2. Убедитесь, что в нём есть запись.
```yaml
- type: styleSheet
  id: nano
  typeDefinition:
    PanelContainer: "Robust.Client.UserInterface.Controls.PanelContainer"
  styles:
    PanelContainer.ChatMainPanel:
      modulate-self: "#22FFFF" # Делает панель темнее, для примера
```
3. Соберите проект:
   ```bash
   dotnet build
   ```
4. Запустите клиент и проверьте, применяются ли стили.

> Вы можете задать id стилевого прототипа любой, но если использовать тот же идентификатор, что и в вызове `MergeStyles` (например, "nano" или "space" в `StylesheetManager.cs`), то система автоматически объединит YAML-стили с программно заданными — без дополнительной настройки.

---
