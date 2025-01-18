using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasMail : ViewCanvas
    {
        public ViewMailSlot mailSlot;
        public ViewGoodRewardSlot rewardSlot;
        public Transform slotParent;
        public TMP_Text _title;

        public BTButton testGetMailBtn;

        public BTButton[] closeBtn;

        public void SetLocalizeText()
        {
            //_title.text = StaticData.Wrapper.localizednamelist[(int)Definition.LocalizeNameKeys.ETC_Attendance].StringToLocal;
        }

      
    }

}