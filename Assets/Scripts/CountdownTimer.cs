using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownTimerText;

    private float _timeLeft;
    private bool _gameStarted;

    public Action OnTimeLeft;
    
    public void Initialize(float startTime)
    {
        _timeLeft = startTime;
        _countdownTimerText.text = _timeLeft.ToString("0");
    }

    public void StartCountdown()
    {
        _gameStarted = true;
    }

    public void StopTimer()
    {
        _gameStarted = false;
    }
    
    public void AddTime(float addTime)
    {
        _timeLeft += addTime;
    }

    private void Update()
    {
        if (!_gameStarted) return;

        _countdownTimerText.text = _timeLeft.ToString("0");
        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0)
        {
            OnTimeLeft?.Invoke();
            _gameStarted = false;
        }
    }
}