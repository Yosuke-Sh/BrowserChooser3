# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## プロジェクト概要

BrowserChooser3は、Windows Forms（.NET 8.0、`net8.0-windows`）ベースのデスクトップアプリケーションで、ユーザーが指定したURLをどのブラウザで開くかを選択させるツールです。Browser Chooser 2の後継。設定はXML（`BrowserChooser3Config.xml`）で`%APPDATA%\BrowserChooser3`配下に保存されます。v0.1.4でポータブルモード機能（`UseExeDirectory`等）は完全に削除され、インストーラー版のみの配布に一本化されています。

## よく使うコマンド

```bash
# ビルド
dotnet build
dotnet build --configuration Release

# 実行
dotnet run --project BrowserChooser3

# 全テスト実行
dotnet test

# 特定のテストクラス/メソッドのみ実行
dotnet test --filter "FullyQualifiedName~LoggerTests"
dotnet test --filter "FullyQualifiedName~LoggerTests.MethodName_Condition_ExpectedResult"

# 並列実行を無効化（静的状態やファイルに触れるテスト向け）
dotnet test --maxcpucount:1

# カバレッジ計測（計測後は必ずHTMLレポートを生成すること）
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
reportgenerator -reports:"BrowserChooser3.Tests\TestResults\*\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
# coverage-report/index.html を確認する

# インストーラー作成（Inno Setup 6が必要）
.\build-inno-setup.bat
```

ソリューションファイルは `BrowserChooser3.sln`。プロジェクトは2つ：`BrowserChooser3/BrowserChooser3.csproj`（アプリ本体）と `BrowserChooser3.Tests/BrowserChooser3.Tests.csproj`（xUnit + FluentAssertions + Moq）。

## アーキテクチャ

### ディレクトリ構成
- `Forms/` — Windows Forms UI（`MainForm`、`OptionsForm`、`AboutForm`、`AddEditBrowserForm`、`AddEditProtocolForm`、`AddEditURLForm`、`IconSelectionForm`）。`OptionsForm`は大きいが、ロジックの大半はハンドラークラスに委譲されており、フォーム自体には持たせない方針。
- `Classes/Models/` — データモデル：`Browser`、`BrowserDefinition`、`DetectedBrowsers`、`Policy`、`Settings`、`URL`。
- `Classes/Services/Browser/` — ブラウザ検出・起動：`BrowserDetector`、`BrowserUtilities`、`DefaultBrowserChecker`。
- `Classes/Services/OptionsForm/` — `OptionsForm`の各機能ごとのハンドラークラス（`OptionsFormBrowserHandlers`、`OptionsFormDisplayHandlers`、`OptionsFormDragDropHandlers`、`OptionsFormFormHandlers`、`OptionsFormPanels`、`OptionsFormProtocolHandlers`、`OptionsFormURLHandlers`、`OptionsFormUtilityHandlers`）— Presenter的な分離により`OptionsForm.cs`本体を薄く保っている。
- `Classes/Services/System/` — `CommandLineProcessor`、`Policy`、`StartupLauncher`。
- `Classes/Services/UI/` — `MessageBoxService`、`FormService`、`FileDialogService`。`MessageBoxService`はテスト時にダイアログを抑制・モックできるように存在する（後述の「テスト」参照）。
- `Classes/Interfaces/` — `IFormService`、`IMessageBoxService`。
- `Classes/Utilities/` — `ExceptionHandler`、`GeneralUtilities`、`ImageUtilities`、`Logger`、`URLUtilities`。
- `CustomControls/` — `FFButton`、`FFCheckBox`。
- `Program.cs` — エントリーポイント。`Settings.cs` — 設定管理の中心クラス（`Settings.Current`）。

### 重要なアーキテクチャ上の事実
- **ブラウザ検出**はレジストリベース（`HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths`）に加え、Program Filesの走査、ユーザー定義エントリを組み合わせる。検出ロジックは`BrowserDetector`にある。
- **アイコン**は`.exe`（Win32 APIによる抽出）、`.ico`（直接読み込み）、ラスター画像（`.png`/`.jpg`/`.bmp`、リサイズ）のいずれかから取得する。ロジックは`ImageUtilities`。
- **設定**はXMLシリアライズ（`System.Xml.Serialization.XmlSerializer`）で`BrowserChooser3Config.xml`に保存される。Settings変更時は既存設定ファイルとの後方互換性を必ず維持すること。
- **パス解決**は`PathManager`が行い、設定ファイル・ログファイルは常に`%APPDATA%\BrowserChooser3`配下に出力される。v0.1.4でiniファイルによる出力先切り替えは廃止された。「Portable」ビルド構成はもう存在しない。
- **ログ**はカスタム`Logger`クラスを使用し、レベルはNone/Error/Warning/Info/Debug/Traceで、`%APPDATA%\BrowserChooser3\logs\`にCSV形式で出力される。素の出力ではなく`Logger.LogDebug/LogInfo/LogWarning/LogError`を使うこと。`Logger.IsTestEnvironment`でテスト実行かどうかを判定している（後述）。
- **テスト環境検出**：Loggerやフォーム初期化など複数の箇所で、テストアセンブリ配下で実行されているかどうかを判定し、実プロセス起動や実UI表示を回避している。UI隣接コードを新規追加する際は、`MessageBox.Show`を直接呼ばず既存パターンに従って`IMessageBoxService`/`MessageBoxService`経由にし、テストでモックできるようにすること。

### テストプロジェクト構成（`BrowserChooser3.Tests/`）
```
UnitTests/Forms, UnitTests/Models, UnitTests/Services, UnitTests/Utilities
IntegrationTests/UI, IntegrationTests/System, IntegrationTests/EndToEnd
TestHelpers/, TestHelpers/MockFactories/
TestData/, TestData/TestImages/
```
テスト命名規則：`MethodName_Condition_ExpectedResult`。テストは実際の外部プロセス（実ブラウザ起動など）を起動したり、ファイル/レジストリに副作用を残したりしてはならない。モックの差し込み口（`IMessageBoxService`など）や一時ファイルを使い、`Dispose`で後片付けすること。

## 制約事項（プロジェクトルールより）

以下はプロジェクト側の Cursor ルールから引き継いだ厳守事項。これらの領域の変更は、実装前に必ずユーザーの承認を得ること：
- **UI/UXの変更禁止**（レイアウト、色、フォント、間隔）。承認なしに変更しない。
- **依存関係・フレームワーク・ライブラリのバージョン変更禁止**。承認なしに変更しない。
- **設定/構成ファイルの後方互換性を維持すること** — 既存の`BrowserChooser3Config.xml`を壊さない。
- 設定アクセスは引き続き`Settings.Current`パターンを使用し、XML構成の構造を維持する。
- ヘルプ/Aboutのリンクは`https://github.com/Yosuke-Sh/BrowserChooser3`を指すこと。
- 型の安易な回避（`!`演算子、雑な`as`キャスト）を避け、適切なnullチェック・型チェックを行うこと。
- 明示的に指示された範囲を超える変更は行わない。範囲外の変更が必要と思われる場合は、まず提案として報告し、承認を得てから実施する。

## コミット・リリースの慣習

- コミットはビルド・テストの両方が通り、警告が解消されてから行う。
- コミットメッセージは日本語で、要約行＋変更内容の箇条書き＋修正対象ファイルの一覧という形式。
- バージョン更新は2箇所を連動して更新する：`BrowserChooser3/BrowserChooser3.csproj`（`Version`、`AssemblyVersion`、`FileVersion`）、`BrowserChooser3-Setup.iss`（`AppVersion`）。
- リリース成果物はインストーラーのみ（`build-inno-setup.bat` → `dist\BrowserChooser3-Setup.exe`、Inno Setup 6使用）。ポータブル版はv0.1.4で廃止済み。
