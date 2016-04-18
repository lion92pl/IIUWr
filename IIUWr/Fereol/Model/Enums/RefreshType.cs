using System;

namespace IIUWr.Fereol.Model.Enums
{
    [Flags]
    public enum RefreshType : int
    {
        Failed =        1 << 0,
        Basic =         1 << 1,
        Full =          1 << 2 | Basic,
        LoggedIn =      1 << 3,
        LoggedInBasic = LoggedIn | Basic,
        LoggedInFull =  LoggedIn | Full
    }
}