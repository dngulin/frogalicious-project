using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    /// <summary>
    /// Base class for UI entities that have appearing and disappearing animations.
    /// </summary>
    public abstract class DynUiEntity : UiEntity
    {
        public abstract Awaitable Show(CancellationToken ct);
        public abstract Awaitable Hide(CancellationToken ct);
        public abstract DynUiEntityState State { get; }
    }

    public enum DynUiEntityState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
    }

    /// <summary>
    /// Extension of DynUiEntity that also provides IContainer interface.
    /// Should be used for simple containers with appearing and disappearing animations.
    /// </summary>
    public abstract class DynUiContainer : DynUiEntity, IUiContainer
    {
        public abstract void AttachContent(Transform content);
        public abstract Transform DetachContent(Transform contentParent);
    }
}