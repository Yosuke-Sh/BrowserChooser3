[Setup]
AppName=Browser Chooser 3
AppVersion=0.1.0
AppPublisher=Your Company
AppPublisherURL=https://github.com/Yosuke-Sh/BrowserChooser3
AppSupportURL=https://github.com/Yosuke-Sh/BrowserChooser3
AppUpdatesURL=https://github.com/Yosuke-Sh/BrowserChooser3
DefaultDirName={autopf}\BrowserChooser3
DefaultGroupName=Browser Chooser 3
OutputDir=dist
OutputBaseFilename=BrowserChooser3-Setup
SetupIconFile=BrowserChooser3\Resources\BrowserChooser3.ico
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

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 10.0; Check: not IsAdminInstallMode
Name: "pintotaskbar"; Description: "タスクバーにピン留めする"; GroupDescription: "追加オプション"; Flags: unchecked
Name: "addtostartmenu"; Description: "スタートメニューに追加する"; GroupDescription: "追加オプション"; Flags: unchecked
Name: "setasdefaultbrowser"; Description: "規定のブラウザとして登録する"; GroupDescription: "追加オプション"; Flags: unchecked

[Files]
Source: "BrowserChooser3\bin\Release\net8.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Browser Chooser 3"; Filename: "{app}\BrowserChooser3.exe"
Name: "{group}\{cm:UninstallProgram,Browser Chooser 3}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Browser Chooser 3"; Filename: "{app}\BrowserChooser3.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Browser Chooser 3"; Filename: "{app}\BrowserChooser3.exe"; Tasks: quicklaunchicon

[Registry]
; Windows 11 Browser App Registration
Root: HKCU; Subkey: "Software\RegisteredApplications"; ValueType: string; ValueName: "BrowserChooser3"; ValueData: "Software\BrowserChooser3\Capabilities"; Flags: uninsdeletevalue

Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "Multi-browser selector for Windows"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "Browser Chooser 3"; Flags: uninsdeletekey

; URL Associations
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\URLAssociations"; ValueType: string; ValueName: "http"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\URLAssociations"; ValueType: string; ValueName: "https"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\URLAssociations"; ValueType: string; ValueName: "ftp"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\URLAssociations"; ValueType: string; ValueName: "ftps"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey

; File Associations
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\FileAssociations"; ValueType: string; ValueName: ".html"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\FileAssociations"; ValueType: string; ValueName: ".htm"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\FileAssociations"; ValueType: string; ValueName: ".xhtml"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\BrowserChooser3\Capabilities\FileAssociations"; ValueType: string; ValueName: ".mhtml"; ValueData: "BrowserChooser3"; Flags: uninsdeletekey

; Protocol Handlers
Root: HKCR; Subkey: "BrowserChooser3"; ValueType: string; ValueData: "Browser Chooser 3"; Flags: uninsdeletekey
Root: HKCR; Subkey: "BrowserChooser3"; ValueType: string; ValueName: "FriendlyTypeName"; ValueData: "Browser Chooser 3"; Flags: uninsdeletekey
Root: HKCR; Subkey: "BrowserChooser3"; ValueType: string; ValueName: "URL Protocol"; ValueData: ""; Flags: uninsdeletekey
Root: HKCR; Subkey: "BrowserChooser3\DefaultIcon"; ValueType: string; ValueData: "{app}\BrowserChooser3.exe,0"; Flags: uninsdeletekey
Root: HKCR; Subkey: "BrowserChooser3\shell\open\command"; ValueType: string; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey

; HTTP Protocol Handler
Root: HKCR; Subkey: "http"; ValueType: string; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCR; Subkey: "http"; ValueType: string; ValueName: "URL Protocol"; ValueData: ""; Flags: uninsdeletekey
Root: HKCR; Subkey: "http\shell\open\command"; ValueType: string; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey

; HTTPS Protocol Handler
Root: HKCR; Subkey: "https"; ValueType: string; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCR; Subkey: "https"; ValueType: string; ValueName: "URL Protocol"; ValueData: ""; Flags: uninsdeletekey
Root: HKCR; Subkey: "https\shell\open\command"; ValueType: string; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey

; FTP Protocol Handler
Root: HKCR; Subkey: "ftp"; ValueType: string; ValueData: "BrowserChooser3"; Flags: uninsdeletekey
Root: HKCR; Subkey: "ftp"; ValueType: string; ValueName: "URL Protocol"; ValueData: ""; Flags: uninsdeletekey
Root: HKCR; Subkey: "ftp\shell\open\command"; ValueType: string; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey

[Run]
Filename: "{app}\BrowserChooser3.exe"; Description: "{cm:LaunchProgram,Browser Chooser 3}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    // タスクバーへのピン留め
    if WizardIsTaskSelected('pintotaskbar') then
    begin
      try
        Exec('powershell.exe', '-Command "& { $shell = New-Object -ComObject Shell.Application; $folder = $shell.Namespace(''shell:AppsFolder''); $item = $folder.ParseName(''BrowserChooser3.exe''); if ($item) { $item.InvokeVerb(''pintotaskbar'') } }"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      except
        // エラーが発生した場合は無視
      end;
    end;
    
    // スタートメニューへの追加
    if WizardIsTaskSelected('addtostartmenu') then
    begin
      try
        // スタートメニューにショートカットを作成
        CreateDir(ExpandConstant('{userstartmenu}\Browser Chooser 3'));
        CreateShellLink(ExpandConstant('{userstartmenu}\Browser Chooser 3\Browser Chooser 3.lnk'), 
          'Browser Chooser 3', 
          ExpandConstant('{app}\BrowserChooser3.exe'), 
          '', 
          ExpandConstant('{app}'), 
          '', 
          0, 
          SW_SHOWNORMAL);
      except
        // エラーが発生した場合は無視
      end;
    end;
    
    // 規定のブラウザとして登録
    if WizardIsTaskSelected('setasdefaultbrowser') then
    begin
      try
        // HTTPとHTTPSのプロトコルハンドラーを規定に設定
        Exec('reg.exe', 'add "HKCU\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice" /v "Progid" /t REG_SZ /d "BrowserChooser3" /f', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
        Exec('reg.exe', 'add "HKCU\Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice" /v "Progid" /t REG_SZ /d "BrowserChooser3" /f', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
        
        // ブラウザ選択ダイアログを表示
        Exec('rundll32.exe', 'shell32.dll,OpenAs_RunDLL', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
      except
        // エラーが発生した場合は無視
      end;
    end;
  end;
end;
