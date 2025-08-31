# BrowserChooser3 リリースノート

## 最新リリース

### v0.1.0 (2024-12-31) - ベータ版リリース

#### 🎉 新機能
- **アイコン選択機能**: .exe、.ico、画像ファイル（.png、.jpg）からのアイコン抽出・選択
- **背景グラデーション**: 縦方向のグラデーション効果による美しいUI
- **Focusタブ**: 専用のフォーカス設定タブ
- **アクセシブルレンダリング対応**: アクセシビリティ機能の強化
- **ポータブルモード**: インストールパスで自動判定
- **システムトレイ常駐機能**: バックグラウンドでの動作
- **起動設定**: 自動起動、最小化起動、システムトレイ起動オプション

#### 🔧 改善点
- **UI/UX改善**: モダンなデザインとユーザビリティの向上
- **アイコン管理**: 複数形式対応とフォールバック機能
- **設定管理**: より柔軟な設定オプション
- **エラーハンドリング**: 改善されたエラー処理とユーザーフィードバック
- **テスト環境**: 自動テストとカバレッジレポート
- **メモリ管理**: 最適化されたリソース管理

#### 🐛 バグ修正
- アイコン表示の問題を修正
- フォームの自動クローズ機能を改善
- メモリリークの修正
- テスト環境での安定性向上

#### 📦 配布パッケージ
- **インストーラー版**: Inno Setup 6による完全インストーラー
- **ポータブル版**: 単体実行可能なZIPパッケージ

#### 🔧 技術仕様
- **フレームワーク**: .NET 8.0 Windows Forms
- **言語**: C#
- **テスト**: xUnit、FluentAssertions
- **インストーラー**: Inno Setup 6
- **アイコン対応**: .exe、.ico、.png、.jpg
- **プロトコル対応**: カスタムプロトコルハンドラー

#### 📥 インストール方法

##### インストーラー版（推奨）
1. [BrowserChooser3-Setup.exe](https://github.com/your-repo/BrowserChooser3/releases/download/v0.1.0/BrowserChooser3-Setup.exe) をダウンロード
2. インストーラーを実行
3. 指示に従ってインストール

##### ポータブル版
1. [BrowserChooser3-v0.1.0.zip](https://github.com/your-repo/BrowserChooser3/releases/download/v0.1.0/BrowserChooser3-v0.1.0.zip) をダウンロード
2. 任意のフォルダに展開
3. BrowserChooser3.exe を実行

#### 📋 システム要件
- Windows 10/11 (x64)
- .NET 8.0 Runtime
- 最小メモリ: 512MB
- ディスク容量: 50MB

#### 🔄 変更履歴

##### v0.1.0 (2024-12-31)
- **ベータ版リリース**
- アイコン選択機能の実装
- 背景グラデーション効果の追加
- Focusタブの新設
- アクセシビリティ機能の強化
- ポータブルモードの実装
- システムトレイ機能の追加
- 起動設定オプションの拡充
- インストーラーとポータブル版の提供
- テスト環境の整備
- ドキュメントの充実

#### 🚀 今後の予定
- **v0.1.1**: バグ修正と安定性向上
- **v0.2.0**: 自動アップデート機能
- **v0.3.0**: 多言語対応
- **v1.0.0**: 正式版リリース

#### 📊 リリース統計
- **ファイルサイズ**: インストーラー版 ~1.5MB、ポータブル版 ~2MB
- **インストールサイズ**: ~5MB
- **対応OS**: Windows 10/11
- **アーキテクチャ**: x64

#### 📞 サポート
- **GitHub Issues**: [問題報告](https://github.com/your-repo/BrowserChooser3/issues)
- **Wiki**: [詳細ドキュメント](https://github.com/your-repo/BrowserChooser3/wiki)
- **FAQ**: [よくある質問](https://github.com/your-repo/BrowserChooser3/wiki/Community/FAQ)

---

## リリース履歴

### 過去のリリース
- **v0.1.0** (2024-12-31) - ベータ版リリース

### 今後のリリース計画

#### v0.1.1 (予定: 2025-01-15)
- バグ修正と安定性向上
- パフォーマンス最適化
- ユーザーフィードバック対応

#### v0.2.0 (予定: 2025-02-01)
- 自動アップデート機能
- GitHub Actions CI/CD
- コード署名対応

#### v0.3.0 (予定: 2025-03-01)
- 多言語対応（日本語、英語）
- ローカライゼーション
- 地域別設定

#### v1.0.0 (予定: 2025-06-01)
- 正式版リリース
- 完全機能実装
- 長期サポート開始

---

## リリース品質

### 品質指標
- **テストカバレッジ**: 85%以上
- **コード品質**: SonarQube A級
- **セキュリティ**: 脆弱性スキャン済み
- **パフォーマンス**: 基準値達成

### リリース基準
- 全テストが成功すること
- セキュリティチェックが通ること
- ドキュメントが最新であること
- インストーラーが正常に動作すること

---

## リリースノート形式

### セクション構成
1. **新機能**: 新しく追加された機能
2. **改善点**: 既存機能の改善
3. **バグ修正**: 修正された問題
4. **技術仕様**: 技術的な詳細
5. **インストール方法**: インストール手順
6. **変更履歴**: 詳細な変更内容
7. **今後の予定**: 次回リリースの予定

### バージョニング
- **メジャー**: 大きな変更、非互換性
- **マイナー**: 新機能追加
- **パッチ**: バグ修正

---

## 関連情報

### ドキュメント
- [インストールガイド](https://github.com/your-repo/BrowserChooser3/wiki/GettingStarted/Installation-Guide)
- [クイックスタート](https://github.com/your-repo/BrowserChooser3/wiki/GettingStarted/Quick-Start-Tutorial)
- [ユーザーガイド](https://github.com/your-repo/BrowserChooser3/wiki/UserGuide/Basic-Usage)
- [トラブルシューティング](https://github.com/your-repo/BrowserChooser3/wiki/AdvancedTopics/Troubleshooting)

### フィードバックチャンネル
- **GitHub Issues**: バグ報告・機能リクエスト
- **GitHub Discussions**: 一般的な質問・議論
- **Wiki**: ドキュメント・ガイド

### 貢献
- [貢献ガイドライン](https://github.com/your-repo/BrowserChooser3/wiki/Development/Contributing-Guidelines)
- [開発者向けドキュメント](https://github.com/your-repo/BrowserChooser3/wiki/Development/API-Reference)
- [テストガイド](https://github.com/your-repo/BrowserChooser3/wiki/Development/Testing)
