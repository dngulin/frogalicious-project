using System;
using Frog.Core.Ui;
using Frog.Meta.Splash;
using Frog.ActivityTracker;
using UnityEngine;

namespace Frog.Meta
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Camera _camera;

        [SerializeField] private SplashScreenUi _splashUiPrefab;
        [SerializeField] private LoadingUi _loadingPrefab;

        private AsyncActivityTracker<RootScope> _activityTracker;
        private RootScope _scope;

        private void Awake()
        {
            RootScope scope;
            {
                scope.Camera = _camera;
                scope.GameObjectStash = RootScope.CreateGameObjectStash();
                scope.Ui = new UiSystem(_canvas, Instantiate(_loadingPrefab, scope.GameObjectStash));
                scope.Mailbox = new Mailbox();
            }

            scope.GameObjectStash.gameObject.SetActive(false);

            _scope = scope;
            _activityTracker = new AsyncActivityTracker<RootScope>();
        }

        private void OnDestroy()
        {
            _activityTracker.Dispose(_scope);
            _scope.Dispose();
        }

        private async void Start()
        {
            try
            {
                var initialHandler = new SplashScreenActivity(_scope, _splashUiPrefab);
                await _activityTracker.ExecuteAsync(_scope, initialHandler, destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
            }

            ExitGame();
        }

        private void Update()
        {
            _activityTracker.Tick(_scope, Time.unscaledDeltaTime);
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