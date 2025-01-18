using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using UnityEngine.Events;
using System;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public class Summon
        {

            public static int adSummonMaxCount = 5;
            /// <summary>
            /// item=0,skill=1,pet=2
            /// </summary>
            public static int currentPopupIndex = -1;

            public static Action<int,bool> summonInGachaResult;
        }
    }
}
