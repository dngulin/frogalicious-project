using System.IO;

namespace Frog.Core.Save
{
    public abstract class Migration
    {
        public abstract string Name { get; }
        public abstract void Execute(Stream data);
    }
}