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
            var uiPrefab = Addressables.LoadAssetAsync<GameObject>("MainMenuUi.prefab");

            await chapterCfg.Task;
            ct.ThrowIfCancellationRequested();

            await uiPrefab.Task;
            ct.ThrowIfCancellationRequested();

            return new MainMenuResources(chapterCfg, uiPrefab);
        }

        private MainMenuResources(
            AsyncOperationHandle<GameChapterConfig> chapterCfgOp,
            AsyncOperationHandle<GameObject> uiPrefab)
        {
            _chapterCfgOp = chapterCfgOp;
            _uiPrefab = uiPrefab;
        }

        private AsyncOperationHandle<GameChapterConfig> _chapterCfgOp;
        private AsyncOperationHandle<GameObject> _uiPrefab;

        public void Dispose()
        {
            _chapterCfgOp.ReleaseSafe();
            _uiPrefab.ReleaseSafe();
        }

        public readonly GameChapterConfig ChapterConfig => _chapterCfgOp.Result;
        public readonly MainMenuUi MenuPrefab => _uiPrefab.Result.GetComponent<MainMenuUi>();
    }
}