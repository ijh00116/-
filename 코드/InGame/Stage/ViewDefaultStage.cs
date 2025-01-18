using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;

namespace BlackTree.Core
{
    public class ViewDefaultStage : MonoBehaviour
    {
        public Transform playerPos;
        public Transform witchPos;
        public Transform[] enemyPos;

        public Transform waveEndPos;

        public  ViewEnemy[] enemyPrefab;
        public ViewEnemy bossPrefab;

        public Transform playerPos_inBoss;
        public Transform bossPos_inBoss;

        public int stageIndex;

        [SerializeField] bool gizmodraw;
        private void OnDrawGizmos()
        {
            if(gizmodraw)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < enemyPos.Length; i++)
                {
                    if (enemyPos[i] == null)
                        continue;
                    Gizmos.DrawSphere(enemyPos[i].position, 0.3f);
                }
            }
          
            
        }
    }
}
