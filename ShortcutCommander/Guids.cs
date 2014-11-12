// Guids.cs
// MUST match guids.h
using System;

namespace Techmatic.ShortcutCommander
{
    static class GuidList
    {
        public const string guidShortcutCommanderPkgString = "db70011e-6e6c-492c-9f2c-469a12dbc067";
        public const string guidShortcutCommanderCmdSetString = "0891dea9-66f2-4b40-9209-cf49eabe0ae4";

        public static readonly Guid guidShortcutCommanderCmdSet = new Guid(guidShortcutCommanderCmdSetString);
    };
}