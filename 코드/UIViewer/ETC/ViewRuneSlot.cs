using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;

namespace BlackTree.Bundles
{
    public class ViewRuneSlot : MonoBehaviour
    {
        public GameObject equipCheck;
        public Image slotImage;
        public Image runeIcon;

        public GameObject lockedImage;
        public BTButton runeInfoBtn;

        public Slider amountBar;
        public TMP_Text lvText;
        public TMP_Text maxlvText;
        public TMP_Text amountText;

        public GameObject redDot;

        public GameObject[] gradeStars;

        public void Init(Model.Player.Rune.RuneCacheData runeCache)
        {
            slotImage.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[runeCache.tabledata.grade - 1];
            runeIcon.sprite = PetResourcesBundle.Loaded.runeImage[runeCache.tabledata.index];
            maxlvText.text = string.Format("Max.{0}", runeCache.tabledata.maxLevel);
            lockedImage.SetActive(!runeCache.IsUnlocked);

            for (int i = 0; i < gradeStars.Length; i++)
            {
                gradeStars[i].SetActive(false);
            }
            for (int i = 0; i < runeCache.tabledata.grade; i++)
            {
                gradeStars[i].SetActive(true);
            }
        }

        public void SyncInfo(Model.Player.Rune.RuneCacheData runeCache)
        {
            lockedImage.SetActive(!runeCache.IsUnlocked);
            equipCheck.gameObject.SetActive(runeCache.IsEquiped);

            lvText.text = string.Format("LV.{0}", runeCache.userRunedata.Obtainlv);

            redDot.SetActive(false);

            if (runeCache.IsMaxLevel())
            {
                amountText.text = string.Format("{0}", runeCache.userRunedata.Obtaincount);
                amountBar.value = 1;

                if (runeCache.userRunedata.Obtaincount > 0)
                {
                    redDot.SetActive(true);
                }
            }
            else
            {
                amountText.text = string.Format("{0}/{1}", runeCache.userRunedata.Obtaincount, StaticData.Wrapper.runeAmountTableData[runeCache.userRunedata.Obtainlv].amountForLvUp);
                var floatvalue = Mathf.Clamp((float)(runeCache.userRunedata.Obtaincount / (float)(StaticData.Wrapper.runeAmountTableData[runeCache.userRunedata.Obtainlv].amountForLvUp)), 0.0f, 1.0f);
                amountBar.value = floatvalue;

                if (runeCache.userRunedata.Obtaincount > (float)(StaticData.Wrapper.runeAmountTableData[runeCache.userRunedata.Obtainlv].amountForLvUp))
                {
                    redDot.SetActive(true);
                }
            }
        }
    }

}
