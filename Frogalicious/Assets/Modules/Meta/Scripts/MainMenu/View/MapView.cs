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
    }
}