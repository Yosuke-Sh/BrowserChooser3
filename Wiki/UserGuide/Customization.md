# カスタマイズ

BrowserChooser3のカスタマイズ機能について詳しく説明します。

## 🎨 カスタマイズの概要

BrowserChooser3は、ユーザーの好みに合わせて様々なカスタマイズが可能です：

- **外観カスタマイズ**: 透明度、角の丸み、背景色
- **ブラウザ管理**: カスタムブラウザの追加・編集
- **プロトコル設定**: プロトコル別のブラウザ指定
- **起動設定**: 起動時の動作とシステムトレイ常駐
- **アクセシビリティ**: フォーカス表示とキーボードナビゲーション
- **パス管理**: 設定ファイルとログファイルの出力先制御（v0.1.2以降）

## 🎯 外観カスタマイズ

### 透明度設定
1. オプションダイアログを開く（`O`キー）
2. 「Display」タブを選択
3. 透明度設定を調整：
   - **透明化を有効化**: 透明化効果の有効/無効
   - **不透明度**: 0.01-1.00の範囲で調整
   - **透明化色**: 透明にする色を指定

### 角の丸み設定
1. 「Display」タブで「角の丸みを有効化」をチェック
2. 角の丸み半径を0-50の範囲で調整
3. リアルタイムでプレビューが表示されます

### 背景色設定
1. 「Display」タブで「背景色」をクリック
2. カラーピッカーで色を選択
3. カスタム色も指定可能

### タイトルバー設定
1. 「Display」タブで「タイトルバーを非表示」をチェック
2. よりクリーンな外観になります
3. ウィンドウエッジでサイズ変更可能

## 🔧 ブラウザカスタマイズ

### カスタムブラウザの追加
1. 「Browsers」タブを選択
2. 「Add」ボタンをクリック
3. 以下の情報を入力：
   - **Name**: ブラウザの表示名
   - **Target**: 実行ファイルのパス
   - **Arguments**: コマンドライン引数
   - **Icon**: アイコンファイルのパス

### ブラウザの編集
1. ブラウザ一覧から編集したいブラウザを選択
2. 「Edit」ボタンをクリック
3. 設定を変更して「OK」をクリック

### ブラウザの順序変更
1. ブラウザ一覧でブラウザを選択
2. 「Up」または「Down」ボタンで順序を変更
3. 表示順序とキーボードナビゲーション順序が変更されます

### アイコンカスタマイズ
1. ブラウザ編集画面で「Icon」フィールドの「...」をクリック
2. 以下のファイル形式から選択：
   - **実行ファイル** (.exe): ブラウザの実行ファイル
   - **アイコンファイル** (.ico): アイコンファイル
   - **画像ファイル** (.png, .jpg, .jpeg): 画像ファイル

## 🌐 プロトコルカスタマイズ

### プロトコルハンドラーの設定
1. 「Protocols」タブを選択
2. 「Add」ボタンをクリック
3. プロトコル名とブラウザを設定

### カスタムプロトコル
1. プロトコル名を指定（例：`myapp`）
2. 対応するアプリケーションをブラウザとして指定
3. アプリケーションの引数に`{url}`を指定

### プロトコル別の引数設定
```cmd
# HTTPSプロトコル用の引数
https -> Chrome --new-window {url}

# HTTPプロトコル用の引数
http -> Chrome --new-tab {url}

# メールプロトコル用の引数
mailto -> Chrome --app={url}
```

## 🚀 起動設定カスタマイズ

### 起動モードの設定
1. 「Startup」タブを選択
2. 以下のオプションから選択：
   - **Normal**: 通常起動
   - **Start Minimized**: 最小化で起動
   - **Start in System Tray**: システムトレイで起動

## 📁 パス管理カスタマイズ（v0.1.2以降）

### INIファイルによる設定
BrowserChooser3 v0.1.2以降では、`BrowserChooser3.ini`ファイルで設定ファイルとログファイルの出力先を制御できます。

#### 設定ファイルの場所
- **インストーラー版**: `%APPDATA%\BrowserChooser3\BrowserChooser3Config.xml`
- **ポータブル版**: exe実行フォルダ内の`BrowserChooser3Config.xml`

#### ログファイルの場所
- **インストーラー版**: `%LOCALAPPDATA%\BrowserChooser3\Logs\`
- **ポータブル版**: exe実行フォルダ内の`Logs\`フォルダ

### INIファイルの設定項目
```ini
[Paths]
; 出力フォルダの選択
; true: exe実行フォルダに出力（ポータブル版用）
; false: ユーザーディレクトリに出力（インストーラー版用）
UseExeDirectory=true

; ログファイルの出力先
; 空の場合: UseExeDirectoryの設定に従う
; 指定した場合: そのフォルダに出力
LogDirectory=

; 設定ファイルの出力先
; 空の場合: UseExeDirectoryの設定に従う
; 指定した場合: そのフォルダに出力
ConfigDirectory=
```

### カスタムパスの設定
特定のフォルダに出力したい場合：

```ini
[Paths]
UseExeDirectory=false
LogDirectory=D:\MyLogs\BrowserChooser3
ConfigDirectory=D:\MyConfig\BrowserChooser3
```

### システムトレイ常駐設定
1. 「Always Resident in System Tray」をチェック
2. アプリケーションがバックグラウンドで継続動作
3. システムトレイアイコンからアクセス可能

### 起動遅延設定
1. 「Startup Delay」で遅延時間を設定（0-60秒）
2. システム起動時の負荷分散に有用

### 起動メッセージ設定
1. 「Startup Message」でメッセージを入力
2. プレースホルダーが使用可能：
   - `{version}`: アプリケーションバージョン
   - `{date}`: 現在の日付
   - `{time}`: 現在の時刻

## ♿ アクセシビリティカスタマイズ

### フォーカスボックス設定
1. 「Focus」タブを選択
2. 「Focus Box Color」で色を設定
3. 「Focus Box Width」で幅を設定

### キーボードナビゲーション
1. 「Enable Keyboard Navigation」をチェック
2. キーボードでブラウザ間を移動可能
3. `Tab`キーでブラウザ間を移動

### アクセシブルレンダリング
1. 「Enable Accessible Rendering」をチェック
2. スクリーンリーダー対応が有効化
3. アクセシビリティツールとの互換性が向上

## 📝 設定ファイルのカスタマイズ

### 設定ファイルの場所
- **ファイル名**: `BrowserChooser3Config.xml`
- **場所**: アプリケーションフォルダ
- **形式**: XML

### 手動編集
```xml
<Settings>
  <DisplaySettings>
    <Transparency>0.8</Transparency>
    <CornerRadius>10</CornerRadius>
    <BackgroundColor>#2D2D30</BackgroundColor>
  </DisplaySettings>
  <Browsers>
    <Browser>
      <Name>Custom Browser</Name>
      <Target>C:\Path\To\Browser.exe</Target>
      <Arguments>--new-window {url}</Arguments>
    </Browser>
  </Browsers>
</Settings>
```

### 設定のインポート/エクスポート
1. 設定ファイルをコピーしてバックアップ
2. 他のマシンに設定ファイルをコピー
3. 設定の共有や移行が可能

## 🎨 カスタマイズ例

### モダンな外観
```xml
<DisplaySettings>
  <Transparency>0.8</Transparency>
  <CornerRadius>15</CornerRadius>
  <BackgroundColor>#1E1E1E</BackgroundColor>
  <HideTitleBar>true</HideTitleBar>
</DisplaySettings>
```

### プロフェッショナルな外観
```xml
<DisplaySettings>
  <Transparency>0.95</Transparency>
  <CornerRadius>5</CornerRadius>
  <BackgroundColor>#FFFFFF</BackgroundColor>
  <HideTitleBar>false</HideTitleBar>
</DisplaySettings>
```

### カスタムブラウザ設定
```xml
<Browsers>
  <Browser>
    <Name>Chrome Work</Name>
    <Target>C:\Program Files\Google\Chrome\Application\chrome.exe</Target>
    <Arguments>--profile-directory="Work" {url}</Arguments>
  </Browser>
  <Browser>
    <Name>Firefox Personal</Name>
    <Target>C:\Program Files\Mozilla Firefox\firefox.exe</Target>
    <Arguments>--new-tab {url}</Arguments>
  </Browser>
</Browsers>
```

## 🔧 高度なカスタマイズ

### コマンドライン引数の活用
```cmd
# 新しいウィンドウで開く
--new-window {url}

# 新しいタブで開く
--new-tab {url}

# プライベートモードで開く
--incognito {url}

# 特定のプロファイルで開く
--profile-directory="Profile 1" {url}

# カスタムユーザーデータディレクトリ
--user-data-dir="C:\CustomProfile" {url}
```

### プロトコル別の詳細設定
```xml
<ProtocolHandlers>
  <ProtocolHandler>
    <Protocol>https</Protocol>
    <Browser>Chrome</Browser>
    <Description>Secure Web Pages</Description>
  </ProtocolHandler>
  <ProtocolHandler>
    <Protocol>mailto</Protocol>
    <Browser>Outlook</Browser>
    <Description>Email Links</Description>
  </ProtocolHandler>
</ProtocolHandlers>
```

### 起動設定の最適化
```xml
<StartupSettings>
  <StartupMode>SystemTray</StartupMode>
  <AlwaysResidentInSystemTray>true</AlwaysResidentInSystemTray>
  <StartupDelay>5</StartupDelay>
  <StartupMessage>BrowserChooser3 v{version} が起動しました</StartupMessage>
</StartupSettings>
```

## 🚨 トラブルシューティング

### よくある問題

#### カスタマイズが反映されない
**原因**:
- 設定ファイルの権限問題
- 設定ファイルの破損
- アプリケーションの再起動が必要

**解決策**:
1. アプリケーションを再起動
2. 設定ファイルの権限を確認
3. 設定ファイルをバックアップから復元

#### カスタムブラウザが動作しない
**原因**:
- 実行ファイルのパスが間違っている
- コマンドライン引数が正しくない
- ブラウザが破損している

**解決策**:
1. ブラウザのパスを確認
2. コマンドライン引数を修正
3. ブラウザを再インストール

#### 透明化設定が動作しない
**原因**:
- Windows透明化効果が無効
- グラフィックドライバーの問題
- 設定値が正しくない

**解決策**:
1. Windows透明化効果を有効化
2. グラフィックドライバーを更新
3. 設定値を確認

### デバッグ方法

#### ログの確認
1. 「Other」タブでログを有効化
2. カスタマイズ操作を実行
3. ログファイルを確認

#### 設定ファイルの検証
1. 設定ファイルをXMLエディタで開く
2. XML構文エラーをチェック
3. 設定値の妥当性を確認

## 💡 ベストプラクティス

### 外観カスタマイズ
- **一貫性**: 他のアプリケーションと調和する設定
- **可読性**: 十分なコントラストを確保
- **パフォーマンス**: システムリソースを考慮

### ブラウザ管理
- **命名規則**: 分かりやすい名前を使用
- **アイコン管理**: 高解像度のアイコンを使用
- **引数設定**: 必要最小限の引数を使用

### 設定管理
- **バックアップ**: 定期的に設定をバックアップ
- **テスト**: 新しい設定を十分にテスト
- **ドキュメント**: カスタム設定を記録

### アクセシビリティ
- **ユーザビリティ**: 実際のユーザーでテスト
- **代替手段**: アクセシビリティ機能を提供
- **柔軟性**: 様々なニーズに対応

## 📚 関連情報

- [設定ガイド](Configuration-Guide)
- [透明化設定](Transparency-Settings)
- [アクセシビリティ機能](Accessibility-Features)
- [ブラウザ管理](Browser-Management)
- [起動設定](Startup-Settings)

---

*より詳細な設定については、[設定ガイド](Configuration-Guide)をご覧ください。*
