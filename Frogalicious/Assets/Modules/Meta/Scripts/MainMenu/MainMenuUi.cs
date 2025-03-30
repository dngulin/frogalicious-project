using Frog.Core;
using Frog.Core.Ui;
using Frog.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Frog.Meta.MainMenu
{
    public class MainMenuUi : UiEntity
    {
        public enum Command
        {
            Continue,
            Exit,
        }

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;

        private Cell<Command> _commandCell;

        private void Start()
        {
            _playButton.onClick.AddListener(() => _commandCell.PushOrReplaceWithAssertion(Command.Continue));
            _exitButton.onClick.AddListener(() => _commandCell.PushOrReplaceWithAssertion(Command.Exit));
            Tr.Msg("foo/bar/baz n={0}");
            Tr.Plu("fizz/buzz k={0}", 2);
        }

        public Command? Poll() => _commandCell.PopNullable();

        public override void SetVisible(bool visible) => _canvasGroup.alpha = visible ? 1 : 0;
        public override void SetInteractable(bool interactable) => _canvasGroup.interactable = interactable;
    }
}