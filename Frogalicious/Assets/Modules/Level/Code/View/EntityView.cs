using System;
using UnityEngine;

namespace Frog.Level.View
{
    public abstract class EntityView : MonoBehaviour
    {
        public virtual void FlipFlop(bool state) => throw new NotSupportedException();

        public virtual void Disappear() => throw new NotSupportedException();
    }
}