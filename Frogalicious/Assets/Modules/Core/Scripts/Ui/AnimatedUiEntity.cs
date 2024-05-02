using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public enum AnimatedUiEntityState
    {
        Appearing,
        Visible,
        Disappearing,
        Hidden,
    }

    public abstract class AnimatedUiEntity : UiEntity
    {
        public abstract Awaitable Show(CancellationToken ct);
        public abstract Awaitable Hide(CancellationToken ct);
        public abstract AnimatedUiEntityState State { get; }
    }
}