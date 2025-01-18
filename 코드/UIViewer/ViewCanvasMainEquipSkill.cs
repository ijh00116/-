using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;
using System.Collections;
using System.Collections.Generic;

namespace BlackTree.Bundles
{
    public class ViewCanvasMainEquipSkill : ViewCanvas
    {
        public ViewSkillEquipslot[] equipedSlotList;

        public BTButton SkillAutoBtn;
        public GameObject skillAutoOffImage;
        public GameObject skillAutoOnImage;

        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;
        Vector2 upPos = new Vector2(0, 85);
        Vector2 downPos = new Vector2(0, 50);

        [Header("skill preset")]
        public BTButton presetBtn;
        public List<GameObject> PresetOnList;

        public GameObject eliteAtkBuffObj;
        public BTButton eliteAtkBuffBtn;
        public GameObject atkbuffDesc;
        private void Awake()
        {
            Player.Guide.tutorialConfigAction += StartPoint;
            Player.Guide.TutorialEndcallback += EndTutorial;

            arrowObject.gameObject.SetActive(false);


            Model.Player.Option.ContentUnlockUpdate += UpdateUIContentUnlock;
            UpdateUIContentUnlock();
        }

        void UpdateUIContentUnlock()
        {
            bool isUnlock = Model.Player.Option.IsContentUIUnlocked(Definition.LockedUIType.MainNavSkillIcon);
            presetBtn.gameObject.SetActive(isUnlock);
        }

        public void SetOnDesc()
        {
            atkbuffDesc.SetActive(true);
            StartCoroutine(ieSetDesc());
        }
        float eliteBuffOnTime = 0;

        IEnumerator ieSetDesc()
        {
            while (true)
            {
                eliteBuffOnTime += Time.deltaTime;
                if(eliteBuffOnTime>=3)
                {
                    eliteBuffOnTime = 0;
                    atkbuffDesc.SetActive(false);
                    break;
                }
                yield return null;
            }
        }
        void StartPoint(Model.TutorialDescData tutoData)
        {
            if (tutoData.descKey != LocalizeDescKeys.None)
            {
                arrowObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }
            if (IsPointTutorial(tutoData) == false)
            {
                arrowObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }

            arrowObject.gameObject.SetActive(true);

            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }

        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isDialogTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.ArrowAutoSkillBtn_0)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
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

        void EndTutorial()
        {
            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);
            arrowObject.gameObject.SetActive(false);
        }
    }

}
