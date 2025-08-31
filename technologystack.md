# BrowserChooser3 技術スタック

## フレームワーク・ランタイム
- **.NET**: 8.0
- **ターゲットフレームワーク**: net8.0-windows
- **アプリケーションタイプ**: Windows Forms デスクトップアプリケーション

## 主要ライブラリ・パッケージ

### 開発・テスト
- **xUnit**: 2.8.2 - 単体テストフレームワーク
- **FluentAssertions**: 6.12.0 - アサーションライブラリ
- **coverlet.collector**: 6.0.0 - コードカバレッジ収集
- **ReportGenerator**: 5.2.4 - カバレッジレポート生成

### システム・ユーティリティ
- **System.Windows.Forms**: Windows Forms UI
- **System.Drawing**: グラフィックス処理
- **System.Xml**: XML設定ファイル処理
- **System.Xml.Serialization**: XMLシリアライゼーション
- **System.Text.Json**: JSON処理（必要に応じて）
- **System.Net.Http**: HTTP通信（URL短縮解除機能）
- **System.Runtime.InteropServices**: Win32 API呼び出し（アイコン抽出）

### レジストリ・システム操作
- **Microsoft.Win32**: レジストリ操作（ブラウザ検出）
- **System.Diagnostics.Process**: プロセス起動（ブラウザ起動）

## アーキテクチャ・パターン

### 設計パターン
- **MVC/Presenter Pattern**: OptionsFormでのハンドラークラス分離
- **Service Layer Pattern**: ビジネスロジックの分離
- **Repository Pattern**: 設定ファイルの読み書き
- **Factory Pattern**: ブラウザ検出・作成
- **Strategy Pattern**: アイコン読み込み方法の切り替え
- **Adapter Pattern**: MessageBoxServiceによるテスト環境対応

### ディレクトリ構造
```
BrowserChooser3/
├── Forms/                 # Windows Forms UI
├── Classes/
│   ├── Models/           # データモデル
│   ├── Services/         # サービス層
│   │   ├── Browser/      # ブラウザ関連サービス
│   │   ├── OptionsForm/  # オプション画面サービス
│   │   ├── System/       # システムサービス
│   │   └── UI/           # UIサービス（MessageBoxService等）
│   ├── Interfaces/       # インターフェース定義
│   └── Utilities/        # ユーティリティクラス
├── CustomControls/       # カスタムコントロール
└── Tests/               # 単体テスト
```

## 設定管理

### 設定ファイル
- **形式**: XML
- **場所**: アプリケーションディレクトリ/BrowserChooser3Config.xml
- **シリアライゼーション**: System.Xml.Serialization.XmlSerializer

### 設定項目
- ブラウザ設定（名前、パス、アイコン、表示設定）
- UI設定（アイコンサイズ、間隔、背景色）
- 動作設定（自動終了、ログ出力、遅延時間）
- 透明化設定（透明度、角丸、タイトルバー非表示）

## ログシステム

### ログレベル
- **None**: ログなし
- **Error**: エラー
- **Warning**: 警告
- **Info**: 情報
- **Debug**: デバッグ
- **Trace**: トレース

### ログ出力
- **ファイル**: %APPDATA%\BrowserChooser3\logs\
- **形式**: CSV形式（時刻、レベル、クラス、メソッド、メッセージ、詳細）
- **ローテーション**: 日付別ファイル

## ブラウザ検出・起動

### 検出方法
- **レジストリ検索**: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths
- **Program Files検索**: 一般的なブラウザのインストール場所
- **カスタム設定**: ユーザー定義のブラウザ設定

### 対応ブラウザ
- Google Chrome
- Microsoft Edge
- Mozilla Firefox
- Opera
- Safari
- その他カスタムブラウザ

### アイコン対応形式
- **実行ファイル**: .exe（Win32 APIによるアイコン抽出）
- **アイコンファイル**: .ico（直接読み込み）
- **画像ファイル**: .png, .jpg, .jpeg, .bmp（リサイズしてアイコン化）
- **その他ファイル**: 関連付けられたアイコン取得

## セキュリティ・制約

### セキュリティ要件
- **最小権限の原則**: 必要最小限の権限で動作
- **入力検証**: URL、ファイルパスの適切な検証
- **例外処理**: 適切なエラーハンドリングとログ出力

### 制約事項
- **後方互換性**: 既存設定ファイルとの互換性維持
- **UI変更制限**: レイアウト、色、フォントの変更は承認必須
- **バージョン固定**: 技術スタックのバージョン変更は承認必須

## 開発・テスト環境

### 開発環境
- **IDE**: Visual Studio 2022 / Visual Studio Code
- **OS**: Windows 10/11
- **.NET SDK**: 8.0.x

### テスト環境
- **テストランナー**: xUnit
- **カバレッジ**: Cobertura XML形式
- **レポート**: HTML形式（ReportGenerator）
- **テスト環境検出**: アセンブリ名による自動判定
- **UI制御**: MessageBoxServiceによるテスト環境でのダイアログ制御

### ビルド・デプロイ
- **ビルドツール**: MSBuild / dotnet CLI
- **インストーラー**: Inno Setup 6
- **配布形式**: Windows Installer (.exe)
- **ビルドスクリプト**: build-inno-setup.bat
- **インストーラー設定**: BrowserChooser3-Setup.iss
