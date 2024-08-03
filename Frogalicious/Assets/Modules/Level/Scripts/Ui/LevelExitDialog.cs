using System.Threading;
using Frog.Core;
using Frog.Core.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Frog.Level.Ui
{
    public class LevelExitDialog : AnimatorAnimatedUiEntity
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        private Cell<bool> _resultCell;

        private void Start()
        {
            _yesButton.onClick.AddListener(() => _resultCell.PushOrReplaceWithAssertion(true));
            _noButton.onClick.AddListener(() => _resultCell.PushOrReplaceWithAssertion(false));
        }

        public bool? Poll() => _resultCell.PopNullable();

        public async Awaitable<bool> WaitConfirmationAsync(CancellationToken ct)
        {
            while (true)
            {
                if (Poll().TryGetValue(out var result))
                    return result;

                await Awaitable.NextFrameAsync(ct);
            }
        }
    }
}