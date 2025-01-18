using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System.Collections;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewCanvasReview : ViewCanvas
    {
        public BTButton confirmBtn;
        public BTButton[] starList;
        public GameObject[] starObjList;

        public BTButton[] closeBtns;

        public GameObject realReviewPopup;
        public BTButton[] realReviewCloseBtns;

        public BTButton realReviewConfirm;

        public TMP_Text reviewTitle;
        public TMP_Text reviewDesc;
        public TMP_Text reviewConfirm;

        public TMP_Text storeReviewTitle;
        public TMP_Text storeReviewDesc;
        public TMP_Text storereviewConfirm;
        public TMP_Text storereviewCancel;
    }

}
