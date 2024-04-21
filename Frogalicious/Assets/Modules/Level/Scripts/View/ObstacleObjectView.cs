using UnityEngine;

namespace Frog.Level.View
{
    public class ObstacleObjectView : EntityView
    {
        [SerializeField] private SpriteRenderer _renderer;

        [Header("Connected state sprites")]
        [SerializeField] private Sprite _left;
        [SerializeField] private Sprite _center;
        [SerializeField] private Sprite _right;

        public ObstacleObjectView Initialized(bool lNeighbour, bool rNeighbour)
        {
            if (lNeighbour && rNeighbour)
            {
                _renderer.sprite = _center;
            }
            else if (lNeighbour)
            {
                _renderer.sprite = _right;
            }
            else if (rNeighbour)
            {
                _renderer.sprite = _left;
            }

            return this;
        }
    }
}