using System.Threading;
using Frog.Level.Data;
using Frog.Level.Ui;
using Frog.Level.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frog.Meta.Level
{
    public struct LevelResources
    {
        public static async Awaitable<LevelResources> Load(AssetReferenceT<LevelData> dataRef, CancellationToken ct)
        {
            var data = dataRef.LoadAssetAsync();
            var viewCfg = Addressables.LoadAssetAsync<LevelViewConfig>("Assets/Modules/Level/Config/LevelViewConfig.asset");
            var ui = Addressables.LoadAssetAsync<GameObject>("Assets/Modules/Level/Prefabs/Ui/LevelPanelUi.prefab");

            await data.Task;
            ct.ThrowIfCancellationRequested();

            await viewCfg.Task;
            ct.ThrowIfCancellationRequested();

            await ui.Task;
            ct.ThrowIfCancellationRequested();

            return new LevelResources(data, viewCfg, ui);
        }

        private LevelResources(
            AsyncOperationHandle<LevelData> dataOp,
            AsyncOperationHandle<LevelViewConfig> viewCfgOp,
            AsyncOperationHandle<GameObject> levelPanelUiOp)
        {
            _dataOp = dataOp;
            _viewCfgOp = viewCfgOp;
            _levelPanelUiOp = levelPanelUiOp;
        }

        private AsyncOperationHandle<LevelData> _dataOp;
        private AsyncOperationHandle<LevelViewConfig> _viewCfgOp;
        private AsyncOperationHandle<GameObject> _levelPanelUiOp;

        public void Release()
        {
            Addressables.Release(_dataOp);
            Addressables.Release(_viewCfgOp);
            Addressables.Release(_levelPanelUiOp);

            this = default;
        }

        public LevelData Data => _dataOp.Result;
        public LevelViewConfig ViewConfig => _viewCfgOp.Result;
        public LevelPanelUi PanelPrefab => _levelPanelUiOp.Result.GetComponent<LevelPanelUi>();
    }
}