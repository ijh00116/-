using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System.Collections;

namespace BlackTree.Bundles
{
    public class ViewCanvasInventory : ViewCanvas
    {
        public BTButton[] closeBtns;
        public GoodsItemSlot goodsSlotPrefab;
        public ScrollRect slotParent;
        public TMP_Text invenTitle;
    }

}
