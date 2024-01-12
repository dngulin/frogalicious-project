using Frog.Level.Primitives;
using UnityEngine;

namespace Frog.Level.View
{
    public readonly struct MoveJob
    {
        private readonly float _startTime;
        public readonly BoardPoint StartPos;
        public readonly BoardPoint EndPos;
        public readonly Transform Target;

        public MoveJob(float startTime, BoardPoint startPos, BoardPoint endPos, Transform target)
        {
            _startTime = startTime;
            StartPos = startPos;
            EndPos = endPos;
            Target = target;
        }

        public bool Update(float time, float duration)
        {
            if (time < _startTime)
                return false;

            var endTime = _startTime + duration;
            var factor = Mathf.Clamp01((time - _startTime) / duration);

            Target.position = Vector2.Lerp(StartPos.ToVector2(), EndPos.ToVector2(), factor);

            return time >= endTime;
        }
    }
}