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

[Run]
Filename: "{app}\BrowserChooser3.exe"; Description: "{cm:LaunchProgram,Browser Chooser 3}"; Flags: nowait postinstall skipifsilent

