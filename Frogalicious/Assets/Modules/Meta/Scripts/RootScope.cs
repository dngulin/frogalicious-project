using System;
using Frog.Core.Ui;

namespace Frog.Meta
{
    public struct RootScope : IDisposable
    {
        public UiSystem Ui;


        public void Dispose()
        {
            Ui.Dispose();
        }
    }
}