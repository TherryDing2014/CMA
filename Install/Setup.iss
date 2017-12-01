; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppVersion "1.0"
#define MyAppName "��Ƶ���"
#define MyAppExeName "��Ƶ���.exe"
#define MySetName "ϵͳ����"
#define MySetExeName "ϵͳ����.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
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
Name: "startupicon"; Description: "��������"; GroupDescription: "{cm:AdditionalIcons}"; 

[Files]                                              
Source: ".\IPCameraMonitor\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\monitor.ico";Tasks: desktopicon
Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"  

[run]  
;[Run] ���ǿ�ѡ�ģ�����ָ��������ɰ�װ���ڰ�װ������ʾ���նԻ���֮ǰҪִ�еĳ���  
;��װ��ɺ����з����loader.exe����  
Filename: "{app}\IPCameraOCXSetup.exe"; Description: "{cm:LaunchProgram,IPCameraOCXSetup.exe}"; Flags: nowait postinstall skipifsilent ;WorkingDir: "{app}"  