using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewWeaponslot : ViewBase
    {
        public Image weaponIcon;
        public Image weaponFrame;

        public TMP_Text Level;
        public TMP_Text ObtainCount;
        public Slider imageObtainFill;
        public Image[] gradeimages;

        public BTButton SeeDetailbutton;

        public Image lockedImage;
        public GameObject Equiped;
        protected bool isUnlock = false;

        public Player.EquipData weaponCache;
        public ViewWeaponslot SetHideImage(bool unlocked)
        {
            lockedImage.gameObject.SetActive(unlocked);
            return this;
        }

        public ViewWeaponslot SetLevel(int level)
        {
            Level.text = $"Lv {level}";
            return this;
        }

        public ViewWeaponslot SetObtainCount(float currentObtain)
        {
            ObtainCount.text = $"{(int)currentObtain} / 5";
            imageObtainFill.value = Mathf.Clamp((float)(currentObtain / 5),0,1);
            return this;
        }

        public ViewWeaponslot SetEquip(bool equip)
        {
            Equiped.SetActive(equip);
            return this;
        }

        public ViewWeaponslot SetGrade(int _grade)
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

        public ViewWeaponslot SetupstaticWeaponData(Player.EquipData _weaponCache)
        {
            weaponCache = _weaponCache;
            if(_weaponCache.equipType!=Definition.EquipType.Weapon)
            {
                weaponIcon.GetComponent<RectTransform>().rotation = Quaternion.identity;
            }
            weaponIcon.sprite = InGameResourcesBundle.Loaded.weaponIcon[weaponCache.tabledata.index];
            //weaponFrame.color = InGameResourcesBundle.Loaded.weaponGradeFrameColor[weaponCache.tabledata.grade - 1];
            weaponFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[weaponCache.tabledata.grade - 1];

            return this;
        }

        public ViewWeaponslot SeeDetailWeapon(UnityEngine.Events.UnityAction callback)
        {
            SeeDetailbutton.onClick.AddListener(callback);
            return this;
        }

    
    }

}
