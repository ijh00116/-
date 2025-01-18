using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasInAppShop : ViewCanvas
    {
        public BTSelector buttonSelector;

        public BTButton openProductwindowBtn;
        public BTButton openPackagewindowBtn;

        public GameObject productWindow;
        public Transform diaProductParent;
        public Transform awakeProductParent;
        public ViewShopItem productItemPrefab;

        public GameObject packageWindow;
        public ScrollRect packageParent;
        public ViewShopPackageItem packageItemPrefab;
        public ViewShopSkillPackageItem packageskillItemPrefab;
        public ViewShopVIPPackageItem packageVIPItemPrefab;

        public BTButton[] closeBtn;

        public TMP_Text goodsTxt;
        public TMP_Text goodsTxt_on;
        public TMP_Text packageTxt;
        public TMP_Text packageTxt_on;

        public void ActiveOffScrollRect()
        {
            productWindow.SetActive(false);
            packageWindow.SetActive(false);
        }
    }

}
