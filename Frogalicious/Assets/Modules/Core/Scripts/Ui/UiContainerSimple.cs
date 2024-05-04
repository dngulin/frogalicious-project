using UnityEngine;

namespace Frog.Core.Ui
{
    public sealed class UiContainerSimple : UiContainer
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        public override void SetVisible(bool visible) => _canvasGroup.alpha = visible ? 1 : 0;
        public override void SetInteractable(bool interactable) => _canvasGroup.interactable = interactable;

        public override void AttachContent(Transform content)
        {
            Debug.Assert(transform.childCount == 0);
            content.SetParent(transform, false);
        }

        public override Transform DetachContent(Transform contentParent)
        {
            Debug.Assert(transform.childCount == 1);

            var content = transform.GetChild(0);
            content.SetParent(contentParent, false);

            return content;
        }

        public static UiContainerSimple Create(bool visible = false, Transform parent = null)
        {
            var go = new GameObject(nameof(UiContainerSimple), typeof(RectTransform));
            go.GetComponent<RectTransform>().SetParentAndExpand(parent);

            var entity = go.AddComponent<UiContainerSimple>();
            entity._canvasGroup = go.AddComponent<CanvasGroup>();

            entity.SetVisible(visible);

            return entity;
        }
    }
}