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
    public class ViewCanvasAwakeGaugeBar : ViewCanvas
    {
        public GameObject gaugeBarObj;
        public Slider gaugeBar;
        public TMP_Text gaugeBarTitle;
        public TMP_Text gaugeText;

        public CanvasGroup canvasGroup;
        public TMP_Text awakeDesc;

        [SerializeField]
        public GameObject touchObj;
        public GameObject touchImageObj;

        public GameObject LockedObj;

        public TMP_Text awakeTouch;
        public void Init()
        {
            Player.Guide.tutorialConfigAction += StartTouchinput;
            Player.Guide.TutorialEndcallback += EndTutorial;

            Player.Option.ContentUnlockUpdate += LockUpdate;

            awakeTouch.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_CharacterTransform].StringToLocal;
            LockUpdate();
        }

        void LockUpdate()
        {
            int skillawakeUnlockLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.CharacterAwakeQuestUnlock].unLockLevel;
            if (Player.Quest.mainQuestCurrentId >= skillawakeUnlockLv)
            {
                gaugeBarObj.SetActive(true);
                //LockedObj.SetActive(false);
            }
            else
            {
                gaugeBarObj.SetActive(false);
                //LockedObj.SetActive(true);
            }

        }

        Coroutine touchAnim;
        void StartTouchinput(Model.TutorialDescData tutoData)
        {
            if (tutoData.descKey != LocalizeDescKeys.None)
            {
                touchObj.SetActive(false);
                if (touchAnim != null)
                    StopCoroutine(touchAnim);
                return;
            }
            if (IsPointTutorial(tutoData) == false)
            {
                touchObj.SetActive(false);
                if (touchAnim != null)
                    StopCoroutine(touchAnim);
                return;
            }


            touchObj.SetActive(true);

            if (touchAnim != null)
                StopCoroutine(touchAnim);
            touchImageObj.transform.localScale = minscale;
            phase = Phase.min;
            touchAnim = StartCoroutine(RoutineTouchAnim());
        }

        void EndTutorial()
        {
            if (touchObj.activeInHierarchy)
                touchObj.SetActive(false);
        }
        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isDialogTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.CharacterAwakeTouch_2)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        enum Phase
        {
            min,max,
        }
        float currentTime= 0;

        Vector3 minscale = new Vector3(0.8f, 0.8f, 0.8f);
        Vector3 maxscale = new Vector3(1.2f, 1.2f, 1.2f);
        Phase phase = Phase.min;
        IEnumerator RoutineTouchAnim()
        {
            phase = Phase.min;
            while (true)
            {
                switch (phase)
                {
                    case Phase.min:
                        currentTime += Time.deltaTime*2;
                        touchImageObj.transform.localScale = Vector3.Lerp(minscale, maxscale,currentTime);
                        if(currentTime>=1)
                        {
                            currentTime = 0;
                            phase = Phase.max;
                        }
                        break;
                    case Phase.max:
                        currentTime += Time.deltaTime * 2;
                        touchImageObj.transform.localScale = Vector3.Lerp(maxscale, minscale, currentTime);
                        if (currentTime >= 1)
                        {
                            currentTime = 0;
                            phase = Phase.max;
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
