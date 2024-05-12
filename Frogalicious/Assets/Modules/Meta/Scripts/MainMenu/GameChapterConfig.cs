using Frog.Level.Data;
using Frog.Meta.MainMenu.View;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frog.Meta.MainMenu
{
    [CreateAssetMenu(menuName = "Frog/Meta/Chapter Config", fileName = nameof(GameChapterConfig))]
    public class GameChapterConfig : ScriptableObject
    {
        [Header("View")]
        public Color BgColor;
        public MapView MapPrefab;

        [Header("Levels")]
        public AssetReferenceT<LevelData>[] LevelList;
    }
}