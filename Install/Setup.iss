; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppVersion "1.0"
#define MyAppName "视频监控"
#define MyAppExeName "视频监控.exe"
#define MySetName "系统配置"
#define MySetExeName "系统配置.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
;AppId={{FB291FE3-B1F3-4DF8-A73F-7DAD6E959120}}
AppId={#MyAppName}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={pf32}\{#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=setup
SetupIconFile=.\IPCameraMonitor\monitor.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkablealone
Name: "startupicon"; Description: "开机启动"; GroupDescription: "{cm:AdditionalIcons}"; 

[Files]                                              
Source: ".\IPCameraMonitor\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\monitor.ico";Tasks: desktopicon
Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"  

[run]  
;[Run] 段是可选的，用来指定程序完成安装后、在安装程序显示最终对话框之前要执行的程序  
;安装完成后运行仿真的loader.exe程序  
Filename: "{app}\IPCameraOCXSetup.exe"; Description: "{cm:LaunchProgram,IPCameraOCXSetup.exe}"; Flags: nowait postinstall skipifsilent ;WorkingDir: "{app}"  