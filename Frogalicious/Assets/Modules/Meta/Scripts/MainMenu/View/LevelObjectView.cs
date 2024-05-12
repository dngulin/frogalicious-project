using Frog.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Frog.Meta.MainMenu.View
{
    public class LevelObjectView : MonoBehaviour, IPointerDownHandler
    {
        private Flag _clickedFlag;

        public void OnPointerDown(PointerEventData _) => _clickedFlag.SetAssertive();

        public bool PollClick() => _clickedFlag.Reset();
    }
}