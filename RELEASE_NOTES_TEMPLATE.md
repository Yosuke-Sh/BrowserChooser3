# BrowserChooser3 v0.1.3 リリースノート

## 🎉 新機能・改善

### AutoURLsとProtocol機能の完全動作
- AutoURLsの自動起動と自動終了機能を修正
- Protocol処理の改善とURL渡し問題を解決
- メイン画面のAutoClose設定との連携を実装

### AutoURLs機能の改善
- ワイルドカードパターン（`*`）による柔軟なURLマッチング
- 遅延起動機能の実装とカウントダウン表示
- メイン画面のAutoClose設定を使用した自動終了制御

### Protocol機能の実装
- カスタムプロトコル（ftp、ftps等）のサポート
- Protocol Header設定のUI追加
- プロトコル処理後の自動終了機能

## 🐛 バグ修正

### AutoURLsの自動終了問題
- 遅延起動後にアプリケーションが閉じない問題を解決
- 個別のAutoClose設定ではなく、メイン画面の設定を使用するように修正

### ProtocolのURL渡し問題
- Protocol処理でURLが正しく渡されない問題を解決
- URL処理の競合を防止し、適切な処理順序を実装

### 設定画面の表示問題
- Protocol設定のリストビュー列のずれを修正
- Protocol Header列の正しい表示を実装

## 🔧 技術的改善

### 処理優先順位の明確化
- AutoURLs > Protocol > 通常処理の優先順位を実装
- 重複処理の防止と効率的なURL処理

### ログ出力の改善
- AutoClose実行時の詳細ログ出力
- 遅延起動後の処理状況の可視化

### UI/UXの改善
- Protocol設定画面にHeader入力フィールドを追加
- 設定項目の配置とサイズの最適化

## 📦 配布パッケージ

### インストーラー版
- ファイル名: `BrowserChooser3-Setup.exe`
- サイズ: 2.5MB
- 設定: 管理者権限でのインストール、既定のブラウザ設定

### ポータブル版
- ファイル名: `BrowserChooser3-v0.1.3.zip`
- サイズ: 1.3MB
- 設定: `UseExeDirectory=true`（exe実行フォルダに出力）

## 🚀 インストール方法

### インストーラー版
1. `BrowserChooser3-Setup.exe`をダウンロード
2. 管理者権限で実行
3. インストールウィザードに従ってインストール
4. 設定ファイルは`%APPDATA%\BrowserChooser3`に保存

### ポータブル版
1. `BrowserChooser3-v0.1.3.zip`をダウンロード
2. 任意のフォルダに展開
3. `BrowserChooser3.exe`を実行
4. 設定ファイルはexe実行フォルダに保存

## 📋 システム要件

- **OS**: Windows 10/11 x64
- **.NET**: .NET 8.0 Runtime
- **メモリ**: 512MB以上
- **ディスク**: 10MB以上の空き容量

## 🔄 アップデート方法

### v0.1.2からのアップデート
- 設定ファイルは自動的に保持されます
- AutoURLsとProtocolの設定が新しく利用可能になります
- メイン画面のAutoClose設定がAutoURLsにも反映されます

## 📝 既知の問題

- なし

## 🙏 謝辞

このリリースに貢献してくださった開発者、テスター、ユーザーの皆様に感謝いたします。

## 📞 サポート

- **GitHub**: https://github.com/Yosuke-Sh/BrowserChooser3
- **Issues**: https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: https://github.com/Yosuke-Sh/BrowserChooser3/wiki

---

**BrowserChooser3 v0.1.3** - AutoURLsとProtocol機能の完全動作と改善されたユーザビリティ

---

# BrowserChooser3 v0.1.2 リリースノート

## 🎉 新機能・改善

### INIファイルによるパス管理の実装
- ビルド設定に依存しない柔軟なパス管理システムを実装
- `BrowserChooser3.ini`による設定ファイルとログファイルの出力先制御
- インストーラー版とポータブル版の切り替えをiniファイルで管理

### ポータブルモードの完全な分離
- ビルド設定の`PORTABLE_MODE`を削除し、iniファイルによる制御に統一
- 実行時設定による動的なパス管理
- インストール方法の自動判定を廃止し、明示的な設定による制御

### パス管理の中央化
- `PathManager`クラスによる統一されたパス管理
- `IniFileReader`による設定ファイルの読み込み
- 設定ファイルとログファイルの出力先を一元管理

## 🐛 バグ修正

### 設定ファイルの永続化問題
- インストーラー版で設定変更が保存されない問題を解決
- 設定ファイルの読み込み・保存パスを統一
- ユーザーディレクトリへの確実な保存を実現

### ポータブル版の設定保存問題
- ポータブル版でもユーザーディレクトリに設定を保存していた問題を解決
- iniファイルによる適切な出力先制御を実装

## 🔧 技術的改善

### アーキテクチャの改善
- ビルド設定に依存しない設計への移行
- 設定ファイルによる実行時制御の実装
- パス管理の責任分離とモジュール化

### コードの簡素化
- `PortableMode`プロパティと`DeterminePortableMode()`メソッドの削除
- 条件付きコンパイル（`#if PORTABLE_MODE`）の削除
- より保守しやすいコード構造への改善

## 📦 配布パッケージ

### インストーラー版
- ファイル名: `BrowserChooser3-Setup.exe`
- サイズ: 2.64MB
- 設定: `UseExeDirectory=false`（ユーザーディレクトリに出力）

### ポータブル版
- ファイル名: `BrowserChooser3-v0.1.2.zip`
- サイズ: 1.33MB
- 設定: `UseExeDirectory=true`（exe実行フォルダに出力）

## 🚀 インストール方法

### インストーラー版
1. `BrowserChooser3-Setup.exe`をダウンロード
2. 管理者権限で実行
3. インストールウィザードに従ってインストール
4. 設定ファイルは`%APPDATA%\BrowserChooser3`に保存

### ポータブル版
1. `BrowserChooser3-v0.1.2.zip`をダウンロード
2. 任意のフォルダに展開
3. `BrowserChooser3.exe`を実行
4. 設定ファイルはexe実行フォルダに保存

## 📋 システム要件

- **OS**: Windows 10/11 x64
- **.NET**: .NET 8.0 Runtime
- **メモリ**: 512MB以上
- **ディスク**: 10MB以上の空き容量

## 🔄 アップデート方法

### v0.1.1からのアップデート
- 設定ファイルは自動的に新しい場所に移行
- 既存の設定は保持されます
- iniファイルによる出力先制御が有効になります

## 📝 既知の問題

- なし

## 🙏 謝辞

このリリースに貢献してくださった開発者、テスター、ユーザーの皆様に感謝いたします。

## 📞 サポート

- **GitHub**: https://github.com/Yosuke-Sh/BrowserChooser3
- **Issues**: https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: https://github.com/Yosuke-Sh/BrowserChooser3/wiki

---

**BrowserChooser3 v0.1.2** - INIファイルによる柔軟なパス管理と改善された設定制御

---

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
