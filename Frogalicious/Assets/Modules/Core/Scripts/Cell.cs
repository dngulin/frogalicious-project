using System;
using UnityEngine;

namespace Frog.Core
{
    public struct Cell<T>
    {
        private bool _hasValue;
        private T _value;

        public bool IsEmpty => !_hasValue;

        public void Push(T value)
        {
            if (_hasValue)
                throw new InvalidOperationException();

            _hasValue = true;
            _value = value;
        }

        public T Peek()
        {
            if (!_hasValue)
                throw new InvalidOperationException();

            return _value;
        }

        public T Pop()
        {
            if (!_hasValue)
                throw new InvalidOperationException();

            var value = _value;

            _hasValue = false;
            _value = default;

            return value;
        }

        public bool TryPop(out T value)
        {
            if (_hasValue)
            {
                value = _value;
                _value = default;
                _hasValue = false;
                return true;
            }

            value = default;
            return false;
        }
    }

    public static class CellExtensions
    {
        public static void PushOrReplaceWithAssertion<T>(ref this Cell<T> cell, T value)
        {
            if (!cell.IsEmpty)
            {
                var oldValue = cell.Pop();
                Debug.LogAssertion($"Replacing old cell value `{oldValue}` with `{value}`");
            }

            cell.Push(value);
        }

        public static T? PopNullable<T>(ref this Cell<T> cell) where T : struct
        {
            if (cell.IsEmpty)
                return null;

            return cell.Pop();
        }
    }
}