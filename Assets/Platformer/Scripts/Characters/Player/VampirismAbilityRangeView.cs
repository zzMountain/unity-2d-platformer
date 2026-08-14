using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(VampirismAbility), typeof(Health))]
    public class VampirismAbilityRangeView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _radiusRenderer;
        [SerializeField] private Color _radiusColor = new Color(1f, 0f, 0f, 0.24f);

        private Health _ownerHealth;
        private VampirismAbility _ability;

        private void Awake()
        {
            _ability = GetComponent<VampirismAbility>();
            _ownerHealth = GetComponent<Health>();
            _radiusRenderer.color = _radiusColor;
            SetRadiusScale();
        }

        private void OnEnable()
        {
            _ability.StateChanged += HandleStateChanged;
            SetRadiusState(_ability.State);
        }

        private void OnDisable()
        {
            _ability.StateChanged -= HandleStateChanged;
            _radiusRenderer.enabled = false;
        }

        private void OnDrawGizmosSelected()
        {
            VampirismAbility ability = GetComponent<VampirismAbility>();

            Gizmos.color = _radiusColor;
            Gizmos.DrawWireSphere(transform.position, ability.Range);
        }

        private void HandleStateChanged(VampirismAbilityState state)
        {
            SetRadiusState(state);
        }

        private void SetRadiusState(VampirismAbilityState state)
        {
            bool isActive = state == VampirismAbilityState.Active;
            _radiusRenderer.enabled = isActive && _ownerHealth.IsAlive;
        }

        private void SetRadiusScale()
        {
            Vector2 spriteSize = _radiusRenderer.sprite.bounds.size;
            float diameter = _ability.Range * 2f;
            float scaleX = diameter / spriteSize.x;
            float scaleY = diameter / spriteSize.y;

            _radiusRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            _radiusRenderer.transform.localPosition = Vector3.zero;
        }
    }
}
