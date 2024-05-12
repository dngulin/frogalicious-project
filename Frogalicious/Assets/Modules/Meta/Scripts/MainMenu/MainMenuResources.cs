using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frog.Meta.MainMenu
{
    public struct MainMenuResources
    {
        public static async Awaitable<MainMenuResources> Load(CancellationToken ct)
        {
            var chapterCfg = Addressables.LoadAssetAsync<GameChapterConfig>("TutorialChapter.asset");
            var menuPrefab = Addressables.LoadAssetAsync<GameObject>("MainMenuUi.prefab");

            await chapterCfg.Task;
            ct.ThrowIfCancellationRequested();

            await menuPrefab.Task;
            ct.ThrowIfCancellationRequested();

            return new MainMenuResources(chapterCfg, menuPrefab);
        }

        private MainMenuResources(
            AsyncOperationHandle<GameChapterConfig> chapterCfgOp,
            AsyncOperationHandle<GameObject> menuPrefab)
        {
            Debug.Assert(chapterCfgOp.IsValidAndDone());
            Debug.Assert(menuPrefab.IsValidAndDone());

            _chapterCfgOp = chapterCfgOp;
            _menuPrefab = menuPrefab;
        }

        private AsyncOperationHandle<GameChapterConfig> _chapterCfgOp;
        private AsyncOperationHandle<GameObject> _menuPrefab;

        public void Dispose()
        {
            _chapterCfgOp.ReleaseSafe();
            _menuPrefab.ReleaseSafe();
        }

        public readonly GameChapterConfig ChapterConfig => _chapterCfgOp.Result;
        public readonly MainMenuUi MenuPrefab => _menuPrefab.Result.GetComponent<MainMenuUi>();
    }
}