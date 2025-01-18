using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasFreeShop : ViewCanvas
    {
        public ScrollRect productParent;
        public ViewAdShopItem productPrefab;

        public BTButton[] closeBtns;

        public TMP_Text titleTxt;
    }
}
