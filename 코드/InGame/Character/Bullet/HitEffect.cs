using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree.Core
{
    public class HitEffect : Poolable
    {
        [SerializeField] ParticleSystem hitparticle;

        int spriteindex;

        [SerializeField] bool randomRotate;
        [SerializeField] float waitSecond;
        public void On()
        {
            if (Player.Cloud.optiondata.appearEffect == false)
            {
                PoolManager.Push(this);
                return;
            }
                
            hitparticle.Play();
            StartCoroutine(StartEffect());
        }

        IEnumerator StartEffect()
        {
            while (true)
            {
                if (hitparticle.isPlaying == false)
                    break;
                yield return null;
            }
            PoolManager.Push(this);
        }
    }

}
