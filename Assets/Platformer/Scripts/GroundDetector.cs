using UnityEngine;

namespace Platformer
{
    public class GroundDetector : MonoBehaviour
    {
        [SerializeField] private Transform _checkPoint;
        [SerializeField] private Vector2 _checkSize = new Vector2(0.55f, 0.12f);
        [SerializeField] private LayerMask _groundLayer;

        private void OnDrawGizmosSelected()
        {
            if (_checkPoint == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(_checkPoint.position, _checkSize);
        }

        public bool IsGrounded()
        {
            Collider2D ground = Physics2D.OverlapBox(_checkPoint.position, _checkSize, 0f, _groundLayer);
            return ground != null;
        }
    }
}
