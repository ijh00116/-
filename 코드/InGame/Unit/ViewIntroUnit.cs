using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BlackTree
{
    public class ViewIntroUnit : MonoBehaviour
    {
        public ParticleSystem particle;
        public SpriteRenderer _renderer;
        public Sprite[] sprites;

        float currentTime;
        int index = 0;

        [SerializeField] float speed;
        public void Init()
        {
            this.gameObject.SetActive(true);

            particle.gameObject.SetActive(true);
            particle.Play();
            
            _renderer.color = new Color(1, 1, 1, 0);

            _renderer.DOFade(1, 0.5f);

        }

        private void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= speed)
            {
                currentTime = 0;
                if (index >= sprites.Length)
                {
                    index = 0;
                }
                _renderer.sprite = sprites[index];
                index++;
            }
        }
    }

}
