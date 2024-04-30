using System;
using System.Collections.Generic;

namespace Frog.Collections
{
    [NoCopy]
    public struct RefList<T> where T : struct
    {
        internal T[] ItemArray;
        internal int ItemCount;

        internal RefList(T[] array, int itemCount)
        {
            ItemArray = array;
            ItemCount = itemCount;
        }
    }

    public static class RefList
    {
        [NoCopyInstanceConstructor]
        public static RefList<T> Empty<T>() where T : struct
        {
            return new RefList<T>(Array.Empty<T>(), 0);
        }

        [NoCopyInstanceConstructor]
        public static RefList<T> WithCapacity<T>(int capacity) where T : struct
        {
            return new RefList<T>(new T[capacity], 0);
        }

        [NoCopyInstanceConstructor]
        public static RefList<T> WithDefaultItems<T>(int count) where T : struct
        {
            return new RefList<T>(new T[count], count);
        }

        [NoCopyInstanceConstructor]
        public static RefList<T> Move<T>(ref RefList<T> other) where T : struct
        {
            var list = new RefList<T>(other.ItemArray, other.ItemCount);
            other = default;
            return list;
        }

        [NoCopyInstanceConstructor]
        public static RefList<T> Copy<T>(in RefList<T> other) where T : struct
        {
            if (other.ItemArray == null)
                return default;

            var items = new T[other.ItemArray.Length];
            Array.Copy(other.ItemArray, items, other.ItemCount);
            return new RefList<T>(other.ItemArray, other.ItemCount);
        }
    }

    public static class RefListImpl
    {
        public static bool IsValid<T>(this in RefList<T> list) where T : struct => list.ItemArray != null;


        public static int Capacity<T>(this in RefList<T> list) where T : struct => list.ItemArray.Length;

        public static int Count<T>(this in RefList<T> list) where T : struct => list.ItemCount;


        public static ref readonly T RefReadonlyAt<T>(this in RefList<T> list, int index) where T : struct => ref list.ItemArray[index];

        public static ref T RefAt<T>(this ref RefList<T> list, int index) where T : struct => ref list.ItemArray[index];


        public static void Add<T>(this ref RefList<T> list, in T item) where T : struct
        {
            list.EnsureCanAdd();
            list.ItemArray[list.ItemCount++] = item;
        }

        public static ref T RefAdd<T>(this ref RefList<T> list) where T : struct
        {
            list.EnsureCanAdd();
            return ref list.ItemArray[list.ItemCount++];
        }

        private static void EnsureCanAdd<T>(this ref RefList<T> list) where T : struct
        {
            if (list.ItemCount < list.ItemArray.Length)
                return;

            var newSize = Math.Max(list.ItemArray.Length * 2, 1);
            Array.Resize(ref list.ItemArray, newSize);
        }

        public static void RemoveAt<T>(this ref RefList<T> list, int index) where T : struct
        {
            if (index < 0 || index >= list.ItemCount)
            {
                throw new IndexOutOfRangeException();
            }

            list.ItemCount--;
            Array.Copy(list.ItemArray, index + 1, list.ItemArray, index, list.ItemCount - index);
            list.ItemArray[list.ItemCount] = default;
        }

        public static void Clear<T>(this ref RefList<T> list) where T : struct
        {
            Array.Clear(list.ItemArray, 0, list.ItemCount);
            list.ItemCount = 0;
        }

        public static void Sort<T>(this ref RefList<T> list) where T : struct
        {
            Array.Sort(list.ItemArray, 0, list.ItemCount);
        }

        public static void Sort<T>(this ref RefList<T> list, IComparer<T> comparer) where T : struct
        {
            Array.Sort(list.ItemArray, 0, list.ItemCount, comparer);
        }
    }
}