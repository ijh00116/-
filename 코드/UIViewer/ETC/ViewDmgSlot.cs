using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Core;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewDmgSlot : MonoBehaviour
    {
        public Image skillImage;
        public TMP_Text skillName;
        public TMP_Text dmgText;
        public Slider dmgValue;

        SkillKey myskillType;
        public void Init(SkillKey _key)
        {
            myskillType = _key;
            if(_key!=SkillKey.End)
            {
                skillImage.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)_key];
                skillName.text = StaticData.Wrapper.localizednamelist[(int)StaticData.Wrapper.skillDatas[(int)myskillType].SkillName].StringToLocal;
                
            }
            else
            {
                skillImage.sprite = GoodResourcesBundle.Loaded.NormalAtkImage;
                string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_NormalAtk].StringToLocal;
                skillName.text = localized;
            }
            
        }
        public double GetDmg()
        {
            double data = 0;
            data = Player.Skill.skillDamageList[(int)myskillType];
            return data;
        }
        public void SetCurrentDmg()
        {
            if(Player.Skill.skillDamageList[(int)myskillType]>0)
            {
                if(gameObject.activeInHierarchy==false)
                    this.gameObject.SetActive(true);
                dmgText.text = GetDmg().ToNumberString();
                dmgValue.value = (float)(GetDmg() / Player.Skill.currentBestDmgIngame);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        public void Refresh()
        {
            Player.Skill.skillDamageList[(int)myskillType] = 0;
        }
    }

}
