using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    [RequireComponent(typeof(Button))]
    public class HealthChangeButton : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private ChangeType _changeType;
        [SerializeField] private int _amount = 20;

        private Button _button;

        private enum ChangeType
        {
            Damage,
            Restore
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(ApplyChange);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(ApplyChange);
        }

        private void ApplyChange()
        {
            switch (_changeType)
            {
                case ChangeType.Damage:
                    _health.TakeDamage(_amount);
                    break;

                case ChangeType.Restore:
                    _health.Restore(_amount);
                    break;
            }
        }
    }
}
