using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Platformer
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string _gameSceneName = "Platformer";
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _authorsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private GameObject _authorsPanel;

        private void Awake()
        {
            _authorsPanel.SetActive(false);
        }

        private void OnEnable()
        {
            _playButton.onClick.AddListener(Play);
            _authorsButton.onClick.AddListener(ToggleAuthorsPanel);
            _exitButton.onClick.AddListener(Exit);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(Play);
            _authorsButton.onClick.RemoveListener(ToggleAuthorsPanel);
            _exitButton.onClick.RemoveListener(Exit);
        }

        private void Play()
        {
            SceneManager.LoadScene(_gameSceneName);
        }

        private void ToggleAuthorsPanel()
        {
            _authorsPanel.SetActive(_authorsPanel.activeSelf == false);
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
