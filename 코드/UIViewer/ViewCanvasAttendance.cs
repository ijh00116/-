using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasAttendance : ViewCanvas
    {
        public ViewAttendSlot attendSlotPrefab;
        public Transform slotParent;
        public TMP_Text _title;

        public BTButton[] closeBtn;

        public void SetLocalizeText()
        {

        }
    }
}