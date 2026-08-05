using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(CoinWallet), typeof(Rigidbody2D), typeof(Health))]
    public class CoinCollector : MonoBehaviour
    {
        private Health _health;
        private CoinWallet _wallet;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _wallet = GetComponent<CoinWallet>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_health.IsAlive == false)
                return;

            if (other.TryGetComponent(out Coin coin) == false)
                return;

            if (coin.TryCollect(out int value) == false)
                return;

            _wallet.Add(value);
        }
    }
}
