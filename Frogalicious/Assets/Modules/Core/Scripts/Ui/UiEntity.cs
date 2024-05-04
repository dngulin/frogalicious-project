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

    /// <summary>
    /// Extension of UiEntity that also provides IContainer interface.
    /// Should be used for simple containers without appearing and disappearing animations.
    /// </summary>
    public abstract class UiContainer : UiEntity, IUiContainer
    {
        public abstract void AttachContent(Transform content);
        public abstract Transform DetachContent(Transform contentParent);
    }
}