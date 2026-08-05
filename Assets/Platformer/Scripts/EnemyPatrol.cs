using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class EnemyPatrol : MonoBehaviour
    {
        [SerializeField] private Transform _leftPoint;
        [SerializeField] private Transform _rightPoint;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _arrivalDistance = 0.05f;

        private float _direction = 1f;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            Transform target = _direction > 0f ? _rightPoint : _leftPoint;
            float distance = target.position.x - _rigidbody.position.x;

            if (Mathf.Abs(distance) <= _arrivalDistance)
                Reverse();

            Vector2 nextPosition = _rigidbody.position + Vector2.right * (_direction * _speed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(nextPosition);
        }

        private void Reverse()
        {
            _direction *= -1f;
            _renderer.flipX = _direction < 0f;
        }
    }
}
