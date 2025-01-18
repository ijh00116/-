using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BlackTree.Core;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;

namespace BlackTree.Bundles
{
    public class ViewCanvasToastMessage : ViewCanvas
    {
        [Header("단순 페이드 토스트")]
        [SerializeField] private TextMeshProUGUI toastMessagedesc;
        [SerializeField] private Image fadePanel;

        [Header("단순 페이드 토스트 쉴드")]
        [SerializeField] private TextMeshProUGUI toastMessagedesc_ShieldStat;
        [SerializeField] private Image fadePanel_ShieldStat;

        [Header("단순 페이드 토스트_스탯")]
        [SerializeField] private TextMeshProUGUI toastMessagedesc_status;
        [SerializeField] private Image fadePanel_status;

        [Header("버튼 팝업")]
        [SerializeField] private GameObject _box;
        [SerializeField] private TextMeshProUGUI _textToastMessage_Title;
        [SerializeField] private BTButton[] _cancelButtons;
        [SerializeField] private BTButton _confirmButton;

        [Header("LocalizedText")]
        [SerializeField] private TMP_Text _textConfirm;
        [SerializeField] private TMP_Text _textCancel;
        [SerializeField] private TMP_Text _textPurchaseRequest;

        private Coroutine _coPurchaseMessage;
        private WaitForSecondsRealtime _wfs = new WaitForSecondsRealtime(1);

        [Header("reward popupToast")]
        [SerializeField] public ViewGoodRewardSlot rewardSlotPrefab;
        [SerializeField] public List<ViewGoodRewardSlot> rewardSlotList = new List<ViewGoodRewardSlot>();
        public Transform rewardParent;
        [SerializeField] CanvasGroup rewardPopup;
        public TMP_Text titleDesc;
        public void ShowBox(string title, string desc,UnityAction checkAction = null, bool isOneButton = false)
        {
            fadePanel.gameObject.SetActive(false);
            _box.SetActive(true);
            _textPurchaseRequest.gameObject.SetActive(true);
            _textPurchaseRequest.text = desc;

            SetVisibleBox(true);
            SetTitle(title);
            SetCheckAction(checkAction);
            SetCancelAction(null);
            SetActiveCancelButton(!isOneButton);
            Open();
        }

        public void Show(string title, UnityAction checkAction, UnityAction cancelAction)
        {
            fadePanel.gameObject.SetActive(false);
            _box.SetActive(true);
            _textPurchaseRequest.gameObject.SetActive(false);

            SetVisibleBox(true);
            SetTitle(title);
            SetCheckAction(checkAction);
            SetCancelAction(cancelAction);
            SetActiveCancelButton(true);
            Open();
        }

        public void ShowPurchaseWaitingMessage()
        {
            _textPurchaseRequest.gameObject.SetActive(true);
            fadePanel.gameObject.SetActive(false);
            _box.SetActive(true);
            SetVisibleBox(false);
            Open(false);

            if (_coPurchaseMessage != null)
                StopCoroutine(_coPurchaseMessage);
            _coPurchaseMessage = StartCoroutine(PurchaseWaitingMessageDuration());
        }

        private IEnumerator PurchaseWaitingMessageDuration()
        {
            float time = 0;
            while (_textPurchaseRequest.gameObject.activeSelf)
            {
                time += Time.unscaledDeltaTime;
                if (time > 12)
                {
                    Close();
                }
                yield return _wfs;
            }
        }

        public void SetLocalizeText()
        {
            //_textConfirm.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ETC_Confirm].StringToLocal;
            //_textCancel.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ETC_Cancel].StringToLocal;
            //_textPurchaseRequest.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ToastMessage_PurchaseRequest].StringToLocal;
        }

        Color originalcolor = new Color(1, 1, 1, 1);
        Color paneloriginalcolor = new Color(0, 0, 0, 1);
        Color panelalphacolor = new Color(0, 0, 0, 0);
        Color alphacolor = new Color(1, 1, 1, 0);
        Coroutine fadeState = null;
        Coroutine fadeState_shieldStat = null;
        Coroutine fadeState_status = null;
        float fadeTime;
        float fadeTime_shieldStat;
        float fadeTime_status;
        public void ShowandFade(string title)
        {
            SetVisible(true);
            toastMessagedesc.text = title;
            fadePanel.gameObject.SetActive(true);
            toastMessagedesc.gameObject.SetActive(true);


            fadePanel.color = paneloriginalcolor;
            toastMessagedesc.color = originalcolor;
            fadeTime = 0;

            SetVisibleBox(false);

            if (fadeState != null)
            {
                StopCoroutine(fadeState);
                fadeState = null;
            }
            fadeState = StartCoroutine(FadeAndOff());
        }

        IEnumerator FadeAndOff()
        {
            while (fadeTime < 1)
            {
                fadeTime += Time.unscaledDeltaTime / 2;
                fadePanel.color = Color.Lerp(paneloriginalcolor, panelalphacolor, fadeTime);
                toastMessagedesc.color = Color.Lerp(originalcolor, alphacolor, fadeTime);
                yield return null;
            }
            fadePanel.gameObject.SetActive(false);
            toastMessagedesc.gameObject.SetActive(false);
        }

        public void ShowandFade_ShieldStat(string title)
        {
            SetVisible(true);
            toastMessagedesc_ShieldStat.text = title;
            fadePanel_ShieldStat.gameObject.SetActive(true);
            toastMessagedesc_ShieldStat.gameObject.SetActive(true);


            fadePanel_ShieldStat.color = paneloriginalcolor;
            toastMessagedesc_ShieldStat.color = originalcolor;
            fadeTime_shieldStat = 0;

            SetVisibleBox(false);

            if (fadeState_shieldStat != null)
            {
                StopCoroutine(fadeState_shieldStat);
                fadeState_shieldStat = null;
            }
            fadeState_shieldStat = StartCoroutine(FadeAndOff_ShieldStat());
        }

        IEnumerator FadeAndOff_ShieldStat()
        {
            while (fadeTime_shieldStat < 1)
            {
                fadeTime_shieldStat += Time.unscaledDeltaTime / 3;
                fadePanel_ShieldStat.color = Color.Lerp(paneloriginalcolor, panelalphacolor, fadeTime_shieldStat);
                toastMessagedesc_ShieldStat.color = Color.Lerp(originalcolor, alphacolor, fadeTime_shieldStat);
                yield return null;
            }
            fadePanel_ShieldStat.gameObject.SetActive(false);
            toastMessagedesc_ShieldStat.gameObject.SetActive(false);
        }

        public void ShowandFade_Status(string title)
        {
            SetVisible(true);
            toastMessagedesc_status.text = title;
            fadePanel_status.gameObject.SetActive(true);
            toastMessagedesc_status.gameObject.SetActive(true);


            fadePanel_status.color = paneloriginalcolor;
            toastMessagedesc_status.color = originalcolor;
            fadeTime_status = 0;

            if (fadeState_status != null)
            {
                StopCoroutine(fadeState_status);
                fadeState_status = null;
            }
            fadeState_status = StartCoroutine(FadeAndOff_Status());
        }

        IEnumerator FadeAndOff_Status()
        {
            while (fadeTime_status < 1)
            {
                fadeTime_status += Time.unscaledDeltaTime;
                fadePanel_status.color = Color.Lerp(paneloriginalcolor, panelalphacolor, fadeTime_status);
                toastMessagedesc_status.color = Color.Lerp(originalcolor, alphacolor, fadeTime_status);
                yield return null;
            }
            SetVisible(false);
        }


        public void Close()
        {
            _box.SetActive(false);
            SetVisible(false);
            //Wrapped.CommonPopupCloseAnimation(() =>
            //{
            //    _box.SetActive(false);
            //    _textPurchaseRequest.gameObject.SetActive(false);
            //    SetVisible(false);
            //});
        }

        private void SetVisibleBox(bool flag)
        {
            _box.SetActive(flag);
            toastMessagedesc.gameObject.SetActive(!flag);
        }

        private void Open(bool isAnimation = true)
        {
            _box.SetActive(true);
            SetVisible(true);
            //if (isAnimation)
              // Wrapped.CommonPopupOpenAnimation();
        }

        private void SetTitle(string name)
        {
            _textToastMessage_Title.text = name;
        }

        private void SetCheckAction(UnityAction action)
        {
            _confirmButton.onClick.RemoveAllListeners();
            if (action != null)
                _confirmButton.onClick.AddListener(() => action?.Invoke());

            _confirmButton.onClick.AddListener(() => Close());
        }

        private void SetCancelAction(UnityAction action)
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveAllListeners();
                if (action != null)
                    button.onClick.AddListener(() => action?.Invoke());

                button.onClick.AddListener(() => Close());
            }
        }

        private void SetActiveCancelButton(bool flag)
        {
            _cancelButtons[0].gameObject.SetActive(flag);
        }


        float rewardPopupfadeTime = 0;
        float waitPopupFadeTime= 0;
        Coroutine rewardPopupfadeState = null;

        public async UniTask RewardPopupShowandFadeAsync()
        {
            await UniTask.Delay(400);
            RewardPopupShowandFade();
        }
        public void RewardPopupShowandFade()
        {
            AudioManager.Instance.Play(AudioSourceKey.GoodsRewarded);
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 _0");
            SetVisible(true);
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 _1_1");
            rewardPopup.gameObject.SetActive(true);
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 _1_2");
            fadePanel.color = paneloriginalcolor;
            rewardPopup.alpha = 1;
            rewardPopupfadeTime = 0;
            waitPopupFadeTime = 0;

            if (rewardPopupfadeState != null)
            {
                StopCoroutine(rewardPopupfadeState);
                rewardPopupfadeState = null;
            }
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 _1_3");
            rewardPopupfadeState = StartCoroutine(RewardPopupFadeAndOff());
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 _1_4");
        }

        IEnumerator RewardPopupFadeAndOff()
        {
            while (rewardPopupfadeTime < 1)
            {
                //Debug.LogError("토스트 메세지 리워드 오픈팝업 _2");
                waitPopupFadeTime += Time.unscaledDeltaTime;
                if(waitPopupFadeTime>=1)
                {
                    rewardPopupfadeTime += Time.unscaledDeltaTime / 2;
                }
                rewardPopup.alpha = Mathf.Lerp(1,0,rewardPopupfadeTime);
                yield return null;
            }
            //Debug.LogError("토스트 메세지 리워드 오픈팝업 _3");
            rewardPopup.gameObject.SetActive(false);
        }
    }
}