using Frog.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Frog.Meta.MainMenu
{
    public class MainMenuUi : MonoBehaviour
    {
        public enum Command
        {
            Play,
            Exit,
        }

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;

        private Cell<Command> _commandCell;

        private void Start()
        {
            _playButton.onClick.AddListener(() => _commandCell.PushOrReplaceWithAssertion(Command.Play));
            _exitButton.onClick.AddListener(() => _commandCell.PushOrReplaceWithAssertion(Command.Exit));
        }

        public Command? Poll() => _commandCell.PopNullable();
    }
}