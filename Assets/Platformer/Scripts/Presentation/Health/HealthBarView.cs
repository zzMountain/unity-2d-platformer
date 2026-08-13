using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    [RequireComponent(typeof(Slider))]
    public class HealthBarView : HealthView
    {
        private Slider _slider;

        protected Slider Slider => _slider;

        protected virtual void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.wholeNumbers = false;
        }

        protected override void UpdateView(int current, int maximum)
        {
            _slider.value = GetNormalizedValue(current, maximum);
        }

        protected float GetNormalizedValue(int current, int maximum)
        {
            if (maximum <= 0)
                return 0f;

            return (float)current / maximum;
        }
    }
}
