using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class ControllerUserDiePopup
    {
        ViewCanvasUserDiePopup viewcanvasDiePopup;
        CancellationTokenSource _cts;

        float currentTime = 0;
        float maxReviveTime = 3;
        public ControllerUserDiePopup(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;

            viewcanvasDiePopup = ViewCanvas.Create<ViewCanvasUserDiePopup>(parent);
            viewcanvasDiePopup.gotoMainBtn.onClick.AddListener(GotoMain);
            viewcanvasDiePopup.SetVisible(false);

            Battle.Field.unitDieEvent += DiePopup;
        }

        bool alreadyGoMain = false;
        void DiePopup()
        {
            if(Battle.Field.IsMainSceneState)
            {
                alreadyGoMain = false;
                viewcanvasDiePopup.gotoMainBtn.enabled = true;
                currentTime = 0;
                string localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AutoRevive].StringToLocal;
                viewcanvasDiePopup.autoReviveDesc.text = string.Format(localized, maxReviveTime - currentTime);
                viewcanvasDiePopup.SetVisible(true);
                AutoReviveUpdate().Forget();
            }
            
        }

        async UniTaskVoid AutoReviveUpdate()
        {
            while (true)
            {
                currentTime += Time.deltaTime;
                string localized = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_AutoRevive].StringToLocal;
                viewcanvasDiePopup.autoReviveDesc.text = string.Format(localized, maxReviveTime - currentTime);
                if(alreadyGoMain)
                {
                    break;
                }
                if (currentTime>=maxReviveTime)
                {
                   
                    GotoMain();
                    break;
                }
                await UniTask.Yield();
            }
        }

        void GotoMain()
        {
            alreadyGoMain = true;
            viewcanvasDiePopup.gotoMainBtn.enabled = false;
            Player.Unit.ResetUnit();

            if (Battle.Field.CurrentFieldStage == 0)
            {
                if(Battle.Field.CurrentFieldChapter>0)
                {
                    Battle.Field.CurrentFieldChapter--;
                    Battle.Field.CurrentFieldStage = 4;
                }
                else
                {
                    Battle.Field.CurrentFieldChapter=0;
                    Battle.Field.CurrentFieldStage = 0;
                }
                
            }
            else
            {
                Battle.Field.CurrentFieldStage--;
            }
            Player.Option.autoBossActive?.Invoke(false);
            Battle.Field.currentKillEnemy = 0;
            Battle.Field.ChangeSceneState(eSceneState.WaitForMainIdle);
        }
    }

}
