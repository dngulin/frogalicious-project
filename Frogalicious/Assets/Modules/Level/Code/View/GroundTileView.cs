using UnityEngine;

namespace Frog.Level.View
{
    public sealed class GroundTileView : EntityView
    {
        [SerializeField] private Sprite[] _sprites;

        [SerializeField] private SpriteRenderer _renderer;

        public void Init()
        {
            var spriteIdx = Random.Range(0, _sprites.Length);
            _renderer.sprite = _sprites[spriteIdx];

            var angle = Random.Range(0, 4) * 90;
            transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}