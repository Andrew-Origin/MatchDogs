using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace MatchingDogs.Core
{
    public enum Difficulty
    {
        Diff1 = 2,
        Diff2 = 4,
        Diff3 = 8
    }

    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private BoardController _boardController;

        private const float CARD_OFFSET = 0.5f;

        [SerializeField] private List<Sprite> _possibleSprites;
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _cardsContainer;
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private Vector2 _cardsSpacing;
        [SerializeField] private float _startMemorizeTime;
        private List<Sprite> _tempPossibleSprites;
        private Difficulty _difficulty;

        private List<Sprite> _usedSprites;
        private List<CardView> _allCards;

        private CardView[,] _grid;

        public Action GameStarted;

        public void GenerateGameGrid(Difficulty difficulty)
        {
            _difficulty = difficulty;

            CenterCardsContainer();
            GenerateGrid();
            RandomizeCards();
            ShuffleCardsPositions();
            StartCoroutine(FlipAll());
        }

        public void RegenerateGameGrid()
        {
            foreach (var card in _grid)
                Destroy(card.gameObject);

            GenerateGameGrid(_difficulty);
        }

        private void CenterCardsContainer()
        {
            float xPos = (_gridSize.x + (_gridSize.x - 1) * _cardsSpacing.x) / 2;
            float yPos = (_gridSize.y + (_gridSize.y - 1) * _cardsSpacing.y) / 2;

            _cardsContainer.transform.position = new Vector3(-(xPos - CARD_OFFSET), -(yPos - CARD_OFFSET), 0);
        }

        [ContextMenu("Generate Grid")]
        private void GenerateGrid()
        {
            _grid = new CardView[_gridSize.y, _gridSize.x];
            _allCards = new List<CardView>();

            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                    _grid[y, x] = SpawnCard(new Vector2Int(x, y));
            }
        }

        private CardView SpawnCard(Vector2Int spawnCoords)
        {
            GameObject cardObject = Instantiate(_cardPrefab, _cardsContainer);
            Vector3 spawnPos = new Vector3(spawnCoords.x + _cardsSpacing.x * spawnCoords.x,
                spawnCoords.y + _cardsSpacing.y * spawnCoords.y, 0);

            cardObject.transform.localPosition = spawnPos;
            cardObject.transform.localRotation = Quaternion.identity;

            CardView cardView = cardObject.GetComponent<CardView>();

            cardView.Initialize(_boardController.MatchCards);

            _allCards.Add(cardView);

            return cardView;
        }


        private int _randomizePointer;

        [ContextMenu("Randomize")]
        private void RandomizeCards()
        {
            _tempPossibleSprites = new List<Sprite>();

            foreach (var sprite in _possibleSprites)
                _tempPossibleSprites.Add(sprite);

            _usedSprites = new List<Sprite>();
            _randomizePointer = _allCards.Count / (int) _difficulty;

            for (int i = 0; i < _allCards.Count / (int) _difficulty; i++)
            {
                Sprite cardSprite = GetRandomSprite();
                _usedSprites.Add(cardSprite);
                _tempPossibleSprites.Remove(cardSprite);

                _allCards[i].ChangeSprite(cardSprite);
                _allCards[i].CardIndex = _usedSprites.IndexOf(cardSprite);

                for (int z = 0; z < (int) _difficulty - 1; z++)
                {
                    _allCards[_randomizePointer].ChangeSprite(cardSprite);
                    _allCards[_randomizePointer].CardIndex = _allCards[i].CardIndex;

                    _randomizePointer++;
                }
            }
        }

        [ContextMenu("Shuffle Cards")]
        private void ShuffleCardsPositions()
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                {
                    Vector3 temp = _grid[y, x].transform.position;

                    int randomX = UnityEngine.Random.Range(0, _gridSize.x);
                    int randomY = UnityEngine.Random.Range(0, _gridSize.y);

                    _grid[y, x].transform.position = _grid[randomY, randomX].transform.position;
                    _grid[randomY, randomX].transform.position = temp;
                    _grid[y, x].name = _grid[y, x].CardIndex.ToString();
                }
            }
        }

        [ContextMenu("Flip All")]
        private IEnumerator FlipAll()
        {
            yield return new WaitForSeconds(_startMemorizeTime);

            for (int y = 0; y < _gridSize.y; y++)
            {
                for (int x = 0; x < _gridSize.x; x++)
                    _grid[y, x].FlipBack();
            }

            _boardController.Initialize(_difficulty, _gridSize);
            GameStarted?.Invoke();
        }


        private Sprite GetRandomSprite()
        {
            return _tempPossibleSprites[UnityEngine.Random.Range(0, _tempPossibleSprites.Count)];
        }

        public void DisableCards()
        {
            foreach (var card in _allCards)
            {
                card.UnsubscribeFromEvents();
            }
        }
    }
}