using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingDogs.Core
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Slider _difficultySlider;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private GameObject _difficultySliderGameObject;
        [SerializeField] private GameObject _nameInputFieldGameObject;
        [SerializeField] private GameObject _winTextGameObject;
        [SerializeField] private GameObject _looseTextGameObject;
        private Action<Difficulty> _startGame;
        private string _playerName;

        private void Awake()
        {
            _nameInputField.onEndEdit.AddListener(val => { OnNicknameEntered(); });

            if (PlayerPrefs.HasKey("playerName"))
                _nameInputField.text = PlayerPrefs.GetString("playerName");
        }

        private void OnNicknameEntered()
        {
            _playerName = _nameInputField.text;
            PlayerPrefs.SetString("playerName", _playerName);
            PlayerPrefs.Save();
        }

        public void Initialize(Action<Difficulty> startGame, Action restartGame)
        {
            _startGame = startGame;
            _startButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(StartGameWithSelectedDifficulty);
            _restartButton.onClick.AddListener(() => { restartGame?.Invoke(); });

            _startButton.gameObject.SetActive(true);
            _restartButton.gameObject.SetActive(false);
            _difficultySliderGameObject.SetActive(true);
            _nameInputFieldGameObject.SetActive(true);
            _winTextGameObject.SetActive(false);
            _looseTextGameObject.SetActive(false);
        }

        public void RestartGame()
        {
            _startButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _difficultySliderGameObject.SetActive(false);
            _nameInputFieldGameObject.SetActive(false);
            _winTextGameObject.SetActive(false);
            _looseTextGameObject.SetActive(false);
        }
        
        private void StartGameWithSelectedDifficulty()
        {
            Difficulty difficulty = (int) _difficultySlider.value switch
            {
                1 => Difficulty.Diff1,
                2 => Difficulty.Diff2,
                3 => Difficulty.Diff3,
                _ => Difficulty.Diff1
            };

            _startGame?.Invoke(difficulty);
            _startButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _difficultySliderGameObject.SetActive(false);
            _nameInputFieldGameObject.SetActive(false);
        }

        public void ShowWinText()
        {
            _winTextGameObject.SetActive(true);
        }

        public void ShowLoseText()
        {
            _looseTextGameObject.SetActive(true);
        }
    }
}