# APIリファレンス

BrowserChooser3のAPIリファレンスです。開発者がプロジェクトを理解し、拡張するための詳細な情報を提供します。

## 📋 概要

BrowserChooser3は、モジュラー設計を採用したWindows Formsアプリケーションです。各コンポーネントは明確に分離され、テスト可能で拡張しやすい構造になっています。

## 🏗️ アーキテクチャ

### プロジェクト構造
```
BrowserChooser3/
├── Forms/                 # UIフォーム
├── Classes/
│   ├── Models/           # データモデル
│   ├── Services/         # ビジネスロジック
│   │   ├── System/       # システムサービス
│   │   └── UI/           # UIサービス
│   ├── Utilities/        # ユーティリティ
│   └── Interfaces/       # インターフェース
└── Controls/             # カスタムコントロール
```

### 設計パターン
- **MVC/Presenter**: UIとビジネスロジックの分離
- **Service Layer**: 機能のサービス化
- **Repository**: データアクセスの抽象化
- **Factory**: オブジェクト生成の統一
- **Strategy**: アルゴリズムの動的切り替え

## 📦 主要クラス

### データモデル

#### Browser
```csharp
public class Browser
{
    /// <summary>
    /// ブラウザの一意のID
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// ブラウザの表示名
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// ブラウザの実行ファイルパス
    /// </summary>
    public string Target { get; set; }
    
    /// <summary>
    /// コマンドライン引数
    /// </summary>
    public string Arguments { get; set; }
    
    /// <summary>
    /// アイコンファイルのパス
    /// </summary>
    public string ImagePath { get; set; }
    
    /// <summary>
    /// アイコンのインデックス
    /// </summary>
    public int IconIndex { get; set; }
    
    /// <summary>
    /// ブラウザが有効かどうか
    /// </summary>
    public bool IsEnabled { get; set; }
}
```

#### Settings
```csharp
public class Settings
{
    /// <summary>
    /// 表示設定
    /// </summary>
    public DisplaySettings DisplaySettings { get; set; }
    
    /// <summary>
    /// ブラウザ設定
    /// </summary>
    public List<Browser> Browsers { get; set; }
    
    /// <summary>
    /// プロトコルハンドラー設定
    /// </summary>
    public List<ProtocolHandler> ProtocolHandlers { get; set; }
    
    /// <summary>
    /// 起動設定
    /// </summary>
    public StartupSettings StartupSettings { get; set; }
    
    /// <summary>
    /// アクセシビリティ設定
    /// </summary>
    public AccessibilitySettings AccessibilitySettings { get; set; }
}
```

### サービス層

#### BrowserDetector
```csharp
public class BrowserDetector
{
    /// <summary>
    /// システムにインストールされているブラウザを検出
    /// </summary>
    /// <returns>検出されたブラウザの一覧</returns>
    public List<Browser> DetectBrowsers();
    
    /// <summary>
    /// 特定のブラウザが存在するかチェック
    /// </summary>
    /// <param name="path">ブラウザのパス</param>
    /// <returns>存在する場合はtrue</returns>
    public bool IsBrowserInstalled(string path);
    
    /// <summary>
    /// ブラウザのアイコンを取得
    /// </summary>
    /// <param name="path">ブラウザのパス</param>
    /// <returns>アイコン</returns>
    public Icon GetBrowserIcon(string path);
}
```

#### SettingsManager
```csharp
public class SettingsManager
{
    /// <summary>
    /// 設定をファイルから読み込み
    /// </summary>
    /// <param name="filePath">設定ファイルのパス</param>
    /// <returns>設定オブジェクト</returns>
    public Settings LoadSettings(string filePath = null);
    
    /// <summary>
    /// 設定をファイルに保存
    /// </summary>
    /// <param name="settings">保存する設定</param>
    /// <param name="filePath">保存先のパス</param>
    public void SaveSettings(Settings settings, string filePath = null);
    
    /// <summary>
    /// デフォルト設定を取得
    /// </summary>
    /// <returns>デフォルト設定</returns>
    public Settings GetDefaultSettings();
}
```

#### Logger
```csharp
public class Logger
{
    /// <summary>
    /// 現在のログレベル
    /// </summary>
    public static LogLevel CurrentLogLevel { get; set; }
    
    /// <summary>
    /// テスト環境かどうか
    /// </summary>
    public static bool IsTestEnvironment { get; }
    
    /// <summary>
    /// ログメッセージを出力
    /// </summary>
    /// <param name="level">ログレベル</param>
    /// <param name="message">メッセージ</param>
    /// <param name="args">フォーマット引数</param>
    public static void Log(LogLevel level, string message, params object[] args);
    
    /// <summary>
    /// デバッグログを出力
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="args">フォーマット引数</param>
    public static void Debug(string message, params object[] args);
    
    /// <summary>
    /// 情報ログを出力
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="args">フォーマット引数</param>
    public static void Info(string message, params object[] args);
    
    /// <summary>
    /// 警告ログを出力
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="args">フォーマット引数</param>
    public static void Warning(string message, params object[] args);
    
    /// <summary>
    /// エラーログを出力
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="args">フォーマット引数</param>
    public static void Error(string message, params object[] args);
}
```

### UIサービス

#### MessageBoxService
```csharp
public class MessageBoxService
{
    /// <summary>
    /// 情報メッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    public static DialogResult ShowInfoStatic(string text, string caption = null);
    
    /// <summary>
    /// 警告メッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    public static DialogResult ShowWarningStatic(string text, string caption = null);
    
    /// <summary>
    /// エラーメッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    public static DialogResult ShowErrorStatic(string text, string caption = null);
    
    /// <summary>
    /// 確認メッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    public static DialogResult ShowQuestionStatic(string text, string caption = null);
}
```

#### FormService
```csharp
public class FormService
{
    /// <summary>
    /// フォームを表示
    /// </summary>
    /// <typeparam name="T">フォームの型</typeparam>
    /// <param name="owner">親フォーム</param>
    /// <returns>フォームの結果</returns>
    public static DialogResult ShowForm<T>(Form owner = null) where T : Form, new();
    
    /// <summary>
    /// フォームをモーダルで表示
    /// </summary>
    /// <typeparam name="T">フォームの型</typeparam>
    /// <param name="owner">親フォーム</param>
    /// <returns>フォームの結果</returns>
    public static DialogResult ShowDialog<T>(Form owner = null) where T : Form, new();
}
```

### ユーティリティ

#### GeneralUtilities
```csharp
public static class GeneralUtilities
{
    /// <summary>
    /// URLを正規化
    /// </summary>
    /// <param name="url">正規化するURL</param>
    /// <returns>正規化されたURL</returns>
    public static string NormalizeUrl(string url);
    
    /// <summary>
    /// ファイルパスが有効かチェック
    /// </summary>
    /// <param name="path">チェックするパス</param>
    /// <returns>有効な場合はtrue</returns>
    public static bool IsValidPath(string path);
    
    /// <summary>
    /// ファイルの存在をチェック
    /// </summary>
    /// <param name="path">チェックするパス</param>
    /// <returns>存在する場合はtrue</returns>
    public static bool FileExists(string path);
    
    /// <summary>
    /// メモリ使用量を取得
    /// </summary>
    /// <returns>メモリ使用量（バイト）</returns>
    public static long GetMemoryUsage();
    
    /// <summary>
    /// ガベージコレクションを実行
    /// </summary>
    public static void ForceGarbageCollection();
}
```

#### IconUtilities
```csharp
public static class IconUtilities
{
    /// <summary>
    /// 実行ファイルからアイコンを抽出
    /// </summary>
    /// <param name="filePath">実行ファイルのパス</param>
    /// <param name="index">アイコンのインデックス</param>
    /// <returns>抽出されたアイコン</returns>
    public static Icon ExtractIconFromExe(string filePath, int index = 0);
    
    /// <summary>
    /// ICOファイルからアイコンを読み込み
    /// </summary>
    /// <param name="filePath">ICOファイルのパス</param>
    /// <returns>読み込まれたアイコン</returns>
    public static Icon LoadIconFromIcoFile(string filePath);
    
    /// <summary>
    /// 画像ファイルからアイコンを読み込み
    /// </summary>
    /// <param name="filePath">画像ファイルのパス</param>
    /// <returns>読み込まれたアイコン</returns>
    public static Icon LoadIconFromImageFile(string filePath);
    
    /// <summary>
    /// 関連付けられたアイコンを取得
    /// </summary>
    /// <param name="filePath">ファイルのパス</param>
    /// <returns>関連付けられたアイコン</returns>
    public static Icon GetAssociatedIcon(string filePath);
}
```

## 🔧 インターフェース

### IMessageBoxService
```csharp
public interface IMessageBoxService
{
    /// <summary>
    /// 情報メッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    DialogResult ShowInfo(string text, string caption = null);
    
    /// <summary>
    /// 警告メッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    DialogResult ShowWarning(string text, string caption = null);
    
    /// <summary>
    /// エラーメッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    DialogResult ShowError(string text, string caption = null);
    
    /// <summary>
    /// 確認メッセージを表示
    /// </summary>
    /// <param name="text">メッセージテキスト</param>
    /// <param name="caption">キャプション</param>
    /// <returns>ダイアログ結果</returns>
    DialogResult ShowQuestion(string text, string caption = null);
}
```

### IFormService
```csharp
public interface IFormService
{
    /// <summary>
    /// フォームを表示
    /// </summary>
    /// <typeparam name="T">フォームの型</typeparam>
    /// <param name="owner">親フォーム</param>
    /// <returns>フォームの結果</returns>
    DialogResult ShowForm<T>(Form owner = null) where T : Form, new();
    
    /// <summary>
    /// フォームをモーダルで表示
    /// </summary>
    /// <typeparam name="T">フォームの型</typeparam>
    /// <param name="owner">親フォーム</param>
    /// <returns>フォームの結果</returns>
    DialogResult ShowDialog<T>(Form owner = null) where T : Form, new();
}
```

## 🎨 カスタムコントロール

### FFButton
```csharp
public class FFButton : Button
{
    /// <summary>
    /// ボタンの背景色
    /// </summary>
    public Color ButtonColor { get; set; }
    
    /// <summary>
    /// ボタンの角の丸み
    /// </summary>
    public int CornerRadius { get; set; }
    
    /// <summary>
    /// ボタンの透明度
    /// </summary>
    public float Opacity { get; set; }
    
    /// <summary>
    /// ボタンの描画
    /// </summary>
    /// <param name="e">描画イベント引数</param>
    protected override void OnPaint(PaintEventArgs e);
}
```

### FFCheckBox
```csharp
public class FFCheckBox : CheckBox
{
    /// <summary>
    /// チェックボックスの背景色
    /// </summary>
    public Color CheckBoxColor { get; set; }
    
    /// <summary>
    /// チェックボックスの角の丸み
    /// </summary>
    public int CornerRadius { get; set; }
    
    /// <summary>
    /// チェックボックスの描画
    /// </summary>
    /// <param name="e">描画イベント引数</param>
    protected override void OnPaint(PaintEventArgs e);
}
```

## 🔍 イベントとデリゲート

### 主要イベント
```csharp
// ブラウザ選択イベント
public event EventHandler<BrowserSelectedEventArgs> BrowserSelected;

// 設定変更イベント
public event EventHandler<SettingsChangedEventArgs> SettingsChanged;

// ログ出力イベント
public event EventHandler<LogEventArgs> LogMessage;
```

### イベント引数
```csharp
public class BrowserSelectedEventArgs : EventArgs
{
    public Browser SelectedBrowser { get; set; }
    public string Url { get; set; }
}

public class SettingsChangedEventArgs : EventArgs
{
    public Settings OldSettings { get; set; }
    public Settings NewSettings { get; set; }
}

public class LogEventArgs : EventArgs
{
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## 🧪 テスト用API

### テスト環境の検出
```csharp
// テスト環境かどうかを確認
if (Logger.IsTestEnvironment)
{
    // テスト用の処理
}
```

### モック可能なサービス
```csharp
// 依存性注入を使用してモック可能
public class MainForm : Form
{
    private readonly IMessageBoxService _messageBoxService;
    
    public MainForm(IMessageBoxService messageBoxService = null)
    {
        _messageBoxService = messageBoxService ?? new MessageBoxService();
    }
}
```

## 📚 使用例

### ブラウザの追加
```csharp
var browser = new Browser
{
    Id = Guid.NewGuid().ToString(),
    Name = "Custom Browser",
    Target = @"C:\Path\To\Browser.exe",
    Arguments = "--new-window {url}",
    IsEnabled = true
};

settings.Browsers.Add(browser);
settingsManager.SaveSettings(settings);
```

### ログの出力
```csharp
Logger.Info("アプリケーションが起動しました");
Logger.Debug("設定ファイルを読み込み中: {Path}", configPath);
Logger.Warning("ブラウザが見つかりません: {Path}", browserPath);
Logger.Error("エラーが発生しました: {Exception}", ex);
```

### カスタムメッセージボックス
```csharp
// テスト環境では自動的にOKを返す
var result = MessageBoxService.ShowErrorStatic("エラーが発生しました");
if (result == DialogResult.OK)
{
    // 処理を続行
}
```

## 🔧 拡張ポイント

### 新しいサービスの追加
```csharp
public interface INewService
{
    void DoSomething();
}

public class NewService : INewService
{
    public void DoSomething()
    {
        // 実装
    }
}
```

### 新しいフォームの追加
```csharp
public class CustomForm : Form
{
    public CustomForm()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        // UI初期化
    }
}
```

## 📚 関連情報

- [ソースからのビルド](Building-from-Source)
- [貢献ガイドライン](Contributing-Guidelines)
- [テスト](Testing)
- [コーディング規約](Coding-Standards)

---

*APIに関する質問がある場合は、[GitHub Discussions](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)でお気軽にお聞きください。*
