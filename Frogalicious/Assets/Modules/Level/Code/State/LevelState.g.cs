// This file is auto-generated by the PlainBuffers compiler
// Generated at 2024-01-18T18:47:10.1355670+01:00

// ReSharper disable All

using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

using Frog.Level.Primitives;

#pragma warning disable 649

namespace Frog.Level.State {
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CharacterState {
        public const int SizeOf = 8;
        public const int AlignmentOf = 2;
        private const int _Padding = 1;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public BoardPoint Position;
        [FieldOffset(4)] public ushort Coins;
        [FieldOffset(6), MarshalAs(UnmanagedType.U1)] public bool IsAlive;

        public void WriteDefault() {
            Position = default;
            Coins = 0;
            IsAlive = false;
            fixed (byte* __ptr = _buffer) {
                UnsafeUtility.MemClear(__ptr + (SizeOf - _Padding), _Padding);
            }
        }

        public static bool operator ==(in CharacterState l, in CharacterState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in CharacterState l, in CharacterState r) => !(l == r);

        public override bool Equals(object obj) => obj is CharacterState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ObjectState {
        public const int SizeOf = 8;
        public const int AlignmentOf = 4;
        private const int _Padding = 2;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public BoardObjectType Type;
        [FieldOffset(4)] public ushort Id;

        public void WriteDefault() {
            Type = BoardObjectType.Nothing;
            Id = 0;
            fixed (byte* __ptr = _buffer) {
                UnsafeUtility.MemClear(__ptr + (SizeOf - _Padding), _Padding);
            }
        }

        public static bool operator ==(in ObjectState l, in ObjectState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in ObjectState l, in ObjectState r) => !(l == r);

        public override bool Equals(object obj) => obj is ObjectState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ButtonState {
        public const int SizeOf = 1;
        public const int AlignmentOf = 1;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0), MarshalAs(UnmanagedType.U1)] public bool IsPressed;

        public void WriteDefault() {
            IsPressed = false;
        }

        public static bool operator ==(in ButtonState l, in ButtonState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in ButtonState l, in ButtonState r) => !(l == r);

        public override bool Equals(object obj) => obj is ButtonState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct SpikesState {
        public const int SizeOf = 1;
        public const int AlignmentOf = 1;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0), MarshalAs(UnmanagedType.U1)] public bool IsActive;

        public void WriteDefault() {
            IsActive = true;
        }

        public static bool operator ==(in SpikesState l, in SpikesState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in SpikesState l, in SpikesState r) => !(l == r);

        public override bool Equals(object obj) => obj is SpikesState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ExtendedTileState {
        public const int SizeOf = 1;
        public const int AlignmentOf = 1;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public ButtonState AsButton;
        [FieldOffset(0)] public SpikesState AsSpikes;

        public static bool operator ==(in ExtendedTileState l, in ExtendedTileState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in ExtendedTileState l, in ExtendedTileState r) => !(l == r);

        public override bool Equals(object obj) => obj is ExtendedTileState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct TileState {
        public const int SizeOf = 8;
        public const int AlignmentOf = 4;
        private const int _Padding = 1;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public BoardTileType Type;
        [FieldOffset(4)] public ushort Id;
        [FieldOffset(6)] public ExtendedTileState State;

        public void WriteDefault() {
            Type = BoardTileType.Nothing;
            Id = 0;
            State = default;
            fixed (byte* __ptr = _buffer) {
                UnsafeUtility.MemClear(__ptr + (SizeOf - _Padding), _Padding);
            }
        }

        public static bool operator ==(in TileState l, in TileState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in TileState l, in TileState r) => !(l == r);

        public override bool Equals(object obj) => obj is TileState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CellState {
        public const int SizeOf = 16;
        public const int AlignmentOf = 4;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public ObjectState Object;
        [FieldOffset(8)] public TileState Tile;

        public void WriteDefault() {
            Object.WriteDefault();
            Tile.WriteDefault();
        }

        public static bool operator ==(in CellState l, in CellState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in CellState l, in CellState r) => !(l == r);

        public override bool Equals(object obj) => obj is CellState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CellsArray {
        public const int SizeOf = 1120;
        public const int AlignmentOf = 4;
        public const int Length = 70;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];


        public void WriteDefault() {
            fixed (byte* ptr = _buffer) {
                for (var i = 0; i < Length; i++) {
                    (*((CellState*)ptr + i)).WriteDefault();
                }
            }
        }

        public static bool operator ==(in CellsArray l, in CellsArray r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in CellsArray l, in CellsArray r) => !(l == r);

        public override bool Equals(object obj) => obj is CellsArray casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();

        public unsafe readonly ref struct RefIterator {
            private readonly CellsArray* _ptr;
            public RefIterator(ref CellsArray array) {
                fixed (CellsArray* ptr = &array) _ptr = ptr;
            }
            public RefEnumerator GetEnumerator() => new RefEnumerator(_ptr);
        }

        public unsafe readonly ref struct RefReadonlyIterator {
            private readonly CellsArray* _ptr;
            public RefReadonlyIterator(in CellsArray array) {
                fixed (CellsArray* ptr = &array) _ptr = ptr;
            }
            public RefReadonlyEnumerator GetEnumerator() => new RefReadonlyEnumerator(_ptr);
        }

        public unsafe ref struct RefEnumerator {
            private readonly CellsArray* _ptr;
            private int _index;
            public RefEnumerator(CellsArray* ptr) {
                _ptr = ptr;
                _index = -1;
            }
            public ref CellState Current => ref *((CellState*)_ptr + _index);
            public bool MoveNext() => ++_index < CellsArray.Length;
            public void Reset() => _index = -1;
            public void Dispose() {}
        }

        public unsafe ref struct RefReadonlyEnumerator {
            private readonly CellsArray* _ptr;
            private int _index;
            public RefReadonlyEnumerator(CellsArray* ptr) {
                _ptr = ptr;
                _index = -1;
            }
            public ref readonly CellState Current => ref *((CellState*)_ptr + _index);
            public bool MoveNext() => ++_index < CellsArray.Length;
            public void Reset() => _index = -1;
            public void Dispose() {}
        }
    }

    public static unsafe class _CellsArray_IndexExtensions {
        public static ref CellState RefAt(this ref CellsArray array, int index) {
            if (index < 0 || sizeof(CellState) * index >= CellsArray.SizeOf) throw new IndexOutOfRangeException();
            fixed (CellsArray* ptr = &array) {
                return ref *((CellState*)ptr + index);
            }
        }
        public static CellsArray.RefIterator RefIter(this ref CellsArray array) => new CellsArray.RefIterator(ref array);

        public static ref readonly CellState RefReadonlyAt(this in CellsArray array, int index) {
            if (index < 0 || sizeof(CellState) * index >= CellsArray.SizeOf) throw new IndexOutOfRangeException();
            fixed (CellsArray* ptr = &array) {
                return ref *((CellState*)ptr + index);
            }
        }
        public static CellsArray.RefReadonlyIterator RefReadonlyIter(this in CellsArray array) => new CellsArray.RefReadonlyIterator(in array);
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CellsState {
        public const int SizeOf = 1124;
        public const int AlignmentOf = 4;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public CellsArray Array;
        [FieldOffset(1120)] public ushort Width;
        [FieldOffset(1122)] public ushort Height;

        public void WriteDefault() {
            Array.WriteDefault();
            Width = 0;
            Height = 0;
        }

        public static bool operator ==(in CellsState l, in CellsState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in CellsState l, in CellsState r) => !(l == r);

        public override bool Equals(object obj) => obj is CellsState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct LevelState {
        public const int SizeOf = 1132;
        public const int AlignmentOf = 4;

        [FieldOffset(0)] private fixed byte _buffer[SizeOf];

        [FieldOffset(0)] public CellsState Cells;
        [FieldOffset(1124)] public CharacterState Character;

        public void WriteDefault() {
            Cells.WriteDefault();
            Character.WriteDefault();
        }

        public static bool operator ==(in LevelState l, in LevelState r) {
            fixed (byte* __l = l._buffer, __r = r._buffer) {
                return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;
            }
        }
        public static bool operator !=(in LevelState l, in LevelState r) => !(l == r);

        public override bool Equals(object obj) => obj is LevelState casted && this == casted;
        public override int GetHashCode() => throw new NotSupportedException();
    }
}
