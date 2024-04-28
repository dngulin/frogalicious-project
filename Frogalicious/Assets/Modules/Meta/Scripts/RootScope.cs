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


        public void Dispose()
        {
            Ui.Dispose();
            LoadingUi.DestroyGameObject();
        }
    }
}