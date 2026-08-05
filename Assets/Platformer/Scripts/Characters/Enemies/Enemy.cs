using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace Platformer
{
    [MovedFrom(true, "Platformer", "Platformer.Runtime", "EnemyPatrol")]
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Health))]
    [RequireComponent(typeof(Collider2D), typeof(MeleeAttacker), typeof(CombatAnimator))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Transform _leftPoint;
        [SerializeField] private Transform _rightPoint;
        [SerializeField] private Health _target;
        [FormerlySerializedAs("_speed")]
        [SerializeField] private float _patrolSpeed = 2f;
        [SerializeField] private float _chaseSpeed = 3f;
        [SerializeField] private float _visionDistance = 6f;
        [SerializeField] private float _maximumVisionHeightDifference = 1.75f;
        [SerializeField] private float _deathAnimationDuration = 0.7f;
        [SerializeField] private LayerMask _obstacleLayer;

        private float _patrolDirection = 1f;
        private Collider2D _collider;
        private Health _health;
        private MeleeAttacker _attacker;
        private Rigidbody2D _rigidbody;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _health = GetComponent<Health>();
            _attacker = GetComponent<MeleeAttacker>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _health.Died += OnDied;
        }

        private void OnDisable()
        {
            _health.Died -= OnDied;
        }

        private void FixedUpdate()
        {
            if (CanSeeTarget())
            {
                ChaseTarget();
                return;
            }

            Patrol();
        }

        private bool CanSeeTarget()
        {
            if (_target == null || _target.IsAlive == false)
                return false;

            Vector2 targetOffset = _target.transform.position - transform.position;

            if (targetOffset.sqrMagnitude > _visionDistance * _visionDistance)
                return false;

            if (Mathf.Abs(targetOffset.y) > _maximumVisionHeightDifference)
                return false;

            RaycastHit2D obstacle = Physics2D.Linecast(
                _rigidbody.position,
                _target.transform.position,
                _obstacleLayer);
            return obstacle.collider == null;
        }

        private void ChaseTarget()
        {
            float targetPositionX = _target.transform.position.x;
            float distance = Vector2.Distance(transform.position, _target.transform.position);

            if (distance <= _attacker.AttackRange)
            {
                _rigidbody.linearVelocityX = 0f;
                Face(targetPositionX - _rigidbody.position.x);
                _attacker.TryAttack(_target);
                return;
            }

            MoveTowards(targetPositionX, _chaseSpeed);
        }

        private void Patrol()
        {
            Transform targetPoint = _patrolDirection > 0f ? _rightPoint : _leftPoint;
            float nextPositionX = MoveTowards(targetPoint.position.x, _patrolSpeed);

            if (Mathf.Approximately(nextPositionX, targetPoint.position.x))
                _patrolDirection *= -1f;
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

        private void OnDied()
        {
            enabled = false;
            _collider.enabled = false;
            _rigidbody.simulated = false;
            Destroy(gameObject, _deathAnimationDuration);
        }
    }
}
