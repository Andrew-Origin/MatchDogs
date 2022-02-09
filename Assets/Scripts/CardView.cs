using System;
using System.Collections;
using UnityEngine;

namespace MatchingDogs.Core
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private int _cardIndex;

        public int CardIndex
        {
            get
            {
                return _cardIndex;
            }
            set
            {
                _cardIndex = value;
            }
        }

        private Action<CardView> _cardSelected;

        private static readonly int FaceTrigger = Animator.StringToHash("FlipFace");
        private static readonly int BackTrigger = Animator.StringToHash("FlipBack");
        

        public void Initialize(Action<CardView> cardSelected)
        {
            _cardSelected = cardSelected;
        }

        public void ChangeSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void FlipBack()
        {
            _animator.SetTrigger(BackTrigger);
        }

        public void FlipFace()
        {
            _animator.SetTrigger(FaceTrigger);
        }

        private void OnMouseDown()
        {
            _cardSelected?.Invoke(this);
        }

        public void UnsubscribeFromEvents()
        {
            _cardSelected = null;
        }

        private void OnDestroy()
        {
            _cardSelected = null;
        }
    }
}