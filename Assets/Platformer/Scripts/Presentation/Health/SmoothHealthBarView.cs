using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    [RequireComponent(typeof(Slider))]
    public class SmoothHealthBarView : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private float _changeSpeed = 0.5f;

        private bool _isStarted;
        private Slider _slider;
        private float _targetValue;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.wholeNumbers = false;
        }

        private void OnEnable()
        {
            _health.ValueChanged += HandleHealthValueChanged;

            if (_isStarted)
                InitializeView();
        }

        private void Start()
        {
            _isStarted = true;
            InitializeView();
        }

        private void Update()
        {
            if (Mathf.Approximately(_slider.value, _targetValue))
                return;

            _slider.value = Mathf.MoveTowards(
                _slider.value,
                _targetValue,
                _changeSpeed * Time.deltaTime);
        }

        private void OnDisable()
        {
            _health.ValueChanged -= HandleHealthValueChanged;
        }

        private void InitializeView()
        {
            _targetValue = GetNormalizedValue(_health.Current, _health.Maximum);
            _slider.value = _targetValue;
        }

        private void HandleHealthValueChanged(int current, int maximum)
        {
            _targetValue = GetNormalizedValue(current, maximum);
        }

        private float GetNormalizedValue(int current, int maximum)
        {
            if (maximum <= 0)
                return 0f;

            return (float)current / maximum;
        }
    }
}
