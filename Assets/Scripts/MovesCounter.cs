using TMPro;
using UnityEngine;

namespace MatchingDogs.Core
{
    public class MovesCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _movesCounterText;
        private int _movesCount;

        public void Initialize()
        {
            _movesCount = 0;
            UpdateMovesCounter();
        }

        public void AddMove()
        {
            _movesCount++;
            UpdateMovesCounter();
        }

        private void UpdateMovesCounter()
        {
            _movesCounterText.text = _movesCount.ToString("0");
        }
    }
}