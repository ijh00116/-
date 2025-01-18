using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerAdsBuff 
    {
        public ViewCanvasAdBuff _viewAdBuff;
        CancellationTokenSource _cts;
        ViewCanvasMainIcons _viewMainIcon;
        float updatetime = 0;

        List<TextPhase> phaseList = new List<TextPhase>();
        List<float> currentTimeList = new List<float>();
        public ControllerAdsBuff(Transform parent, CancellationTokenSource cts)
        {
            _viewAdBuff = ViewCanvas.Create<ViewCanvasAdBuff>(parent);
            _viewMainIcon = ViewCanvas.Get<ViewCanvasMainIcons>();
            _cts = cts;

            string titleTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AdBuff].StringToLocal;
            _viewAdBuff.titleTxt.text = titleTxt;
            for (int i=0; i< _viewAdBuff.closeButton.Length; i++)
            {
                int index = i;
                _viewAdBuff.closeButton[index].onClick.AddListener(CloseWindow);
            }

            for (int i=0; i< _viewAdBuff.buffIconList.Length; i++)
            {
                int index = i;
                _viewAdBuff.buffIconList[index].buffButton.onClick.AddListener(() =>
                {
                    if(Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        Model.Player.AdsBuff.ActivateBuff(_viewAdBuff.buffIconList[index].abilKey, true);
                        _viewAdBuff.buffIconList[index].buffButton.gameObject.SetActive(false);
                        _viewAdBuff.buffIconList[index].leftTime.gameObject.SetActive(true);
                        _viewAdBuff.buffIconList[index].activatedParticle.gameObject.SetActive(true);
                        Player.Quest.TryCountUp(Definition.QuestType.ActivateAdBuff, 1);
                        Player.Unit.StatusSync();
                    }
                    else
                    {
                        if(Player.Cloud.adsBuffData.adsFreeBuffComplete[index]==false)
                        {
                            Model.Player.AdsBuff.ActivateBuff(_viewAdBuff.buffIconList[index].abilKey, true);
                            _viewAdBuff.buffIconList[index].buffButton.gameObject.SetActive(false);
                            _viewAdBuff.buffIconList[index].leftTime.gameObject.SetActive(true);
                            _viewAdBuff.buffIconList[index].activatedParticle.gameObject.SetActive(true);
                            Player.Quest.TryCountUp(Definition.QuestType.ActivateAdBuff, 1);
                            Player.Unit.StatusSync();
                            Player.Cloud.adsBuffData.adsFreeBuffComplete[index] = true;
                            string activateTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Activate].StringToLocal;
                            _viewAdBuff.buffIconList[index].btnText.text = activateTxt;
                        }
                        else
                        {
                            AdmobManager.Instance.ShowRewardedAd(() => {
                                Model.Player.AdsBuff.ActivateBuff(_viewAdBuff.buffIconList[index].abilKey, true);
                                _viewAdBuff.buffIconList[index].buffButton.gameObject.SetActive(false);
                                _viewAdBuff.buffIconList[index].leftTime.gameObject.SetActive(true);
                                _viewAdBuff.buffIconList[index].activatedParticle.gameObject.SetActive(true);
                                Player.Quest.TryCountUp(Definition.QuestType.ActivateAdBuff, 1);
                                Player.Unit.StatusSync();
                            });
                        }
                    }
                });

                if (Player.Cloud.adsBuffData.adsFreeBuffComplete[index] == false)
                {
                    string activateTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Activate_Free].StringToLocal;
                    _viewAdBuff.buffIconList[index].btnText.text = activateTxt;
                }
                else
                {
                    string activateTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Activate].StringToLocal;
                    _viewAdBuff.buffIconList[index].btnText.text = activateTxt;
                }

                if (_viewAdBuff.buffIconList[index].abilKey==AdsBuffType.AttackIncrease)
                {
                    string abilTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AttackDouble].StringToLocal;
                    _viewAdBuff.buffIconList[index].abilText.text = abilTxt;
                }
                if (_viewAdBuff.buffIconList[index].abilKey == AdsBuffType.ShieldInrease)
                {
                    string abilTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ShieldDouble].StringToLocal;
                    _viewAdBuff.buffIconList[index].abilText.text = abilTxt;
                }
                if (_viewAdBuff.buffIconList[index].abilKey == AdsBuffType.CoinGetIncrease)
                {
                    string abilTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoldDouble].StringToLocal;
                    _viewAdBuff.buffIconList[index].abilText.text = abilTxt;
                }
                if (_viewAdBuff.buffIconList[index].abilKey == AdsBuffType.ExpGetIncrease)
                {
                    string abilTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Exp].StringToLocal+"x2";
                    _viewAdBuff.buffIconList[index].abilText.text = abilTxt;
                }
              

            }

            for (int i = 0; i < Player.Cloud.adsBuffData.adsValudata.Count; i++)
            {
                if (Player.Cloud.adsBuffData.adsValudata[i] == Player.AdsBuff.activatedBuffValuedata)
                {
                    _viewAdBuff.buffIconList[i].buffButton.gameObject.SetActive(false);
                    _viewAdBuff.buffIconList[i].leftTime.gameObject.SetActive(true);
                    _viewAdBuff.buffIconList[i].activatedParticle.gameObject.SetActive(true);

                    int min = (int)(Player.Cloud.adsBuffData.adsLeftTimeData[i] / 60);
                    int sec = (int)(Player.Cloud.adsBuffData.adsLeftTimeData[i] % 60);

                    _viewAdBuff.buffIconList[i].leftTime.text = string.Format("{0:D2}:{1:D2}", min, sec);
                }
                else
                {
                    _viewAdBuff.buffIconList[i].buffButton.gameObject.SetActive(true);
                    _viewAdBuff.buffIconList[i].leftTime.gameObject.SetActive(false);
                    _viewAdBuff.buffIconList[i].activatedParticle.gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < Player.Cloud.adsBuffData.adsValudata.Count; i++)
            {
                if (Player.Cloud.adsBuffData.adsValudata[i] == Player.AdsBuff.activatedBuffValuedata)
                {
                    if (_viewMainIcon.buffInactiveObj[i].gameObject.activeInHierarchy)
                        _viewMainIcon.buffInactiveObj[i].gameObject.SetActive(false);
                }
                else
                {
                    if (_viewMainIcon.buffInactiveObj[i].gameObject.activeInHierarchy == false)
                    {
                        _viewMainIcon.buffInactiveObj[i].gameObject.SetActive(true);
                    }
                }
            }

            for(int i=0; i<Player.Cloud.adsBuffData.adsValudata.Count; i++)
            {
                currentTimeList.Add(0);
                phaseList.Add(TextPhase.phase1);
            }

            Main().Forget();
        }

        async UniTaskVoid Main()
        {
            updatetime = 0;
            while (true)
            {
                UpdateBuffUI();
                updatetime += Time.deltaTime;
                if(updatetime>=10)
                {
                    updatetime = 0;
                    Player.Cloud.adsBuffData.UpdateHash().SetDirty(true);
                }

                for (int i = 0; i < Player.Cloud.adsBuffData.adsValudata.Count; i++)
                {
                    if (Player.Cloud.adsBuffData.adsValudata[i] == Player.AdsBuff.activatedBuffValuedata)
                    {
                        if(_viewMainIcon.buffInactiveObj[i].gameObject.activeInHierarchy)
                            _viewMainIcon.buffInactiveObj[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        if (_viewMainIcon.buffInactiveObj[i].gameObject.activeInHierarchy==false)
                        {
                            _viewMainIcon.buffInactiveObj[i].gameObject.SetActive(true);
                        }
                    }
                }
                    

                await UniTask.Yield(_cts.Token);
            }
        }
        enum TextPhase
        {
            phase1, phase2,
        }
        Vector3 MaxScale = new Vector2(1.1f, 1.1f);
        Vector3 MinScale = new Vector2(0.9f, 0.9f);
        float currentTime = 0;
        void UpdateBuffUI()
        {
            for (int i = 0; i < Player.Cloud.adsBuffData.adsValudata.Count; i++)
            {
                if (Player.Cloud.adsBuffData.adsValudata[i] == Player.AdsBuff.activatedBuffValuedata)
                {
                    Player.Cloud.adsBuffData.adsLeftTimeData[i] -= Time.deltaTime;
                    int min = (int)(Player.Cloud.adsBuffData.adsLeftTimeData[i] / 60);
                    int sec = (int)(Player.Cloud.adsBuffData.adsLeftTimeData[i] % 60);

                    _viewAdBuff.buffIconList[i].leftTime.text = string.Format("{0:D2}:{1:D2}", min, sec);
                    switch (phaseList[i])
                    {
                        case TextPhase.phase1:
                            currentTimeList[i] += Time.deltaTime * 3;
                            _viewAdBuff.buffIconList[i].iconObj.transform.localScale = Vector2.Lerp(MinScale, MaxScale, currentTimeList[i]);
                            if (currentTimeList[i] >= 1)
                            {
                                currentTimeList[i] = 0;
                                phaseList[i] = TextPhase.phase2;
                            }
                            break;
                        case TextPhase.phase2:
                            currentTimeList[i] += Time.deltaTime * 3;
                            _viewAdBuff.buffIconList[i].iconObj.transform.localScale = Vector2.Lerp(MaxScale, MinScale, currentTimeList[i]);
                            if (currentTimeList[i] >= 1)
                            {
                                currentTimeList[i] = 0;
                                phaseList[i] = TextPhase.phase1;
                            }
                            break;
                        default:
                            break;
                    }

                    //inactivate
                    if (Player.Cloud.adsBuffData.adsLeftTimeData[i] <= 0)
                    {
                        Model.Player.AdsBuff.ActivateBuff(_viewAdBuff.buffIconList[i].abilKey, false);
                        _viewAdBuff.buffIconList[i].buffButton.gameObject.SetActive(true);
                        _viewAdBuff.buffIconList[i].leftTime.gameObject.SetActive(false);
                        _viewAdBuff.buffIconList[i].activatedParticle.gameObject.SetActive(false);
                        _viewAdBuff.buffIconList[i].iconObj.transform.localScale = Vector3.one;
                    }
                }
                //activate if adfree
                if (Player.Cloud.adsBuffData.adsLeftTimeData[i] <= 0)
                {
                    int index = i;
                    if (Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        Model.Player.AdsBuff.ActivateBuff(_viewAdBuff.buffIconList[index].abilKey, true);
                        _viewAdBuff.buffIconList[index].buffButton.gameObject.SetActive(false);
                        _viewAdBuff.buffIconList[index].leftTime.gameObject.SetActive(true);
                        _viewAdBuff.buffIconList[index].activatedParticle.gameObject.SetActive(true);
                        Player.Quest.TryCountUp(Definition.QuestType.ActivateAdBuff, 1);
                        Player.Unit.StatusSync();
                    }
                }
            }
        }

        void CloseWindow()
        {
            _viewAdBuff.blackBG.PopupCloseColorFade();
            _viewAdBuff.Wrapped.CommonPopupCloseAnimationUp(()=> {
                _viewAdBuff.SetVisible(false);
            });
        }
    }
}
