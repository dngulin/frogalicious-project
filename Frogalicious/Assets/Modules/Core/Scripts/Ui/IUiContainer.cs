using UnityEngine;

namespace Frog.Core.Ui
{
    /// <summary>
    /// Base interface for UI containers that allows to attach and detach content.
    /// </summary>
    public interface IUiContainer
    {
        public void AttachContent(Transform content);
        public Transform DetachContent(Transform contentParent);
    }
}