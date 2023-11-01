using System;
using UnityEngine.UIElements;

namespace Frog.LevelEditor
{
    public class LevelSettingsView : VisualElement
    {
        private readonly SliderInt _widthField = new SliderInt("Width", 5, 15) { showInputField = true };
        private readonly SliderInt _heightField = new SliderInt("Height", 5, 15)  { showInputField = true };

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