using System;
using UnityEngine;

namespace Platformer
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _maximum = 100;

        public event Action Died;
        public event Action<int, int> ValueChanged;

        public int Current { get; private set; }
        public int Maximum => _maximum;
        public bool IsAlive => Current > 0;
        public bool IsFull => Current == Maximum;

        private void Awake()
        {
            Current = _maximum;
        }

        public bool TakeDamage(int damage)
        {
            return TakeDamage(damage, out _);
        }

        public bool TakeDamage(int damage, out int appliedDamage)
        {
            appliedDamage = 0;

            if (damage <= 0 || IsAlive == false)
                return false;

            int previousValue = Current;
            Current = Mathf.Max(Current - damage, 0);
            appliedDamage = previousValue - Current;
            ValueChanged?.Invoke(Current, Maximum);

            if (IsAlive == false)
                Died?.Invoke();

            return appliedDamage > 0;
        }

        public bool Restore(int amount)
        {
            if (amount <= 0 || IsAlive == false || IsFull)
                return false;

            Current = Mathf.Min(Current + amount, Maximum);
            ValueChanged?.Invoke(Current, Maximum);
            return true;
        }
    }
}
