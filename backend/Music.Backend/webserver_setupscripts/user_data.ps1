<powershell>
New-Item -Path "C:\temp" -ItemType "directory" -Force
New-Item -Path "C:\ProgramData\Amazon\CodeDeploy" -ItemType "directory" -Force
Add-MpPreference -ExclusionPath ("C:\ProgramData\Amazon\CodeDeploy","$env:windir\Temp")
Invoke-WebRequest -Uri "https://aws-codedeploy-ap-southeast-2.s3.amazonaws.com/latest/codedeploy-agent.msi" -OutFile "C:\temp\codedeploy-agent.msi"
C:\temp\codedeploy-agent.msi /quiet /l C:\temp\host-agent-install-log.txt
Start-Service -Name codedeployagent
</powershell>