[Setup]
AppName=Browser Chooser 3
AppVersion=0.1.2
AppPublisher=Your Company
AppPublisherURL=https://github.com/Yosuke-Sh/BrowserChooser3
AppSupportURL=https://github.com/Yosuke-Sh/BrowserChooser3
AppUpdatesURL=https://github.com/Yosuke-Sh/BrowserChooser3
DefaultDirName={autopf}\BrowserChooser3
DefaultGroupName=Browser Chooser 3
OutputDir=dist
OutputBaseFilename=BrowserChooser3-Setup
SetupIconFile=BrowserChooser3\Resources\BrowserChooser2.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\BrowserChooser3.exe
UninstallDisplayName=Browser Chooser 3

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[Files]
Source: "BrowserChooser3\bin\Release\net8.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Browser Chooser 3"; Filename: "{app}\BrowserChooser3.exe"
Name: "{group}\{cm:UninstallProgram,Browser Chooser 3}"; Filename: "{uninstallexe}"

[Registry]
; ブラウザアプリとして認識させるためのレジストリキー
Root: HKLM; Subkey: "SOFTWARE\Clients\StartMenuInternet\BrowserChooser3"; ValueType: string; ValueName: ""; ValueData: "Browser Chooser 3"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Clients\StartMenuInternet\BrowserChooser3\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\BrowserChooser3.exe,0"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Clients\StartMenuInternet\BrowserChooser3\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Clients\StartMenuInternet\BrowserChooser3\shell\open\command"; ValueType: string; ValueName: "DelegateExecute"; ValueData: ""; Flags: uninsdeletekey

; プロトコルハンドラーの登録
Root: HKCR; Subkey: "http"; ValueType: string; ValueName: ""; ValueData: "URL:HTTP Protocol"; Flags: uninsdeletekey
Root: HKCR; Subkey: "http\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey

Root: HKCR; Subkey: "https"; ValueType: string; ValueName: ""; ValueData: "URL:HTTPS Protocol"; Flags: uninsdeletekey
Root: HKCR; Subkey: "https\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey

; URLファイルの関連付け
Root: HKCR; Subkey: ".url"; ValueType: string; ValueName: ""; ValueData: "InternetShortcut"; Flags: uninsdeletekey
Root: HKCR; Subkey: "InternetShortcut\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\BrowserChooser3.exe"" ""%1"""; Flags: uninsdeletekey

[Run]
Filename: "{app}\BrowserChooser3.exe"; Description: "{cm:LaunchProgram,Browser Chooser 3}"; Flags: nowait postinstall skipifsilent

