using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree
{
    public class SpritesAnimation : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriterenderer;
        [SerializeField] Sprite[] sprites;

        [SerializeField] float speed;
        float currentTime;
        int index = 0;
        // Start is called before the first frame update
        void Start()
        {
            if (spriterenderer == null)
                spriterenderer = GetComponent<SpriteRenderer>();
            currentTime = 0;
            index = 0;
        }

        private void Update()
        {
            currentTime += Time.deltaTime;
            if(currentTime>=speed)
            {
                currentTime = 0;
                if (index >= sprites.Length)
                {
                    index = 0;
                }
                spriterenderer.sprite = sprites[index];
                index++;
            }
        }
        
    }

}
