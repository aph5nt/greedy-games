$path = "C:\Workplace\camdora software\GreedyGames\src\QBitNinja.Server\bin\Debug"        
$args = " --CancelInit --Web --Port 85"
$list = get-process | where-object {$_.Path -eq $path }     

$proc = Get-Process "QBitNinja.Server"  | Select *
Write-Output $proc


Get-WmiObject Win32_Process -Filter "name = 'perl.exe'" | where {$_.CommandLine -eq '"C:\strawberry\perl\bin\perl.exe" t/Server_PreFork.t'} | ForEach-Object { Invoke-WmiMethod -Path $_.__Path –Name Terminate }