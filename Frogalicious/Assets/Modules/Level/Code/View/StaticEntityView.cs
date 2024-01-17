using UnityEngine;

namespace Frog.Level.View
{
    public sealed class StaticEntityView : EntityView
    {
        [SerializeField]
        private SpriteRenderer _renderer;

        public void Init(Sprite sprite)
        {
            _renderer.sprite = sprite;
        }
    }
}