using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;
using TMPro;
using DG.Tweening;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewCanvasSleepMode : ViewCanvas
    {
        [SerializeField] private Image[] _moonFadeImages;

        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _ingText;
        [SerializeField] private TextMeshProUGUI _waveInStageText;

        [SerializeField] private TextMeshProUGUI _goldEarnText;
        [SerializeField] private TextMeshProUGUI _expEarnText;
        private Color _color;

        private Vector2 _originPosition;

        [SerializeField] public Image _character_0;
        [SerializeField] public Image _character_1;

        [SerializeField] public Sprite[] character_0Images;
        [SerializeField] public Sprite[] character_1Images;

        [SerializeField] CanvasGroup canvasgroup;

        public Transform slotParent;
        public ViewGoodRewardSlot rewardSlotPrefab;
        public ViewGoodRewardSlot rewardSlotPrefab_skill;
        public ViewGoodRewardSlot rewardSlotPrefab_pet;

        public GameObject earnItemObj;

        public TMP_Text descTxt;
        public ViewCanvasSleepMode Init()
        {
            descTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_UnlockSaveMode].StringToLocal;
            return this;
        }

        public ViewCanvasSleepMode SetInit()
        {
            canvasgroup.alpha = 1;
            

            return this;
        }

        public ViewCanvasSleepMode SetCanvasAlpha(float clamp)
        {
          
            canvasgroup.alpha = clamp;

            return this;
        }

        public ViewCanvasSleepMode SetOrigin()
        {
            
            canvasgroup.DOFade(1, 0.3f);

            return this;
        }

        public ViewCanvasSleepMode SetTimeText(string text)
        {
            _timeText.text = text;
            return this;
        }

        public ViewCanvasSleepMode SetWaveInStageText(string text)
        {
            _waveInStageText.text = text;
            return this;
        }

        public ViewCanvasSleepMode SetGoldText(string text)
        {
            _goldEarnText.text = text;
            return this;
        }

        public ViewCanvasSleepMode SetExpText(string text)
        {
            _expEarnText.text = text;
            return this;
        }
    }
}
