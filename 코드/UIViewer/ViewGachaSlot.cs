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
    public class ViewGachaSlot : MonoBehaviour
    {
        [SerializeField] Image FrameBackground;
        [SerializeField] Image itemIcon;
        [SerializeField] GameObject[] gradeStars;
        [SerializeField] TMPro.TMP_Text count;
        public ParticleSystem particle;
        public ParticleSystem goodParticle;
        public void SetData(Player.EquipData _itemCache,int count)
        {
            switch (_itemCache.equipType)
            {
                case EquipType.Weapon:
                    itemIcon.sprite = InGameResourcesBundle.Loaded.weaponIcon[_itemCache.tabledata.index];
                    break;
                case EquipType.Armor:
                    itemIcon.sprite = InGameResourcesBundle.Loaded.armorUIIcon[_itemCache.tabledata.index];
                    break;
                case EquipType.Bow:
                    itemIcon.sprite = InGameResourcesBundle.Loaded.bowIcon[_itemCache.tabledata.index];
                    break;
                default:
                    break;
            }
            

            FrameBackground.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[_itemCache.tabledata.grade - 1];

            for(int i=0; i<gradeStars.Length; i++)
            {
                gradeStars[i].SetActive(false);
            }

            for(int i=0; i<_itemCache.tabledata.grade; i++)
            {
                gradeStars[i].SetActive(true);
            }
            this.count.text = count.ToString();

            if (_itemCache.tabledata.grade >= 5)
            {
                goodParticle.gameObject.SetActive(true);
                goodParticle.Clear();
                goodParticle.time = 0;
                goodParticle.Play();
            }
            else if (_itemCache.tabledata.grade>=4)
            {
                particle.gameObject.SetActive(true);
                particle.Clear();
                particle.time = 0;
                particle.Play();
            }
            else
            {
                particle.gameObject.SetActive(false);
                goodParticle.gameObject.SetActive(false);
            }
            
        }

        //for skill
        public void SetData(Sprite sprite,int grade,int count)
        {
            itemIcon.sprite = sprite;
            FrameBackground.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[grade - 1];

            for (int i = 0; i < gradeStars.Length; i++)
            {
                gradeStars[i].SetActive(false);
            }
            for (int i = 0; i < grade; i++)
            {
                gradeStars[i].SetActive(true);
            }
            this.count.text = count.ToString();

            if (grade >= 5)
            {
                goodParticle.gameObject.SetActive(true);
                goodParticle.Clear();
                goodParticle.time = 0;
                goodParticle.Play();
            }
            else if (grade >= 4)
            {
                particle.gameObject.SetActive(true);
                particle.Clear();
                particle.time = 0;
                particle.Play();
            }
            else
            {
                particle.gameObject.SetActive(false);
                goodParticle.gameObject.SetActive(false);
            }
        }
    }
}
