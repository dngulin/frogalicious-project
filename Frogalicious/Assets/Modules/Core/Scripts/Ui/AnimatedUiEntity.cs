using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public enum AnimatedUiEntityState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
    }

    public abstract class AnimatedUiEntity : UiEntity
    {
        public abstract Awaitable Show(CancellationToken ct);
        public abstract Awaitable Hide(CancellationToken ct);
        public abstract AnimatedUiEntityState State { get; }
    }
}