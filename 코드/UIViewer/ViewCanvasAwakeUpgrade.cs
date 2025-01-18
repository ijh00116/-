using BlackTree.Core;
using BlackTree.Definition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewCanvasAwakeUpgrade : ViewCanvas
    {
        public ScrollRect scrollRect;

        public ObtainGood awakestoneObtain;

        public BTSelector lvTypeSelector;
        public BTButton lvup_1;
        public BTButton lvup_10;
        public BTButton lvup_max;

        [Header("Advancement")]
        public BTButton advanceBtn;

        public GameObject advancementObject;
        public Image advancementBG;
        public RectTransform advancementWindow;

        public ViewAdvanceSlot advanceSlotPrefab;

        public Transform[] slotParentList;
        public GameObject[] lockedObject;

        public TMP_Text advanceTitle;
        public TMP_Text advanceDesc;

        public BTButton[] advanceWindowCloseBtns;

        public BTButton AdvancementProgressBtn;
        public BTButton changeAdvanceProgressBtn;

        [Header("advanceDetail")]
        public GameObject advanceChangeWindow;
        public Image changeImage;
        public TMP_Text desc;
        public BTButton changeBtninDetail;
        public TMP_Text changeCost;
        public BTButton[] closeDetailBtn;

        [Header("ArrowObject")]
        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;
        Vector2 upPos = new Vector2(0, 130);
        Vector2 downPos = new Vector2(0, 70);

        public TMP_Text advanceBtnTxt;
        public TMP_Text advanceWindowTitleTxt;
        public TMP_Text advanceWindowDescTxt;
        public TMP_Text advanceWindowBtnTxt;
        public ViewCanvasAwakeUpgrade SetDesc(string text)
        {
            return this;
        }

        public void DetailInfoUpdate(int grade,int index)
        {
            var advanceData = StaticData.Wrapper.advancementDatas[index];
            string desc = null;

            
            for (int i=0; i< advanceData.abilityType.Length; i++)
            {
                var descData = string.Format(StaticData.Wrapper.localizeddesclist[(int)advanceData.descLmk[i]].StringToLocal, advanceData.abilityValue[i]);
                desc += descData + "\n";
            }
            advanceDesc.text = desc;
            advanceTitle.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AdvanceDetail].StringToLocal, grade + 1);
        }
        public void SetArrowObject()
        {
            arrowObject.gameObject.SetActive(true);
            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }

        public void OffArrowObject()
        {
            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);
            arrowObject.gameObject.SetActive(false);
        }


        enum moveType
        {
            up, down
        }
        moveType movetype;
        float currenttime;
        private IEnumerator RoutineMoveArrow()
        {
            movetype = moveType.up;
            currenttime = 0;
            while (true)
            {
                switch (movetype)
                {
                    case moveType.up:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(downPos, upPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                        break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(upPos, downPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.up;
                            currenttime = 0;
                        }
                        break;
                    default:
                        break;
                }
                yield return null;
            }
        }
    }
}
