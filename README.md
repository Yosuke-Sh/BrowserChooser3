# BrowserChooser3

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-MIT-green)

BrowserChooser3は、Windows環境で複数のブラウザから選択してURLを開くためのアプリケーションです。Browser Chooser 2の後継として開発され、現代的なUIと高度な機能を提供します。

## 🚀 主要機能

### 🌐 ブラウザ選択
- 複数のブラウザから選択してURLを開く
- 自動ブラウザ検出機能
- カスタムブラウザの追加・編集
- デフォルトブラウザの設定

### 🎨 透明化とカスタマイズ
- **透明化機能**: ウィンドウの透明度設定（0.01-1.0）
- **透明化色**: カスタム透明化色の設定
- **角の丸み**: ウィンドウの角を丸くする設定（半径0-50）
- **タイトルバー**: タイトルバーの表示/非表示切り替え
- **背景色**: カスタム背景色の設定

### ♿ アクセシビリティ
- フォーカス表示機能
- キーボードナビゲーション対応
- 視覚的フォーカス表示
- カスタムフォーカスボックス色と線幅

### 📁 ファイルタイプとプロトコル
- カスタムファイルタイプの登録
- プロトコルハンドラーの設定
- URL正規化機能
- 短縮URL展開

### ⚙️ 高度な設定
- グリッドレイアウトのカスタマイズ
- アイコンスケール調整
- カウントダウンタイマー
- ログ機能
- ポータブルモード

## 📋 システム要件

- **OS**: Windows 10/11
- **.NET**: .NET 8.0 Runtime
- **アーキテクチャ**: x64

## 🛠️ インストール

### 📦 リリース版のダウンロード（推奨）
1. [Releases](https://github.com/Yosuke-Sh/BrowserChooser3/releases)ページから最新版をダウンロード
2. `BrowserChooser3-vX.X.X.zip`をダウンロード
3. ZIPファイルを展開して任意のフォルダに配置
4. `.NET 8.0 Runtime`をインストール（未インストールの場合）
   - [.NET 8.0 Runtime ダウンロード](https://dotnet.microsoft.com/download/dotnet/8.0)
5. `BrowserChooser3.exe`をダブルクリックして起動

### 🔧 ビルド（開発者向け）
```bash
git clone https://github.com/Yosuke-Sh/BrowserChooser3.git
cd BrowserChooser3
dotnet build
```

### 📋 システム要件
- **OS**: Windows 10/11
- **.NET**: .NET 8.0 Runtime
- **アーキテクチャ**: x64
- **ディスク容量**: 約2MB（アプリケーションのみ）

## 🎯 使用方法

### 基本的な使用
1. BrowserChooser3を起動
2. 開きたいURLをコマンドライン引数として渡す
   ```bash
   BrowserChooser3.exe https://example.com
   ```
3. 表示されたブラウザ一覧から選択

### 設定のカスタマイズ
1. アプリケーション起動後、`O`キーを押してオプション画面を開く
2. 各タブで設定を調整：
   - **Display**: 透明化、背景色、アクセシビリティ設定
   - **Browsers**: ブラウザの追加・編集
   - **Protocols**: プロトコルハンドラーの設定
   - **File Types**: ファイルタイプの設定
   - **Categories**: ブラウザカテゴリの管理

## ⚙️ 設定詳細

### 透明化設定
- **Enable Transparency**: 透明化の有効/無効
- **Transparency Color**: 透明化色（デフォルト: Magenta）
- **Opacity**: 透明度（0.01-1.0、デフォルト: 0.9）
- **Hide Title Bar**: タイトルバーの非表示
- **Rounded Corners Radius**: 角の丸み（0-50、デフォルト: 0）

### アクセシビリティ設定
- **Show Focus**: フォーカス表示の有効/無効
- **Focus Box Color**: フォーカスボックスの色
- **Focus Box Width**: フォーカスボックスの線幅
- **Visual Focus**: 視覚的フォーカス表示

### ブラウザ設定
- **Name**: ブラウザ名
- **Target**: 実行ファイルパス
- **Arguments**: 起動引数
- **Icon**: アイコン画像またはインデックス
- **Scale**: アイコンスケール

## 🔧 コマンドラインオプション

```bash
BrowserChooser3.exe [URL] [オプション]
```

### オプション
- `--log`: ログ機能を有効化
- `--extract` または `--e`: 埋め込みDLLを抽出
- `--help`: ヘルプを表示

## 📁 ファイル構成

```
BrowserChooser3/
├── BrowserChooser3.exe          # メインアプリケーション
├── BrowserChooser3Config.xml    # 設定ファイル
├── Resources/                   # リソースファイル
│   ├── *.ico                   # アイコンファイル
│   └── *.png                   # 画像ファイル
└── Logs/                       # ログファイル（ログ有効時）
```

## 🐛 トラブルシューティング

### よくある問題

**Q: ブラウザが検出されません**
A: オプション画面の「Browsers」タブで手動でブラウザを追加してください。

**Q: 透明化が動作しません**
A: Windowsの透明化設定が有効になっているか確認してください。また、グラフィックドライバーが最新か確認してください。

**Q: アプリケーションが起動しません**
A: .NET 8.0 Runtimeがインストールされているか確認してください。

### ログの確認
問題が発生した場合は、`--log`オプションでログを有効化し、`Logs/`フォルダ内のログファイルを確認してください。

## 🤝 貢献

プロジェクトへの貢献を歓迎します！

### 開発環境のセットアップ
1. リポジトリをクローン
2. Visual Studio 2022またはVS Codeでプロジェクトを開く
3. .NET 8.0 SDKをインストール
4. プロジェクトをビルド

### テストの実行
```bash
dotnet test
```

### プルリクエスト
1. 新しいブランチを作成
2. 変更を実装
3. テストを実行
4. プルリクエストを作成

## 📄 ライセンス

このプロジェクトはMITライセンスの下で公開されています。詳細は[LICENSE](LICENSE)ファイルを参照してください。

## 🙏 謝辞

- Browser Chooser 2の開発者に感謝
- コミュニティからのフィードバックと貢献に感謝
- オープンソースライブラリの開発者に感謝

## 📞 サポート

- **Issues**: [GitHub Issues](https://github.com/Yosuke-Sh/BrowserChooser3/issues)
- **Discussions**: [GitHub Discussions](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)
- **Wiki**: [GitHub Wiki](https://github.com/Yosuke-Sh/BrowserChooser3/wiki)

---

**BrowserChooser3** - より良いブラウザ選択体験を提供します。