using System;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Health), typeof(Collider2D))]
    public class MeleeAttacker : MonoBehaviour
    {
        private const int MaxOverlapResults = 16;

        private readonly Collider2D[] _overlapResults = new Collider2D[MaxOverlapResults];

        [SerializeField] private Team _team;
        [SerializeField] private int _damage = 20;
        [SerializeField] private float _attackRange = 1.25f;
        [SerializeField] private float _cooldown = 0.75f;

        private Health _health;
        private float _nextAttackTime;

        public event Action AttackPerformed;

        public enum Team
        {
            Player,
            Enemy
        }

        public float AttackRange => _attackRange;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }

        public bool TryAttack()
        {
            if (CanAttack() == false)
                return false;

            ContactFilter2D contactFilter = ContactFilter2D.noFilter;
            int overlapCount = Physics2D.OverlapCircle(
                transform.position,
                _attackRange,
                contactFilter,
                _overlapResults);
            MeleeAttacker closestTarget = FindClosestTarget(overlapCount);

            if (closestTarget != null)
                closestTarget._health.TakeDamage(_damage);

            PerformAttack();
            return true;
        }

        public bool TryAttack(Health target)
        {
            if (CanAttack() == false || IsValidTarget(target) == false)
                return false;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance > _attackRange)
                return false;

            target.TakeDamage(_damage);
            PerformAttack();
            return true;
        }

        private bool CanAttack()
        {
            return _health.IsAlive && Time.time >= _nextAttackTime;
        }

        private MeleeAttacker FindClosestTarget(int overlapCount)
        {
            MeleeAttacker closestTarget = null;
            float closestSqrDistance = float.MaxValue;

            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D candidateCollider = _overlapResults[i];

                if (candidateCollider.TryGetComponent(out MeleeAttacker candidate) == false)
                    continue;

                if (IsValidTarget(candidate._health) == false)
                    continue;

                float sqrDistance = (candidate.transform.position - transform.position).sqrMagnitude;

                if (sqrDistance >= closestSqrDistance)
                    continue;

                closestTarget = candidate;
                closestSqrDistance = sqrDistance;
            }

            return closestTarget;
        }

        private bool IsValidTarget(Health target)
        {
            if (target == null || target == _health || target.IsAlive == false)
                return false;

            if (target.TryGetComponent(out MeleeAttacker targetAttacker) == false)
                return false;

            return targetAttacker._team != _team;
        }

        private void PerformAttack()
        {
            _nextAttackTime = Time.time + _cooldown;
            AttackPerformed?.Invoke();
        }
    }
}
