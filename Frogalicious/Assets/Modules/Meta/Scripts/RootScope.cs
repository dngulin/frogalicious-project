using System;
using Frog.Core.Ui;
using UnityEngine;

namespace Frog.Meta
{
    public struct RootScope : IDisposable
    {
        public Camera Camera;
        public UiSystem Ui;


        public void Dispose()
        {
            Ui.Dispose();
        }
    }
}