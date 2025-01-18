using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class StatusSlotListLevelView : MonoBehaviour
    {
        public TMP_Text levelName;
        public BTButton lockedObject;
        public Transform parent;
     
        public void Init(int level)
        {
            levelName.text = $"Level: {level + 1}";
        }
    }

}
