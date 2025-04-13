using System.Threading;
using Frog.Core;
using Frog.Core.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Frog.Meta.Splash
{
    public class SplashScreenUi : UiEntity, IPointerClickHandler
    {
        [SerializeField]
        private TMP_Text _text;

        private readonly AwaitableOperation _waitForClick = new AwaitableOperation();

        private void OnDestroy()
        {
            _waitForClick.Dispose();
        }

        private void Awake()
        {
            Debug.Assert(!_text.gameObject.activeSelf);
        }

        public override void SetVisible(bool visible) => gameObject.SetActive(visible);

        public override void SetInteractable(bool interactable)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_waitForClick.IsRunning)
                _waitForClick.EndAssertive();
        }

        public Awaitable WaitForTap(CancellationToken ct)
        {
            _text.gameObject.SetActive(true);
            return _waitForClick.ExecuteAsync(ct);
        }
    }
}