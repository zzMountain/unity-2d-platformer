using UnityEngine;

namespace Platformer
{
    public class EnemyVision : MonoBehaviour
    {
        [SerializeField] private Health _target;
        [SerializeField] private float _visionDistance = 6f;
        [SerializeField] private float _maximumVisionHeightDifference = 1.75f;
        [SerializeField] private LayerMask _obstacleLayer;

        public Health Target => _target;

        public bool CanSeeTarget()
        {
            if (_target == null || _target.IsAlive == false)
                return false;

            Vector2 targetOffset = _target.transform.position - transform.position;

            if (targetOffset.sqrMagnitude > _visionDistance * _visionDistance)
                return false;

            if (Mathf.Abs(targetOffset.y) > _maximumVisionHeightDifference)
                return false;

            RaycastHit2D obstacle = Physics2D.Linecast(
                transform.position,
                _target.transform.position,
                _obstacleLayer);
            return obstacle.collider == null;
        }
    }
}
