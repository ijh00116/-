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
    public class ControllerInventory 
    {
        ViewCanvasInventory viewcanvasInven;
        CancellationTokenSource _cts;

        List<GoodsItemSlot> goodsSlotList = new List<GoodsItemSlot>();
        public ControllerInventory(Transform parent, CancellationTokenSource cts)
        {
            viewcanvasInven = ViewCanvas.Create<ViewCanvasInventory>(parent);
            _cts = cts;

            for(int i=0; i<viewcanvasInven.closeBtns.Length; i++)
            {
                viewcanvasInven.closeBtns[i].onClick.AddListener(()=> {
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().SetVisible(false);
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().blackBG.PopupCloseColorFade();
                    Bundles.ViewCanvas.Get<Bundles.ViewCanvasInventory>().Wrapped.CommonPopupCloseAnimationUp();
                });
            }

            for(int i=0; i<StaticData.Wrapper.goods.Length; i++)
            {
                var _data = StaticData.Wrapper.goods[i];
                var obj = UnityEngine.Object.Instantiate(viewcanvasInven.goodsSlotPrefab);
                obj.Set(_data);
                obj.transform.SetParent(viewcanvasInven.slotParent.content,false);

                goodsSlotList.Add(obj);
            }

            viewcanvasInven.BindOnChangeVisible(o=> { 
                if(o)
                {
                    OpenInventoryCallback();
                }
            });

            viewcanvasInven.invenTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Inventory].StringToLocal;
        }

        void OpenInventoryCallback()
        {
            for (int i = 0; i < StaticData.Wrapper.goods.Length; i++)
            {
                var _data = StaticData.Wrapper.goods[i];
                var obj = goodsSlotList[i];
                obj.Set(_data);
            }
        }
    }

}
