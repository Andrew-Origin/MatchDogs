using System;
using UnityEngine;

namespace MatchingDogs.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Parameters")] [SerializeField]
        private float _startTime;

        [Space] [SerializeField] private GameMenu _gameMenu;
        [SerializeField] private GridGenerator _gridGenerator;
        [SerializeField] private MovesCounter _movesCounter;
        [SerializeField] private CountdownTimer _countdownTimer;
        [SerializeField] private BoardController _boardController;
        [SerializeField] private VFXController _vfxController;

        private void Awake()
        {
            _movesCounter.gameObject.SetActive(false);
            _countdownTimer.gameObject.SetActive(false);
            _gameMenu.Initialize(StartGame, RestartGame);
        }

        private void StartGame(Difficulty difficulty)
        {
            _gridGenerator.GameStarted += OnGameStarted;
            _boardController.GameFinished += OnGameFinished;
            _countdownTimer.OnTimeLeft += OnGameLose;

            _gridGenerator.GenerateGameGrid(difficulty);
            _movesCounter.gameObject.SetActive(true);
            _countdownTimer.gameObject.SetActive(true);
            _movesCounter.Initialize();
            _countdownTimer.Initialize(_startTime);
            _boardController.SubscribeToEvents(OnSuccessMatching, OnFailMatching);
        }

        private void OnGameStarted()
        {
            _countdownTimer.StartCountdown();
        }

        private void OnGameFinished()
        {
            _gridGenerator.DisableCards();
            _countdownTimer.StopTimer();
            _gameMenu.ShowWinText();
        }

        private void OnGameLose()
        {
            _gameMenu.ShowLoseText();
            _gridGenerator.DisableCards();
        }

        private void RestartGame()
        {
            _gameMenu.RestartGame();
            _gridGenerator.RegenerateGameGrid();
            _movesCounter.Initialize();
            _countdownTimer.Initialize(_startTime);
        }

        private void OnSuccessMatching(int pairsMathed)
        {
            _movesCounter.AddMove();
            _countdownTimer.AddTime(pairsMathed * 5);
        }

        private void OnFailMatching()
        {
            _movesCounter.AddMove();
        }
    }
}