using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Health), typeof(MeleeAttacker), typeof(EnemyMover))]
    [RequireComponent(typeof(EnemyVision), typeof(CombatAnimator), typeof(EnemyDeath))]
    public class Enemy : MonoBehaviour
    {
        private Health _health;
        private MeleeAttacker _attacker;
        private EnemyMover _mover;
        private EnemyVision _vision;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _attacker = GetComponent<MeleeAttacker>();
            _mover = GetComponent<EnemyMover>();
            _vision = GetComponent<EnemyVision>();
        }

        private void FixedUpdate()
        {
            if (_health.IsAlive == false)
                return;

            if (_vision.CanSeeTarget() == false)
            {
                _mover.Patrol();
                return;
            }

            Health target = _vision.Target;
            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance <= _attacker.AttackRange)
            {
                _mover.StopAndFace(target.transform);
                _attacker.TryAttack(target);
                return;
            }

            _mover.Chase(target.transform);
        }
    }
}
