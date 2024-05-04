using Frog.Core.Ui;
using UnityEngine.EventSystems;

namespace Frog.Meta.Splash
{
    public class SplashUi : UiEntity, IPointerClickHandler
    {
        private bool _clicked;

        public override void SetVisible(bool visible) => gameObject.SetActive(visible);

        public override void SetInteractable(bool interactable)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _clicked = true;
        }

        public bool Poll() => _clicked;
    }
}