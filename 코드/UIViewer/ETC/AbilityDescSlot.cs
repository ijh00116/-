using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using System.Collections.Generic;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class AbilityDescSlot : MonoBehaviour
    {
        public TMP_Text desc;
        public GameObject lockedObj;

        public void SetAbilityDesc(Model.Player.EquipData equipdata,int index,bool equipabil)
        {
            var currentEquipedItem = Player.EquipItem.Get(equipdata.equipType, Player.EquipItem.currentEquipIndex[equipdata.equipType]);

            bool sameItem = currentEquipedItem.equipType == equipdata.equipType && equipdata.tabledata.index == currentEquipedItem.tabledata.index;
            if (equipabil)
            {
                double equipabilValue = equipdata._equipabilitycaches[equipdata.tabledata.equipabilitylist[index]];
                string currentdata = string.Format(StaticData.Wrapper.localizeddesclist[(int)equipdata.tabledata.equipabilitylmk[index]].StringToLocal, equipabilValue.ToNumberString());

                double currentEquipedValue = 0;
                if(index<currentEquipedItem.tabledata.equipabilitylist.Length && !sameItem)
                {
                    if(equipdata.tabledata.equipabilitylist[index]== currentEquipedItem.tabledata.equipabilitylist[index])
                    {
                        currentEquipedValue = currentEquipedItem._equipabilitycaches[currentEquipedItem.tabledata.equipabilitylist[index]];
                        double calculdata = equipabilValue - currentEquipedValue;
                        desc.text = string.Format("{0}({1}%)", currentdata, (calculdata > 0) ? $"+<color=green>{calculdata.ToNumberString()}</color>" : $"-<color=red>{System.Math.Abs(calculdata).ToNumberString()}");
                    }
                    else
                    {
                        desc.text = currentdata;
                    }
                }
                else
                {
                    desc.text = currentdata;
                }
                
                if (equipdata.userEquipdata.AwakeLv < index)
                {
                    int abilIndex = index - 1;
                    
                    if(equipdata.tabledata.awakeLevelList[abilIndex]>0)
                    {
                        string unlockDesc = string.Format(StaticData.Wrapper.localizeddesclist[(int)Definition.LocalizeDescKeys.Etc_LevelUnlockInItemDesc].StringToLocal, equipdata.tabledata.awakeLevelList[abilIndex]);
                        desc.text = currentdata + unlockDesc;
                    }
                }
            }
            else
            {
                double possesabilValue = equipdata._possessabilitycaches[equipdata.tabledata.possessabilitylist[index]];
                string currentdata =string.Format(StaticData.Wrapper.localizeddesclist[(int)equipdata.tabledata.possessabilitylmk[index]].StringToLocal, possesabilValue.ToNumberString());

                double currentEquipedValue = 0;
                if (index < currentEquipedItem.tabledata.possessabilitylist.Length && !sameItem)
                {
                    if (equipdata.tabledata.equipabilitylist[index] == currentEquipedItem.tabledata.equipabilitylist[index])
                    {
                        currentEquipedValue = currentEquipedItem._possessabilitycaches[currentEquipedItem.tabledata.possessabilitylist[index]];
                        double calculdata = possesabilValue - currentEquipedValue;

                        desc.text = string.Format("{0}({1}%)", currentdata, (calculdata > 0) ? $"+<color=green>{calculdata.ToNumberString()}</color>" : $"-<color=red>{System.Math.Abs(calculdata).ToNumberString()}");
                    }
                    else
                    {
                        desc.text = currentdata;
                    }
                    
                }
                else
                {
                    desc.text = currentdata;
                }

                if (equipdata.userEquipdata.AwakeLv < index)
                {
                    int abilIndex = index - 1;

                    if (equipdata.tabledata.awakeLevelList[abilIndex] > 0)
                    {
                        string unlockDesc = string.Format(StaticData.Wrapper.localizeddesclist[(int)Definition.LocalizeDescKeys.Etc_LevelUnlockInItemDesc].StringToLocal, equipdata.tabledata.awakeLevelList[abilIndex]);
                        desc.text = currentdata + unlockDesc;
                    }
                }
            }

            if (equipdata.userEquipdata.AwakeLv >= index)
            {
                lockedObj.SetActive(false);
            }
            else
            {
                int abilIndex = index - 1;

                if (equipdata.tabledata.awakeLevelList[abilIndex] > 0)
                {
                    lockedObj.SetActive(true);
                }
                else
                {
                    lockedObj.SetActive(false);
                }
            }
        }

        public void SetSkillAbilityDesc(Model.Player.Skill.SkillCacheData skilldata, int index)
        {
            string localizingdesc = StaticData.Wrapper.localizeddesclist[(int)skilldata.tabledataSkill.SkillDesc[index]].StringToLocal;
            
            string coolTime = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Cooltime].StringToLocal, skilldata.tabledataSkill.coolTime);
            string effectTime= string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EffectTime].StringToLocal, skilldata.tabledataSkill.effectTime);

            string valueData=null;
            List<float> skillvalueList = new List<float>();
            if(index==0)
            {
                if(skilldata.tabledataSkill.skillStartValue_0.Length==1)
                {
                    valueData = string.Format(localizingdesc, skilldata.SkillValue(index)) +((skilldata.tabledataSkill.coolTime>0)? coolTime:"") + ((skilldata.tabledataSkill.effectTime > 0) ? effectTime : "");
                }
                else
                {
                    valueData = string.Format(localizingdesc, skilldata.SkillValue(index), skilldata.SkillValue(index,1)) + ((skilldata.tabledataSkill.coolTime > 0) ? coolTime : "") + ((skilldata.tabledataSkill.effectTime > 0) ? effectTime : "");
                }
                
            }
            else if (index == 1)
            {
                if (skilldata.tabledataSkill.skillStartValue_1.Length == 1)
                {
                    valueData = string.Format(localizingdesc, skilldata.SkillValue(index));
                }
                else
                {
                    valueData = string.Format(localizingdesc, skilldata.SkillValue(index), skilldata.SkillValue(index, 1));
                }
            }
            else
            {
                if (skilldata.tabledataSkill.skillStartValue_2.Length == 1)
                {
                    valueData = string.Format(localizingdesc, skilldata.SkillValue(index));
                }
                else
                {
                    valueData = string.Format(localizingdesc, skilldata.SkillValue(index), skilldata.SkillValue(index, 1));
                }
            }

            desc.text = valueData;


            if (skilldata.userSkilldata.AwakeLv >= index)
            {
                lockedObj.SetActive(false);
            }
            else
            {
                lockedObj.SetActive(true);
            }
        }
    }

}
