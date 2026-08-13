using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    [RequireComponent(typeof(Text))]
    public class HealthTextView : HealthView
    {
        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        protected override void UpdateView(int current, int maximum)
        {
            _text.text = $"{current}/{maximum}";
        }
    }
}
