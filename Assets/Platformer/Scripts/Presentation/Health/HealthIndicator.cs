using UnityEngine;

namespace Platformer
{
    public class HealthIndicator : MonoBehaviour
    {
        [SerializeField] private Health _health;

        private bool _isInitialized;

        private void OnEnable()
        {
            _health.Died += HandleDied;

            if (_isInitialized && _health.IsAlive == false)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            _isInitialized = true;

            if (_health.IsAlive == false)
                gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _health.Died -= HandleDied;
        }

        private void HandleDied()
        {
            gameObject.SetActive(false);
        }
    }
}
