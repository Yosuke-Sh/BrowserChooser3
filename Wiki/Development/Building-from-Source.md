# ソースからのビルド

BrowserChooser3をソースコードからビルドする方法を説明します。

## 📋 前提条件

### 必要なソフトウェア
- **Visual Studio 2022** (Community/Professional/Enterprise)
- **.NET 10.0 SDK**
- **Git**

### 推奨環境
- **OS**: Windows 10/11 (x64)
- **RAM**: 8GB以上
- **ディスク容量**: 2GB以上の空き容量

## 🔧 開発環境のセットアップ

### 1. .NET 10.0 SDKのインストール
```cmd
# .NET 10.0 SDKのダウンロード
# https://dotnet.microsoft.com/download/dotnet/10.0

# インストール確認
dotnet --version
```

### 2. Visual Studio 2022のインストール
```cmd
# Visual Studio Installerを使用
# 以下のワークロードを選択：
# - .NET デスクトップ開発
# - C++ によるデスクトップ開発（オプション）
```

### 3. Gitのインストール
```cmd
# Git for Windowsをダウンロード
# https://git-scm.com/download/win

# インストール確認
git --version
```

## 📥 ソースコードの取得

### リポジトリのクローン
```cmd
# リポジトリをクローン
git clone https://github.com/Yosuke-Sh/BrowserChooser3.git
cd BrowserChooser3

# 最新のコードを取得
git pull origin main
```

### ブランチの選択
```cmd
# 開発ブランチに切り替え
git checkout developer

# または、特定のタグをチェックアウト
git checkout v0.1.0
```

## 🏗️ プロジェクトのビルド

### 1. 依存関係の復元
```cmd
# プロジェクトディレクトリに移動
cd BrowserChooser3

# NuGetパッケージを復元
dotnet restore
```

### 2. デバッグビルド
```cmd
# デバッグビルド
dotnet build --configuration Debug

# または、Visual Studioでビルド
# Build > Build Solution (Ctrl+Shift+B)
```

### 3. リリースビルド
```cmd
# リリースビルド
dotnet build --configuration Release

# 自己完結型のビルド
dotnet publish --configuration Release --self-contained --runtime win-x64
```

### 4. テストの実行
```cmd
# テストプロジェクトのビルド
dotnet build BrowserChooser3.Tests

# テストの実行
dotnet test
```

## 🔄 ビルド設定の変更（v0.1.2以降）

### ポータブルモードの制御
v0.1.2以降では、ビルド設定によるポータブルモード制御は廃止され、INIファイルによる実行時制御に変更されました。

#### 変更前（v0.1.1以前）
```xml
<!-- 条件付きコンパイルによる制御 -->
<PropertyGroup Condition="'$(Configuration)' == 'Portable'">
  <DefineConstants>$(DefineConstants);PORTABLE_MODE=true</DefineConstants>
</PropertyGroup>
```

#### 変更後（v0.1.2以降）
```ini
; BrowserChooser3.iniによる制御
[Paths]
UseExeDirectory=true  ; ポータブルモード
UseExeDirectory=false ; インストーラーモード
```

### ビルド設定の簡素化
- **Debug**: デバッグビルド
- **Release**: リリースビルド
- **Portable**: 削除（INIファイルで制御）
dotnet test

# カバレッジレポートの生成
dotnet test --collect:"XPlat Code Coverage"
```

## 🔍 ビルド設定

### プロジェクトファイルの構成
```xml
<!-- BrowserChooser3.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Version>0.1.7</Version>
    <AssemblyVersion>0.1.7.0</AssemblyVersion>
    <FileVersion>0.1.7.0</FileVersion>
  </PropertyGroup>
</Project>
```

### ビルド設定のカスタマイズ
```xml
<!-- デバッグ設定 -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
  <DefineConstants>DEBUG;TRACE</DefineConstants>
  <Optimize>false</Optimize>
</PropertyGroup>

<!-- リリース設定 -->
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|AnyCPU'">
  <DefineConstants>TRACE</DefineConstants>
  <Optimize>true</Optimize>
</PropertyGroup>
```

## 🧪 テスト環境

### テストプロジェクトの構成
```xml
<!-- BrowserChooser3.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.9.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="FluentAssertions" Version="8.6.0" />
    <PackageReference Include="Moq" Version="4.20.72" />
  </ItemGroup>
</Project>
```

### テストの実行方法
```cmd
# 全テストの実行
dotnet test

# 特定のテストクラスの実行
dotnet test --filter "FullyQualifiedName~LoggerTests"

# 並列実行の無効化
dotnet test --maxcpucount:1

# 詳細な出力
dotnet test --verbosity normal
```

## 📦 インストーラーのビルド

### Inno Setupのインストール
```cmd
# Inno Setup 6をダウンロード
# https://jrsoftware.org/isdl.php

# インストール確認
iscc /?
```

### インストーラーのビルド
```cmd
# インストーラースクリプトのビルド
iscc BrowserChooser3-Setup.iss

# または、バッチファイルを使用
build-inno-setup.bat
```

### インストーラーの設定
```pascal
; BrowserChooser3-Setup.iss
[Setup]
AppName=BrowserChooser3
AppVersion=0.1.0
DefaultDirName={autopf}\BrowserChooser3
DefaultGroupName=BrowserChooser3
OutputDir=dist
OutputBaseFilename=BrowserChooser3-Setup
```

## 🚀 デバッグと開発

### Visual Studioでのデバッグ
1. **ブレークポイントの設定**: コード行をクリックしてブレークポイントを設定
2. **デバッグの開始**: F5キーまたは「デバッグ開始」ボタン
3. **ステップ実行**: F10（ステップオーバー）、F11（ステップイン）
4. **変数の監視**: ウォッチウィンドウで変数を監視

### ログの確認
```cmd
# ログファイルの場所
Logs/BrowserChooser3.log

# ログレベルの設定
# Logger.csでCurrentLogLevelを変更
```

### パフォーマンス分析
```cmd
# プロファイリングの有効化
dotnet build --configuration Release
dotnet run --configuration Release
```

## 🔧 トラブルシューティング

### よくある問題

#### ビルドエラー
```cmd
# クリーンビルド
dotnet clean
dotnet restore
dotnet build

# キャッシュのクリア
dotnet nuget locals all --clear
```

#### テストエラー
```cmd
# テストプロジェクトの再ビルド
dotnet build BrowserChooser3.Tests
dotnet test --verbosity normal
```

#### 依存関係の問題
```cmd
# パッケージの更新
dotnet list package --outdated
dotnet add package [PackageName] --version [Version]
```

### デバッグのヒント

#### ログの活用
```csharp
// デバッグ情報の出力
Logger.Log(LogLevel.Debug, "デバッグ情報: {Value}", value);

// エラー情報の出力
Logger.Log(LogLevel.Error, "エラーが発生: {Exception}", ex);
```

#### 例外処理
```csharp
try
{
    // 処理
}
catch (Exception ex)
{
    Logger.Log(LogLevel.Error, "例外が発生: {Exception}", ex);
    throw;
}
```

## 📚 関連情報

- [貢献ガイドライン](Contributing-Guidelines)
- [APIリファレンス](API-Reference)
- [テスト](Testing)
- [トラブルシューティング](../Troubleshooting)

---

*開発に関する質問がある場合は、[GitHub Discussions](https://github.com/Yosuke-Sh/BrowserChooser3/discussions)でお気軽にお聞きください。*
