using UnityEngine;

namespace Platformer
{
    public abstract class HealthView : MonoBehaviour
    {
        [SerializeField] private Health _health;

        private bool _isStarted;

        protected virtual void OnEnable()
        {
            _health.ValueChanged += HandleHealthValueChanged;

            if (_isStarted)
                InitializeView(_health.Current, _health.Maximum);
        }

        protected virtual void Start()
        {
            _isStarted = true;
            InitializeView(_health.Current, _health.Maximum);
        }

        protected virtual void OnDisable()
        {
            _health.ValueChanged -= HandleHealthValueChanged;
        }

        protected virtual void InitializeView(int current, int maximum)
        {
            UpdateView(current, maximum);
        }

        protected abstract void UpdateView(int current, int maximum);

        private void HandleHealthValueChanged(int current, int maximum)
        {
            UpdateView(current, maximum);
        }
    }
}
