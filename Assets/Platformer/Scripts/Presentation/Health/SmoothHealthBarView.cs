using UnityEngine;

namespace Platformer
{
    public class SmoothHealthBarView : HealthBarView
    {
        [SerializeField] private float _changeSpeed = 0.5f;

        private float _targetValue;

        private void Update()
        {
            Slider.value = Mathf.MoveTowards(
                Slider.value,
                _targetValue,
                _changeSpeed * Time.deltaTime);
        }

        protected override void InitializeView(int current, int maximum)
        {
            _targetValue = GetNormalizedValue(current, maximum);
            Slider.value = _targetValue;
        }

        protected override void UpdateView(int current, int maximum)
        {
            _targetValue = GetNormalizedValue(current, maximum);
        }
    }
}
