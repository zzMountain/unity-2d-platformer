using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class EnemyMover : MonoBehaviour
    {
        [SerializeField] private Transform _leftPoint;
        [SerializeField] private Transform _rightPoint;
        [SerializeField] private float _patrolSpeed = 2f;
        [SerializeField] private float _chaseSpeed = 3f;

        private float _patrolDirection = 1f;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Chase(Transform target)
        {
            MoveTowards(target.position.x, _chaseSpeed);
        }

        public void Patrol()
        {
            Transform targetPoint = _patrolDirection > 0f ? _rightPoint : _leftPoint;
            float nextPositionX = MoveTowards(targetPoint.position.x, _patrolSpeed);

            if (Mathf.Approximately(nextPositionX, targetPoint.position.x))
                _patrolDirection *= -1f;
        }

        public void StopAndFace(Transform target)
        {
            _rigidbody.linearVelocityX = 0f;
            Face(target.position.x - _rigidbody.position.x);
        }

        private float MoveTowards(float targetPositionX, float speed)
        {
            float direction = Mathf.Sign(targetPositionX - _rigidbody.position.x);
            float distance = Mathf.Abs(targetPositionX - _rigidbody.position.x);
            float velocity = Mathf.Min(speed, distance / Time.fixedDeltaTime) * direction;

            _rigidbody.linearVelocityX = velocity;
            Face(direction);
            return _rigidbody.position.x + velocity * Time.fixedDeltaTime;
        }

        private void Face(float direction)
        {
            if (Mathf.Approximately(direction, 0f) == false)
                _renderer.flipX = direction < 0f;
        }
    }
}
