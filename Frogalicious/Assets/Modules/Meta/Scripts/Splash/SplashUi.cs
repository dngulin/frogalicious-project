using UnityEngine;
using UnityEngine.EventSystems;

namespace Frog.Meta.Splash
{
    public class SplashUi : MonoBehaviour, IPointerClickHandler
    {
        private bool _clicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            _clicked = true;
        }

        public bool Poll() => _clicked;

    }
}