$appState = Get-IISAppPool -Name "YourApplicationPoolName" | Select-Object State

if ($appState.State -ne "Started") {
    throw "Application pool did not start properly."
}
