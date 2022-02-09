using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchingDogs.Core
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private float _startDelay;
        [SerializeField] private float _wrongMatchingCloseTime;

        private bool _canInteract = true;
        private List<CardView> _activeCards;
        private CardView _activeCard;

        private int _needCardsCount;

        private event Action _failMatching;
        private event Action<int> _successMatching;
        public Action GameFinished;
        private int _successPairs;
        private Vector2Int _gridSize;

        public void Initialize(Difficulty difficulty, Vector2Int gridSize)
        {
            _activeCards = new List<CardView>();

            _needCardsCount = (int) difficulty;

            _successPairs = 0;

            _gridSize = gridSize;

            StartCoroutine(StartDelay());
        }

        public void SubscribeToEvents(Action<int> successMatching, Action failMatching)
        {
            _failMatching = failMatching;
            _successMatching = successMatching;
        }

        public void MatchCards(CardView newCard)
        {
            if (!_canInteract) return;

            newCard.FlipFace();

            if (_activeCard == null)
            {
                _activeCard = newCard;
                _activeCards.Add(_activeCard);
                return;
            }

            _activeCards.Add(_activeCard);

            if (newCard.CardIndex == _activeCard.CardIndex && _activeCards.Count == _needCardsCount)
            {
                foreach (var card in _activeCards)
                {
                    card.UnsubscribeFromEvents();
                }

                _activeCard = null;
                _activeCards.Clear();
                _successMatching?.Invoke(_needCardsCount / 2);
                _successPairs++;

                if ((_successPairs * _needCardsCount) == _gridSize.x * _gridSize.y)
                {
                    Debug.LogError("Game Has Finished");
                    GameFinished?.Invoke();
                }
            }
            else if (newCard.CardIndex == _activeCard.CardIndex && _activeCards.Count != _needCardsCount)
            {
                _activeCard = newCard;
            }
            else
            {
                CloseNotMatchedCards(newCard);
                _failMatching?.Invoke();
            }
        }

        private void CloseNotMatchedCards(CardView cardView)
        {
            StartCoroutine(CloseCardsRoutine(cardView));
        }

        private IEnumerator CloseCardsRoutine(CardView cardView)
        {
            _canInteract = false;
            float elapsedTime = 0;

            while (elapsedTime < _wrongMatchingCloseTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cardView.FlipBack();

            foreach (var card in _activeCards)
                card.FlipBack();

            _activeCard = null;
            _activeCards.Clear();
            _canInteract = true;
        }

        private IEnumerator StartDelay()
        {
            float elapsedTime = 0;
            _canInteract = false;

            while (elapsedTime < _startDelay)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _canInteract = true;
        }
    }
}