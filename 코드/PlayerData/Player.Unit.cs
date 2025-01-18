    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;

namespace BlackTree.Model
{
    public static partial class Player
    {
       
       
        public static class Unit
        {
            public static Dictionary<UnitAbility, double> _passiveSkillAbilitycaches;
            public static Dictionary<SkillAbilityKey, double> _activeSkillAbilitycaches;
            public static List<SkillKey> playingskillList;

            public static Core.ControllerUnitInGame userUnit=null;
            public static Core.ControllerSubUnitInGame usersubUnit = null;
            public static int criticalUpgradeIdx;
            public static double SwordAtk { get; set; }
            public static double BowAtk { get; set; }
            public static double AtkSpeed { get; set; }
            public static double CriRate { get; set; }
            public static double CriDmg { get; set; }
            public static double SuperRate { get; set; }
            public static double SuperDmg { get; set; }
            public static double MoveSpeed { get; set; }
            public static double Hp{ get; set; }
            public static double MaxHp { get; set; }
            public static double HpRecover { get; set; }
            public static double Shield { get; set; }
            public static double MaxShield { get; set; }
            public static double ShieldRecover { get; set; }
            public static double DecreaseDmgFromMonster { get; set; }
            public static double IncreaseAtkToNormalMonster { get; set; }
            public static double IncreaseAtkToBossMonster { get; set; }
            public static double MegaRate { get; set; }
            public static double MegaDmg { get; set; }
            public static double IncreasePetAtk { get; set; }

            //touchAttack
            public static double maxWitchAttackSpeed { get; set; }
            //touchAttack

            const int defaultMaxWitchAtkSpeed = 700;

            const int defaultAutoWitchAtkSpeed = 200;

            const double defaultAtk = 231;
            const double defaultHp = 1000;
            const double defaultShield = 700;

            const double defaultMoveSpeed = 10;
            const double defaultAttackSpeed = 1;

            const double defaultHpRecover = 10;
            const double defaultShieldRecover = 8;

            public static ShieldState shieldState;
            public static System.Action<ShieldState> shieldStateChangeCallback;

            public static System.Action syncHpUI;

            public static float BowRateValueAtGhost = 0.7f;

            //buffskill logic
            public static System.Action buffUpdate;

            //awakeCharacter
            public static CharacterAwakeState awakeState;
            public static CharacterAwakeChange awakeChange;
            public static float normalawakeTime=8.0f;
            public static float currentAwakeTime = 0.0f;
            public static float currentAwakeTimeForQuest = 0.0f;

            public static int characterAwakeTouchCount = 0;
            public static int characterAwakeMaxTouchCount =30;
            public static System.Action<CharacterAwakeState> characterAwakeCallback;
            public static System.Action characterAwakeChangeCallback;

            public static bool isEliteAtkBuff = false;
            public static float atkBuffValue = 1.5f;

            public static float AwakeTime
            {
                get
                {
                    return normalawakeTime + (float)Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseCharacterAwakeTime) +
                        (float)Player.Research.GetValue(ResearchUpgradeKey.WitchAwakeTimeIncrease)
                         + (float)(Player.EquipItem._equipAbilitycaches[EquipAbilityKey.awakeTimeIncrease]);
                         
                }
            }
            public static void Init()
            {
                playingskillList = new List<SkillKey>();
                _activeSkillAbilitycaches = new Dictionary<SkillAbilityKey, double>();
                for (int i=0; i<(int)SkillAbilityKey.End; i++)
                {
                    _activeSkillAbilitycaches.Add((SkillAbilityKey)i, 0);
                }
        
                Hp = defaultHp;
                MaxHp = Hp;

                Shield = defaultShield;
                MaxShield = Shield;

                shieldState = ShieldState.Idle;

                StatusSync();

                if(Player.Cloud.userStatusValue.hp<=0)
                {
                    Player.Cloud.userStatusValue.hp = MaxHp;
                    Player.Cloud.userStatusValue.shield = MaxShield;
                }
                Hp = Player.Cloud.userStatusValue.hp;
                Shield = Player.Cloud.userStatusValue.shield;

                awakeState = CharacterAwakeState.Normal;

                //int startindex = Cloud.chapterRewardedData.rewardedList.Count;
                //int lastindex = 5 * StaticData.Wrapper.chapterRewardTableDatas.Length;
                //for (int i = startindex; i < lastindex; i++)
                //{
                //    Cloud.chapterRewardedData.rewardedList.Add(false);
                //}
                //Cloud.chapterRewardedData.rewardedList[0] = true;

                
                if(Cloud.chapterRewardedData.LastRewardIndex<0)
                {
                    Cloud.chapterRewardedData.LastRewardIndex = 0;
                    LocalSaveLoader.SaveUserCloudData();
                    Cloud.chapterRewardedData.UpdateHash().SetDirty(true);
                }
                
                
                    
            }

            public static void StatusSync()
            {
                var baseAtk = (defaultAtk + Player.GoldUpgrade.GetValue(GoldUpgradeKey.AttackIncrease) + GetStatusAttackIncrease() * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseTotalAttack] * 0.01f))
                    *(isEliteAtkBuff? atkBuffValue:1);

                var baseSwordAtk = baseAtk+Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.SwordAttackIncrease) + GetStatusSwordIncrease();

                SwordAtk = baseSwordAtk * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.AttackIncrease]) * 0.01f + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.AttackIncrease]) * 0.01f
                    + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.SwordAttack_Increase]) * 0.01f + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.SwordAttack_Increase]) * 0.01f)
                    * (1 + (Player.Pet._equipAbilitycaches[EquipAbilityKey.AttackIncrease]) * 0.01f + (Player.Pet._equipAbilitycaches[EquipAbilityKey.SwordAttack_Increase]) * 0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseSwordAttack] * 0.01f)
                    * (1 + EquipItem._equipAbilitycaches[EquipAbilityKey.SwordAttackLastDMG_Increase] * 0.01f)
                    * (1 + GetSkillAttackPowerAbility() * 0.01f)
                    * Player.AdsBuff.GetBuffValueData(AdsBuffType.AttackIncrease)
                    * (1+Player.Research.GetValue(ResearchUpgradeKey.CommonAttackIncrease)*0.01f)
                    *(1+ Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseTotalAttack) * 0.01f)
                    * (1 + Player.Research.GetValue(ResearchUpgradeKey.SwordAttackIncrease_2) * 0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseSwordAttack_2] * 0.01f)
                    *(1+ GetRuneSwordDmgIncrease()*0.01f)
                    *(1+ Player.Pet._passiveSkillAbilitycaches[PetSkillAbilityKey.IncreaseSwordAttackPower]*0.01f);

                //double calculatedbowrateValue = defaultBowRateValue;
                
                var baseBowAtk = (baseAtk)+Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.MagicAttackIncrease) + GetStatusMagicIncrease();

                var calculatedBowAtk= baseBowAtk * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.AttackIncrease]) * 0.01f + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.AttackIncrease]) * 0.01f
                        + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.BowAttack_Increase]) * 0.01f + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.BowAttack_Increase]) * 0.01f)
                    * (1 + (Player.Pet._equipAbilitycaches[EquipAbilityKey.AttackIncrease])*0.01f + (Player.Pet._equipAbilitycaches[EquipAbilityKey.BowAttack_Increase]) * 0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseBowAttack] * 0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseBowAttack_2] * 0.01f)
                    * (1 + EquipItem._equipAbilitycaches[EquipAbilityKey.BowAttackLastDMG_Increase] * 0.01f)
                    * (1 + GetSkillAttackPowerAbility()*0.01f) 
                    * Player.AdsBuff.GetBuffValueData(AdsBuffType.AttackIncrease)
                    * (1 + Player.Research.GetValue(ResearchUpgradeKey.CommonAttackIncrease) * 0.01f)
                    *(1+ Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseTotalAttack) * 0.01f)
                    * (1 + Player.Research.GetValue(ResearchUpgradeKey.MagicAttackIncrease_2) * 0.01f)
                    * (1 + GetRuneMagicDmgIncrease() * 0.01f)
                    * (1 + Player.Pet._passiveSkillAbilitycaches[PetSkillAbilityKey.IncreaseMagicAttackPower] * 0.01f);

                if (shieldState==ShieldState.Recovering)
                {
                    BowAtk= calculatedBowAtk * GetBowRateValueAtGhost();
                }
                else
                {
                    BowAtk = calculatedBowAtk;
                }

                AtkSpeed = (1 + (GetStatusAttackSpeedIncrease()*0.01f) * (1 + GetSkillattackSpeedAbility() * 0.01f) ) * (awakeState==CharacterAwakeState.Awake?2:1);

                MoveSpeed = defaultMoveSpeed * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.MoveSpeed] * 0.01f))
                    * (1 + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.MoveSpeed] * 0.01f))
                    * (1 + GetSkillMoveAbility() * 0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMoveSpeed] * 0.01f)
                    * (awakeState == CharacterAwakeState.Awake ? 2 : 1);

                double rateHp = Hp / MaxHp;

                var baseHp= defaultHp + Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseHp) + Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseHp) + GetStatusMaxHpIncrease() ;
                MaxHp = (baseHp * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.HpIncrease]) * 0.01f
                    + (Player.EquipItem._possessAbilitycaches[EquipAbilityKey.HpIncrease]) * 0.01f)) 
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxHp] * 0.01f)* (1 + Player.Pet._equipAbilitycaches[EquipAbilityKey.HpIncrease]*0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxHp_2] * 0.01f)
                    * (1+ Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseMaxHp)*0.01f)
                     * (1 + Player.Research.GetValue(ResearchUpgradeKey.IncreaseMaxHp_2) * 0.01f)
                     *(1+ GetRuneMaxHPIncrease()*0.01f);

                Hp = MaxHp * rateHp;

                double rateSH = Shield/ MaxShield;

                var baseshield = defaultShield + Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseShield) + Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseShield) + GetStatusMaxShieldIncrease();

                MaxShield = (baseshield * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.ShieldIncrease]) * 0.01f + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.ShieldIncrease] * 0.01f))
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxShield] * 0.01f)* (1+Player.Pet._equipAbilitycaches[EquipAbilityKey.ShieldIncrease] * 0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMaxShield_2] * 0.01f)
                    * Player.AdsBuff.GetBuffValueData(AdsBuffType.ShieldInrease)
                    *(1+ Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseMaxShield)*0.01f)
                    * (1 + Player.Research.GetValue(ResearchUpgradeKey.IncreaseMaxShield_2) * 0.01f)
                    * (1 + GetRuneMaxShieldIncrease() * 0.01f);

                Shield = MaxShield * rateSH;

                var basehpRecover = defaultHpRecover + Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseHpRecover) + Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseHpRecover) + GetStatusRecoverHpIncrease();
                 
                HpRecover = (basehpRecover * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.HpRecover]) * 0.01f + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.HpRecover] * 0.01f))
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseRecoverHp] * 0.01f)
                       * (1+ Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseRecoverHp)*0.01f);

                var baseShieldRecover = defaultShieldRecover + Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseShieldRecover) + Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseShieldRecover) + GetStatusRecoverShieldIncrease();
                ShieldRecover = (baseShieldRecover * (1 + (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.ShieldRecover]) * 0.01f + Player.EquipItem._possessAbilitycaches[EquipAbilityKey.ShieldRecover] * 0.01f))
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseRecoverShield] * 0.01f)
                     * (1 + Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseRecoverShield) * 0.01f)
                     * (1 + GetRuneShieldRecoverIncrease() * 0.01f);

                CriRate = Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseCriticalRate);

                var baseCriDmg = (Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseCriticalDmg) + GetStatusCriDmgIncrease() )*(1+ Player.Research.GetValue(ResearchUpgradeKey.IncreaseCriDmg)*0.01f);
                CriDmg = baseCriDmg;

                SuperRate = Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseSuperRate);

                var baseSuperDmg =( Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseSuperDmg) + GetStatusSuperDmgIncrease() )* (1 + Player.Research.GetValue(ResearchUpgradeKey.IncreaseSuperDmg) * 0.01f);
                SuperDmg = baseSuperDmg;

                maxWitchAttackSpeed = defaultMaxWitchAtkSpeed + Player.GoldUpgrade.GetValue(GoldUpgradeKey.IncreaseMaxAtkSpeedForWitch);

                DecreaseDmgFromMonster = Player.EquipItem._equipAbilitycaches[EquipAbilityKey.DmgDecreaseFromNormalMonster] * 0.01f;

                IncreaseAtkToNormalMonster =(1+ GetStatusNormalDmgIncrease()*0.01f)*(1+Player.EquipItem._equipAbilitycaches[EquipAbilityKey.IncreaseDmgToNormalMonster]*0.01f);
                IncreaseAtkToBossMonster = (1+GetStatusBossDmgIncrease() * 0.01f);

                MegaRate = Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseMegaHitRate);

                var baseMegaDmg =( Player.SecondGoldUpgrade.GetValue(Tier2GoldUpgradeKey.IncreaseMegaHitDmg) + GetMegaDmgIncrease()+GetStatusMegaDmgIncrease()+
                     Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMegaDmg]+ Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseMegaDmg_2])
                     *(1+Player.Research.GetValue(ResearchUpgradeKey.IncreaseMegaDmg)*0.01f);
                MegaDmg= baseMegaDmg;

                IncreasePetAtk =  (1 + GetEquipItemPetDmgIncrease() * 0.01f) * (1 + GetStatusPetDmgIncrease() * 0.01f)
                    *(1+ GetResearchPetDmgIncrease()*0.01f)*(1+ GetAdvancePetDmgIncrease()*0.01f)
                    *(1+ GetRunePetDmgIncrease()*0.01f);
            }

            #region statusValue
            public static double GetSkillAttackPowerAbility()
            {
                double defaultData = _activeSkillAbilitycaches[SkillAbilityKey.IncreaseAttackPower] * GetSkillIncreaseValue();

                return defaultData;
            }

            public static double GetSkillMoveAbility()
            {
                double defaultData = _activeSkillAbilitycaches[SkillAbilityKey.IncreaseMoveSpeed];
                return defaultData;
            }

            public static double GetSkillattackSpeedAbility()
            {
                double defaultData = _activeSkillAbilitycaches[SkillAbilityKey.IncreaseAttackSpeed];

                return defaultData;
            }

            public static double GetSkillIncreaseValue()
            {
                double defaultdata = (1 + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_0) * 0.01f
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_1) * 0.01f
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_2) * 0.01f
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_3) * 0.01f
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_4) * 0.01f
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_5) * 0.01f
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_6) * 0.01f)
                    * (1 + Player.Pet._equipAbilitycaches[EquipAbilityKey.SkillDmgIncrease] * 0.01f)
                    * (1 + Player.AwakeUpgrade.GetValue(AwakeUpgradeKey.IncreaseSkillAttack) * 0.01f)
                     * (1 + Player.Research.GetValue(ResearchUpgradeKey.SkillAbilityIncrease) * 0.01f)
                     * (1 + Player.Research.GetValue(ResearchUpgradeKey.SkillAbilityIncrease_2) * 0.01f)
                    * (1 + Player.EquipItem._equipAbilitycaches[EquipAbilityKey.SkillDmgIncrease] * 0.01f)
                    * (1 + Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreaseSkillDmg] * 0.01f)
                    *(1+ GetRuneSkillDmgIncrease()*0.01f);
                return defaultdata;
            }

            public static double GetCoinIncreaseValue()
            {
                double data = (1 + Player.Pet._equipAbilitycaches[EquipAbilityKey.CoinGetIncrease] * 0.01f)
                        * (1 + Player.Research.GetValue(ResearchUpgradeKey.IncreaseEarnGold) * 0.01f);

                return data;
            }
            public static double GetExpIncreaseValue()
            {
                double data = (1 + Player.Pet._equipAbilitycaches[EquipAbilityKey.GetExpIncrease] * 0.01f);

                return data;
            }
            public static double GetBowRateValueAtGhost()
            {
                return BowRateValueAtGhost;
            }
            public static double GetStatusAttackIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.AttackIncrease_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.AttackIncrease_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.AttackIncrease_2)
                 + Player.StatusUpgrade.GetValue(StatusUpgradeKey.AttackIncrease_3) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.AttackIncrease_4) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.AttackIncrease_5);
                return data;
            }
            public static double GetStatusSwordIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_2)
             + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_3) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_4)+ Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_5)
             + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_6) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_7) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSwordDmg_8);
                return data;
            }
            public static double GetStatusMagicIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_2)
                            + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_3) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_4) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_5)
                            + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_6) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_7) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMagicDmg_8);
                return data;
            }
            public static double GetStatusMaxHpIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_2)
                            + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_3) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_4) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_5)
                            + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_6) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_7) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxHp_8);
                return data;
            }
            public static double GetStatusMaxShieldIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_2)
                       + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_3) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_4) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_5)
                       + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_6) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_7) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMaxShield_8);
                return data;
            }
            public static double GetStatusAttackSpeedIncrease()
            {
                var data = (Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseAttackSpeed_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseAttackSpeed_1));
                    
                return data;
            }
            /// <summary>
            /// //
            /// </summary>
            /// <returns></returns>
            public static double GetStatusNormalDmgIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToNormalMonster_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToNormalMonster_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToNormalMonster_2)
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToNormalMonster_3);
                return data;
            }
            public static double GetStatusBossDmgIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToBossMonster_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToBossMonster_1) 
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToBossMonster_2)
                  + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseDmgToBossMonster_3);
                return data;
            }
            public static double GetPetBossHpDecrease()
            {
                var data = Player.Pet._equipAbilitycaches[EquipAbilityKey.BosshpDecrease];
                return data;
            }
            public static double GetPetMonsterHpDecrease()
            {
                var data = Player.Pet._equipAbilitycaches[EquipAbilityKey.MonstaerhpDecrease];
                return data;
            }

            public static double GetResearchBossAtkDecrease()
            {
                var data = Player.Research.GetValue(ResearchUpgradeKey.DecreaseBossMonsterDmg)+ Player.Research.GetValue(ResearchUpgradeKey.DecreaseBossMonsterDmg_2);
                return data;
            }
            public static double GetResearchMonsterAtkDecrease()
            {
                var data = Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg)+ Player.Research.GetValue(ResearchUpgradeKey.DecreaseNormalMonsterDmg_2);
                return data;
            }
            /// <summary>
            /// /
            /// </summary>
            /// <returns></returns>
            public static double GetStatusCriDmgIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseCriticalDmg_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseCriticalDmg_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseCriticalDmg_2);
                return data;
            }
            public static double GetMegaDmgIncrease()
            {
                var data = (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.IncreaseMegaHitDmg]);
                return data;
            }
            public static double GetStatusMegaDmgIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMegaDmg_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMegaDmg_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseMegaDmg_2);
                return data;
            }
            public static double GetEquipItemPetDmgIncrease()
            {
                var data = (Player.EquipItem._equipAbilitycaches[EquipAbilityKey.IncreasePetAttack]);
                return data;
            }

            public static double GetResearchPetDmgIncrease()
            {
                var data = Player.Research.GetValue(ResearchUpgradeKey.PetAttackIncrease) + Player.Research.GetValue(ResearchUpgradeKey.PetAttackIncrease_2);
                return data;
            }
            public static double GetStatusRecoverHpIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseRecoverHp_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseRecoverHp_1)
                     + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseRecoverHp_2);
                return data;
            }
            public static double GetStatusRecoverShieldIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseRecoverShield_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseRecoverShield_1)
                     + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseRecoverShield_2);
                return data;
            }
            public static double GetStatusSuperDmgIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSuperDmg_0) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSuperDmg_1);
                return data;
            }

            public static double GetStatusSkillDmgIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_0)
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_2) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_3)
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_4) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_5) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreaseSkillDmg_6);

                return data;
            }

            public static double GetStatusPetDmgIncrease()
            {
                var data = Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreasePetDmg_0)
                    + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreasePetDmg_1) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreasePetDmg_2) + Player.StatusUpgrade.GetValue(StatusUpgradeKey.IncreasePetDmg_3);

                return data;
            }

            public static double GetAdvancePetDmgIncrease()
            {
                var data = Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreasePetDmg]+ Player.AwakeUpgrade._advanceAbilityData[AdvancementAbilityKey.IncreasePetDmg_2];

                return data;
            }

            /////////////////////////////////////////////////////////RUNE/////////////////////////////////////////////////////////

            public static double GetRuneSwordDmgIncrease()
            {
                var data = Rune._equipAbilitycaches[RuneAbilityKey.SwordAttackIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.SATKIncreaseWhenUseGodmodeSkill]
                    + Rune._equipAbilitycaches[RuneAbilityKey.SATKIncreaseWhenUseSummonSpawnSkill]
                    + Rune._equipAbilitycaches[RuneAbilityKey.ThirdgradeSwordTotalLevelSwordtkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FourthgradeSwordTotalLevelSwordtkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FifthgradeSwordTotalLevelSwordtkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.ThirdgradeSkillTotalLevelSwordtkIncrease];

                return data;
            }
            public static double GetRuneMagicDmgIncrease()
            {
                var data = Rune._equipAbilitycaches[RuneAbilityKey.MagicAttackIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.MATKIncreaseWhenUseGuideMissileSkill]
                    + Rune._equipAbilitycaches[RuneAbilityKey.MATKIncreaseWhenUseLaserSkill]
                    + Rune._equipAbilitycaches[RuneAbilityKey.ThirdgradeStaffTotalLevelMagictkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FourthgradeStaffTotalLevelMagictkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FifthgradeStaffTotalLevelMagictkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FourthgradeSkillTotalLevelMagictkIncrease];

                return data;
            }

            public static double GetRuneSkillDmgIncrease()
            {
                var data = Rune._equipAbilitycaches[RuneAbilityKey.SkillAttackIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.ThirdgradePetTotalLevelSkillAtkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FourthgradePetTotalLevelSkillAtkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FifthgradePetTotalLevelSkillAtkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.OneToTwocountForSkillAwake_AtkIncrease];

                return data;
            }
            public static double GetRunePetDmgIncrease()
            {
                var data = Rune._equipAbilitycaches[RuneAbilityKey.PetAttackIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.ThirdgradeArmorTotalLevelPettkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FourthgradeArmorTotalLevelPettkIncrease]
                    + Rune._equipAbilitycaches[RuneAbilityKey.FifthgradeArmorTotalLevelPettkIncrease];

                return data;
            }

            public static double GetRuneMaxHPIncrease()
            {
                var data = Rune._equipAbilitycaches[RuneAbilityKey.HpIncrease];

                return data;
            }
            public static double GetRuneMaxShieldIncrease()
            {
                var data = Rune._equipAbilitycaches[RuneAbilityKey.ShieldIncrease];

                return data;
            }
            public static double GetRuneShieldRecoverIncrease()
            {
                var data = Rune._equipAbilitycaches[RuneAbilityKey.ShieldIncrease];

                return data;
            }

            ////////////////////////////////////////////////////////////RUNE/////////////////////////////////////////////////////////
            #endregion
            public static void RecoverForLvUp()
            {
                Hp = MaxHp;
                Shield = MaxShield;

                if (shieldState == ShieldState.Recovering)
                {
                    shieldState = ShieldState.Idle;
                    shieldStateChangeCallback?.Invoke(ShieldState.Idle);
                    StatusSync();
                }
                shieldState = ShieldState.Idle;
            }

            public static void ResetUnit()
            {
                Hp = MaxHp;
                Shield = MaxShield;
                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.hp = Hp;
                    Player.Cloud.userStatusValue.shield = Shield;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);

                    shieldState = ShieldState.Idle;
                    shieldStateChangeCallback?.Invoke(ShieldState.Idle);
                    SkillCoolTimeInit();

                    StatusSync();

                    Player.Unit.usersubUnit._state.ChangeState(eActorState.Idle);
                }
            }

            public static void SkillCoolTimeInit()
            {
                foreach (var skillcache in Player.Skill.skillCaches)
                {
                    if (skillcache.Value.IsEquiped)
                    {
                        skillcache.Value.userSkilldata.elapsedCooltime= skillcache.Value.tabledataSkill.coolTime;
                    }
                }
            }

            public static void IncreaseHp(double increasehpValue)
            {
                Hp += increasehpValue;
                if(Hp>=MaxHp)
                {
                    Hp = MaxHp;
                }

                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.hp = Hp;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);
                }

                syncHpUI?.Invoke();
            }
            public static void IncreaseHpPercent(float percentage)
            {
                double hpValue = MaxHp * percentage * 0.01f;
                Hp += hpValue;
                if (Hp >= MaxHp)
                {
                    Hp = MaxHp;
                }
                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.hp = Hp;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);
                }
                syncHpUI?.Invoke();
            }

            public static void IncreaseHpPercentAtHprecoverSkill(float percentage,float skillvalue1,float skillvalue2)
            {
                double hpValue = MaxHp * (percentage * 0.01f) *(Hp>(MaxHp/2)?1:(1+(skillvalue2*0.01f)) );
                double forShieldValue = 0;
                Hp += hpValue;
                if(Hp>MaxHp)
                {
                    forShieldValue = MaxHp - Hp;
                }
                if (Hp >= MaxHp)
                {
                    Hp = MaxHp;
                }
                if(forShieldValue>0)
                {
                    IncreaseShield(forShieldValue*(skillvalue1 * 0.01f));
                }
                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.hp = Hp;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);
                }
                syncHpUI?.Invoke();
            }
            public static void IncreaseShieldPercent(double increaseShield)
            {
                var shieldValue = (increaseShield / 100.0f) * MaxShield;
                Shield += shieldValue;

                if (Shield >= MaxShield)
                {
                    Shield = MaxShield;
                }
                if (shieldState == ShieldState.Recovering)
                {
                    if (Shield >= MaxShield / 2)
                    {
                        shieldState = ShieldState.Idle;
                        shieldStateChangeCallback?.Invoke(ShieldState.Idle);
                        StatusSync();
                    }
                }
                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.shield = Shield;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);
                }
                
                syncHpUI?.Invoke();
            }

            public static void IncreaseShield(double increaseShield)
            {
                Shield += increaseShield;

                if (Shield >= MaxShield)
                {
                    Shield = MaxShield;
                }
                if(shieldState==ShieldState.Recovering)
                {
                    if(Shield>=MaxShield/2)
                    {
                        shieldState = ShieldState.Idle;
                        shieldStateChangeCallback?.Invoke(ShieldState.Idle);
                        StatusSync();
                    }
                }
                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.shield = Shield;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);
                }
                syncHpUI?.Invoke();
            }

            public static void IncreaseHpForAbsorbSkill(double percentage)
            {
                double hpValue = MaxHp * percentage * 0.01f;
                
                if(Skill.Get(SkillKey.AbsorbLife).userSkilldata.AwakeLv>=2)
                {
                    var tempHP = Hp + hpValue;
                    if (tempHP >= MaxHp)
                    {
                        var remainHp = tempHP - MaxHp;
                        float skillvalue = Player.Skill.Get(SkillKey.AbsorbLife).SkillValue(2);
                        Hp = MaxHp;
                        IncreaseShield(remainHp*(skillvalue*0.01f));
                    }
                    else
                    {
                        Hp = tempHP;
                    }
                }
                else
                {
                    Hp += hpValue;
                    if (Hp >= MaxHp)
                    {
                        Hp = MaxHp;
                    }
                }
                if (Battle.Field.currentSceneState == eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.hp = Hp;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);
                }

        
                syncHpUI?.Invoke();
            }
            public static void DecreaseHp(double dmgValue)
            {
#if UNITY_EDITOR
                //return;
#endif

                if (Player.Unit.IsSkillActive(SkillKey.RecoverShield) && Player.Skill.Get(SkillKey.RecoverShield).userSkilldata.AwakeLv >= 2)
                {
                    dmgValue = dmgValue * 0.4f;
                }
                double resultdmgValue = dmgValue;
                if(IsSkillActive(Definition.SkillKey.IncreaseMoving))
                {
                    float increaseMoveskillValue = Player.Skill.Get(SkillKey.IncreaseMoving).SkillValue(2);
                    if (increaseMoveskillValue > 0)
                    {
                        resultdmgValue = dmgValue * (1 - increaseMoveskillValue * 0.01f);
                    }
                }

                double dmgdecrease = Player.Pet._passiveSkillAbilitycaches[PetSkillAbilityKey.DecreaseDmgFromMonster];
                if (dmgdecrease > 0)
                {
                    resultdmgValue = resultdmgValue * (1 - dmgdecrease * 0.01f);
                }
                

                if (shieldState == ShieldState.Idle)
                {
                    if(Shield>0)
                    {
                        Shield -= resultdmgValue;
                    }
                    double leftValue = Shield;
                    if(leftValue <= 0)
                    {
                        Hp += leftValue;
                        Shield = 0;
                        shieldState = ShieldState.Recovering;
                        shieldStateChangeCallback?.Invoke(ShieldState.Recovering);
                        StatusSync();
                    }
                }
                else
                {
                    Hp -= resultdmgValue;
                }

                if(Battle.Field.currentSceneState==eSceneState.RaidDungeon)
                {
                    double totalHp = Player.Unit.MaxHp + Player.Unit.MaxShield;
                    double minusTimeValue = resultdmgValue / totalHp;

                    Battle.Raid.timeMinusEvent?.Invoke(minusTimeValue);
                }

                if(Battle.Field.currentSceneState==eSceneState.MainIdle)
                {
                    Player.Cloud.userStatusValue.hp = Hp;
                    Player.Cloud.userStatusValue.shield = Shield;
                    Player.Cloud.userStatusValue.UpdateHash().SetDirty(true);
                }
           
                syncHpUI?.Invoke();
                if (Hp <= 0)
                {
                    userUnit._state.ChangeState(eActorState.Die);
                }
            }



            public static void SkillActiveUpdate(SkillKey _key, bool use = true)
            {
                if (use)
                {
                    playingskillList.Add(_key);
                }
                else
                {
                    playingskillList.Remove(_key);
                }
                SkillValueUpdate(_key);
            }

            static void SkillValueUpdate(SkillKey _key)
            {
                for (int i = 0; i < (int)SkillAbilityKey.End; i++)
                {
                    _activeSkillAbilitycaches[(SkillAbilityKey)i] = 0;
                }

                foreach (var skill in playingskillList)
                {
                    var skilldata = StaticData.Wrapper.skillDatas[(int)skill];

                    for(int i=0; i<skilldata.skillAbilityList_0.Length; i++)
                    {
                        if(skilldata.skillAbilityList_0[i]!=SkillAbilityKey.None)
                        {
                            _activeSkillAbilitycaches[skilldata.skillAbilityList_0[i]] += Skill.Get(skill).SkillValue(0, i);
                        }
                    }
                    for (int i = 0; i < skilldata.skillAbilityList_1.Length; i++)
                    {
                        if (skilldata.skillAbilityList_1[i] != SkillAbilityKey.None)
                        {
                            _activeSkillAbilitycaches[skilldata.skillAbilityList_1[i]] += Skill.Get(skill).SkillValue(1, i);
                        }
                    }
                    for (int i = 0; i < skilldata.skillAbilityList_2.Length; i++)
                    {
                        if (skilldata.skillAbilityList_2[i] != SkillAbilityKey.None)
                        {
                            _activeSkillAbilitycaches[skilldata.skillAbilityList_2[i]] += Skill.Get(skill).SkillValue(2, i);
                        }
                    }
                }
                StatusSync();
            }

            public static void BackToMainfromDungeon()
            {
                Hp = Player.Cloud.userStatusValue.hp;
                Shield = Player.Cloud.userStatusValue.shield;

                syncHpUI?.Invoke();
            }
            public static void ResetUnitWhenGoDungeon()
            {
                Hp = MaxHp;
                Shield = MaxShield;

                syncHpUI?.Invoke();
            }


            public static bool IsSkillActive(SkillKey _key)
            {
                return playingskillList.Contains(_key);
            }
        }
    }

}
