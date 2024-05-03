using System;
using Frog.Core;
using Frog.Core.Ui;
using UnityEngine;

namespace Frog.Meta
{
    public struct RootScope : IDisposable
    {
        public Camera Camera;
        public UiSystem Ui;
        public LoadingUi LoadingUi;
        public Transform GameObjectStash;


        public void Dispose()
        {
            Ui.Dispose();
            LoadingUi.DestroyGameObject();
            GameObjectStash.DestroyGameObject();
        }

        public static Transform CreateGameObjectStash()
        {
            var go = new GameObject("Stash");
            go.SetActive(false);
            return go.transform;
        }
    }
}