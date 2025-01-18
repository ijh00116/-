using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;

namespace BlackTree.Bundles
{
    public class ViewAbilityDesc : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text valueText;

        public void Init(CharacterAbilityTypes abilType)
        {
            switch (abilType)
            {
                case CharacterAbilityTypes.SwordAtk:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtk].StringToLocal);
                    valueText.text = $"+{Player.Unit.SwordAtk.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.BowAtk:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BowAtk].StringToLocal);
                    valueText.text = $"+{Player.Unit.BowAtk.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.SwordAtkSpeed:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtkSpeed].StringToLocal);
                    valueText.text = $"+{Player.Unit.AtkSpeed.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.MaxMagicAtkSpeed:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxMagicAtkSpeed].StringToLocal);
                    valueText.text = $"{Player.Unit.maxWitchAttackSpeed.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.CriRate:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CriRate].StringToLocal);
                    valueText.text = $"+{Player.Unit.CriRate.ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.CriDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CriDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.CriDmg.ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.SuperRate:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SuperRate].StringToLocal);
                    valueText.text = $"+{Player.Unit.SuperRate.ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.SuperDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SuperDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.SuperDmg.ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.MegaRate:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MegaRate].StringToLocal);
                    valueText.text = $"+{Player.Unit.MegaRate.ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.MegaDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MegaDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.MegaDmg.ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.MoveSpeed:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MoveSpeed].StringToLocal);
                    valueText.text = $"+{Player.Unit.MoveSpeed.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.MaxHp:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    valueText.text = $"{Player.Unit.MaxHp.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.HpRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_HpRecover].StringToLocal);
                    valueText.text = $"+{Player.Unit.HpRecover.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.MaxShield:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    valueText.text = $"{Player.Unit.MaxShield.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.ShieldRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ShieldRecover].StringToLocal);
                    valueText.text = $"+{Player.Unit.ShieldRecover.ToNumberString()}";
                    break;
                case CharacterAbilityTypes.MonsterAtkDecrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MonsterAtkDecrease].StringToLocal);
                    valueText.text = $"-{Player.Unit.GetResearchMonsterAtkDecrease().ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.BossAtkDecrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BossAtkDecrease].StringToLocal);
                    valueText.text = $"-{Player.Unit.GetResearchBossAtkDecrease().ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.AtkToNormalMonsterIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkToMonsterIncrease].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusNormalDmgIncrease().ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.AtkToBossMonsterIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkToBossIncrease].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusBossDmgIncrease().ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.MonsterHpDecrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MonsterHpDecrease].StringToLocal);
                    valueText.text = $"-{Player.Unit.GetPetMonsterHpDecrease().ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.BossHpDecrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BossHpDecrease].StringToLocal);
                    valueText.text = $"-{Player.Unit.GetPetBossHpDecrease().ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.SkillAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    valueText.text = $"+{(Player.Unit.GetSkillIncreaseValue()*100).ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.AwakeTime:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AwakeTime].StringToLocal);
                    valueText.text = $"+{(int)Player.Unit.AwakeTime}sec";
                    break;
                case CharacterAbilityTypes.CoinIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CoinIncrease].StringToLocal);
                    valueText.text = $"+{(Player.Unit.GetCoinIncreaseValue() * 100).ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.ExpIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ExpIncrease].StringToLocal);
                    valueText.text = $"+{(Player.Unit.GetExpIncreaseValue() * 100).ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.PetDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_PetDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.IncreasePetAtk.ToNumberString()}%";
                    break;
                case CharacterAbilityTypes.End:
                    break;
                default:
                    break;
            }
  
        }

        public void Init(GoldUpgradeKey abilType)
        {
            switch (abilType)
            {
                case GoldUpgradeKey.None:
                    break;
                case GoldUpgradeKey.AttackIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkIncrease].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.AttackIncrease).ToNumberString()}";
                    break;
                case GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AwakeAtkSpeed].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch).ToNumberString()}";
                    break;
                case GoldUpgradeKey.IncreaseHp:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseHp).ToNumberString()}";
                    break;
                case GoldUpgradeKey.IncreaseHpRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_HpRecover].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseHpRecover).ToNumberString()}";
                    break;
                case GoldUpgradeKey.IncreaseShield:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseShield).ToNumberString()}";
                    break;
                case GoldUpgradeKey.IncreaseShieldRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ShieldRecover].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseShieldRecover).ToNumberString()}";
                    break;
                case GoldUpgradeKey.IncreaseCharacterAwakeTime:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AwakeTime].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseCharacterAwakeTime).ToNumberString()}s";
                    break;
                case GoldUpgradeKey.IncreaseCriticalRate:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CriRate].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseCriticalRate).ToNumberString()}%";
                    break;
                case GoldUpgradeKey.IncreaseCriticalDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CriDmg].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseCriticalDmg).ToNumberString()}%";
                    break;
                case GoldUpgradeKey.IncreaseSuperRate:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SuperRate].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseSuperRate).ToNumberString()}%";
                    break;
                case GoldUpgradeKey.IncreaseSuperDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SuperDmg].StringToLocal);
                    valueText.text = $"+{Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseSuperDmg).ToNumberString()}%";
                    break;
                default:
                    break;
            }
          
        }

        public void Init(Tier2GoldUpgradeKey abilType)
        {
            switch (abilType)
            {
                case Tier2GoldUpgradeKey.None:
                    break;
                case Tier2GoldUpgradeKey.SwordAttackIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtk].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.SwordAttackIncrease).ToNumberString()}";
                    break;
                case Tier2GoldUpgradeKey.MagicAttackIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BowAtk].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.MagicAttackIncrease).ToNumberString()}";
                    break;
                case Tier2GoldUpgradeKey.IncreaseHp:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseHp).ToNumberString()}";
                    break;
                case Tier2GoldUpgradeKey.IncreaseShield:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseShield).ToNumberString()}";
                    break;
                case Tier2GoldUpgradeKey.IncreaseHpRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_HpRecover].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseHpRecover).ToNumberString()}";
                    break;
                case Tier2GoldUpgradeKey.IncreaseShieldRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ShieldRecover].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseShieldRecover).ToNumberString()}";
                    break;
                case Tier2GoldUpgradeKey.IncreaseMegaHitRate:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MegaRate].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseMegaHitRate).ToNumberString()}%";
                    break;
                case Tier2GoldUpgradeKey.IncreaseMegaHitDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MegaDmg].StringToLocal);
                    valueText.text = $"+{Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseMegaHitDmg).ToNumberString()}%";
                    break;
                case Tier2GoldUpgradeKey.End:
                    break;
                default:
                    break;
            }
        }
        public void Init(StatAbilityTypes abilType)
        {
         
            switch (abilType)
            {
                case StatAbilityTypes.AtkSpeedIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtkSpeed].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusAttackSpeedIncrease().ToNumberString()}";
                    break;
                case StatAbilityTypes.AtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkIncrease].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusAttackIncrease().ToNumberString()}";
                    break;
                case StatAbilityTypes.HpIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusMaxHpIncrease().ToNumberString()}";
                    break;
                case StatAbilityTypes.ShieldIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusMaxShieldIncrease().ToNumberString()}";
                    break;
                case StatAbilityTypes.SwordAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtk].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusSwordIncrease().ToNumberString()}";
                    break;
                case StatAbilityTypes.MagicAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BowAtk].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusMagicIncrease().ToNumberString()}";
                    break;
                case StatAbilityTypes.AtkToNormalMonsterIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkToMonsterIncrease].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusNormalDmgIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.AtkToBossMonsterIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkToBossIncrease].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusBossDmgIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.CriDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CriDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusCriDmgIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.SkillDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusSkillDmgIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.SuperDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SuperDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusSuperDmgIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.HpRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_HpRecover].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusRecoverHpIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.ShieldRecover:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ShieldRecover].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusRecoverShieldIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.MegaDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.IncreaseMegaHitDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusMegaDmgIncrease().ToNumberString()}%";
                    break;
                case StatAbilityTypes.PetDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_PetDmg].StringToLocal);
                    valueText.text = $"+{Player.Unit.GetStatusPetDmgIncrease().ToNumberString()}%";
                    break;
                default:
                    break;
            }
        }
        public void Init(AwakeUpgradeKey abilType)
        {
            switch (abilType)
            {
                case AwakeUpgradeKey.IncreaseTotalAttack:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkIncrease].StringToLocal);
                    valueText.text = $"+{Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseTotalAttack).ToNumberString()}";
                    break;
                case AwakeUpgradeKey.IncreaseSkillAttack:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    valueText.text = $"+{Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseSkillAttack).ToNumberString()}";
                    break;
                case AwakeUpgradeKey.IncreaseMaxHp:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    valueText.text = $"+{Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseMaxHp).ToNumberString()}";
                    break;
                case AwakeUpgradeKey.IncreaseRecoverHp:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_HpRecover].StringToLocal);
                    valueText.text = $"+{Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseRecoverHp).ToNumberString()}";
                    break;
                case AwakeUpgradeKey.IncreaseMaxShield:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    valueText.text = $"+{Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseMaxShield).ToNumberString()}";
                    break;
                case AwakeUpgradeKey.IncreaseRecoverShield:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ShieldRecover].StringToLocal);
                    valueText.text = $"+{Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseRecoverShield).ToNumberString()}";
                    break;
                case AwakeUpgradeKey.End:
                    break;
                default:
                    break;
            }
    
        }

        public void Init(ItemAbilityTypes abilType)
        {
            double datavalue = 0;
            switch (abilType)
            {
                case ItemAbilityTypes.SwordAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtk].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.SwordAttack_Increase] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.SwordAttack_Increase];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.HpIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.HpIncrease] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.HpIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.LastSwordAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_LastSwordAtk].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.SwordAttackLastDMG_Increase] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.SwordAttackLastDMG_Increase];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.MagicAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BowAtk].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.BowAttack_Increase] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.BowAttack_Increase];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.CommonAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkIncrease].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.BowAttackLastDMG_Increase] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.BowAttackLastDMG_Increase];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.LastMagicAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_LastBowAtk].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.BowAttackLastDMG_Increase] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.BowAttackLastDMG_Increase];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.ShieldIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.ShieldIncrease] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.ShieldIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.ShieldRecoverIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ShieldRecover].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.ShieldRecover] + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.ShieldRecover];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.MegaHitDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MegaDmg].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.IncreaseMegaHitDmg];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.SkillDmgIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.SkillDmgIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.DecreaseDmgFromNormal:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_DecreaseDmgFromNormal].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.DmgDecreaseFromNormalMonster];
                    valueText.text = $"-{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.IncreaseDmgToNormal:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkToMonsterIncrease].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.IncreaseDmgToNormalMonster];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.IncreaseAwakeTime:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AwakeTime].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.awakeTimeIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}s";
                    break;
                case ItemAbilityTypes.IncreasePetAttack:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_PetDmg].StringToLocal);
                    datavalue = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.IncreasePetAttack];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ItemAbilityTypes.End:
                    break;
                default:
                    break;
            }
        }

        public void Init(PetAbilityTypes abilType)
        {
            double datavalue = 0;
            switch (abilType)
            {
                case PetAbilityTypes.SwordAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtk].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.SwordAttack_Increase];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.MagicAtkIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BowAtk].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.BowAttack_Increase];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.HpIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.HpIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.ShieldIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.ShieldIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.MonsterHpDecrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MonsterHpDecrease].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.MonstaerhpDecrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.BossHpDecrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BossHpDecrease].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.BosshpDecrease];
                    valueText.text = $"-{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.SkillDmgIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.SkillDmgIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.CoinIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CoinIncrease].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.CoinGetIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.ExpIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ExpIncrease].StringToLocal);
                    datavalue = Player.Pet._equipAbilitycaches[EquipAbilityKey.GetExpIncrease];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case PetAbilityTypes.End:
                    break;
                default:
                    break;
            }
        }

        public void Init(AdvancementAbilityKey abilType)
        {
            double datavalue = 0;
            switch (abilType)
            {
                case AdvancementAbilityKey.IncreaseTotalAttack:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkIncrease].StringToLocal);
                    datavalue = Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseTotalAttack];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseSwordAttack:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtk].StringToLocal);
                    datavalue = (1+Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseSwordAttack]*0.01f)*(1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseSwordAttack_2]*0.01f)*100;
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseBowAttack:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BowAtk].StringToLocal);
                    datavalue = (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseBowAttack] * 0.01f) * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseBowAttack_2] * 0.01f) * 100;
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseMaxHp:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    datavalue = (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxHp] * 0.01f) * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxHp_2] * 0.01f) * 100;
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseRecoverHp:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_HpRecover].StringToLocal);
                    datavalue = Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseRecoverHp];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseMaxShield:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    datavalue = (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxShield] * 0.01f) * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxShield_2] * 0.01f) * 100;
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseRecoverShield:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_ShieldRecover].StringToLocal);
                    datavalue = Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseRecoverShield];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseMoveSpeed:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MoveSpeed].StringToLocal);
                    datavalue = Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMoveSpeed];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseSkillDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    datavalue = Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseSkillDmg];
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreasePetDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_PetDmg].StringToLocal);
                    datavalue = (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreasePetDmg] * 0.01f) * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreasePetDmg_2] * 0.01f) * 100;
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.IncreaseMegaDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MegaDmg].StringToLocal);
                    datavalue = (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMegaDmg] * 0.01f) * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMegaDmg_2] * 0.01f) * 100;
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case AdvancementAbilityKey.End:
                    break;
                default:
                    break;
            }
        }

        public void Init(ResearchUpgradeKey abilType)
        {
            double datavalue = 0;
            switch (abilType)
            {
                case ResearchUpgradeKey.None:
                    break;
                case ResearchUpgradeKey.ExpandResearchSlot:
                    break;
                case ResearchUpgradeKey.IncreaseEarnGold:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_GoldIncrease].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.IncreaseEarnGold);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.CommonAttackIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AtkIncrease].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.CommonAttackIncrease);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.WitchAwakeTimeIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_AwakeTime].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.WitchAwakeTimeIncrease);
                    valueText.text = $"+{datavalue.ToNumberString()}s";
                    break;
                case ResearchUpgradeKey.SkillAbilityIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.SkillAbilityIncrease);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.PetAttackIncrease:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_PetDmg].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.PetAttackIncrease);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.DecreaseNormalMonsterDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_DecreaseDmgFromNormal].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.DecreaseBossMonsterDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_DecreaseDmgFromBoss].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.DecreaseBossMonsterDmg);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.IncreaseCriDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_CriDmg].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.IncreaseCriDmg);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.IncreaseSuperDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SuperDmg].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.IncreaseSuperDmg);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.SwordAttackIncrease_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SwordAtk].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.SwordAttackIncrease_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.MagicAttackIncrease_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_BowAtk].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.MagicAttackIncrease_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.SkillAbilityIncrease_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_SkillDmgIncrease].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.SkillAbilityIncrease_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.PetAttackIncrease_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_PetDmg].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.PetAttackIncrease_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.DecreaseNormalMonsterDmg_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_DecreaseDmgFromNormal].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.DecreaseBossMonsterDmg_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_DecreaseDmgFromBoss].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.DecreaseBossMonsterDmg_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.IncreaseMaxHp_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxHp].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.IncreaseMaxHp_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.IncreaseMaxShield_2:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MaxShield].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.IncreaseMaxShield_2);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.IncreaseMegaDmg:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_MegaDmg].StringToLocal);
                    datavalue = Player.Research.GetValue(ResearchUpgradeKey.IncreaseMegaDmg);
                    valueText.text = $"+{datavalue.ToNumberString()}%";
                    break;
                case ResearchUpgradeKey.End:
                    break;
                default:
                    break;
            }
        }


        public void Init(ShieldAbilityTypes abilType)
        {
            switch (abilType)
            {
                case ShieldAbilityTypes.ShieldDecreaseWitchAtk:
                    titleText.text = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.ProfileDetail_atkDecreaseAtNoneSheild].StringToLocal);
                    valueText.text = string.Format("-{0:F0}%", (1.0f-Player.Unit.GetBowRateValueAtGhost())*100.0f); 
                    break;
                case ShieldAbilityTypes.End:
                    break;
                default:
                    break;
            }
        }
    }

}
