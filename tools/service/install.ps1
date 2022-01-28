if (Get-Service "GreedyQuartzServer" -ErrorAction SilentlyContinue)
{
    sc.exe stop GreedyQuartzServer
    sc.exe delete GreedyQuartzServer
    Start-Sleep -s 30
}

Copy-Item b:\deployment\scheduler\*.* c:\scheduler -recurse
Set-Location c:\scheduler

.\GreedyGames.Scheduler install

Start-Service -Name "GreedyQuartzServer"