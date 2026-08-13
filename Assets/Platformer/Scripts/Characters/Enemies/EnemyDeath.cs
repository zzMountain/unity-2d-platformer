using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Collider2D), typeof(Health), typeof(Rigidbody2D))]
    public class EnemyDeath : MonoBehaviour
    {
        [SerializeField] private float _animationDuration = 0.7f;

        private Collider2D _collider;
        private Health _health;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _health = GetComponent<Health>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _health.Died += HandleDied;
        }

        private void OnDisable()
        {
            _health.Died -= HandleDied;
        }

        private void HandleDied()
        {
            _collider.enabled = false;
            _rigidbody.simulated = false;
            Destroy(gameObject, _animationDuration);
        }
    }
}
