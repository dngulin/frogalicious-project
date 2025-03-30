using TMPro;
using UnityEngine;

namespace Frog.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StaticTextLocalizer : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        private string _id;

        private void Start()
        {
            _textMesh = gameObject.GetComponent<TextMeshProUGUI>();
            _id = _textMesh.text;

            Tr.ProviderChanged += UpdateText;
            UpdateText();
        }

        private void OnDestroy()
        {
            Tr.ProviderChanged -= UpdateText;
        }

        private void UpdateText()
        {
            _textMesh.text = Tr.Msg(_id);
        }
    }
}