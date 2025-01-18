using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;
using TMPro;
using AssetKits.ParticleImage;

namespace BlackTree.Bundles
{
    [System.Serializable]
    public class BuffIcon
    {
        public Model.AdsBuffType abilKey;
        public BTButton buffButton;
        public TMP_Text leftTime;
        public TMP_Text btnText;
        public TMP_Text abilText;
        public ParticleImage activatedParticle;
        public GameObject iconObj;
    }
    public class ViewCanvasAdBuff : ViewCanvas
    {
        public TMP_Text titleTxt;
        public BTButton[] closeButton;
        public BuffIcon[] buffIconList;

       
    }
}
