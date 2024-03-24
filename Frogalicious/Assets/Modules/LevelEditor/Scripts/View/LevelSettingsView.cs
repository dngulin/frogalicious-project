using System;
using Frog.Level;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class LevelSettingsView : VisualElement
    {
        private readonly SliderInt _widthField = new SliderInt("Width", 3, LevelConventions.MaxWidth) { showInputField = true };
        private readonly SliderInt _heightField = new SliderInt("Height", 3, LevelConventions.MaxHeight)  { showInputField = true };

        public int Width
        {
            get => _widthField.value;
            set => _widthField.value = value;
        }

        public int Height
        {
            get => _heightField.value;
            set => _heightField.value = value;
        }

        public event Action<int, int> OnSizeUpdated;

        public void ClearSubscriptions()
        {
            OnSizeUpdated = null;
        }

        public LevelSettingsView()
        {
            Add(new Label("BoardSize"));
            Add(_widthField);
            Add(_heightField);

            _widthField.RegisterCallback<ChangeEvent<int>, LevelSettingsView>(
                (e, view) => view.OnSizeUpdated?.Invoke(e.newValue, view.Height),
                this
            );

            _heightField.RegisterCallback<ChangeEvent<int>, LevelSettingsView>(
                (e, view) => view.OnSizeUpdated?.Invoke(view.Width, e.newValue),
                this
            );
        }
    }
}