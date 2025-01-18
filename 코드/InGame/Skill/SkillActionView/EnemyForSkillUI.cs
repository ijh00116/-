using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree
{
    public class EnemyForSkillUI : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite[] idleSprite;
        int animIndex;
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        public void Init()
        {
            animIndex = 0;
            StartCoroutine(IdleAnimStart());
        }
        IEnumerator IdleAnimStart()
        {
            while (true)
            {
                spriteRenderer.sprite = idleSprite[animIndex];
                animIndex++;
                if(animIndex>=idleSprite.Length)
                {
                    animIndex = 0;
                }
                yield return wait;
            }
        }
    }


}
