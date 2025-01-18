using BlackTree.Core;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace BlackTree.Bundles
{
    public class GoodsParticleAttraction : Poolable
    {
        [SerializeField] public AssetKits.ParticleImage.ParticleImage goodsParticlePrefab;
        RectTransform rt;
        private void Start()
        {
            rt = GetComponent<RectTransform>();
        }
        public void ActivateGoodsParticle(Vector2 startPoint,Transform target)
        {
            if(rt==null)
            {
                rt=this.GetComponent<RectTransform>();
            }
            rt.anchoredPosition = startPoint;
            goodsParticlePrefab.attractorTarget = target;

            goodsParticlePrefab.Play();

            StartCoroutine(TimeCheck());
        }
        
        IEnumerator TimeCheck()
        {
            float currentTime = 0;
            while (true)
            {
                currentTime += Time.deltaTime;
                if(currentTime>=2.5f)
                {
                    break;
                }
                yield return null;
            }
            PoolManager.Push(this);
        }
    }
}
