$appState = Get-IISAppPool -Name "DefaultAppPool" | Select-Object State

if ($appState.State -ne "Started") {
    throw "Application pool did not start properly."
}
