using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Views
{
    public class HealthBarWorldView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _container;

        [SerializeField]
        private Image _fill;

        [SerializeField]
        private Slider _slider;

        public void Setup(bool isLocalPlayer, bool isFrendly)
        {
            if (isLocalPlayer)
            {
                _container.SetActive(false);
                return;
            }
            
            _fill.color= isFrendly? Color.green : Color.red;
        }
        public void UpdateValue(float current, float max)
        {
            _slider.value = current / max;
        }
        private void Update()
        {
            _container.transform.rotation = Quaternion.identity;
        }
    }
}
