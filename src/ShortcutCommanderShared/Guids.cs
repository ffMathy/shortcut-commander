// Guids.cs
// MUST match guids.h
using System;

namespace ShortcutCommander
{
    static class GuidList
    {
        public const string guidShortcutCommanderPkgString = "dc0cfafa-5021-458c-85a8-bd9e9c7e6096";
        public const string guidShortcutCommanderCmdSetString = "279c207e-af5d-4e4e-a8c6-9a0bda220a9a";

        public static readonly Guid guidShortcutCommanderCmdSet = new Guid(guidShortcutCommanderCmdSetString);
    };
}