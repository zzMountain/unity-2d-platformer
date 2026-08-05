using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(CoinWallet), typeof(Rigidbody2D))]
    public class CoinCollector : MonoBehaviour
    {
        private CoinWallet _wallet;

        private void Awake()
        {
            _wallet = GetComponent<CoinWallet>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Coin coin) == false)
                return;

            if (coin.TryCollect(out int value) == false)
                return;

            _wallet.Add(value);
        }
    }
}
