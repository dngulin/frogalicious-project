using System;
using Frog.Core.Ui;
using Frog.Meta.Splash;
using Frog.StateTracker;
using UnityEngine;

namespace Frog.Meta
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Camera _camera;

        [SerializeField] private SplashUi _splashUiPrefab;
        [SerializeField] private LoadingUi _loadingPrefab;

        private AsyncStateTracker<RootScope> _stateTracker;
        private RootScope _scope;

        private void Awake()
        {
            RootScope scope;
            {
                scope.Camera = _camera;
                scope.Ui = new UiSystem(_canvas);
                scope.LoadingUi = Instantiate(_loadingPrefab);
                scope.GameObjectStash = RootScope.CreateGameObjectStash();
            }

            scope.GameObjectStash.gameObject.SetActive(false);

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
                var initialHandler = new SplashStateHandler(_scope, _splashUiPrefab);
                await _stateTracker.ExecuteAsync(_scope, initialHandler, destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
            }

            ExitGame();
        }

        private void Update()
        {
            _stateTracker.Tick(_scope, Time.unscaledDeltaTime);
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