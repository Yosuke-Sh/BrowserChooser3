[Setup]
AppName=Browser Chooser 3
AppVersion=0.1.3
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
Source: "BrowserChooser3\bin\Release\net8.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

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
Root: HKLM; Subkey: "SOFTWARE\BrowserChooser3"; ValueType: string; ValueName: "Version"; ValueData: "0.1.3"; Flags: uninsdeletekey

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

