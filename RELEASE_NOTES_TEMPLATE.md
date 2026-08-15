# BrowserChooser3 v0.1.3 リリースノ�EチE

## 🎉 新機�E・改喁E

### AutoURLsとProtocol機�Eの完�E動佁E
- AutoURLsの自動起動と自動終亁E���Eを修正
- Protocol処琁E�E改喁E��URL渡し問題を解決
- メイン画面のAutoClose設定との連携を実裁E

### AutoURLs機�Eの改喁E
- ワイルドカードパターン�E�E*`�E�による柔軟なURLマッチング
- 遁E��起動機�Eの実裁E��カウントダウン表示
- メイン画面のAutoClose設定を使用した自動終亁E��御

### Protocol機�Eの実裁E
- カスタムプロトコル�E�Etp、ftps等）�Eサポ�EチE
- Protocol Header設定�EUI追加
- プロトコル処琁E���E自動終亁E���E

## 🐛 バグ修正

### AutoURLsの自動終亁E��顁E
- 遁E��起動後にアプリケーションが閉じなぁE��題を解決
- 個別のAutoClose設定ではなく、メイン画面の設定を使用するように修正

### ProtocolのURL渡し問顁E
- Protocol処琁E��URLが正しく渡されなぁE��題を解決
- URL処琁E�E競合を防止し、E��刁E��処琁E��E��を実裁E

### 設定画面の表示問顁E
- Protocol設定�Eリストビュー列�Eずれを修正
- Protocol Header列�E正しい表示を実裁E

## 🔧 技術的改喁E

### 処琁E��先頁E���E明確匁E
- AutoURLs > Protocol > 通常処琁E�E優先頁E��を実裁E
- 重褁E�E琁E�E防止と効玁E��なURL処琁E

### ログ出力�E改喁E
- AutoClose実行時の詳細ログ出劁E
- 遁E��起動後�E処琁E��況�E可視化

### UI/UXの改喁E
- Protocol設定画面にHeader入力フィールドを追加
- 設定頁E��の配置とサイズの最適匁E

## 📦 配币E��チE��ージ

### インスト�Eラー牁E
- ファイル吁E `BrowserChooser3-Setup.exe`
- サイズ: 2.5MB
- 設宁E 管琁E��E��限でのインスト�Eル、既定�Eブラウザ設宁E

### ポ�Eタブル牁E
- ファイル吁E `BrowserChooser3-v0.1.3.zip`
- サイズ: 1.3MB
- 設宁E `UseExeDirectory=true`�E�Exe実行フォルダに出力！E

## 🚀 インスト�Eル方況E

### インスト�Eラー牁E
1. `BrowserChooser3-Setup.exe`をダウンローチE
2. 管琁E��E��限で実衁E
3. インスト�Eルウィザードに従ってインスト�Eル
4. 設定ファイルは`%APPDATA%\BrowserChooser3`に保孁E

### ポ�Eタブル牁E
1. `BrowserChooser3-v0.1.3.zip`をダウンローチE
2. 任意�Eフォルダに展開
3. `BrowserChooser3.exe`を実衁E
4. 設定ファイルはexe実行フォルダに保孁E

## 📋 シスチE��要件

- **OS**: Windows 10/11 x64
- **.NET**: .NET 10.0 Desktop Runtime
- **メモリ**: 512MB以丁E
- **チE��スク**: 10MB以上�E空き容釁E

## 🔄 アチE�EチE�Eト方況E

### v0.1.2からのアチE�EチE�EチE
- 設定ファイルは自動的に保持されまぁE
- AutoURLsとProtocolの設定が新しく利用可能になりまぁE
- メイン画面のAutoClose設定がAutoURLsにも反映されまぁE

## 📝 既知の問顁E

- なぁE

## 🙏 謝辁E

こ�Eリリースに貢献してくださった開発老E��テスター、ユーザーの皁E��に感謝いたします、E

## 📞 サポ�EチE

- **GitHub**: https://github.com/Yosuke-Sh/BrowserChooser3
- **Issues**: https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: https://github.com/Yosuke-Sh/BrowserChooser3/wiki

---

**BrowserChooser3 v0.1.3** - AutoURLsとProtocol機�Eの完�E動作と改喁E��れたユーザビリチE��

---

# BrowserChooser3 v0.1.2 リリースノ�EチE

## 🎉 新機�E・改喁E

### INIファイルによるパス管琁E�E実裁E
- ビルド設定に依存しなぁE��軟なパス管琁E��スチE��を実裁E
- `BrowserChooser3.ini`による設定ファイルとログファイルの出力�E制御
- インスト�Eラー版とポ�Eタブル版�E刁E��替えをiniファイルで管琁E

### ポ�Eタブルモード�E完�Eな刁E��
- ビルド設定�E`PORTABLE_MODE`を削除し、iniファイルによる制御に統一
- 実行時設定による動的なパス管琁E
- インスト�Eル方法�E自動判定を廁E��し、�E示皁E��設定による制御

### パス管琁E�E中央匁E
- `PathManager`クラスによる統一されたパス管琁E
- `IniFileReader`による設定ファイルの読み込み
- 設定ファイルとログファイルの出力�Eを一允E��琁E

## 🐛 バグ修正

### 設定ファイルの永続化問顁E
- インスト�Eラー版で設定変更が保存されなぁE��題を解決
- 設定ファイルの読み込み・保存パスを統一
- ユーザーチE��レクトリへの確実な保存を実現

### ポ�Eタブル版�E設定保存問顁E
- ポ�Eタブル版でもユーザーチE��レクトリに設定を保存してぁE��問題を解決
- iniファイルによる適刁E��出力�E制御を実裁E

## 🔧 技術的改喁E

### アーキチE��チャの改喁E
- ビルド設定に依存しなぁE��計への移衁E
- 設定ファイルによる実行時制御の実裁E
- パス管琁E�E責任刁E��とモジュール匁E

### コード�E簡素匁E
- `PortableMode`プロパティと`DeterminePortableMode()`メソチE��の削除
- 条件付きコンパイル�E�E#if PORTABLE_MODE`�E��E削除
- より保守しめE��ぁE��ード構造への改喁E

## 📦 配币E��チE��ージ

### インスト�Eラー牁E
- ファイル吁E `BrowserChooser3-Setup.exe`
- サイズ: 2.64MB
- 設宁E `UseExeDirectory=false`�E�ユーザーチE��レクトリに出力！E

### ポ�Eタブル牁E
- ファイル吁E `BrowserChooser3-v0.1.2.zip`
- サイズ: 1.33MB
- 設宁E `UseExeDirectory=true`�E�Exe実行フォルダに出力！E

## 🚀 インスト�Eル方況E

### インスト�Eラー牁E
1. `BrowserChooser3-Setup.exe`をダウンローチE
2. 管琁E��E��限で実衁E
3. インスト�Eルウィザードに従ってインスト�Eル
4. 設定ファイルは`%APPDATA%\BrowserChooser3`に保孁E

### ポ�Eタブル牁E
1. `BrowserChooser3-v0.1.2.zip`をダウンローチE
2. 任意�Eフォルダに展開
3. `BrowserChooser3.exe`を実衁E
4. 設定ファイルはexe実行フォルダに保孁E

## 📋 シスチE��要件

- **OS**: Windows 10/11 x64
- **.NET**: .NET 10.0 Desktop Runtime
- **メモリ**: 512MB以丁E
- **チE��スク**: 10MB以上�E空き容釁E

## 🔄 アチE�EチE�Eト方況E

### v0.1.1からのアチE�EチE�EチE
- 設定ファイルは自動的に新しい場所に移衁E
- 既存�E設定�E保持されまぁE
- iniファイルによる出力�E制御が有効になりまぁE

## 📝 既知の問顁E

- なぁE

## 🙏 謝辁E

こ�Eリリースに貢献してくださった開発老E��テスター、ユーザーの皁E��に感謝いたします、E

## 📞 サポ�EチE

- **GitHub**: https://github.com/Yosuke-Sh/BrowserChooser3
- **Issues**: https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: https://github.com/Yosuke-Sh/BrowserChooser3/wiki

---

**BrowserChooser3 v0.1.2** - INIファイルによる柔軟なパス管琁E��改喁E��れた設定制御

---

# BrowserChooser3 v0.1.1 リリースノ�EチE

## 🎉 新機�E・改喁E

### ポ�Eタブルモード�E改喁E
- ビルド時定数によるポ�Eタブルモード制御を実裁E
- インスト�Eラー経由インスト�Eルの正確な判定を追加
- Program Files以下�E判定による確実なインスト�Eル方法�E検�E

### 設定ファイルとログファイルの出力場所の統一
- 設定ファイルを常にユーザーチE��レクトリ�E�EAPPDATA%\BrowserChooser3�E�に出劁E
- ログファイルを常にユーザーチE��レクトリ�E�ELOCALAPPDATA%\BrowserChooser3\Logs�E�に出劁E
- 実行フォルダへの出力を停止し、ユーザーチE��レクトリに統一

### ビルド設定�E改喁E
- Debug/Release/Portableの吁E��定に対忁E
- ポ�Eタブル版リリース作�Eスクリプトの改喁E
- ビルド�E力ディレクトリの適刁E��管琁E

## 🐛 バグ修正

### オプション画面の表示問顁E
- Settings頁E��が正しく表示されるよぁE��修正
- チE��ト環墁E�E離による誤判定�E問題を解決

### ポ�Eタブルモード判定�E問顁E
- インスト�Eラーでインスト�Eル時にProgram Files以下に設定を書き込もうとする問題を解決
- ビルド時定数による確実な制御を実裁E

## 🔧 技術的改喁E

### コード品質の向丁E
- 設定ファイルとログファイルの出力場所を統一
- ポ�Eタブルモード�E判定ロジチE��を改喁E
- ビルド設定による制御の実裁E

### ユーザビリチE��の向丁E
- 設定ファイルとログファイルが適刁E��ユーザーチE��レクトリに出劁E
- インスト�Eル方法�E正確な判定と適刁E��動佁E

## 📦 配币E��チE��ージ

### インスト�Eラー牁E
- ファイル吁E `BrowserChooser3-Setup.exe`
- サイズ: 2.52MB
- インスト�Eル允E Program Files\BrowserChooser3

### ポ�Eタブル牁E
- ファイル吁E `BrowserChooser3-v0.1.1.zip`
- サイズ: 1.28MB
- 展開後サイズ: 2.97MB

## 🚀 インスト�Eル方況E

### インスト�Eラー牁E
1. `BrowserChooser3-Setup.exe`をダウンローチE
2. 管琁E��E��限で実衁E
3. インスト�Eルウィザードに従ってインスト�Eル

### ポ�Eタブル牁E
1. `BrowserChooser3-v0.1.1.zip`をダウンローチE
2. 任意�Eフォルダに展開
3. `BrowserChooser3.exe`を実衁E

## 📋 シスチE��要件

- **OS**: Windows 10/11 x64
- **.NET**: .NET 10.0 Desktop Runtime
- **メモリ**: 512MB以丁E
- **チE��スク**: 10MB以上�E空き容釁E

## 🔄 アチE�EチE�Eト方況E

### v0.1.0からのアチE�EチE�EチE
- 設定ファイルは自動的に新しい場所�E�EAPPDATA%\BrowserChooser3�E�に移衁E
- 既存�E設定�E保持されまぁE
- ログファイルも新しい場所�E�ELOCALAPPDATA%\BrowserChooser3\Logs�E�に出劁E

## 📝 既知の問顁E

- .NET SDK 9.0でのビルド警告（機�Eには影響なし！E
- 一部のチE��トがスキチE�Eされる場合がある�E�EptionsFormのモチE��化�E困難性による�E�E

## 🙏 謝辁E

こ�Eリリースに貢献してくださった開発老E��テスター、ユーザーの皁E��に感謝いたします、E

## 📞 サポ�EチE

- **GitHub**: https://github.com/Yosuke-Sh/BrowserChooser3
- **Issues**: https://github.com/Yosuke-Sh/BrowserChooser3/issues
- **Wiki**: https://github.com/Yosuke-Sh/BrowserChooser3/wiki

---

**BrowserChooser3 v0.1.1** - より安定した�Eータブルモードと改喁E��れた設定管琁E
