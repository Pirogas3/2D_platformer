using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.UI.Widgets
{
    public class TooltipView : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _costText; // текст стоимости

        public void SetData(string header, string description, string cost)
        {
            _headerText.text = header;
            _descriptionText.text = description;

            if (!string.IsNullOrEmpty(cost))
            {
                _costText.text = cost;
                _costText.gameObject.SetActive(true);
            }
            else
            {
                _costText.gameObject.SetActive(false);
            }
        }

        public void SetPosition(Vector2 position)
        {
            Vector3 pos = new Vector3(position.x, position.y, 0f);
            pos += new Vector3(10f, -10f, 0f);
            transform.position = pos;
        }
    }
}
