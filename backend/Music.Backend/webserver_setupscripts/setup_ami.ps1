function InstallDotnetIfNotInstalled {
    dotnet

    if (! $?) {
        mkdir C:\dotnet-installer
        Invoke-WebRequest "https://builds.dotnet.microsoft.com/dotnet/scripts/v1/dotnet-install.ps1" -OutFile "C:\dotnet-installer\dotnet-install.ps1"
        C:\dotnet-installer\dotnet-install.ps1 -Channel 9.0 -Runtime aspnetcore
    }
}

function InstallIISIfNotInstalled {
    if (((Get-WindowsFeature Web-Server).InstallState -ne "Installed")) {
        Install-WindowsFeature Web-Server
    }
}

function InstallDotnetCoreHostingBundleIfNotInstalled {
    if (!((Get-WindowsFeature IIS-ASPNET45).InstallState -eq "Installed")) {
        $aspHostingUrl = "https://download.visualstudio.microsoft.com/download/pr/450a6e4e-e4e3-4ed6-86a2-6a6f840e5a51/3629f0822ccc2ce265cf5e88b5b567cb/dotnet-hosting-9.0.1-win.exe"
        $bundlePathName = "C:\temp" + [System.IO.Path]::GetFileName( $aspHostingUrl )
        
        Invoke-WebRequest -Uri $aspHostingUrl -OutFile $bundlePathName
        
        Start-Process -Wait -FilePath $bundlePathName -ArgumentList "/S" -PassThru
    }
}

InstallDotnetIfNotInstalled
InstallIISIfNotInstalled
InstallDotnetCoreHostingBundleIfNotInstalled
