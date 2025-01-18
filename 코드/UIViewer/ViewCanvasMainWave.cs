using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System.Collections;

namespace BlackTree.Bundles
{
    public class ViewCanvasMainWave : ViewCanvas
    {
        [Header("Stage")]
        public TMPro.TMP_Text chapterInfoText;
        public TMPro.TMP_Text leftEnemyText;
        public GameObject BossObj;

        public GameObject monsterImage;

        public GameObject goToBossObj;
        public BTButton goToBossBtn;

        [SerializeField] public Slider progressingWave;
        public BTButton stageBtn;
        public GameObject stageRedDot;

        public BTButton FeverTimeBtn;

        public CanvasGroup feverTimecanvas;
        public TMP_Text feverTimeDesc;

        public TMP_Text stageMoveTxt;
        public TMP_Text bossChallengeTxt;
        public TMP_Text feverTime;

        Coroutine descCoroutine;
        public void SetLeftEnemy()
        {
            chapterInfoText.text = string.Format("{0}-{1}", Battle.Field.CurrentFieldChapter+1, Battle.Field.CurrentFieldStage+1);
            leftEnemyText.text = string.Format("{0}",Battle.Field.leftEnemy);
            progressingWave.value = (float)(Battle.Field.TotalCountEnemyinStage - Battle.Field.leftEnemy) / (float)Battle.Field.TotalCountEnemyinStage;

         
        }

        public void AnimateDesc()
        {
            if(descCoroutine!=null)
            {
                StopCoroutine(descCoroutine);
                descCoroutine = null;
            }

            descCoroutine = StartCoroutine(UIProcess());
        }

        float currentTime = 0;
        enum UIState
        {
            On, Off
        }
        UIState _state = UIState.On;
        IEnumerator UIProcess()
        {
            feverTimecanvas.gameObject.SetActive(true);
            float activeCurrentTime = 0;
            while (true)
            {
                switch (_state)
                {
                    case UIState.On:
                        currentTime += Time.deltaTime;
                        feverTimecanvas.alpha = Mathf.Lerp(0.5f, 1.0f, currentTime);
                        if (currentTime >= 1)
                        {
                            _state = UIState.Off;
                            currentTime = 0;
                        }
                        break;
                    case UIState.Off:
                        currentTime += Time.deltaTime;
                        feverTimecanvas.alpha = Mathf.Lerp(1.0f, 0.5f, currentTime);
                        if (currentTime >= 1)
                        {
                            _state = UIState.On;
                            currentTime = 0;
                        }
                        break;
                    default:
                        break;
                }

                activeCurrentTime += Time.deltaTime;
                if (activeCurrentTime >= 3)
                {
                    feverTimecanvas.gameObject.SetActive(false);
                    break;
                }

                yield return null;
            }
        }
    }
}

