# define installer name
outFile "Installer.exe"
 
# set desktop as install directory
InstallDir $DESKTOP
 
# default section start
section
 SetOutPath $INSTDIR\Prerequisites
  MessageBox MB_YESNO "Install Microsoft XNA Framework Redistributable 4.0?" /SD IDYES IDNO endActiveSync
    File "http://download.microsoft.com/download/A/C/2/AC2C903B-E6E8-42C2-9FD7-BEBAC362A930/xnafx40_redist.msi"
    ExecWait '"msiexec" /i "http://download.microsoft.com/download/A/C/2/AC2C903B-E6E8-42C2-9FD7-BEBAC362A930/xnafx40_redist.msi"'
    Goto endActiveSync
  endActiveSync:
  MessageBox MB_YESNO "Install the Microsoft .NET Framework 4.0 Redistributable?" /SD IDYES IDNO endNetCF
    File "http://download.microsoft.com/download/1/B/E/1BE39E79-7E39-46A3-96FF-047F95396215/dotNetFx40_Full_setup.exe"
    ExecWait "http://download.microsoft.com/download/1/B/E/1BE39E79-7E39-46A3-96FF-047F95396215/dotNetFx40_Full_setup.exe"
  endNetCF:
sectionEnd