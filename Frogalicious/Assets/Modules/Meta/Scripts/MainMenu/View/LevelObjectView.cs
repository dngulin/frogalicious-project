using System;
using Frog.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Frog.Meta.MainMenu.View
{
    public class LevelObjectView : MonoBehaviour, IPointerDownHandler
    {
        [Header("Colors")]
        [SerializeField] private Color _locked;
        [SerializeField] private Color _unlocked;
        [SerializeField] private Color _completed;

        [Header("Sprite")]
        [SerializeField] private SpriteRenderer _background;

        private LevelObjectState _state;

        private Flag _clickedFlag;

        public void OnPointerDown(PointerEventData _)
        {
            if (_state != LevelObjectState.Locked)
            {
                _clickedFlag.SetAssertive();
            }
        }

        public bool PollClick() => _clickedFlag.TryReset();

        public void SetState( LevelObjectState state )
        {
            _state = state;
            UpdateBgColor();
        }

        private void UpdateBgColor()
        {
            _background.color = _state switch
            {
                LevelObjectState.Locked => _locked,
                LevelObjectState.Unlocked => _unlocked,
                LevelObjectState.Completed => _completed,
                _ => throw new ArgumentOutOfRangeException(nameof(_state), _state, null),
            };
        }
    }
}