using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewHelmetSlot : ViewBase
    {
        public Image HelmetIcon;
        public Image HelmetFrame;
        public Image HelmetInnerFrame;

        public TMP_Text Level;
        public TMP_Text ObtainCount;
        public Slider imageObtainFill;
        public Image[] gradeimages;

        public Image lockedImage;

        public GameObject Equiped;

        protected bool isUnlock = false;

        public BTButton SeeDetailbutton;

        public Player.EquipData helmetCache;
        public ViewHelmetSlot SetHideImage(bool unlocked)
        {
            lockedImage.gameObject.SetActive(unlocked);
            return this;
        }

        public ViewHelmetSlot SetLevel(int level)
        {
            Level.text = $"Lv {level}";
            return this;
        }

        public ViewHelmetSlot SetObtainCount(float currentObtain)
        {
            ObtainCount.text = $"{(int)currentObtain} / 5";
            imageObtainFill.value = Mathf.Clamp((float)(currentObtain / 5), 0, 1);
            return this;
        }

        public ViewHelmetSlot SetEquip(bool equip)
        {
            Equiped.SetActive(equip);
            return this;
        }

        public ViewHelmetSlot SetGrade(int _grade)
        {
            var grade = _grade - 1;
            for (int i = 0; i < gradeimages.Length; i++)
            {
                gradeimages[i].gameObject.SetActive(true);
                if (i >= grade)
                {
                    gradeimages[i].gameObject.SetActive(false);
                }
            }

            return this;
        }

        public ViewHelmetSlot SetupstaticHelmetData(Player.EquipData _HelmetCache)
        {
            helmetCache = _HelmetCache;
            HelmetIcon.sprite = InGameResourcesBundle.Loaded.bowIcon[helmetCache.tabledata.index];
            HelmetInnerFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[helmetCache.tabledata.grade - 1];

            return this;
        }
        public ViewHelmetSlot SeeDetail(UnityEngine.Events.UnityAction callback)
        {
            SeeDetailbutton.onClick.AddListener(callback);
            return this;
        }


    }

}