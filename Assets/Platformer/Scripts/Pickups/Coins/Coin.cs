using UnityEngine;

namespace Platformer
{
    public class Coin : Pickup
    {
        [SerializeField] private int _value = 1;

        public bool TryCollect(out int value)
        {
            return TryCollectValue(_value, out value);
        }
    }
}
