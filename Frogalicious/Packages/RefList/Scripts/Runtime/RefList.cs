using System;

namespace Frog.RefList
{
    public struct RefList<T> where T : struct
    {
        internal T[] ItemArray;
        internal int ItemCount;

        private RefList(T[] array, int itemCount)
        {
            ItemArray = array;
            ItemCount = itemCount;
        }

        public static RefList<T> CreateEmpty() => new RefList<T>(Array.Empty<T>(), 0);
        public static RefList<T> CreateWithCapacity(int capacity) => new RefList<T>(new T[capacity], 0);
        public static RefList<T> CreateWithDefaultItems(int count) => new RefList<T>(new T[count], count);
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
    }
}