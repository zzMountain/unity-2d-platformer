using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class CoinSpawner : MonoBehaviour
    {
        private readonly Dictionary<Coin, Transform> _activeCoins = new Dictionary<Coin, Transform>();
        private readonly HashSet<Transform> _occupiedPoints = new HashSet<Transform>();
        private readonly List<float> _respawnTimers = new List<float>();

        [SerializeField] private Coin _coinPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private int _maxActiveCoins = 6;
        [SerializeField] private float _respawnDelay = 3f;

        private void Start()
        {
            int initialCoinCount = Mathf.Min(_maxActiveCoins, _spawnPoints.Length);

            for (int i = 0; i < initialCoinCount; i++)
                SpawnCoin();
        }

        private void Update()
        {
            for (int i = _respawnTimers.Count - 1; i >= 0; i--)
            {
                _respawnTimers[i] -= Time.deltaTime;

                if (_respawnTimers[i] > 0f)
                    continue;

                _respawnTimers.RemoveAt(i);
                SpawnCoin();
            }
        }

        private void OnDestroy()
        {
            foreach (Coin coin in _activeCoins.Keys)
                coin.Collected -= OnCoinCollected;
        }

        private void SpawnCoin()
        {
            if (_activeCoins.Count >= _maxActiveCoins)
                return;

            Transform spawnPoint = GetAvailableSpawnPoint();

            if (spawnPoint == null)
                return;

            Coin coin = Instantiate(_coinPrefab, spawnPoint.position, Quaternion.identity, transform);
            coin.Collected += OnCoinCollected;
            _activeCoins.Add(coin, spawnPoint);
            _occupiedPoints.Add(spawnPoint);
        }

        private Transform GetAvailableSpawnPoint()
        {
            if (_occupiedPoints.Count >= _spawnPoints.Length)
                return null;

            int startIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);

            for (int offset = 0; offset < _spawnPoints.Length; offset++)
            {
                int index = (startIndex + offset) % _spawnPoints.Length;
                Transform spawnPoint = _spawnPoints[index];

                if (_occupiedPoints.Contains(spawnPoint) == false)
                    return spawnPoint;
            }

            return null;
        }

        private void OnCoinCollected(Coin coin)
        {
            if (_activeCoins.Remove(coin, out Transform spawnPoint) == false)
                return;

            coin.Collected -= OnCoinCollected;
            _occupiedPoints.Remove(spawnPoint);
            _respawnTimers.Add(_respawnDelay);
            Destroy(coin.gameObject);
        }
    }
}
