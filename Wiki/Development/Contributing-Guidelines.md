# 貢献ガイドライン

BrowserChooser3プロジェクトへの貢献をありがとうございます！このガイドラインに従って、効率的で協力的な開発を進めましょう。

## 🤝 貢献の種類

### バグ報告
- 既存の機能に関する問題の報告
- 再現手順の詳細な説明
- 環境情報の提供

### 機能要望
- 新機能の提案
- 既存機能の改善案
- ユーザビリティの向上

### コード貢献
- バグ修正
- 新機能の実装
- ドキュメントの改善
- テストの追加

### ドキュメント
- Wikiページの改善
- READMEの更新
- コメントの追加

## 📋 貢献の手順

### 1. 環境の準備

#### 必要なツール
```cmd
# Gitのインストール
# https://git-scm.com/download/win

# .NET 8.0 SDKのインストール
# https://dotnet.microsoft.com/download/dotnet/8.0

# Visual Studio 2022（推奨）
# https://visualstudio.microsoft.com/
```

#### リポジトリのフォーク
1. [GitHubリポジトリ](https://github.com/Yosuke-Sh/BrowserChooser3)にアクセス
2. 「Fork」ボタンをクリック
3. 自分のアカウントにリポジトリをフォーク

#### ローカル環境のセットアップ
```cmd
# フォークしたリポジトリをクローン
git clone https://github.com/[YourUsername]/BrowserChooser3.git
cd BrowserChooser3

# 上流リポジトリを追加
git remote add upstream https://github.com/Yosuke-Sh/BrowserChooser3.git

# 最新の変更を取得
git fetch upstream
git checkout main
git merge upstream/main
```

### 2. ブランチの作成

#### ブランチ命名規則
```cmd
# 機能追加
git checkout -b feature/新機能名

# バグ修正
git checkout -b fix/バグの説明

# ドキュメント
git checkout -b docs/ドキュメント名

# リファクタリング
git checkout -b refactor/リファクタリング内容
```

#### 例
```cmd
git checkout -b feature/auto-update
git checkout -b fix/memory-leak
git checkout -b docs/api-reference
git checkout -b refactor/logger-service
```

### 3. 開発とテスト

#### コードの実装
```cmd
# プロジェクトのビルド
dotnet build

# テストの実行
dotnet test

# コードの実行
dotnet run
```

#### コーディング規約
```csharp
// 命名規則
public class BrowserManager { }           // PascalCase
private string browserPath;               // camelCase
private const int MAX_RETRY_COUNT = 3;    // UPPER_CASE

// メソッドの実装
public async Task<bool> LoadBrowsersAsync()
{
    try
    {
        // 処理
        return true;
    }
    catch (Exception ex)
    {
        Logger.Log(LogLevel.Error, "ブラウザ読み込みエラー: {Exception}", ex);
        return false;
    }
}

// XMLドキュメントコメント
/// <summary>
/// ブラウザの一覧を取得します。
/// </summary>
/// <param name="includeHidden">非表示ブラウザを含めるかどうか</param>
/// <returns>ブラウザの一覧</returns>
public List<Browser> GetBrowsers(bool includeHidden = false)
{
    // 実装
}
```

#### テストの追加
```csharp
[Fact]
public void GetBrowsers_WithValidSettings_ShouldReturnBrowsers()
{
    // Arrange
    var manager = new BrowserManager();
    
    // Act
    var browsers = manager.GetBrowsers();
    
    // Assert
    browsers.Should().NotBeNull();
    browsers.Should().NotBeEmpty();
}
```

### 4. コミットとプッシュ

#### コミットメッセージの規約
```cmd
# 形式: <type>(<scope>): <description>

# 例
git commit -m "feat(browser): 自動ブラウザ検出機能を追加"
git commit -m "fix(ui): 透明化設定のバグを修正"
git commit -m "docs(readme): インストール手順を更新"
git commit -m "test(logger): ログ機能のテストを追加"
```

#### コミットタイプ
- **feat**: 新機能
- **fix**: バグ修正
- **docs**: ドキュメント
- **style**: コードスタイル
- **refactor**: リファクタリング
- **test**: テスト
- **chore**: その他の変更

#### プッシュ
```cmd
# 変更をプッシュ
git push origin feature/新機能名

# 複数のコミットがある場合
git push origin feature/新機能名 --force-with-lease
```

### 5. プルリクエストの作成

#### プルリクエストの準備
1. GitHubでプルリクエストを作成
2. タイトルと説明を記入
3. 関連するIssueをリンク

#### プルリクエストのテンプレート
```markdown
## 概要
このプルリクエストの目的と変更内容を説明してください。

## 変更内容
- [ ] 新機能の追加
- [ ] バグ修正
- [ ] ドキュメントの更新
- [ ] テストの追加

## テスト
- [ ] 単体テストを追加/更新
- [ ] 手動テストを実行
- [ ] 既存のテストが通ることを確認

## チェックリスト
- [ ] コードがプロジェクトのスタイルガイドに従っている
- [ ] 自己レビューを実行した
- [ ] コメントを追加した（特に理解しにくい部分）
- [ ] ドキュメントを更新した
- [ ] 変更が既存の機能を壊していない
- [ ] 新しいテストを追加した（バグ修正や新機能の場合）

## 関連Issue
Closes #123
```

## 🧪 テストガイドライン

### テストの種類
```csharp
// 単体テスト
[Fact]
public void Method_WithValidInput_ShouldReturnExpectedResult()
{
    // テスト実装
}

// 統合テスト
[Fact]
public async Task Integration_WithRealData_ShouldWorkCorrectly()
{
    // 統合テスト実装
}

// パラメータ化テスト
[Theory]
[InlineData("chrome.exe", true)]
[InlineData("invalid.exe", false)]
public void IsValidBrowser_WithVariousInputs_ShouldReturnExpectedResult(string input, bool expected)
{
    // テスト実装
}
```

### テストカバレッジ
```cmd
# カバレッジレポートの生成
dotnet test --collect:"XPlat Code Coverage"

# カバレッジの確認
# coverage-report/index.html を開く
```

## 📝 ドキュメントガイドライン

### Wikiページの作成
```markdown
# ページタイトル

## 概要
ページの目的と内容を説明

## 詳細
具体的な内容

## 例
```code
// コード例
```

## 関連情報
- [関連ページ](リンク)
```

### コメントの追加
```csharp
/// <summary>
/// ブラウザの設定を管理するクラス
/// </summary>
public class BrowserSettings
{
    /// <summary>
    /// ブラウザの一覧を取得します
    /// </summary>
    /// <param name="includeHidden">非表示ブラウザを含めるかどうか</param>
    /// <returns>ブラウザの一覧</returns>
    /// <exception cref="InvalidOperationException">設定ファイルが見つからない場合</exception>
    public List<Browser> GetBrowsers(bool includeHidden = false)
    {
        // 実装
    }
}
```

## 🔍 レビュープロセス

### レビューの準備
1. コードの自己レビュー
2. テストの実行
3. ドキュメントの更新
4. コミットメッセージの確認

### レビュー時の注意点
- **機能性**: 要件を満たしているか
- **品質**: コードの品質は適切か
- **テスト**: 十分なテストがあるか
- **ドキュメント**: ドキュメントは更新されているか
- **パフォーマンス**: パフォーマンスに問題はないか

### レビューコメント
```markdown
## 良いレビューコメントの例

### 具体的な提案
```csharp
// 現在のコード
var result = GetData();

// 提案
var result = await GetDataAsync(); // 非同期処理を使用
```

### 質問
この部分でなぜこの実装を選択したのですか？

### 改善提案
このメソッドは長すぎるので、複数のメソッドに分割することを検討してください。
```

## 🚀 リリースプロセス

### バージョニング
```cmd
# セマンティックバージョニング
# MAJOR.MINOR.PATCH

# 例
0.1.0  # 初回リリース
0.1.1  # バグ修正
0.2.0  # 新機能追加
1.0.0  # メジャーリリース
```

### リリース準備
```cmd
# タグの作成
git tag v0.1.0
git push origin v0.1.0

# リリースノートの作成
# GitHub Releasesでリリースノートを記入
```

## 📞 サポート

### 質問や相談
- [GitHub Discussions](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)
- [GitHub Issues](https://github.com/Yosuke-Sh/BrowserChooser3/issues)

### コミュニティガイドライン
- 敬意を持って接する
- 建設的なフィードバックを提供
- 他の貢献者をサポート
- プロジェクトの目標を理解する

## 📚 関連情報

- [ソースからのビルド](Building-from-Source)
- [APIリファレンス](API-Reference)
- [テスト](Testing)
- [コーディング規約](Coding-Standards)

---

*貢献について質問がある場合は、[GitHub Discussions](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)でお気軽にお聞きください。*
