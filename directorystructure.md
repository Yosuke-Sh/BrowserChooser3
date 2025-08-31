# BrowserChooser3 ディレクトリ構造詳細説明

## プロジェクトルート構造

```
BrowserChooser3/
├── BrowserChooser3/                    # メインプロジェクト
│   ├── .cursor/                        # Cursor IDE設定
│   │   └── rules/                      # プロジェクトルール
│   │       ├── global.mdc             # グローバルルール
│   │       ├── technologystack.md     # 技術スタック定義
│   │       └── directorystructure.md  # ディレクトリ構造定義
│   ├── Forms/                         # Windows Forms UI
│   ├── Classes/                       # ビジネスロジック
│   ├── CustomControls/                # カスタムコントロール
│   ├── Resources/                     # リソースファイル
│   ├── Properties/                    # プロジェクトプロパティ
│   ├── Policy/                        # ポリシー設定
│   ├── Tests/                         # テストファイル
│   ├── Program.cs                     # エントリーポイント
│   ├── Settings.cs                    # 設定管理クラス
│   └── BrowserChooser3.csproj         # プロジェクトファイル
├── BrowserChooser3.Tests/             # テストプロジェクト
├── dist/                              # 配布ファイル（インストーラー出力）
├── coverage/                          # カバレッジレポート
├── coverage-report/                   # カバレッジレポート（HTML）
├── TestResults/                       # テスト結果
├── BrowserChooser3.sln               # ソリューションファイル
├── build-inno-setup.bat              # インストーラービルドスクリプト
├── BrowserChooser3-Setup.iss         # Inno Setup設定ファイル
└── coverlet.runsettings              # カバレッジ設定
```

## メインプロジェクト詳細構造

### Forms/ - Windows Forms UI
```
Forms/
├── MainForm.cs                       # メイン画面
├── MainForm.Designer.cs              # メイン画面デザイナー
├── MainForm.resx                     # メイン画面リソース
├── OptionsForm.cs                    # オプション画面
├── OptionsForm.Designer.cs           # オプション画面デザイナー
├── OptionsForm.resx                  # オプション画面リソース
├── AboutForm.cs                      # アバウト画面
├── AboutForm.resx                    # アバウト画面リソース
├── AddEditBrowserForm.cs             # ブラウザ追加・編集画面
├── AddEditProtocolForm.cs            # プロトコル追加・編集画面
├── AddEditProtocolForm.resx          # プロトコル画面リソース
├── AddEditURLForm.cs                 # URL追加・編集画面
├── IconSelectionForm.cs              # アイコン選択画面
└── IconSelectionForm.resx            # アイコン選択画面リソース
```

### Classes/ - ビジネスロジック

#### Models/ - データモデル
```
Classes/Models/
├── Browser.cs                        # ブラウザモデル
├── BrowserDefinition.cs              # ブラウザ定義モデル
├── DetectedBrowsers.cs               # 検出されたブラウザコレクション
├── Policy.cs                         # ポリシーモデル
├── Settings.cs                       # 設定モデル
└── URL.cs                           # URLモデル
```

#### Services/ - サービス層

##### Browser/ - ブラウザ関連サービス
```
Classes/Services/Browser/
├── BrowserDetector.cs                # ブラウザ検出サービス
├── BrowserUtilities.cs               # ブラウザユーティリティ
└── DefaultBrowserChecker.cs          # デフォルトブラウザチェッカー
```

##### OptionsForm/ - オプション画面サービス
```
Classes/Services/OptionsForm/
├── OptionsFormBrowserHandlers.cs     # ブラウザパネルハンドラー
├── OptionsFormDisplayHandlers.cs     # 表示パネルハンドラー
├── OptionsFormDragDropHandlers.cs    # ドラッグ&ドロップハンドラー
├── OptionsFormFormHandlers.cs        # フォームハンドラー
├── OptionsFormPanels.cs              # パネル作成
├── OptionsFormProtocolHandlers.cs    # プロトコルパネルハンドラー
├── OptionsFormURLHandlers.cs         # URLパネルハンドラー
└── OptionsFormUtilityHandlers.cs     # ユーティリティハンドラー
```

##### System/ - システムサービス
```
Classes/Services/System/
├── CommandLineProcessor.cs           # コマンドライン引数処理
├── Policy.cs                         # ポリシー管理
└── StartupLauncher.cs                # 起動時処理
```

##### UI/ - UIサービス
```
Classes/Services/UI/
├── MessageBoxService.cs              # メッセージボックスサービス（テスト環境対応）
├── FormService.cs                    # フォームサービス
└── FileDialogService.cs              # ファイルダイアログサービス
```

#### Utilities/ - ユーティリティクラス
```
Classes/Utilities/
├── ExceptionHandler.cs               # 例外処理
├── GeneralUtilities.cs               # 汎用ユーティリティ
├── ImageUtilities.cs                 # 画像処理ユーティリティ
├── Logger.cs                         # ログ出力
└── URLUtilities.cs                   # URL処理ユーティリティ
```

#### Interfaces/ - インターフェース定義
```
Classes/Interfaces/
├── IFormService.cs                   # フォームサービスインターフェース
└── IMessageBoxService.cs             # メッセージボックスサービスインターフェース
```

### CustomControls/ - カスタムコントロール
```
CustomControls/
├── FFButton.cs                       # カスタムボタン
└── FFCheckBox.cs                     # カスタムチェックボックス
```

### Resources/ - リソースファイル
```
Resources/
├── *.ico                            # アイコンファイル
├── *.png                            # 画像ファイル
├── PublicVersion.xml                # バージョン情報
├── Resources.Designer.cs            # リソースデザイナー
└── Resources.resx                   # リソース定義
```

### Properties/ - プロジェクトプロパティ
```
Properties/
├── AssemblyInfo.cs                  # アセンブリ情報
├── Resources.Designer.cs            # リソースデザイナー
└── Resources.resx                   # リソース定義
```

### Policy/ - ポリシー設定
```
Policy/
└── (ポリシー関連ファイル)
```

### Tests/ - テストファイル
```
Tests/
└── (テスト用ファイル)
```

## テストプロジェクト構造

### BrowserChooser3.Tests/ - テストプロジェクト
```
BrowserChooser3.Tests/
├── BrowserDetectorTests.cs           # ブラウザ検出テスト
├── BrowserUtilitiesTests.cs          # ブラウザユーティリティテスト
├── CommandLineProcessorTests.cs      # コマンドライン処理テスト
├── DefaultBrowserCheckerTests.cs     # デフォルトブラウザテスト
├── DetectedBrowsersTests.cs          # 検出ブラウザテスト
├── ExceptionHandler.cs               # 例外処理テスト
├── FFButtonTests.cs                  # カスタムボタンテスト
├── FFCheckBoxTests.cs                # カスタムチェックボックステスト
├── FormTests.cs                      # フォームテスト
├── GeneralUtilitiesTests.cs          # 汎用ユーティリティテスト
├── ImageUtilitiesTests.cs            # 画像処理テスト
├── LoggerTests.cs                    # ログ出力テスト
├── MainFormTests.cs                  # メインフォームテスト
├── ModelTests.cs                     # モデルテスト
├── OptionsFormPanelsTests.cs         # オプションパネルテスト
├── OptionsFormTests.cs               # オプションフォームテスト
├── PolicyTests.cs                    # ポリシーテスト
├── RemainingTests.cs                 # その他テスト
├── ServiceTests.cs                   # サービステスト
├── SettingsTests.cs                  # 設定テスト
├── StartupLauncherTests.cs           # 起動処理テスト
├── TestConfig.cs                     # テスト設定
├── TestResults/                      # テスト結果
├── URLUtilitiesTests.cs              # URL処理テスト
└── BrowserChooser3.Tests.csproj      # テストプロジェクトファイル
```

## ビルド・配布構造

### bin/ - ビルド出力
```
bin/
├── Debug/
│   └── net8.0-windows/               # デバッグビルド
│       ├── BrowserChooser3.exe       # 実行ファイル
│       ├── BrowserChooser3.dll       # メインDLL
│       ├── BrowserChooser3Config.xml # 設定ファイル
│       ├── Logs/                     # ログファイル
│       └── (依存DLL)
└── Release/
    └── net8.0-windows/               # リリースビルド
        ├── BrowserChooser3.exe       # 実行ファイル
        ├── BrowserChooser3.dll       # メインDLL
        ├── BrowserChooser3Config.xml # 設定ファイル
        └── (依存DLL)
```

### obj/ - 中間ファイル
```
obj/
├── Debug/
│   └── net8.0-windows/               # デバッグ中間ファイル
└── Release/
    └── net8.0-windows/               # リリース中間ファイル
```

### dist/ - 配布ファイル
```
dist/
└── BrowserChooser3-Setup.exe         # インストーラー
```

### coverage/ - カバレッジレポート
```
coverage/
└── [GUID]/                           # カバレッジレポート
    ├── coverage.cobertura.xml        # Cobertura XML
    └── index.html                    # HTMLレポート
```

## ファイル命名規則

### クラスファイル
- **PascalCase**: `MainForm.cs`, `BrowserDetector.cs`
- **機能別プレフィックス**: `OptionsForm*`, `AddEdit*`

### テストファイル
- **テスト対象名 + Tests**: `MainFormTests.cs`, `BrowserDetectorTests.cs`
- **テストメソッド**: `[MethodName]_[Condition]_[ExpectedResult]`

### リソースファイル
- **対応する.csファイル名 + .resx**: `MainForm.resx`, `OptionsForm.resx`

### 設定ファイル
- **プロジェクト名 + Config.xml**: `BrowserChooser3Config.xml`
- **ログファイル**: `bc3_YYYY-MM-DD.log`

## 重要なファイル

### 設定・設定ファイル
- `Settings.cs` - 設定管理の中心クラス
- `BrowserChooser3Config.xml` - ユーザー設定ファイル
- `app.config` - アプリケーション設定

### エントリーポイント
- `Program.cs` - アプリケーション起動処理
- `MainForm.cs` - メイン画面

### テスト関連
- `TestConfig.cs` - テスト設定
- `ExceptionHandler.cs` - テスト用例外処理

### ビルド・デプロイ
- `BrowserChooser3.csproj` - メインプロジェクト設定
- `BrowserChooser3.Tests.csproj` - テストプロジェクト設定
- `BrowserChooser3.sln` - ソリューションファイル
- `BrowserChooser3-Setup.iss` - Inno Setupスクリプト
