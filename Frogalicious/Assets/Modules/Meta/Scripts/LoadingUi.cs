using Frog.Core.Ui;
using UnityEngine;

namespace Frog.Meta
{
    public class LoadingUi : UiEntity
    {
        [SerializeField]
        private CanvasGroup _contentsRoot;

        [SerializeField]
        private AnimationCurve _alphaCurve;

        private float _time;
        private float _maxTime;

        public override CanvasGroup ContentsRoot => _contentsRoot;

        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
            _time = 0;
            _contentsRoot.alpha = 0;
        }

        private void Awake()
        {
            _maxTime = _alphaCurve.keys[_alphaCurve.length - 1].time;
        }

        private void Update()
        {
            if (_time >= _maxTime)
                return;

            _time = Mathf.Clamp(_time + Time.deltaTime, 0 , _maxTime);
            _contentsRoot.alpha = _alphaCurve.Evaluate(_time);
        }
    }
}