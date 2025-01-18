using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree
{
    public class OrderInLayer : MonoBehaviour
    {
        [SerializeField] bool isSprite = true;
        [SerializeField] private SpriteRenderer[] sprites;
        [SerializeField] private ParticleSystemRenderer[] particles;
        private Transform tr;

        private void Start()
        {
            //if (sprite == null)
            //    sprite = GetComponent<SpriteRenderer>();

            tr = GetComponent<Transform>();
        }

        private void Update()
        {
            if(isSprite)
            {
                foreach (var sprite in sprites)
                {
                    sprite.sortingOrder = -1 * (int)(tr.position.y * 100);
                }
            }
            else
            {
                foreach(var psr in particles)
                {
                    psr.sortingOrder = -1 * (int)(tr.position.y * 100);
                }
                
            }
        }
    }
}