using System.Threading;
using Frog.Level;
using Frog.Level.Data;
using Frog.Level.View;
using Frog.StateTracker;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frog.Meta.Level
{
    public class LevelStateHandler : AsyncStateHandler<RootScope>
    {
        public override void Dispose(in RootScope scope) {}

        public override void Tick(in RootScope scope, float dt) {}

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            var data = await Addressables.LoadAssetAsync<LevelData>("Assets/Levels/Castle1.asset").Task;
            var viewConfig = await Addressables.LoadAssetAsync<LevelViewConfig>("Assets/Modules/Level/Config/LevelViewConfig.asset").Task;

            var level = LevelController.Create(data, viewConfig, scope.Camera);

            while (true)
            {
                level.Tick(Time.deltaTime);
                await Awaitable.NextFrameAsync(ct);
            }
        }
    }
}