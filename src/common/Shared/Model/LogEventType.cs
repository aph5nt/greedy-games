namespace Shared.Model
{
    public enum LogEventType
    {
        GameLock = 0,
        ReleaseGameLock = 1,
        GameWin = 2,
        GameLose = 3,

        CreateAccount = 4,
        Deposit = 5,
        Withdraw = 6,
        Rollback = 7,

        Profit = 8,
        Dividend = 9,
        WithdrawLock = 10,
        ReleaseWithdrawLock = 11
    }
}