# BrowserChooser3 v0.1.0 リリースノート

## 🎉 ベータ版リリース

BrowserChooser3のベータ版リリースです。Browser Chooser 2の後継として、現代的なUIと高度な機能を提供します。

## ✨ 新機能

### 🌐 ブラウザ選択機能
- 複数のブラウザから選択してURLを開く
- 自動ブラウザ検出機能
- カスタムブラウザの追加・編集
- アイコン選択機能（.exe、.ico、画像ファイル対応）
- デフォルトブラウザの設定

### 🎨 透明化とカスタマイズ
- **透明化機能**: ウィンドウの透明度設定（0.01-1.0）
- **透明化色**: カスタム透明化色の設定
- **角の丸み**: ウィンドウの角を丸くする設定（半径0-50）
- **タイトルバー**: タイトルバーの表示/非表示切り替え
- **背景色**: カスタム背景色の設定
- **背景グラデーション**: 縦方向のグラデーション効果

### ♿ アクセシビリティ機能
- **Focusタブ**: 専用のフォーカス設定タブ
- フォーカス表示機能
- キーボードナビゲーション対応
- 視覚的フォーカス表示
- カスタムフォーカスボックス色と線幅
- アクセシブルレンダリング対応

### 📁 ファイルタイプとプロトコル
- カスタムファイルタイプの登録
- プロトコルハンドラーの設定
- URL正規化機能
- 短縮URL展開

### ⚙️ 高度な設定
- **ポータブルモード**: インストールパスで自動判定
- グリッドレイアウトのカスタマイズ
- アイコンスケール調整
- カウントダウンタイマー
- ログ機能
- システムトレイ常駐機能

### 🚀 起動設定
- **Start Minimized**: 最小化で起動
- **Start in System Tray**: システムトレイで起動
- **Always Resident in System Tray**: システムトレイに常駐
- 相互排他設定（Start MinimizedとStart in System Tray）
- 起動遅延設定
- 起動メッセージ設定

## 🔧 技術仕様

- **フレームワーク**: .NET 8.0
- **UI**: Windows Forms
- **アーキテクチャ**: x64
- **ファイルサイズ**: 約1.5MB（インストーラー）、約1.26MB（ポータブル版ZIP）
- **インストーラー**: Inno Setup 6

## 📋 システム要件

- **OS**: Windows 10/11
- **.NET**: .NET 8.0 Runtime
- **アーキテクチャ**: x64

## 🚀 インストール方法

### インストーラー版（推奨）
1. [Releases](https://github.com/Yosuke-Sh/BrowserChooser3/releases)ページから`BrowserChooser3-Setup.exe`をダウンロード
2. インストーラーを実行してインストール
3. アプリケーションを起動

### ポータブル版
1. [Releases](https://github.com/Yosuke-Sh/BrowserChooser3/releases)ページから`BrowserChooser3-v0.1.0.zip`をダウンロード
2. ZIPファイルを展開して任意のフォルダに配置
3. `.NET 8.0 Runtime`をインストール（未インストールの場合）
   - [.NET 8.0 Runtime ダウンロード](https://dotnet.microsoft.com/download/dotnet/8.0)
4. `BrowserChooser3.exe`をダブルクリックして起動

## 🎯 使用方法

### 基本的な使用
```bash
BrowserChooser3.exe https://example.com
```

### 設定変更
- アプリ起動後、`O`キーを押してオプション画面を開く

## 📝 変更履歴

### v0.1.0 (2024-12-XX)
- ベータ版リリース
- 主要機能の実装完了
- アイコン選択機能の強化
- テスト環境でのUI制御改善
- インストーラー対応

## 🐛 既知の問題

現在、既知の問題はありません。

## 🔮 今後の予定

- さらなるUI/UXの改善
- 追加のカスタマイズオプション
- パフォーマンスの最適化
- テストカバレッジの向上
- 自動化の強化

## 📞 サポート

- **GitHub Issues**: [問題報告・機能要望](https://github.com/Yosuke-Sh/BrowserChooser3/issues)
- **GitHub Discussions**: [ディスカッション](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)

## 📄 ライセンス

MIT License - 詳細は[LICENSE](https://github.com/Yosuke-Sh/BrowserChooser3/blob/main/LICENSE)ファイルを参照してください。

---

**ダウンロード**: 
- [BrowserChooser3-Setup.exe](https://github.com/Yosuke-Sh/BrowserChooser3/releases/download/v0.1.0/BrowserChooser3-Setup.exe) (インストーラー版)
- [BrowserChooser3-v0.1.0.zip](https://github.com/Yosuke-Sh/BrowserChooser3/releases/download/v0.1.0/BrowserChooser3-v0.1.0.zip) (ポータブル版)
