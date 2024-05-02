using UnityEngine;

namespace Frog.Core.Ui
{
    public sealed class UiEntityStatic : UiEntity
    {
        [SerializeField] private CanvasGroup _contentsRoot;

        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public override CanvasGroup ContentsRoot => _contentsRoot;

        public static UiEntityStatic Create(bool visible = false, Transform parent = null)
        {
            var go = new GameObject(nameof(UiEntityStatic), typeof(RectTransform));
            go.GetComponent<RectTransform>().SetParentAndExpand(parent);

            var entity = go.AddComponent<UiEntityStatic>();
            entity._contentsRoot = go.AddComponent<CanvasGroup>();

            entity.SetVisible(visible);

            return entity;
        }
    }
}