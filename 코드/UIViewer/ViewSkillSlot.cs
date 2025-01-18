using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BlackTree.Bundles
{
    public class ViewSkillSlot : ViewBase
    {
        public GameObject equipCheck;
        public Image slotImage;
        public Image skillIcon;
        public Image cooltimeImage;

        public GameObject lockedImage;

        public BTButton skillInfoBtn;

        public GameObject[] gradestars;

        public Slider amountBar;
        public TMP_Text maxlvText;
        public TMP_Text lvText;
        public TMP_Text amountText;

        public GameObject redDot;
        public void Init(Model.Player.Skill.SkillCacheData skillCache)
        {
            slotImage.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[(int)skillCache.tabledataSkill.grade - 1];
            skillIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)skillCache.tabledataSkill.skillKey];
            lockedImage.SetActive(!skillCache.userSkilldata.Unlock);

            amountBar.value = (float)skillCache.userSkilldata.Obtaincount / (float)(skillCache.GetAmountForLevelUp());
            lvText.text = string.Format("LV.{0}", skillCache.userSkilldata.level);
            amountText.text = string.Format("{0}/{1}", skillCache.userSkilldata.Obtaincount, skillCache.GetAmountForLevelUp());
            maxlvText.text = string.Format("Max.{0}", skillCache.tabledataSkill.maxLevel.ToString());

            for (int i=0; i< gradestars.Length; i++)
            {
                gradestars[i].SetActive(i < skillCache.tabledataSkill.grade);
            }
            redDot.SetActive(skillCache.userSkilldata.Obtaincount>= skillCache.GetAmountForLevelUp());

        }

        public void SyncInfo(Model.Player.Skill.SkillCacheData skillCache)
        {
            skillIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)skillCache.tabledataSkill.skillKey];
            lockedImage.SetActive(!skillCache.userSkilldata.Unlock);
            cooltimeImage.gameObject.SetActive(false);
            equipCheck.gameObject.SetActive(skillCache.IsEquiped);

            amountBar.value = (float)skillCache.userSkilldata.Obtaincount / (float)skillCache.GetAmountForLevelUp();
            lvText.text = string.Format("LV.{0}", skillCache.userSkilldata.level);
            amountText.text = string.Format("{0}/{1}", skillCache.userSkilldata.Obtaincount, skillCache.GetAmountForLevelUp());

            for (int i = 0; i < gradestars.Length; i++)
            {
                gradestars[i].SetActive(i < skillCache.tabledataSkill.grade);
            }

            redDot.SetActive(skillCache.userSkilldata.Obtaincount >= skillCache.GetAmountForLevelUp());
        }
    }

}
