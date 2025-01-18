using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ObtainGood : MonoBehaviour
    {
        public GoodsKey GoodsType => _key;
        public RectTransform Icon => _icon;
        public UnityAction OnAddAnimation;

        [SerializeField] private GoodsKey _key;

        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _textValue;
        [SerializeField] private RectTransform _icon;

        private Coroutine _coUpdateView;
        private Coroutine _coAddAnimation;

        private float _animationScaleSize = 0.3f;
        private double _traceValue;
        private double _value;

        public void Init()
        {
            _iconImage.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)_key];
            OnAddAnimation += StartRoutineAddAnimation;
        }

        public void StartRoutineUpdateView(double value)
        {
            _value = value;

            if (_coUpdateView != null)
            {
                StopCoroutine(_coUpdateView);
            }
            _coUpdateView = StartCoroutine(CoUpdateView());
        }

        private void StartRoutineAddAnimation()
        {
            if (_coAddAnimation != null) StopCoroutine(_coAddAnimation);
            _coAddAnimation = StartCoroutine(CoAddAnimation());
        }

        private IEnumerator CoAddAnimation()
        {
            _icon.localScale = Vector3.one;
            float maxScale = _animationScaleSize + _icon.localScale.x;

            while (Mathf.Abs(_icon.localScale.x - maxScale) > 0.01f)
            {
                _icon.localScale = Vector3.one * Mathf.Lerp(_icon.localScale.x, maxScale, 0.45f);
                yield return null;
            }

            while (_icon.localScale.x > 1.01f)
            {
                _icon.localScale = Vector3.one * Mathf.Lerp(_icon.localScale.x, 1f, 0.45f);
                yield return null;
            }
        }

        private IEnumerator CoUpdateView()
        {
            while (true)
            {
                if (_traceValue < _value - 1)
                    _traceValue = Core.BTHelper.Lerp(_traceValue + 1, _value, Time.deltaTime * 20f);
                else if (_traceValue > _value + 1)
                    _traceValue = Core.BTHelper.Lerp(_traceValue - 1, _value, Time.deltaTime * 20f);
                else
                    _traceValue = _value;

                _textValue.text = _traceValue.ToNumberString();

                yield return null;

            }
        }

        public void SyncUpdate(double value)
        {
            _value = value;
            _textValue.text = _value.ToNumberString();
        }
    }
}