namespace Frog.Core.Save
{
    public abstract class Migration
    {
        public abstract string Name { get; }
        public abstract string Execute(string data);
    }
}