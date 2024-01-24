namespace Frog.Level.View
{
    public sealed class StaticEntityView : EntityView
    {
        public override void Disappear() => gameObject.SetActive(false);
    }
}