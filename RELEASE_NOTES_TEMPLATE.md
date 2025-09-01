# BrowserChooser3 v0.1.1 リリースノート

## 🎉 新機能・改善

### ポータブルモードの改善
- ビルド時定数によるポータブルモード制御を実装
- インストーラー経由インストールの正確な判定を追加
- Program Files以下の判定による確実なインストール方法の検出

### 設定ファイルとログファイルの出力場所の統一
- 設定ファイルを常にユーザーディレクトリ（%APPDATA%\BrowserChooser3）に出力
- ログファイルを常にユーザーディレクトリ（%LOCALAPPDATA%\BrowserChooser3\Logs）に出力
- 実行フォルダへの出力を停止し、ユーザーディレクトリに統一

### ビルド設定の改善
- Debug/Release/Portableの各設定に対応
- ポータブル版リリース作成スクリプトの改善
- ビルド出力ディレクトリの適切な管理

## 🐛 バグ修正

### オプション画面の表示問題
- Settings項目が正しく表示されるように修正
- テスト環境分離による誤判定の問題を解決

### ポータブルモード判定の問題
- インストーラーでインストール時にProgram Files以下に設定を書き込もうとする問題を解決
- ビルド時定数による確実な制御を実装

## 🔧 技術的改善

### コード品質の向上
- 設定ファイルとログファイルの出力場所を統一
- ポータブルモードの判定ロジックを改善
- ビルド設定による制御の実装

### ユーザビリティの向上
- 設定ファイルとログファイルが適切なユーザーディレクトリに出力
- インストール方法の正確な判定と適切な動作

## 📦 配布パッケージ

### インストーラー版
- ファイル名: `BrowserChooser3-Setup.exe`
- サイズ: 2.52MB
- インストール先: Program Files\BrowserChooser3

### ポータブル版
- ファイル名: `BrowserChooser3-v0.1.1.zip`
- サイズ: 1.28MB
- 展開後サイズ: 2.97MB

## 🚀 インストール方法

### インストーラー版
1. `BrowserChooser3-Setup.exe`をダウンロード
2. 管理者権限で実行
3. インストールウィザードに従ってインストール

### ポータブル版
1. `BrowserChooser3-v0.1.1.zip`をダウンロード
2. 任意のフォルダに展開
3. `BrowserChooser3.exe`を実行

## 📋 システム要件

- **OS**: Windows 10/11 x64
- **.NET**: .NET 8.0 Runtime
- **メモリ**: 512MB以上
- **ディスク**: 10MB以上の空き容量

## 🔄 アップデート方法

### v0.1.0からのアップデート
- 設定ファイルは自動的に新しい場所（%APPDATA%\BrowserChooser3）に移行
- 既存の設定は保持されます
- ログファイルも新しい場所（%LOCALAPPDATA%\BrowserChooser3\Logs）に出力

## 📝 既知の問題

- .NET SDK 9.0でのビルド警告（機能には影響なし）
- 一部のテストがスキップされる場合がある（OptionsFormのモック化の困難性による）

## 🙏 謝辞

このリリースに貢献してくださった開発者、テスター、ユーザーの皆様に感謝いたします。

## 📞 サポート

- **GitHub**: https://github.com/Yosuke-Sh/BrowserChooser3
- **Issues**: https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: https://github.com/Yosuke-Sh/BrowserChooser3/wiki

---

**BrowserChooser3 v0.1.1** - より安定したポータブルモードと改善された設定管理
