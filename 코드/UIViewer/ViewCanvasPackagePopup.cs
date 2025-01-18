using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasPackagePopup : ViewCanvas
    {
        public TMP_Text titleDesc;
        public Image characterAnimImage;
        public Sprite[] animSprites;
        public TMP_Text expiredDay;

        public Transform rewardSlotParent;
        public ViewGoodRewardSlot rewardSlotPrefab;
        public BTButton purchaseBtn;
        public TMP_Text btnText;

        public BTButton[] closeBtn;
    }
}
