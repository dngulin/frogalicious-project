using Frog.Core.Ui;
using UnityEngine;

namespace Frog.Meta
{
    public sealed class LoadingUi : UiEntity
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private AnimationCurve _alphaCurve;

        private float _time;
        private float _maxTime;

        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
            _time = 0;
            _canvasGroup.alpha = 0;
        }

        public override void SetInteractable(bool interactable) => _canvasGroup.interactable = interactable;

        private void Awake()
        {
            _maxTime = _alphaCurve.keys[_alphaCurve.length - 1].time;
        }

        private void Update()
        {
            if (_time >= _maxTime)
                return;

            _time = Mathf.Clamp(_time + Time.deltaTime, 0 , _maxTime);
            _canvasGroup.alpha = _alphaCurve.Evaluate(_time);
        }
    }
}