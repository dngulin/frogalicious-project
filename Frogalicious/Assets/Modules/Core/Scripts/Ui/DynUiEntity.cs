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
}