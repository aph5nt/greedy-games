// Add-AzureAccount
// Login-AzureRmAccount
// Get-AzureRmADApplication

$x509 = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$x509.Import("C:\Workplace\camdora software\GreedyGames\tools\certs\greedygamesdev.cer")
$credValue = [System.Convert]::ToBase64String($x509.GetRawCertData())

# If you used different dates for makecert then adjust these values
$now = [System.DateTime]::Now
$yearfromnow = $now.AddYears(1)

// create AD app with certificate
$adapp = New-AzureRmADApplication -DisplayName "GreedyGamesDevCert" -HomePage "http://GreedyGamesDevCert" -IdentifierUris "http://GreedyGamesDevCert" -CertValue $credValue -StartDate $now -EndDate $yearfromnow
$sp = New-AzureRmADServicePrincipal -ApplicationId $adapp.ApplicationId

// manually assign app to keyvault
//Set-AzureRmKeyVaultAccessPolicy -VaultName 'greedygamesdev' -ServicePrincipalName $sp.ServicePrincipalName -PermissionsToSecrets all -ResourceGroupName 'greedygames'

# get the thumbprint to use in your app settings
$x509.Thumbprint