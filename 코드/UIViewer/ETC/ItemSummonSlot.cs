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
    public class ItemSummonSlot : MonoBehaviour
    {
        public BTButton adButton;
        public TMP_Text adSummon_text;
        public BTButton summon_10;
        public TMP_Text summon_10_text;
        public TMP_Text summon_10_Prce_text;
        public BTButton summon_100;
        public TMP_Text summon_100_text;
        public TMP_Text summon_100_Prce_text;


        public TMPro.TMP_Text summonLevel;
        public Slider summonExpSlider;
        public TMP_Text summonExpText;

        public void SyncView(int level,int exp, SummonType summonType)
        {
            summonLevel.text = level.ToString();

            if (level >= StaticData.Wrapper.dataChance.Length)
            {
                summonExpSlider.value = 1;
                summonExpText.text = $"MAX";
            }
            else
            {
                var maxexp = StaticData.Wrapper.dataMaxExp[level - 1].maxExp;
                summonExpSlider.value = (float)exp / (float)maxexp;
                summonExpText.text = $"{exp}/{maxexp}";
            }


            string localized = "";
            localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_FreeForBattlePass].StringToLocal;
            switch (summonType)
            {
                case SummonType.Item:
                    if(Player.Cloud.tutorialData.isFirstSummonItems)
                    {
                        summon_10_Prce_text.text = localized;
                    }
                    else
                    {
                        summon_10_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForItem10}";
                    }
                    summon_100_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForItem100}";
                    adSummon_text.text = $"{Player.Summon.adSummonMaxCount-Player.Cloud.weapondata.adsummonCount}/{Player.Summon.adSummonMaxCount }";
                    break;
                case SummonType.Skill:
                    if (Player.Cloud.tutorialData.isFirstSummonSkills)
                    {
                        summon_10_Prce_text.text = localized;
                    }
                    else
                    {
                        summon_10_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForSkill10}";
                    }
                    summon_100_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForSkill100}";
                    adSummon_text.text = $"{Player.Summon.adSummonMaxCount-Player.Cloud.skilldata.adsummonCount} / {Player.Summon.adSummonMaxCount }";
                    break;
                case SummonType.Pet:
                    summon_10_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForPet10}";
                    summon_100_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForPet100}";
                    adSummon_text.text = $"{Player.Summon.adSummonMaxCount-Player.Cloud.petdata.adsummonCount} / {Player.Summon.adSummonMaxCount }";
                    break;
                case SummonType.Rune:
                    summon_10_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForRune10}";
                    summon_100_Prce_text.text = $"{StaticData.Wrapper.summonTableData.needDiaForRune100}";
                    adSummon_text.text = $"{Player.Summon.adSummonMaxCount - Player.Cloud.runeData.adsummonCount} / {Player.Summon.adSummonMaxCount }";
                    break;
                default:
                    break;
            }
            
        }
    }

}
