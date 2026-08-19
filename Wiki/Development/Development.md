# 開発ガイド

## セットアップ

- Visual Studio 2022（.NETデスクトップ開発ワークロード）または .NET 10.0 SDK
- `git clone https://github.com/Yosuke-Sh/BrowserChooser3.git`

```bash
dotnet build --configuration Release
dotnet test
```

ソリューションは`BrowserChooser3.sln`。プロジェクトは`BrowserChooser3`（アプリ本体）と`BrowserChooser3.Tests`（xUnit + FluentAssertions + Moq）の2つです。

## テスト

```bash
dotnet test --filter "FullyQualifiedName~LoggerTests"
dotnet test --maxcpucount:1   # 静的状態やファイルに触れるテスト向け
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
reportgenerator -reports:"BrowserChooser3.Tests\TestResults\*\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

テスト命名規則は`MethodName_Condition_ExpectedResult`。実際の外部プロセス起動や、ファイル・レジストリへの副作用を残すテストは書かないでください。`IMessageBoxService`/`IFormService`/`IFileDialogService`（`Classes/Interfaces/IFormService.cs`）がモック差し込み口として用意されています。

## コーディング規約

- Loggerは`Logger.LogDebug/LogInfo/LogWarning/LogError`を呼び出し元名（`クラス名.メソッド名`）付きで使用する（生の`Console.Write`等は使わない）
- `!`演算子や雑な`as`キャストを避け、適切なnullチェックを行う
- WHYが非自明な場合にのみコメントを書く（WHATは識別子名で表現する）

## インストーラー作成

```bash
.\build-inno-setup.bat
```

内部で`dotnet publish -c Release -r win-x64 --self-contained false`（ReadyToRun有効）を行った上でInno Setup 6をコンパイルします。`dist\BrowserChooser3-Setup.exe`が生成されます。単体で`iscc BrowserChooser3-Setup.iss`を実行しても公開済みバイナリが揃わずビルドできません。

## リリース手順

バージョンは3箇所を連動して更新します：`BrowserChooser3.csproj`（Version/AssemblyVersion/FileVersion）、`BrowserChooser3-Setup.iss`の`AppVersion`、同ファイルの`SOFTWARE\BrowserChooser3`レジストリキー`Version`値。

1. バージョン更新 → `RELEASE_NOTES_vX.Y.Z.md`作成 → ビルド・テスト確認
2. コミット・`git push origin master`
3. 注釈付きタグ作成・push：`git tag -a vX.Y.Z -m "vX.Y.Z"` → `git push origin vX.Y.Z`
4. `.\build-inno-setup.bat`でインストーラー生成
5. `gh release create vX.Y.Z dist\BrowserChooser3-Setup.exe --notes-file RELEASE_NOTES_vX.Y.Z.md --title "vX.Y.Z"`

詳細はリポジトリ直下の`CLAUDE.md`と`GITHUB_RELEASE_GUIDE.md`を参照してください。

## 貢献

1. リポジトリをフォーク・クローン
2. `feature/`・`fix/`等のブランチを作成
3. テストを追加し、`dotnet test`が通ることを確認
4. プルリクエストを作成（変更内容とテスト計画を記載）

質問は[GitHub Discussions](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)へ。
