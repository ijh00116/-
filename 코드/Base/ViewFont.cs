using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BlackTree.Bundles
{
    public class ViewFont : ViewBase
    {
        public RectTransform RectTr => _rectTr;
        public TMP_Text Text => _text;

        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _rectTr;

        public ViewFont SetText(string text)
        {
            _text.text = text;
            return this;
        }

        public ViewFont SetPosition(Vector2 pos)
        {
            transform.position = pos;
            return this;
        }
    }
}