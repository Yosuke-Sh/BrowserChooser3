# テスト

BrowserChooser3のテスト戦略と実装方法について説明します。

## 📋 テスト戦略

### テストの種類
- **単体テスト**: 個別のクラスやメソッドのテスト
- **統合テスト**: 複数のコンポーネントの連携テスト
- **UIテスト**: フォームやコントロールのテスト
- **パフォーマンステスト**: メモリリークやパフォーマンスのテスト

### テストフレームワーク
- **xUnit**: メインのテストフレームワーク
- **FluentAssertions**: アサーションライブラリ
- **Moq**: モックライブラリ
- **coverlet.collector**: コードカバレッジ測定

## 🧪 テストプロジェクトの構成

### プロジェクト構造
```
BrowserChooser3.Tests/
├── UnitTests/           # 単体テスト
│   ├── Forms/          # フォームのテスト
│   ├── Services/       # サービスのテスト
│   └── Utilities/      # ユーティリティのテスト
├── IntegrationTests/   # 統合テスト
│   └── UI/            # UI統合テスト
└── TestData/          # テストデータ
```

### テストプロジェクトファイル
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="coverlet.collector" Version="6.0.0" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
  </ItemGroup>
  
  <ItemGroup>
    <ProjectReference Include="..\BrowserChooser3\BrowserChooser3.csproj" />
  </ItemGroup>
</Project>
```

## 🔧 単体テスト

### 基本的なテスト構造
```csharp
[Fact]
public void MethodName_WithValidInput_ShouldReturnExpectedResult()
{
    // Arrange - テストの準備
    var input = "test";
    var expected = "expected";
    
    // Act - テスト対象の実行
    var result = MethodUnderTest(input);
    
    // Assert - 結果の検証
    result.Should().Be(expected);
}
```

### パラメータ化テスト
```csharp
[Theory]
[InlineData("chrome.exe", true)]
[InlineData("firefox.exe", true)]
[InlineData("invalid.exe", false)]
[InlineData("", false)]
[InlineData(null, false)]
public void IsValidBrowser_WithVariousInputs_ShouldReturnExpectedResult(string input, bool expected)
{
    // Arrange
    var detector = new BrowserDetector();
    
    // Act
    var result = detector.IsValidBrowser(input);
    
    // Assert
    result.Should().Be(expected);
}
```

### 例外テスト
```csharp
[Fact]
public void LoadSettings_WithInvalidPath_ShouldThrowFileNotFoundException()
{
    // Arrange
    var manager = new SettingsManager();
    var invalidPath = "invalid/path/config.xml";
    
    // Act & Assert
    var action = () => manager.LoadSettings(invalidPath);
    action.Should().Throw<FileNotFoundException>();
}
```

### 非同期テスト
```csharp
[Fact]
public async Task LoadBrowsersAsync_WithValidSettings_ShouldReturnBrowsers()
{
    // Arrange
    var manager = new BrowserManager();
    
    // Act
    var browsers = await manager.LoadBrowsersAsync();
    
    // Assert
    browsers.Should().NotBeNull();
    browsers.Should().NotBeEmpty();
}
```

## 🎨 UIテスト

### フォームテスト
```csharp
[Fact]
public void MainForm_WithValidSettings_ShouldInitializeCorrectly()
{
    // Arrange
    var settings = new Settings();
    
    // Act
    using var form = new MainForm(settings);
    
    // Assert
    form.Should().NotBeNull();
    form.IsHandleCreated.Should().BeTrue();
    form.Controls.Count.Should().BeGreaterThan(0);
}
```

### コントロールテスト
```csharp
[Fact]
public void FFButton_WithCustomProperties_ShouldRenderCorrectly()
{
    // Arrange
    var button = new FFButton
    {
        Text = "Test Button",
        ButtonColor = Color.Red,
        CornerRadius = 10,
        Opacity = 0.8f
    };
    
    // Act
    button.PerformLayout();
    
    // Assert
    button.Text.Should().Be("Test Button");
    button.ButtonColor.Should().Be(Color.Red);
    button.CornerRadius.Should().Be(10);
    button.Opacity.Should().Be(0.8f);
}
```

### イベントテスト
```csharp
[Fact]
public void Button_Click_ShouldRaiseClickEvent()
{
    // Arrange
    var button = new Button();
    var clickRaised = false;
    button.Click += (sender, e) => clickRaised = true;
    
    // Act
    button.PerformClick();
    
    // Assert
    clickRaised.Should().BeTrue();
}
```

## 🔗 統合テスト

### サービス統合テスト
```csharp
[Fact]
public void BrowserDetector_WithSettingsManager_ShouldWorkTogether()
{
    // Arrange
    var detector = new BrowserDetector();
    var manager = new SettingsManager();
    var settings = manager.GetDefaultSettings();
    
    // Act
    var browsers = detector.DetectBrowsers();
    settings.Browsers = browsers;
    manager.SaveSettings(settings);
    
    // Assert
    var loadedSettings = manager.LoadSettings();
    loadedSettings.Browsers.Should().NotBeEmpty();
    loadedSettings.Browsers.Should().BeEquivalentTo(browsers);
}
```

### UI統合テスト
```csharp
[Fact]
public void OptionsForm_WithBrowserManagement_ShouldUpdateSettings()
{
    // Arrange
    var settings = new Settings();
    using var form = new OptionsForm(settings);
    
    // Act
    form.ShowBrowserTab();
    form.AddBrowser("Test Browser", "test.exe");
    form.SaveSettings();
    
    // Assert
    settings.Browsers.Should().Contain(b => b.Name == "Test Browser");
    settings.Browsers.Should().Contain(b => b.Target == "test.exe");
}
```

## 🧪 モックとスタブ

### Moqを使用したモック
```csharp
[Fact]
public void MainForm_WithMockedServices_ShouldWorkCorrectly()
{
    // Arrange
    var mockMessageBox = new Mock<IMessageBoxService>();
    mockMessageBox.Setup(x => x.ShowError(It.IsAny<string>()))
                 .Returns(DialogResult.OK);
    
    var mockSettings = new Mock<ISettingsManager>();
    mockSettings.Setup(x => x.LoadSettings())
                .Returns(new Settings());
    
    // Act
    using var form = new MainForm(mockMessageBox.Object, mockSettings.Object);
    
    // Assert
    form.Should().NotBeNull();
    mockSettings.Verify(x => x.LoadSettings(), Times.Once);
}
```

### テスト用のスタブ
```csharp
public class TestMessageBoxService : IMessageBoxService
{
    public DialogResult ShowInfo(string text, string caption = null) => DialogResult.OK;
    public DialogResult ShowWarning(string text, string caption = null) => DialogResult.OK;
    public DialogResult ShowError(string text, string caption = null) => DialogResult.OK;
    public DialogResult ShowQuestion(string text, string caption = null) => DialogResult.Yes;
}
```

## 📊 テストカバレッジ

### カバレッジの測定
```cmd
# カバレッジレポートの生成
dotnet test --collect:"XPlat Code Coverage"

# カバレッジの確認
# coverage-report/index.html を開く
```

### カバレッジ設定
```xml
<!-- coverlet.runsettings -->
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>opencover</Format>
          <Exclude>[*]*.Program,[*]*.Startup</Exclude>
          <Include>[BrowserChooser3]*</Include>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### カバレッジ目標
- **単体テスト**: 80%以上
- **統合テスト**: 60%以上
- **UIテスト**: 40%以上

## 🔍 テストデータ

### テストデータの管理
```csharp
public static class TestData
{
    public static Settings CreateTestSettings()
    {
        return new Settings
        {
            DisplaySettings = new DisplaySettings
            {
                Transparency = 0.8f,
                CornerRadius = 10,
                BackgroundColor = Color.Black
            },
            Browsers = new List<Browser>
            {
                new Browser
                {
                    Id = "test-chrome",
                    Name = "Test Chrome",
                    Target = "chrome.exe",
                    IsEnabled = true
                }
            }
        };
    }
    
    public static Browser CreateTestBrowser()
    {
        return new Browser
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Browser",
            Target = "test.exe",
            Arguments = "--new-window {url}",
            IsEnabled = true
        };
    }
}
```

### テストファイルの管理
```csharp
public class TestFileManager
{
    private readonly string _testDataPath;
    
    public TestFileManager()
    {
        _testDataPath = Path.Combine(Path.GetTempPath(), "BrowserChooser3Tests");
        Directory.CreateDirectory(_testDataPath);
    }
    
    public string CreateTestConfigFile()
    {
        var configPath = Path.Combine(_testDataPath, "test-config.xml");
        File.WriteAllText(configPath, TestData.CreateTestConfigXml());
        return configPath;
    }
    
    public void Cleanup()
    {
        if (Directory.Exists(_testDataPath))
        {
            Directory.Delete(_testDataPath, true);
        }
    }
}
```

## 🚀 テストの実行

### 基本的な実行
```cmd
# 全テストの実行
dotnet test

# 特定のテストクラスの実行
dotnet test --filter "FullyQualifiedName~LoggerTests"

# 特定のテストメソッドの実行
dotnet test --filter "FullyQualifiedName~LoggerTests&TestCategory=Unit"
```

### 並列実行の制御
```cmd
# 並列実行の無効化
dotnet test --maxcpucount:1

# 特定の並列度
dotnet test --maxcpucount:4
```

### 詳細な出力
```cmd
# 詳細な出力
dotnet test --verbosity normal

# より詳細な出力
dotnet test --verbosity detailed
```

## 🔧 テストのベストプラクティス

### テストの命名規則
```csharp
// 形式: MethodName_Scenario_ExpectedResult
[Fact]
public void LoadSettings_WithValidFile_ShouldReturnSettings()
[Fact]
public void LoadSettings_WithInvalidFile_ShouldThrowException()
[Fact]
public void LoadSettings_WithEmptyFile_ShouldReturnDefaultSettings()
```

### テストの独立性
```csharp
public class LoggerTests : IDisposable
{
    private readonly string _testLogPath;
    
    public LoggerTests()
    {
        _testLogPath = Path.GetTempFileName();
    }
    
    [Fact]
    public void Log_WithValidMessage_ShouldWriteToFile()
    {
        // テスト実装
    }
    
    public void Dispose()
    {
        if (File.Exists(_testLogPath))
        {
            File.Delete(_testLogPath);
        }
    }
}
```

### テストの可読性
```csharp
[Fact]
public void BrowserDetector_ShouldDetectChrome()
{
    // Given: Chromeがインストールされている
    var detector = new BrowserDetector();
    
    // When: ブラウザを検出する
    var browsers = detector.DetectBrowsers();
    
    // Then: Chromeが検出される
    browsers.Should().Contain(b => b.Name.Contains("Chrome"));
}
```

## 🚨 トラブルシューティング

### よくある問題

#### テストが失敗する
```cmd
# テストプロジェクトの再ビルド
dotnet build BrowserChooser3.Tests

# 依存関係の復元
dotnet restore

# キャッシュのクリア
dotnet clean
```

#### カバレッジが正しく測定されない
```cmd
# カバレッジツールの再インストール
dotnet tool install --global dotnet-reportgenerator-globaltool

# カバレッジレポートの生成
dotnet test --collect:"XPlat Code Coverage" --results-directory coverage
```

#### UIテストが不安定
```csharp
// テスト環境の検出を追加
[Fact]
public void FormTest_ShouldWorkInTestEnvironment()
{
    // Arrange
    if (!Logger.IsTestEnvironment)
    {
        // テスト環境でない場合はスキップ
        return;
    }
    
    // テスト実装
}
```

## 📚 関連情報

- [ソースからのビルド](Building-from-Source)
- [貢献ガイドライン](Contributing-Guidelines)
- [APIリファレンス](API-Reference)
- [コーディング規約](Coding-Standards)

---

*テストに関する質問がある場合は、[GitHub Discussions](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)でお気軽にお聞きください。*
