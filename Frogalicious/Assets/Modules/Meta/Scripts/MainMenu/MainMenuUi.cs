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

        private readonly AwaitableProcess<Command> _process = new AwaitableProcess<Command>();

        private void Start()
        {
            _playButton.onClick.AddListener(() => _process.End(Command.Play));
            _exitButton.onClick.AddListener(() => _process.End(Command.Exit));
        }

        public Awaitable<Command> WaitForCommand() => _process.Begin();
    }
}