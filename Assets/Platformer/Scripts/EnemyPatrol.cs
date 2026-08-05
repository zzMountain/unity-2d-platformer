using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class EnemyPatrol : MonoBehaviour
    {
        [SerializeField] private Transform _leftPoint;
        [SerializeField] private Transform _rightPoint;
        [SerializeField] private float _speed = 2f;

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
            float nextPositionX = Mathf.MoveTowards(
                _rigidbody.position.x,
                target.position.x,
                _speed * Time.fixedDeltaTime);
            Vector2 nextPosition = new Vector2(nextPositionX, _rigidbody.position.y);

            _rigidbody.MovePosition(nextPosition);

            if (Mathf.Approximately(nextPositionX, target.position.x))
                Reverse();
        }

        private void Reverse()
        {
            _direction *= -1f;
            _renderer.flipX = _direction < 0f;
        }
    }
}
