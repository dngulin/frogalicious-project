using Frog.Collections;

namespace Frog.Core.Save
{
    public abstract class Migration
    {
        public abstract string Name { get; }
        public abstract void Execute(ref RefList<byte> data);
    }
}