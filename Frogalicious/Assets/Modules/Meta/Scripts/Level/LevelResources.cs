using System;
using System.Threading;
using Frog.Level.Data;
using Frog.Level.Ui;
using Frog.Level.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frog.Meta.Level
{
    public struct LevelResources : IDisposable
    {
        public static async Awaitable<LevelResources> Load(AssetReferenceT<LevelData> dataRef, CancellationToken ct)
        {
            var data = dataRef.LoadAssetAsync();
            var viewCfg = Addressables.LoadAssetAsync<LevelViewConfig>("Assets/Modules/Level/Config/LevelViewConfig.asset");
            var panelPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Modules/Level/Prefabs/Ui/LevelPanelUi.prefab");
            var exitDialogPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Modules/Level/Prefabs/Ui/LevelExitDialog.prefab");

            await data.Task;
            ct.ThrowIfCancellationRequested();

            await viewCfg.Task;
            ct.ThrowIfCancellationRequested();

            await panelPrefab.Task;
            ct.ThrowIfCancellationRequested();

            await exitDialogPrefab.Task;
            ct.ThrowIfCancellationRequested();

            return new LevelResources(data, viewCfg, panelPrefab, exitDialogPrefab);
        }

        private LevelResources(
            AsyncOperationHandle<LevelData> dataOp,
            AsyncOperationHandle<LevelViewConfig> viewCfgOp,
            AsyncOperationHandle<GameObject> panelPrefabOp,
            AsyncOperationHandle<GameObject> exitDialogPrefabOp)
        {
            Debug.Assert(dataOp.IsValidAndDone());
            Debug.Assert(viewCfgOp.IsValidAndDone());
            Debug.Assert(panelPrefabOp.IsValidAndDone());
            Debug.Assert(exitDialogPrefabOp.IsValidAndDone());

            _dataOp = dataOp;
            _viewCfgOp = viewCfgOp;
            _panelPrefabOp = panelPrefabOp;
            _exitDialogPrefabOp = exitDialogPrefabOp;
        }

        private AsyncOperationHandle<LevelData> _dataOp;
        private AsyncOperationHandle<LevelViewConfig> _viewCfgOp;
        private AsyncOperationHandle<GameObject> _panelPrefabOp;
        private AsyncOperationHandle<GameObject> _exitDialogPrefabOp;

        public void Dispose()
        {
            _dataOp.ReleaseSafe();
            _viewCfgOp.ReleaseSafe();
            _panelPrefabOp.ReleaseSafe();
            _exitDialogPrefabOp.ReleaseSafe();
        }

        public readonly LevelData Data => _dataOp.Result;
        public readonly LevelViewConfig ViewConfig => _viewCfgOp.Result;
        public readonly LevelPanelUi PanelPrefab => _panelPrefabOp.Result.GetComponent<LevelPanelUi>();
        public readonly LevelExitDialog ExitDialogPrefab => _exitDialogPrefabOp.Result.GetComponent<LevelExitDialog>();
    }
}