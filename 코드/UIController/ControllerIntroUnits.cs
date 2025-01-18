using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using BlackTree.Bundles;
using System;
using BackEnd;

namespace BlackTree.Core
{
    public class ControllerIntroUnits : MonoBehaviour
    {
        [SerializeField] ViewIntroUnit[] units;
        public async UniTask AllUnitSpawn()
        {
            for(int i=0; i< units.Length; i++)
            {
                units[i].Init();
                await UniTask.DelayFrame(20);
            }
        }
    }
}

