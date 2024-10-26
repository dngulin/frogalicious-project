using System;

namespace Frog.ProtoPuff
{
    public class ProtoPuffException : Exception
    {
        public ProtoPuffException(string msg) : base(msg) { }
    }
}