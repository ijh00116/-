using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;
using BackEnd;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using Google.Play.Review;
#endif

namespace BlackTree.Core
{
    public class ControllerReviewPopup
    {
        private ViewCanvasReview _viewCanvasoption;
        private CancellationTokenSource _cts;
        int currentStarCount = 4;

        const string PlayStoreLink = "market://details?id=com.blacktree.idlehero";
        const string AppStoreLink = "market://details?id=com.blacktree.idlehero";
        public ControllerReviewPopup(Transform parent, CancellationTokenSource cts)
        {
            _viewCanvasoption = ViewCanvas.Create<ViewCanvasReview>(parent);

            for(int i=0; i<_viewCanvasoption.closeBtns.Length; i++)
            {
                int index = i;
                _viewCanvasoption.closeBtns[index].onClick.AddListener(() => {
                    _viewCanvasoption.blackBG.PopupCloseColorFade();
                    _viewCanvasoption.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewCanvasoption.SetVisible(false);
                    });
                });
            }

            for (int i = 0; i < _viewCanvasoption.realReviewCloseBtns.Length; i++)
            {
                int index = i;
                _viewCanvasoption.realReviewCloseBtns[index].onClick.AddListener(() => {

                    string review = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ThanksReview].StringToLocal;
                    var toast = ViewCanvas.Get<ViewCanvasToastMessage>();
                    toast.ShowandFade(review);

                    _viewCanvasoption.realReviewPopup.SetActive(false);
                    _viewCanvasoption.blackBG.PopupCloseColorFade();
                    _viewCanvasoption.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewCanvasoption.SetVisible(false);
                    });
                });
            }

            currentStarCount = 4;
            for (int j = 0; j < _viewCanvasoption.starObjList.Length; j++)
            {
                if(j<=currentStarCount)
                {
                    _viewCanvasoption.starObjList[j].SetActive(true);
                }
                else
                {
                    _viewCanvasoption.starObjList[j].SetActive(false);
                }
            }

            for (int i=0; i< _viewCanvasoption.starList.Length; i++)
            {
                int index = i;

                _viewCanvasoption.starList[index].onClick.AddListener(()=> {

                    currentStarCount = index;
                    for (int j = 0; j < _viewCanvasoption.starObjList.Length; j++)
                    {
                        if (j <= currentStarCount)
                        {
                            _viewCanvasoption.starObjList[j].SetActive(true);
                        }
                        else
                        {
                            _viewCanvasoption.starObjList[j].SetActive(false);
                        }
                    }
                });
            }

            _viewCanvasoption.confirmBtn.onClick.AddListener(PushConfirmBtn);
            _viewCanvasoption.realReviewConfirm.onClick.AddListener(GoRealReview);

            _viewCanvasoption.reviewTitle.text=StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Review].StringToLocal;
            _viewCanvasoption.reviewDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ReviewDesc_1].StringToLocal;
            _viewCanvasoption.reviewConfirm.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Confirm].StringToLocal;

            _viewCanvasoption.storeReviewTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_StoreReview].StringToLocal;
            _viewCanvasoption.storeReviewDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ReviewDesc_2].StringToLocal;
            _viewCanvasoption.storereviewConfirm.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Confirm].StringToLocal;
            _viewCanvasoption.storereviewCancel.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Cancel].StringToLocal;
        }

        void PushConfirmBtn()
        {
            if(currentStarCount<4)
            {
                string review = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ThanksReview].StringToLocal;
                var toast = ViewCanvas.Get<ViewCanvasToastMessage>();
                toast.ShowandFade(review);

                _viewCanvasoption.blackBG.PopupCloseColorFade();
                _viewCanvasoption.Wrapped.CommonPopupCloseAnimationDown(() => {
                    _viewCanvasoption.SetVisible(false);
                });
            }
            else
            {
                _viewCanvasoption.realReviewPopup.SetActive(true);
            }
        }

        void GoRealReview()
        {
#if UNITY_ANDROID
            var reviewManager = new ReviewManager();

            // start preloading the review prompt in the background
            var playReviewInfoAsyncOperation = reviewManager.RequestReviewFlow();

            // define a callback after the preloading is done
            playReviewInfoAsyncOperation.Completed += playReviewInfoAsync => {

                if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
                {

                    // display the review prompt
                    var playReviewInfo = playReviewInfoAsync.GetResult();
                    reviewManager.LaunchReviewFlow(playReviewInfo);
                    Debug.LogError("¸®ºäÆË¾÷ ¿ÀÇÂ");
                }
                else
                {
                    Debug.LogError("¸®ºä¾ÈµÊ");
                    // handle error when loading review prompt
                }
                string review = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_ThanksReview].StringToLocal;
                var toast = ViewCanvas.Get<ViewCanvasToastMessage>();
                toast.ShowandFade(review);

                _viewCanvasoption.realReviewPopup.SetActive(false);
                _viewCanvasoption.blackBG.PopupCloseColorFade();
                _viewCanvasoption.Wrapped.CommonPopupCloseAnimationDown(() => {
                    _viewCanvasoption.SetVisible(false);
                });
            };
#elif UNITY_IOS
            if (!UnityEngine.iOS.Device.RequestStoreReview())
            {
                Application.OpenURL(AppStoreLink);
            }
#endif

        }
    }


}
