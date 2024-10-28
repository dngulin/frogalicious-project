using System;

namespace Frog.ProtoPuff.Editor
{
    public struct CodeGenOptions
    {
        public string Namespace;
        public ApiTargets SerialisationApi;
        public ApiTargets DeserialisationApi;
    }

    [Flags]
    public enum ApiTargets
    {
        None = 0,
        RefList = 1 << 1,
        Stream = 1 << 2,
    }
}