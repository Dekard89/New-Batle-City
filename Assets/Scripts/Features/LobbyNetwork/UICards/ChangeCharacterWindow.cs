using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ChangeCharacterWindow : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
