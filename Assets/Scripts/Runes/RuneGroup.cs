using System;

namespace Runes
{
    [Flags]
    public enum RuneGroup : byte
    {
        None = 0,
        
        Alpha =   1 << 0,
        Beta =    1 << 1,
        Gamma =   1 << 2,
        Delta =   1 << 3,
        Epsilon = 1 << 4,
        Zeta =    1 << 5,
    }
}