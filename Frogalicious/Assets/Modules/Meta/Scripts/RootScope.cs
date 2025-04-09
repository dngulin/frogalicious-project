using System;
using Frog.Core;
using Frog.Core.Localization;
using Frog.Core.Save;
using Frog.Core.Ui;
using UnityEngine;

namespace Frog.Meta
{
    public sealed class RootScope : IDisposable
    {
        public readonly Camera Camera;
        public readonly UiSystem Ui;
        public readonly SaveSystem Save;
        public readonly LocalizationSystem Localization;
        public readonly Transform GameObjectStash;

        public RootScope(Camera camera, Canvas canvas, LoadingUi loadingPrefab)
        {
            Camera = camera;
            GameObjectStash = CreateGameObjectStash();

            Ui = new UiSystem(canvas, UnityEngine.Object.Instantiate(loadingPrefab, GameObjectStash));
            Save = new SaveSystem();
            Localization = new LocalizationSystem();
        }

        public void Dispose()
        {
            GameObjectStash.DestroyGameObject();
            Ui.Dispose();
            Save.Dispose();
            Localization.Dispose();
        }

        private static Transform CreateGameObjectStash()
        {
            var go = new GameObject(nameof(GameObjectStash));
            go.SetActive(false);
            return go.transform;
        }
    }
}