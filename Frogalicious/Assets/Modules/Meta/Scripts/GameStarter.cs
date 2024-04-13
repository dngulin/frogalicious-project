using Frog.Core.Ui;
using Frog.Meta.MainMenu;
using Frog.StateTracker;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frog.Meta
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private AssetReferenceGameObject _mainMenuPrefabRef;

        private async void Start()
        {
            RootScope scope;
            scope.Ui = new UiSystem(_canvas);

            var go = await _mainMenuPrefabRef.LoadAssetAsync().Task;
            var mainMenuPrefab = go.GetComponent<MainMenuUi>();
            var initialStateHandler = new MainMenuStateHandler(mainMenuPrefab);

            using (scope)
            {
                await AsyncStateTracker<RootScope>.Run(scope, initialStateHandler, destroyCancellationToken);
            }


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}