# define installer name
outFile "Installer.exe"
 
# set desktop as install directory
InstallDir $DESKTOP
 
# default section start
section

  MessageBox MB_YESNO "Install Microsoft XNA Framework Redistributable 4.0?" /SD IDYES IDNO endXNA
    File "xnafx40_redist.msi"
    ExecWait '"msiexec" /i "xnafx40_redist.msi"'
    Goto endXNA
  endXNA:
  MessageBox MB_YESNO "Install the Microsoft .NET Framework 4.0 Redistributable?" /SD IDYES IDNO endNetCF
    File "dotNetFx40_Full_setup.exe"
    ExecWait "dotNetFx40_Full_setup.exe"
  endNetCF:
   SetOutPath $INSTDIR\VikingSpel
  File /r "Bin"
sectionEnd