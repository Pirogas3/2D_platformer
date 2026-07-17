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

        [Header("Containers")]
        [SerializeField] private GameObject _costContainer;

        private RectTransform _panel;

        public void SetData(string header, string description, string cost)
        {
            _headerText.text = header;
            _descriptionText.text = description;

            if (!string.IsNullOrEmpty(cost))
            {
                if (_costContainer != null)
                    _costContainer.SetActive(true);

                _costText.text = cost;
                _costText.gameObject.SetActive(true);
            }
            else
            {
                if (_costContainer != null)
                    _costContainer.SetActive(false);

                _costText.gameObject.SetActive(false);
            }
        }

        public void SetPosition(Vector2 position)
        {
            if (_panel == null)
                _panel = GetComponent<RectTransform>();

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Массив углов экрана
            Vector2[] corners = new Vector2[]
            {
            new Vector2(0, 0),                      // левый нижний
            new Vector2(screenWidth, 0),            // правый нижний
            new Vector2(0, screenHeight),           // левый верхний
            new Vector2(screenWidth, screenHeight)  // правый верхний
            };

            Vector2[] pivotValues = new Vector2[]
            {
            new Vector2(0, 0),   // левый нижний
            new Vector2(1, 0),   // правый нижний
            new Vector2(0, 1),   // левый верхний
            new Vector2(1, 1)    // правый верхний
            };

            Vector2[] offsets = new Vector2[]
            {
                new Vector2(15, 15),   // левый нижний
                new Vector2(-15, 15),  // правый нижний
                new Vector2(15, -15),  // левый верхний
                new Vector2(-15, -15)  // правый верхний
            };

            // Поиск ближайшего угла
            int closestIndex = 0;
            float minDist = float.MaxValue;
            for (int i = 0; i < corners.Length; i++)
            {
                float dist = Vector2.Distance(position, corners[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }

            _panel.pivot = pivotValues[closestIndex];
            Vector3 pos = new Vector3(position.x + offsets[closestIndex].x, position.y + offsets[closestIndex].y, 0f);
            transform.position = pos;
        }
    }
}
