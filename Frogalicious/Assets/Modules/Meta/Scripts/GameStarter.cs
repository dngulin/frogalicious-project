using System;
using Frog.Core.Ui;
using Frog.Meta.MainMenu;
using Frog.StateTracker;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frog.Meta
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Camera _camera;

        [SerializeField] private AssetReferenceGameObject _mainMenuPrefabRef;

        private AsyncStateTracker<RootScope> _stateTracker;
        private RootScope _scope;

        private void Awake()
        {
            RootScope scope;
            {
                scope.Camera = _camera;
                scope.Ui = new UiSystem(_canvas);
            }

            _scope = scope;
            _stateTracker = new AsyncStateTracker<RootScope>();
        }

        private void OnDestroy()
        {
            _stateTracker.Dispose(_scope);
            _scope.Dispose();
        }

        private async void Start()
        {
            try
            {
                var go = await _mainMenuPrefabRef.LoadAssetAsync().Task;

                var mainMenuPrefab = go.GetComponent<MainMenuUi>();
                var initialStateHandler = new MainMenuStateHandler(mainMenuPrefab);

                await _stateTracker.Run(_scope, initialStateHandler, destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
            }


            ExitGame();
        }

        private static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}