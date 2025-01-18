using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class ViewCanvasGoldUpgrade : ViewCanvas
    {
        public ScrollRect scrollRect;

        public BTSelector lvTypeSelector;
        public BTButton lvup_1;
        public BTButton lvup_10;
        public BTButton lvup_max;

        public TMP_Text tierText;
        public BTButton nextTier;
        public BTButton prevTier;

        public ViewCanvasGoldUpgrade SetDesc(string text)
        {
            return this;
        }
    }

}
