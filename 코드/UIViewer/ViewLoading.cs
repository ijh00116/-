using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;


namespace BlackTree.Bundles
{
    public class ViewLoading : ViewCanvas
    {
        [SerializeField] private float _animTime;
        [SerializeField] private Image _curtain;
        [SerializeField] GameObject LoadingAnim;

        [HideInInspector] public float maxInvokeTime = 0;
        private float currentInvokeTime = 0;

        public async UniTaskVoid Action(UnityAction action, CancellationTokenSource cts, int orderInLayer = 30000)
        {
            LoadingAnim.SetActive(true);
            GetComponent<Canvas>().sortingOrder = orderInLayer;
            SetVisible(true);
            await FadeIn(cts);
            LoadingAnim.SetActive(currentInvokeTime < maxInvokeTime);
            while (currentInvokeTime < maxInvokeTime)
            {
                currentInvokeTime += Time.unscaledDeltaTime;
                await UniTask.Yield(cts.Token);
            }
            LoadingAnim.SetActive(false);

            WorldUIManager.Instance.introBlackBG.SetActive(false);
            action.Invoke();
            FadeOut(cts).Forget();
        }

        public async UniTaskVoid SimpleAction(UnityAction action, CancellationTokenSource cts, int orderInLayer = 30000)
        {
            LoadingAnim.SetActive(false);

            GetComponent<Canvas>().sortingOrder = orderInLayer;
            SetVisible(true);
            await FadeIn(cts);
            //LoadingAnim.SetActive(currentInvokeTime < maxInvokeTime);
            while (currentInvokeTime < maxInvokeTime)
            {
                currentInvokeTime += Time.unscaledDeltaTime;
                await UniTask.Yield(cts.Token);
            }
            

            WorldUIManager.Instance.introBlackBG.SetActive(false);
            action.Invoke();
            FadeOut(cts).Forget();
        }


        public void SetInvokeTime(float max, float current = 0)
        {
            maxInvokeTime = max;
            currentInvokeTime = current;
        }

        private async UniTask FadeOut(CancellationTokenSource cts)
        {
            float time = 0f;
            Color fadeColor = Color.black;

            while (fadeColor.a > 0)
            {
                time += Time.unscaledDeltaTime / _animTime;
                fadeColor.a = Mathf.Lerp(1, 0, time);
                _curtain.color = fadeColor;
                await UniTask.Yield(cts.Token);
            }
            SetVisible(false);
        }

        public async UniTask FadeIn(CancellationTokenSource cts)
        {
            float time = 0f;
            Color fadeColor = Color.black;
            fadeColor.a = 0;

            while (fadeColor.a < 1)
            {
                time += Time.unscaledDeltaTime / _animTime;
                fadeColor.a = Mathf.Lerp(0, 1, time);
                _curtain.color = fadeColor;
                await UniTask.Yield(cts.Token);
            }
        }

        public async UniTask FadeIn(CancellationTokenSource cts,System.Action callback)
        {
            float time = 0f;
            Color fadeColor = Color.black;
            fadeColor.a = 0;

            while (fadeColor.a < 1)
            {
                time += Time.unscaledDeltaTime / _animTime;
                fadeColor.a = Mathf.Lerp(0, 1, time);
                _curtain.color = fadeColor;
                await UniTask.Yield(cts.Token);
            }

            callback?.Invoke();
        }
    }
}
