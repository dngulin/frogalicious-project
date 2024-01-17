using UnityEngine;

namespace Frog.Level.View
{
    public sealed class GroundTileView : EntityView
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void Init(Sprite[] sprites)
        {
            var spriteIdx = Random.Range(0, sprites.Length);
            _renderer.sprite = sprites[spriteIdx];

            var angle = Random.Range(0, 4) * 90;
            transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}