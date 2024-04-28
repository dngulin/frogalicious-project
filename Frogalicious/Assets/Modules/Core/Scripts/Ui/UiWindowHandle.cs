using System;

namespace Frog.Core.Ui
{
    public enum UiWindowHandle : uint
    {
    }

    public enum UiDialogWindowHandle : uint
    {
    }

    public enum UiStaticWindowHandle : uint
    {
    }

    public static class UiWindowHandleExtensions
    {
        public static DisposableUiStaticWindow AsDisposable(this UiStaticWindowHandle handle, UiSystem uiSystem)
        {
            return new DisposableUiStaticWindow(uiSystem, handle);
        }
    }

    public readonly struct DisposableUiStaticWindow : IDisposable
    {
        private readonly UiSystem _uiSystem;
        private readonly UiStaticWindowHandle _handle;

        public DisposableUiStaticWindow(UiSystem uiSystem, UiStaticWindowHandle handle)
        {
            _uiSystem = uiSystem;
            _handle = handle;
        }

        public void Dispose() => _uiSystem.RemoveStaticWindow(_handle);
    }
}