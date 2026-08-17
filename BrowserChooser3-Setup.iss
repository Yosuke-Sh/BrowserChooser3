[Setup]
AppName=Browser Chooser 3
AppVersion=0.2.1
AppPublisher=Your Company
AppPublisherURL=https://github.com/Yosuke-Sh/BrowserChooser3
AppSupportURL=https://github.com/Yosuke-Sh/BrowserChooser3
AppUpdatesURL=https://github.com/Yosuke-Sh/BrowserChooser3
DefaultDirName=C:\Program Files\BrowserChooser3
DefaultGroupName=Browser Chooser 3
DisableDirPage=yes
OutputDir=dist
OutputBaseFilename=BrowserChooser3-Setup
SetupIconFile=BrowserChooser3\Resources\BrowserChooser2.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\BrowserChooser3.exe
UninstallDisplayName=Browser Chooser 3

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[Files]
Source: "BrowserChooser3\bin\Release\net10.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb,*.map,*.ilk"

[Icons]
Name: "{group}\Browser Chooser 3"; Filename: "{app}\BrowserChooser3.exe"
Name: "{group}\{cm:UninstallProgram,Browser Chooser 3}"; Filename: "{uninstallexe}"

[Tasks]
Name: "set_default_browser"; Description: "BrowserChooser3を既定のブラウザとして設定する"; Flags: checkedonce
Name: "open_default_apps"; Description: "インストール後に既定のアプリ設定を開く"; Flags: unchecked

[Registry]
; HTTPプロトコルハンドラーの設定
Root: HKLM; Subkey: "SOFTWARE\Classes\http\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\http\shell\open\ddeexec"; ValueType: string; ValueName: ""; ValueData: ""; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\http\shell\open\ddeexec\Application"; ValueType: string; ValueName: ""; ValueData: "BrowserChooser3"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\http\shell\open\ddeexec\Topic"; ValueType: string; ValueName: ""; ValueData: "WWW_OpenURL"; Flags: uninsdeletekey; Tasks: set_default_browser

; HTTPSプロトコルハンドラーの設定
Root: HKLM; Subkey: "SOFTWARE\Classes\https\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\https\shell\open\ddeexec"; ValueType: string; ValueName: ""; ValueData: ""; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\https\shell\open\ddeexec\Application"; ValueType: string; ValueName: ""; ValueData: "BrowserChooser3"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\https\shell\open\ddeexec\Topic"; ValueType: string; ValueName: ""; ValueData: "WWW_OpenURL"; Flags: uninsdeletekey; Tasks: set_default_browser

; ファイル拡張子の関連付け
Root: HKLM; Subkey: "SOFTWARE\Classes\.htm\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\.html\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser

; アプリケーションの登録
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3"; ValueType: string; ValueName: "Version"; ValueData: "0.2.0"; Flags: uninsdeletekey

; Windowsの既定アプリ一覧に表示されるための登録
Root: HKLM; Subkey: "SOFTWARE\RegisteredApplications"; ValueType: string; ValueName: "BrowserChooser3"; ValueData: "SOFTWARE\BrowserChooser3\Capabilities"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "Browser Chooser 3 - 複数のブラウザから選択できるアプリケーション"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey; Tasks: set_default_browser

; プロトコル関連付けの登録
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3\Capabilities\URLAssociations"; ValueType: string; ValueName: "http"; ValueData: "BrowserChooser3.http"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3\Capabilities\URLAssociations"; ValueType: string; ValueName: "https"; ValueData: "BrowserChooser3.https"; Flags: uninsdeletekey; Tasks: set_default_browser

; ファイル関連付けの登録
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3\Capabilities\FileAssociations"; ValueType: string; ValueName: ".htm"; ValueData: "BrowserChooser3.htm"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3\Capabilities\FileAssociations"; ValueType: string; ValueName: ".html"; ValueData: "BrowserChooser3.html"; Flags: uninsdeletekey; Tasks: set_default_browser

; カスタムプロトコルクラスの登録
Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.http"; ValueType: string; ValueName: ""; ValueData: "BrowserChooser3 HTTP Protocol"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.http\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser

Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.https"; ValueType: string; ValueName: ""; ValueData: "BrowserChooser3 HTTPS Protocol"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.https\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser

Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.htm"; ValueType: string; ValueName: ""; ValueData: "BrowserChooser3 HTML File"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.htm\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser

Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.html"; ValueType: string; ValueName: ""; ValueData: "BrowserChooser3 HTML File"; Flags: uninsdeletekey; Tasks: set_default_browser
Root: HKLM; Subkey: "SOFTWARE\Classes\BrowserChooser3.html\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: set_default_browser

[Run]
Filename: "{app}\BrowserChooser3.exe"; Description: "{cm:LaunchProgram,Browser Chooser 3}"; Flags: nowait postinstall skipifsilent
Filename: "{cmd}"; Parameters: "/c start ms-settings:defaultapps"; Tasks: open_default_apps; Flags: postinstall skipifsilent nowait

[Code]
const
  DotNetRuntimeDownloadUrl = 'https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe';
  DotNetRuntimeManualDownloadPage = 'https://dotnet.microsoft.com/download/dotnet/10.0';

// {commonpf64}\dotnet\shared\Microsoft.WindowsDesktop.App 配下に "10." で始まるディレクトリがあるか確認する
function FindFirstDotNet10SharedFxDir(): Boolean;
var
  FindRec: TFindRec;
  BaseDir: string;
begin
  Result := False;
  BaseDir := ExpandConstant('{commonpf64}\dotnet\shared\Microsoft.WindowsDesktop.App');

  if FindFirst(BaseDir + '\10.*', FindRec) then
  begin
    try
      Result := True;
    finally
      FindClose(FindRec);
    end;
  end;
end;

// Microsoft.WindowsDesktop.App 10.x が導入済みかどうかを判定する。
// レジストリでの確認を主とし、フォルダー存在確認をフォールバックとして併用する。
function IsWindowsDesktopRuntime10Installed(): Boolean;
var
  Names: TArrayOfString;
  I: Integer;
begin
  Result := False;

  if RegGetSubkeyNames(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', Names) then
  begin
    for I := 0 to GetArrayLength(Names) - 1 do
    begin
      if (Length(Names[I]) > 0) and (Names[I][1] = '1') then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  if DirExists(ExpandConstant('{commonpf64}\dotnet\shared\Microsoft.WindowsDesktop.App')) then
  begin
    if FindFirstDotNet10SharedFxDir() then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

function OnDownloadProgress(const Url, FileName: string; const Progress, ProgressMax: Int64): Boolean;
begin
  Result := True;
end;

// .NET Desktop Runtime 10 をダウンロードし、サイレントインストールする。
// 失敗した場合は手動導入用のURLを提示してセットアップを中断する。
function EnsureDotNetDesktopRuntimeInstalled(): Boolean;
var
  ResultCode: Integer;
  DownloadOk: Boolean;
begin
  Result := True;

  if IsWindowsDesktopRuntime10Installed() then
    Exit;

  if not WizardSilent() then
  begin
    if MsgBox('Browser Chooser 3 の実行には .NET 10 Desktop Runtime が必要です。' + #13#10 +
              '未導入のため、ダウンロードしてインストールします。よろしいですか？',
              mbConfirmation, MB_YESNO) = IDNO then
    begin
      Result := False;
      Exit;
    end;
  end;

  DownloadOk := False;
  try
    DownloadTemporaryFile(DotNetRuntimeDownloadUrl, 'windowsdesktop-runtime-win-x64.exe', '', @OnDownloadProgress);
    DownloadOk := True;
  except
    DownloadOk := False;
  end;

  if not DownloadOk then
  begin
    MsgBox('.NET 10 Desktop Runtime のダウンロードに失敗しました。' + #13#10 +
           '以下のURLから手動でダウンロード・インストールしてから、再度セットアップを実行してください:' + #13#10 +
           DotNetRuntimeManualDownloadPage,
           mbError, MB_OK);
    Result := False;
    Exit;
  end;

  if not Exec(ExpandConstant('{tmp}\windowsdesktop-runtime-win-x64.exe'), '/install /quiet /norestart', '',
              SW_SHOW, ewWaitUntilTerminated, ResultCode) then
  begin
    MsgBox('.NET 10 Desktop Runtime のインストールに失敗しました。' + #13#10 +
           '以下のURLから手動でダウンロード・インストールしてから、再度セットアップを実行してください:' + #13#10 +
           DotNetRuntimeManualDownloadPage,
           mbError, MB_OK);
    Result := False;
    Exit;
  end;

  if not IsWindowsDesktopRuntime10Installed() then
  begin
    MsgBox('.NET 10 Desktop Runtime のインストールを完了できませんでした。' + #13#10 +
           '以下のURLから手動でダウンロード・インストールしてから、再度セットアップを実行してください:' + #13#10 +
           DotNetRuntimeManualDownloadPage,
           mbError, MB_OK);
    Result := False;
    Exit;
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): string;
begin
  Result := '';
  if not EnsureDotNetDesktopRuntimeInstalled() then
    Result := '.NET 10 Desktop Runtime が導入されなかったため、セットアップを中断しました。';
end;

