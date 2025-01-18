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
    public class ControllerFeverTime
    {
        ViewCanvasMainWave viewmainWave;
        CancellationTokenSource _cts;

        public ControllerFeverTime(Transform parent, CancellationTokenSource cts)
        {
            viewmainWave = ViewCanvas.Get<ViewCanvasMainWave>();
            _cts = cts;

            Main().Forget();

            viewmainWave.FeverTimeBtn.onClick.AddListener(()=> {
                viewmainWave.AnimateDesc();
            });

            viewmainWave.stageMoveTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_StageMove].StringToLocal; 
            viewmainWave.bossChallengeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ChallengeBoss].StringToLocal;
            viewmainWave.feverTime.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FeverTime].StringToLocal;
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                bool isFeverActive = false;
                int index = 0;
                for(int i=0; i<StaticData.Wrapper.feverTimedata.Length; i++)
                {
                    int minute = System.DateTime.Now.Minute;
                    int hour= System.DateTime.Now.Hour;
                    int nowTime = minute + hour * 60;
                    if (nowTime >= StaticData.Wrapper.feverTimedata[i].startMinute&& nowTime <= StaticData.Wrapper.feverTimedata[i].endMinute)
                    {
                        isFeverActive = true;
                        index = i;
                        break;
                    }
                    else
                    {
                        isFeverActive = false;
                    }
                }

                if(isFeverActive)
                {
                    if (Battle.Field.IsFeverTime==false)
                    {
                        Battle.Field.IsFeverTime = true;
                        Battle.Field.goldexpRate = StaticData.Wrapper.feverTimedata[index].goldexpRate;
                        Battle.Field.itemRate = StaticData.Wrapper.feverTimedata[index].itemrate;

                        viewmainWave.FeverTimeBtn.gameObject.SetActive(true);
                        viewmainWave.feverTimeDesc.text = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.FeverTime_desc].StringToLocal,
                            Battle.Field.goldexpRate, Battle.Field.itemRate);
                        viewmainWave.AnimateDesc();
                    }
                }
                else
                {
                    viewmainWave.FeverTimeBtn.gameObject.SetActive(false);
                    Battle.Field.IsFeverTime = false;
                    viewmainWave.feverTimecanvas.gameObject.SetActive(false);
                }
                await UniTask.Delay(2000);
            }
        }

     
    }

}
