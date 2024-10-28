using System;
using Frog.Core;
using Frog.Core.Save;
using Frog.Core.Ui;
using UnityEngine;

namespace Frog.Meta
{
    public struct RootScope : IDisposable
    {
        public Camera Camera;
        public UiSystem Ui;
        public SaveSystem Save;
        public Transform GameObjectStash;


        public void Dispose()
        {
            GameObjectStash.DestroyGameObject();
            Ui.Dispose();
            Save.Dispose();
        }

        public static Transform CreateGameObjectStash()
        {
            var go = new GameObject(nameof(GameObjectStash));
            go.SetActive(false);
            return go.transform;
        }
    }
}