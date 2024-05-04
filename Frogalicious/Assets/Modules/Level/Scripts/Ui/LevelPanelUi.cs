using Frog.Core;
using Frog.Core.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Frog.Level.Ui
{
    public class LevelPanelUi : AnimatorAnimatedUiEntity
    {
        public enum Command
        {
            Exit,
        }

        [SerializeField] private Button _backButton;

        private Cell<Command> _commandCell;

        private void Start()
        {
            _backButton.onClick.AddListener(() => _commandCell.PushOrReplaceWithAssertion(Command.Exit));
        }

        public Command? Poll() => _commandCell.PopNullable();
    }
}