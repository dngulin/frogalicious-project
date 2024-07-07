using UnityEngine;

namespace Frog.Meta.MainMenu.View
{
    public class MapView : MonoBehaviour
    {
        [SerializeField] private LevelObjectView[] _levels;
        [SerializeField] private DecorObjectView[] _decorations;

        public int? PollLevelClick()
        {
            for (var i = 0; i < _levels.Length; i++)
            {
                if (_levels[i].PollClick())
                    return i;
            }

            return null;
        }

        public void SetCurrentLevel( int currIdx )
        {
            for (var idx = 0; idx < _levels.Length; idx++)
            {
                _levels[idx].SetState(GetLevelState(idx, currIdx));
            }
        }

        private static LevelObjectState GetLevelState(int idx, int currIdx)
        {
            if (idx == currIdx)
                return LevelObjectState.Unlocked;

            return idx > currIdx ? LevelObjectState.Locked : LevelObjectState.Completed;
        }
    }
}