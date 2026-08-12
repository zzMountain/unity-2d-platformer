using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Platformer
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class MenuButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Color _normalColor = new Color(0.08f, 0.14f, 0.24f, 1f);
        [SerializeField] private Color _highlightedColor = new Color(0.12f, 0.72f, 0.66f, 1f);
        [SerializeField] private Color _pressedColor = new Color(1f, 0.38f, 0.33f, 1f);
        [SerializeField] private float _normalScale = 1f;
        [SerializeField] private float _highlightedScale = 1.05f;
        [SerializeField] private float _pressedScale = 0.96f;
        [SerializeField] private float _animationSpeed = 16f;

        private Image _background;
        private RectTransform _rectTransform;
        private Color _targetColor;
        private float _targetScale;
        private bool _isPointerOver;

        private void Awake()
        {
            _background = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            SetState(_normalColor, _normalScale);
            ApplyStateImmediately();
        }

        private void Update()
        {
            float interpolation = 1f - Mathf.Exp(-_animationSpeed * Time.unscaledDeltaTime);
            Vector3 targetScale = Vector3.one * _targetScale;

            _background.color = Color.Lerp(_background.color, _targetColor, interpolation);
            _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, targetScale, interpolation);
        }

        private void OnDisable()
        {
            _isPointerOver = false;
            SetState(_normalColor, _normalScale);
            ApplyStateImmediately();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerOver = true;
            SetState(_highlightedColor, _highlightedScale);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerOver = false;
            SetState(_normalColor, _normalScale);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetState(_pressedColor, _pressedScale);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isPointerOver)
            {
                SetState(_highlightedColor, _highlightedScale);
            }
            else
            {
                SetState(_normalColor, _normalScale);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetState(_highlightedColor, _highlightedScale);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_isPointerOver == false)
                SetState(_normalColor, _normalScale);
        }

        private void SetState(Color color, float scale)
        {
            _targetColor = color;
            _targetScale = scale;
        }

        private void ApplyStateImmediately()
        {
            _background.color = _targetColor;
            _rectTransform.localScale = Vector3.one * _targetScale;
        }
    }
}
