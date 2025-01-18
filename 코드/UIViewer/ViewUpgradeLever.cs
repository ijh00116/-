using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace BlackTree.Bundles
{
    public class ViewUpgradeLever : ViewBase
    {
        [SerializeField] private GameObject[] _leverSprites;
        [SerializeField] private BTButton _leverBtn;
        [SerializeField] private TextMeshProUGUI _textAuto;

        private bool _isOn = false;

        public void IsOn()
        {
            _isOn = !_isOn;
            _leverSprites[0].gameObject.SetActive(_isOn);
            _leverSprites[1].gameObject.SetActive(!_isOn);
        }

        public void SetOnClick(UnityAction action)
        {
            _leverBtn.onClick.RemoveAllListeners();
            _leverBtn.onClick.AddListener(IsOn);

            AutoUpgrade(action).Forget();
        }

        public void SetTextAuto(string text)
        {
            _textAuto.text = text;
        }

        async UniTaskVoid AutoUpgrade(UnityAction action)
        {
            while (true)
            {
                if (_isOn)
                    action?.Invoke();
                await UniTask.Delay(80, true);
            }
        }
    }
}