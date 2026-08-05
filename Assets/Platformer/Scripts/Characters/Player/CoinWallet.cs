using System;
using UnityEngine;

namespace Platformer
{
    public class CoinWallet : MonoBehaviour
    {
        public event Action<int> AmountChanged;

        public int Amount { get; private set; }

        public void Add(int amount)
        {
            if (amount <= 0)
                return;

            Amount += amount;
            AmountChanged?.Invoke(Amount);
        }
    }
}
