# GitHub Releases 配布ガイド

配布物はインストーラーのみ。ポータブル版はv0.1.4で廃止済み。

## 📋 リリース手順

### 1. バージョン更新

以下3箇所を連動して更新する：

- `BrowserChooser3/BrowserChooser3.csproj`（`Version`、`AssemblyVersion`、`FileVersion`）
- `BrowserChooser3-Setup.iss`（3行目 `AppVersion`）
- `BrowserChooser3-Setup.iss`（`SOFTWARE\BrowserChooser3` レジストリキーの `ValueName: "Version"` の `ValueData`）

### 2. リリースノートの作成

`RELEASE_NOTES_vX.Y.Z.md` を新規作成する。直近の `RELEASE_NOTES_v0.2.1.md` を書式の手本にする。

### 3. ビルド・テスト確認

```bash
dotnet build --configuration Release
dotnet test
```

両方成功し、警告が0件であることを確認する。

### 4. コミット・プッシュ

日本語で、要約行＋変更内容の箇条書き＋修正対象ファイルの一覧という形式でコミットし、`master` ブランチへプッシュする。

```bash
git push origin master
```

### 5. タグの作成

注釈付きタグ（`-a`）を使う。軽量タグとの混在を避けるため統一する。

```bash
git tag -a vX.Y.Z -m "vX.Y.Z"
git push origin vX.Y.Z
```

### 6. インストーラーのビルド

```cmd
REM Inno Setup 6が必要
.\build-inno-setup.bat
```

`dist\BrowserChooser3-Setup.exe` が生成される（PublishReadyToRun=trueでReadyToRun最適化済み。ファイルサイズは約2.8MB前後）。

### 7. GitHub Releaseの作成

`gh` CLIで作成する（Webの手動操作でも可）。

```bash
gh release create vX.Y.Z dist\BrowserChooser3-Setup.exe --notes-file RELEASE_NOTES_vX.Y.Z.md --title "vX.Y.Z"
```

Webから作成する場合：

1. https://github.com/Yosuke-Sh/BrowserChooser3 の「Releases」→「Create a new release」
2. Tag version: `vX.Y.Z`、Target: `master`
3. Release title: `vX.Y.Z`
4. Description: `RELEASE_NOTES_vX.Y.Z.md` の内容を貼り付け
5. `dist\BrowserChooser3-Setup.exe` をアップロード
6. 「Publish release」

### 8. Wikiの更新

`Wiki/`配下は5ページに集約されている。特に以下を更新する。Wikiの更新もリリース作業の一部。

- `Wiki/Community/Release-Notes.md`（バージョン別変更履歴）
- `Wiki/UserGuide/User-Guide.md`（設定・機能に変更がある場合）
- `Wiki/AdvancedTopics/Troubleshooting-FAQ.md`（既知の問題・トラブルシューティングに変更がある場合）

## ⚠️ 注意事項

`git tag`・`git push`（タグ含む）・`gh release create` はいずれもリモートへ公開する操作であり、実行前にユーザーへ確認すること。

## 📦 配布パッケージの内容

```
BrowserChooser3-Setup.exe        # Inno Setup インストーラー
```

- **依存関係**: .NET 10.0 Desktop Runtime（未導入の場合はインストーラーが自動検出・導入）
- **対応OS**: Windows 10/11 x64

## 🔍 トラブルシューティング

### .NET SDKが見つからない
- .NET 10.0 SDKのインストール確認
- `dotnet --version`でバージョン確認

### Inno Setup 6が見つからない
- `build-inno-setup.bat` は `C:\Program Files (x86)\Inno Setup 6\ISCC.exe` または `C:\Program Files\Inno Setup 6\ISCC.exe` を探索する
- インストールされていない場合は https://jrsoftware.org/isinfo.php から取得する

## 📈 今後の改善案

- **自動化**: GitHub Actionsでの自動リリース（現状 `.github/workflows/ci.yml` はビルド・テストのみで、リリース作成は含まれない）
- **署名**: コード署名の追加
- **パッケージマネージャー**: Chocolatey/Scoop対応
- **自動アップデート機能**: アプリケーション内での自動更新チェック・ダウンロード・インストール
- **多言語対応**: 日本語、英語、その他言語のサポート
