using Frog.Core;
using Frog.Core.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Frog.Meta.MainMenu
{
    public class MainMenuUi : UiEntity
    {
        public enum Command
        {
            Play,
            Exit,
        }

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;

        private Cell<Command> _commandCell;

        private void Start()
        {
            _playButton.onClick.AddListener(() => _commandCell.PushOrReplaceWithAssertion(Command.Play));
            _exitButton.onClick.AddListener(() => _commandCell.PushOrReplaceWithAssertion(Command.Exit));
        }

        public Command? Poll() => _commandCell.PopNullable();

        public override void SetVisible(bool visible) => _canvasGroup.alpha = visible ? 1 : 0;
        public override void SetInteractable(bool interactable) => _canvasGroup.interactable = interactable;
    }
}