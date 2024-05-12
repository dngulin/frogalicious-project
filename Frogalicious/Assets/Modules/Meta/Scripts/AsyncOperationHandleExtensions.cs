using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frog.Meta
{
    public static class AsyncOperationHandleExtensions
    {
        public static void ReleaseSafe<T>(this ref AsyncOperationHandle<T> handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
                handle = default;
            }
        }
    }
}