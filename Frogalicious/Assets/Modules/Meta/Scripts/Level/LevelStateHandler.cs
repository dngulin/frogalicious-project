using System.Threading;
using Frog.Collections;
using Frog.Core;
using Frog.Core.Ui;
using Frog.Level;
using Frog.Level.Simulation;
using Frog.Level.Ui;
using Frog.Level.View;
using Frog.StateTracker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Meta.Level
{
    public class LevelStateHandler : AsyncStateHandler<RootScope>
    {
        private LevelResources _res;

        private readonly LevelView _view;
        private readonly LevelPanelUi _panel;
        private readonly LevelExitDialog _exitDlg;

        private SimState _state;

        private readonly AwaitableOperation<GameplayEvent> _waitForGameplayEvent = new AwaitableOperation<GameplayEvent>();

        private enum GameplayEvent
        {
            ExitClicked,
            LevelCompleted,
        }

        public LevelStateHandler(in RootScope scope, in LevelResources res)
        {
            _res = res;
            _view = new LevelView(res.ViewConfig, res.Data, scope.Camera);
            _panel = Object.Instantiate(res.PanelPrefab, scope.GameObjectStash);
            _exitDlg = Object.Instantiate(res.ExitDialogPrefab, scope.GameObjectStash);

            LevelSimulation.SetupInitialState(ref _state, res.Data);
            _view.CreateInitialObjects(in _state.Level);
        }

        public override void Dispose(in RootScope scope)
        {
            _view.Dispose();
            _panel.DestroyGameObject();
            _exitDlg.DestroyGameObject();
            _res.Dispose();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (!_waitForGameplayEvent.IsRunning)
                return;

            if (_panel.Poll().TryGetValue(out _))
            {
                _waitForGameplayEvent.EndAssertive(GameplayEvent.ExitClicked);
                return;
            }

            _view.Tick(dt);
            if (_view.IsPlayingTimeline)
                return;

            if (LevelSimulation.TryGetResult(_state.Level, out _))
            {
                _waitForGameplayEvent.EndAssertive(GameplayEvent.LevelCompleted);
                return;
            }

            LevelSimulation.Simulate(ref _state, InputStateProvider.Poll());

            if (_state.TimeLine.Events.Count() == 0)
                return;

            _view.StartPlayingTimeline(_state.TimeLine.Events);
            _state.TimeLine.Reset();
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            await using (await scope.Ui.AnimatedUi(_panel, ct))
            {
                while (true)
                {
                    var evt = await _waitForGameplayEvent.ExecuteAsync(ct);
                    switch (evt)
                    {
                        case GameplayEvent.ExitClicked:
                            if (await ConfirmExitAsync(scope, ct))
                                return GetExitTransition(scope);
                            break;

                        case GameplayEvent.LevelCompleted:
                            return GetExitTransition(scope);
                    }
                }
            }
        }

        private async Awaitable<bool> ConfirmExitAsync(RootScope scope, CancellationToken ct)
        {
            await using (await scope.Ui.AnimatedUi(_exitDlg, ct))
            {
                return await _exitDlg.WaitConfirmationAsync(ct);
            }
        }

        private Transition GetExitTransition(RootScope scope)
        {
            scope.Mailbox.LevelCompletedFlag.SetIf(LevelSimulation.TryGetResult(_state.Level, out var res) && res);
            return Transition.Pop();
        }
    }
}