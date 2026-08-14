using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    [RequireComponent(typeof(VampirismAbility))]
    public class VampirismAbilityProgressBarView : MonoBehaviour
    {
        [SerializeField] private string _abilityKeyLabel = "Q";
        [SerializeField] private RectTransform _abilityIndicator;
        [SerializeField] private Image _backgroundRenderer;
        [SerializeField] private Image _fillRenderer;
        [SerializeField] private Text _keyLabel;
        [SerializeField] private Color _activeColor = new Color(0.9f, 0f, 0f, 0.95f);
        [SerializeField] private Color _cooldownColor = new Color(0.95f, 0.78f, 0.09f, 0.95f);
        [SerializeField] private Color _readyColor = new Color(0.25f, 0.85f, 0.55f, 0.95f);
        [SerializeField] private Color _backgroundColor = new Color(0f, 0f, 0f, 0.45f);
        [SerializeField] private Color _labelColor = new Color(1f, 1f, 1f, 0.95f);

        private VampirismAbility _ability;

        private void Awake()
        {
            _ability = GetComponent<VampirismAbility>();
        }

        private void OnEnable()
        {
            _ability.StateChanged += HandleStateChanged;
            _ability.ProgressChanged += HandleProgressChanged;
            _abilityIndicator.gameObject.SetActive(true);
            ApplyState(_ability.State);
            ApplyProgress(_ability.Progress);
        }

        private void OnDisable()
        {
            _ability.StateChanged -= HandleStateChanged;
            _ability.ProgressChanged -= HandleProgressChanged;
        }

        private void HandleStateChanged(VampirismAbilityState state)
        {
            ApplyState(state);
        }

        private void HandleProgressChanged(float normalizedProgress)
        {
            ApplyProgress(normalizedProgress);
        }

        private void ApplyState(VampirismAbilityState state)
        {
            Color fillColor = _readyColor;

            if (state == VampirismAbilityState.Active)
                fillColor = _activeColor;
            else if (state == VampirismAbilityState.Cooldown)
                fillColor = _cooldownColor;

            _backgroundRenderer.color = _backgroundColor;
            _fillRenderer.color = fillColor;
            _keyLabel.text = _abilityKeyLabel;
            _keyLabel.color = _labelColor;
        }

        private void ApplyProgress(float normalizedProgress)
        {
            _fillRenderer.fillAmount = Mathf.Clamp01(normalizedProgress);
        }
    }
}
