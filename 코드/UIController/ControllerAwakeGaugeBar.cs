using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace BlackTree.Core
{
    public class ControllerAwakeGaugeBar
    {
        enum TextPhase
        {
            phase1,phase2,
        }
        ViewCanvasAwakeGaugeBar _view;
        private CancellationTokenSource _cts;

        float alphatime = 0;
        bool toTransParent= true;
        TextPhase phase = TextPhase.phase1;
        public ControllerAwakeGaugeBar(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasAwakeGaugeBar>(parent);
            _view.SetVisible(true);
            _view.Init();

            Player.Unit.characterAwakeCallback += CharacterAwake;
            Player.Unit.characterAwakeChangeCallback += CharacterAwakeChange;

            string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_TouchForAwake].StringToLocal;
            _view.gaugeBarTitle.text = localized;
            Main().Forget();
        }

        void CharacterAwake(CharacterAwakeState state)
        {
            string localized = "";
            if (state==CharacterAwakeState.Normal)
            {
                currentTime = 0;
                localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_TouchForAwake].StringToLocal;
                _view.gaugeBarTitle.transform.localScale = Vector3.one;
                _view.canvasGroup.gameObject.SetActive(false);
            }
            else if (state == CharacterAwakeState.ToAwake)
            {
                currentTime = 0;
                localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_BecomeAwake].StringToLocal;
                _view.gaugeBarTitle.color = textOriginalColor;
                AudioManager.Instance.Play(AudioSourceKey.CharacterAwake);
            }
            _view.gaugeBarTitle.text = localized;
        }
        void CharacterAwakeChange()
        {
            if(Battle.Field.currentSceneState==eSceneState.MainBossEvent|| Battle.Field.currentSceneState == eSceneState.MainIdle||
                Battle.Field.currentSceneState == eSceneState.MainPause|| Battle.Field.currentSceneState == eSceneState.WaitForMainIdle)
            {
                _view.canvasGroup.gameObject.SetActive(true);
                _view.canvasGroup.alpha = 1;
                alphatime = 0;
                toTransParent = true;
                string localized = "";
                switch (Player.Unit.awakeChange)
                {
                    case Definition.CharacterAwakeChange.HpOne:
                        localized = "Monster become weak!";
                        break;
                    case Definition.CharacterAwakeChange.ExpTwice:
                        localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Exp_X2].StringToLocal;
                        break;
                    case Definition.CharacterAwakeChange.GoldTwice:
                        localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Gold_X2].StringToLocal;
                        break;
                    case Definition.CharacterAwakeChange.MonsterGenTwice:
                        localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Monster_X2].StringToLocal;
                        break;
                    default:
                        break;
                }
                _view.awakeDesc.text = localized;
            }
          
            
        }

        Color textOriginalColor = Color.white;
        Color textAlphaColor = new Color(1,1,1,0);
        Vector3 textMaxScale = new Vector2(1.2f, 1.2f);
        Vector3 textMinScale = new Vector2(0.8f, 0.8f);
        float currentTime = 0;
        async UniTask Main()
        {
            while (true)
            {
                if(Player.Unit.awakeState==CharacterAwakeState.Normal)
                {
                    _view.gaugeBar.DOValue((float)Player.Unit.characterAwakeTouchCount / (float)Player.Unit.characterAwakeMaxTouchCount, 1.0f);
                    _view.gaugeText.text = string.Format("{0:F0}%", ((float)Player.Unit.characterAwakeTouchCount / (float)Player.Unit.characterAwakeMaxTouchCount)*100.0f);

                    switch (phase)
                    {
                        case TextPhase.phase1:
                            currentTime += Time.deltaTime;
                            _view.gaugeBarTitle.color = Color.Lerp(textOriginalColor, textAlphaColor, currentTime);
                            if(currentTime>=1)
                            {
                                currentTime = 0;
                                phase = TextPhase.phase2;
                            }
                            break;
                        case TextPhase.phase2:
                            currentTime += Time.deltaTime;
                            _view.gaugeBarTitle.color = Color.Lerp(textAlphaColor, textOriginalColor, currentTime);
                            if (currentTime >= 1)
                            {
                                currentTime = 0;
                                phase = TextPhase.phase1;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else// if (Player.Unit.awakeState == CharacterAwakeState.Awake)
                {
                    _view.gaugeBar.DOValue((Player.Unit.AwakeTime - Player.Unit.currentAwakeTime) / Player.Unit.AwakeTime, 1.0f);
                    _view.gaugeText.text = string.Format("{0:F0}%({1:F1})", ((Player.Unit.AwakeTime - Player.Unit.currentAwakeTime) / Player.Unit.AwakeTime) * 100.0f, Player.Unit.AwakeTime-Player.Unit.currentAwakeTime);

                    alphatime += Time.deltaTime;
                    if (toTransParent)
                    {
                        _view.canvasGroup.alpha = Mathf.Lerp(1, 0.5f, alphatime);
                        if(alphatime>=1)
                        {
                            alphatime = 0;
                            toTransParent = false;
                        }
                    }
                    else
                    {
                        _view.canvasGroup.alpha = Mathf.Lerp(0.5f, 1, alphatime);
                        if (alphatime >= 1)
                        {
                            alphatime = 0;
                            toTransParent = true;
                        }
                    }
                    switch (phase)
                    {
                        case TextPhase.phase1:
                            currentTime += Time.deltaTime*3;
                            _view.gaugeBarTitle.transform.localScale = Vector2.Lerp(textMinScale, textMaxScale, currentTime);
                            if (currentTime >= 1)
                            {
                                currentTime = 0;
                                phase = TextPhase.phase2;
                            }
                            break;
                        case TextPhase.phase2:
                            currentTime += Time.deltaTime * 3;
                            _view.gaugeBarTitle.transform.localScale = Vector2.Lerp(textMaxScale, textMinScale, currentTime);
                            if (currentTime >= 1)
                            {
                                currentTime = 0;
                                phase = TextPhase.phase1;
                            }
                            break;
                        default:
                            break;
                    }
                }
                

                await UniTask.Yield(_cts.Token);
            }
        }
    }

}
