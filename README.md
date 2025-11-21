# StyleSheetify
Дополнительная либа для манипуляции со стилями с помощью YAML

# Файл стилей
Пишутся они в `Prototypes` и выглядят примерно так:
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

где в `typeDefinition` пишутся типы ui классов
в `styles` прописываются правила стилей
```yaml
styles:
  <TypeDefinition>.<Class>:[PseudoClass]:
    <FieldOfControl>:
      valueType: <Тип поля класса контроля>
      value: <data for serializing value>
```

в `styles` можно прописать множества правил, селектор которых не совпадает друг с другом.
в `TypeDefinition` прописывается то, что прописали выше в `typeDefinition:`
в `Class` прописывается класс стиля
`PseudoClass` необязателен, но используется для указания правил при нажатии кнопки стилей, либо hover.
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
      modulate-self: "#FFFFFF"
    ContainerButton.APCControlButton:hover:
      modulate-self: "#FFFFFF"
    ContainerButton.APCControlButton:normal:
      modulate-self: "#FFFFFF"
```

Поля стилей можно найти в классах стилей Space Station 14, либо в самих Control

Пример вы можете найти в [репозитории WWDP](https://github.com/WWhiteDreamProject/wwdpublic/tree/738af69d63984c93ee45c3a8f636d0abe7c339cc/Resources/Prototypes/_White/Styles)

# Подключение либы
находясь в корневой папке проекта, добавляем сабмодуль

Windows
```ps1
git submodule add https://github.com/Cinkafox/StyleSheetify
.\StyleSheetify\Tools\AddToSolution.ps1
.\StyleSheetify\Tools\InstallDependency.ps1
```

Linux
```ps1
git submodule add https://github.com/Cinkafox/StyleSheetify
.\StyleSheetify\Tools\AddToSolution.sh
.\StyleSheetify\Tools\InstallDependency.sh
```

Добавьте в Content.Packaging/ClientPackaging.cs строку
```csharp
 await RobustSharedPackaging.WriteContentAssemblies(
            inputPass,
            contentDir,
            "Content.Client",
            new[] { "Content.Client", "Content.Shared", "Content.Shared.Database", "Content.StyleSheetify.Client", "Content.StyleSheetify.Shared" },
            // "Content.StyleSheetify.Client", "Content.StyleSheetify.Shared"
            cancel: cancel);
```

Добавьте в Content.Packaging/ServerPackaging.cs строку
```csharp
  private static readonly List<string> ServerContentAssemblies = new()
    {
        "Content.Server.Database",
        "Content.Server",
        "Content.Shared",
        "Content.Shared.Database",
        "Content.StyleSheetify.Shared", // Add this
        "Content.StyleSheetify.Server" // And this
    };
```


Измените код в `Content.Client/Stylesheets/StylesheetManager.cs` как [тут](https://github.com/WWhiteDreamProject/wwdpublic/blob/738af69d63984c93ee45c3a8f636d0abe7c339cc/Content.Client/Stylesheets/StylesheetManager.cs)

Теперь пропишите где то в прототипах тестовый стиль и соберите проект
```bash
dotnet build
```

Проверьте на работоспособность
