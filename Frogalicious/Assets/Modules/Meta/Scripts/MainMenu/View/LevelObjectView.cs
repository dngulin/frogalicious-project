using Frog.Core;
using UnityEngine;

namespace Frog.Meta.MainMenu.View
{
    [RequireComponent(typeof(Collider2D))]
    public class LevelObjectView : MonoBehaviour
    {
        private Flag _clickedFlag;

        private void OnMouseDown() => _clickedFlag.SetAssertive();

        public bool PollClick() => _clickedFlag.Reset();
    }
}