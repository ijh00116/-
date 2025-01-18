using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;

namespace BlackTree.Bundles
{
    public class ViewPetSlot : ViewBase
    {
        public GameObject equipCheck;
        public Image slotImage;
        public Image petIcon;

        public GameObject lockedImage;
        public BTButton petInfoBtn;

        public Slider amountBar;
        public TMP_Text lvText;
        public TMP_Text maxlvText;
        public TMP_Text amountText;

        public GameObject redDot;

        public GameObject[] gradeStars;

        public void Init(Model.Player.Pet.PetCacheData petCache)
        {
            slotImage.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[petCache.tabledata.grade - 1];
            petIcon.sprite = PetResourcesBundle.Loaded.petImage[petCache.tabledata.index].slotIconsprite;
            maxlvText.text = string.Format("Max.{0}", petCache.tabledata.maxLevel); 
            lockedImage.SetActive(!petCache.IsUnlocked);

            for(int i=0;i< gradeStars.Length;i++)
            {
                gradeStars[i].SetActive(false);
            }
            for (int i = 0; i < petCache.tabledata.grade; i++)
            {
                gradeStars[i].SetActive(true);
            }
        }

        public void SyncInfo(Model.Player.Pet.PetCacheData petCache)
        {
            lockedImage.SetActive(!petCache.IsUnlocked);
            equipCheck.gameObject.SetActive(petCache.IsEquiped);

            lvText.text = string.Format("LV.{0}", petCache.userPetdata.level);

            redDot.SetActive(false);

            if (petCache.IsMaxLevel())
            {
                amountText.text = string.Format("{0}", petCache.userPetdata.Obtaincount);
                amountBar.value = 1;

                if(petCache.userPetdata.Obtaincount>0)
                {
                    redDot.SetActive(true);
                }
            }
            else
            {
                amountText.text = string.Format("{0}/{1}", petCache.userPetdata.Obtaincount, StaticData.Wrapper.petAmountTableData[petCache.userPetdata.level].amountForLvUp);
                var floatvalue = Mathf.Clamp((float)(petCache.userPetdata.Obtaincount / (float)(StaticData.Wrapper.petAmountTableData[petCache.userPetdata.level].amountForLvUp)), 0.0f, 1.0f);
                amountBar.value = floatvalue;

                if(petCache.userPetdata.Obtaincount > (float)(StaticData.Wrapper.petAmountTableData[petCache.userPetdata.level].amountForLvUp))
                {
                    redDot.SetActive(true);
                }
            }
        }

    }
}
