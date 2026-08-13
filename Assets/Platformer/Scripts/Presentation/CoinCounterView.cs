using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    [RequireComponent(typeof(Text))]
    public class CoinCounterView : MonoBehaviour
    {
        private const string CounterPrefix = "COINS: ";

        [SerializeField] private CoinWallet _wallet;

        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void OnEnable()
        {
            _wallet.AmountChanged += HandleAmountChanged;
            HandleAmountChanged(_wallet.Amount);
        }

        private void OnDisable()
        {
            _wallet.AmountChanged -= HandleAmountChanged;
        }

        private void HandleAmountChanged(int amount)
        {
            _text.text = CounterPrefix + amount;
        }
    }
}
