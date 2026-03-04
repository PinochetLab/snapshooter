using TMPro;
using UnityEngine;

namespace Interaction
{
    public class InteractionText : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;

        public void SetActive(bool active)
        {
            tmpText.gameObject.SetActive(active);
        }

        public void SetActionText(string actionText)
        {
            tmpText.text = $"Press  <sprite name=ml>  to {actionText}";
        }

        public void SetText(string text)
        {
            tmpText.text = text;
        }
    }
}