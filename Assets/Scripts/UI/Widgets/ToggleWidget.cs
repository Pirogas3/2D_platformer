using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Widgets
{
    public class ToggleWidget : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private float _showTime = 2f;

        public void OnPointerClick(PointerEventData eventData)
        {
            StartCoroutine(Show());
        }

        private IEnumerator Show()
        {
            _expText.enabled = true;
            yield return new WaitForSeconds(_showTime);
            _expText.enabled = false;
        }
    }
}
