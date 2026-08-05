using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Health), typeof(Rigidbody2D))]
    public class FirstAidKitCollector : MonoBehaviour
    {
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_health.IsAlive == false || _health.IsFull)
                return;

            if (other.TryGetComponent(out FirstAidKit firstAidKit) == false)
                return;

            if (firstAidKit.TryCollect(out int healing) == false)
                return;

            _health.Restore(healing);
        }
    }
}
