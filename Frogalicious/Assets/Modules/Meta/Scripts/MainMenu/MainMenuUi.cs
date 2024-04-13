using System.Threading;
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
            _playButton.onClick.AddListener(() => _process.EndWithAssert(Command.Play));
            _exitButton.onClick.AddListener(() => _process.EndWithAssert(Command.Exit));
        }

        public Awaitable<Command> WaitForCommand(CancellationToken ct) => _process.Begin(ct);

        private void OnDestroy() => _process.Dispose();
    }
}