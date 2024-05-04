using UnityEngine;

namespace Frog.Core.Ui
{
    /// <summary>
    /// Base UI Entity type that provides API to control visibility and interactability.
    /// Should be used as a base class for UI items without appearing and disappearing animations.
    /// </summary>
    public abstract class UiEntity : MonoBehaviour
    {
        public abstract void SetVisible(bool visible);
        public abstract void SetInteractable(bool interactable);
    }
}