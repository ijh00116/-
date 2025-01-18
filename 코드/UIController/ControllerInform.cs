using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using BlackTree.Bundles;
using BlackTree.Model;
using LitJson;
using BackEnd;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerInform
    {
        ViewCanvasInform view;
        InformData currentInformdata;
        public ControllerInform(Transform parent, CancellationTokenSource cts)
        {
            view = ViewCanvas.Create<ViewCanvasInform>(parent);

            currentInformdata = StaticData.Wrapper.informData[0];

            for(int i=0; i< view.closeBtn.Length; i++)
            {
                view.closeBtn[i].onClick.AddListener(()=> {
                    view.blackBG.PopupCloseColorFade();
                    view.Wrapped.CommonPopupCloseAnimationUp(() =>
                    {
                        view.SetVisible(false);
                    });

                });
            }

            view.informText.text = currentInformdata.desc;
            view.goPatchDetail.onClick.AddListener(() => {
                Application.OpenURL(currentInformdata.url);
            });
         
            

            view.BindOnChangeVisible((o)=> { 
                if(o)
                {
                    if(currentInformdata!=null)
                    {
                        Player.Cloud.optiondata.informVersion = currentInformdata.version;
                        Player.Cloud.optiondata.SetDirty(true).UpdateHash();

                        Player.Option.menuRedDotCallback?.Invoke(MenuType.Inform, false);
                    }
                }
            
            });

            bool isRedDot = false;
            if (Player.Cloud.optiondata.informVersion!=currentInformdata.version)
            {
                isRedDot = true;
            }
        

            if(isRedDot)
            {
                Player.Option.menuRedDotCallback?.Invoke(MenuType.Inform, true);
            }

        }
    }

}
