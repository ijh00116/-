using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Core;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewCanvasPetInventory : ViewCanvas
    {
        public ScrollRect scrollRect;
        public ViewEquipPetSlot[] equipedSlotList;

        [Header("detail view")]
        public BTButton EquipPetBtn;
        public BTButton enforcePetBtn;
        public Image PetImage;
        public TMP_Text petName;

        public TMP_Text petDesc_equipAbil_0;
        public TMP_Text petDesc_equipAbil_1;
        public TMP_Text petDesc_atkRate;
        public TMP_Text petDesc_moveRate;

        public Slider currentAmount;
        public TMP_Text currentAmountText;
        public TMP_Text petlevel;

        public BTButton[] closeBtn;

        Model.PetSpriteInfo spriteInfo;
        public AnimSpriteInfo currentAnimspriteInfo;
        [SerializeField] int waitFrame=20;

        int currentIndex = 0;
        int currentFrame = 0;

        bool isDetailSelected = false;

        public BTButton AllEnforceBtn;
        public BTButton DisAssembleBtn;

        public TMP_Text petSkillDesc;
        public GameObject petSkillUnlockObj;
        public BTButton petSkillUnlockOpenBtn;
        public GameObject petSkillUnlockOpenBtnIgnore;

        public GameObject petSkillUnlockWindow;
        public TMP_Text petSkillDescInUnlockWindow;
        public TMP_Text awakeStoneAmountInUnlockWindow;
        public BTButton[] closeSkillUnlockWindowBtns;
        public BTButton petSkillUnlockBtn;
        public GameObject ignoreUnlockBtnObj;

        [Header("pet disassemble")]
        public GameObject disAssembleWindow;
        public Image skillIconImageFrame_da;
        public Image skillIconImage_da;
        public TMP_Text skillDisassembleDesc;
        public TMP_Text obtainSkillawakeStoneAmount;
        public BTButton disassembleBtnInWindow;
        public BTButton[] closeDisassembleWindowBtn;

        [Header("guideQuest")]
        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;

        Vector2 upPos = new Vector2(0, 120);
        Vector2 downPos = new Vector2(0, 72);

        [Header("rune view")]
        public BTSelector TopSelector;
        public BTButton PetButton;
        public GameObject petReddot;
        public BTButton RuneButton;
        public GameObject runeReddot;
        public GameObject runeBtnLocked;

        public GameObject petWindow;
        public GameObject runeWindow;

        public ScrollRect scrollRect_rune;
        public ViewRuneEquipSlot[] equipedSlotList_rune;

        public ViewRuneSlot runeSlotPrefab;
       
        public BTButton EquipRuneBtn;
        public BTButton enforceRuneBtn;
        public Image RuneImage;
        public GameObject runeLockedObj;
        public Image RuneFrameImage;

        public TMP_Text runeDesc_equipAbil_0;
        public TMP_Text runeDesc_equipAbil_1;

        public Slider currentAmount_rune;
        public TMP_Text currentAmountText_rune;
        public TMP_Text runelevel;

        public BTButton AllEnforceBtn_rune;
        public BTButton DisAssembleBtn_rune;

        public BTButton expandAbilDescDownBtn;
        public BTButton expandAbilDescUpBtn;
        public RectTransform abilDescWindow;
        public TMP_Text abilDesc;

        [Header("rune disassemble")]
        public GameObject disAssembleWindow_rune;
        public Image IconImageFrame_rune;
        public Image IconImage_rune;
        public TMP_Text DisassembleDesc_rune;
        public TMP_Text obtainSkillawakeStoneAmount_rune;
        public BTButton disassembleBtnInWindow_rune;
        public BTButton[] closeDisassembleWindowBtn_rune;

        public BTButton presetBtn;
        public List<GameObject> PresetOnList;

        public TMP_Text petTxt;
        public TMP_Text petTxt_on;
        public TMP_Text runeTxt;
        public TMP_Text runeTxt_on;
        public TMP_Text equipeffectTxt;
        public TMP_Text abilityTxt;
        public TMP_Text petskillTxt;

        public TMP_Text equipTxt;
        public TMP_Text enforceTxt;
        public TMP_Text decomposeTxt;
        public TMP_Text awakeTxt;
        public TMP_Text allenforceTxt;
        public TMP_Text decomposeDescTxt;

        [Header("localize")]
        public TMP_Text petSkillUnlock_title;
        public TMP_Text petskillUnlock_BtnText;

        public TMP_Text petDa_Title;
        public TMP_Text petDa_BtnText;

        public TMP_Text runeEquipEffectBtn;
        public TMP_Text runeDetailDesc;
        public TMP_Text runeEquipBtn;
        public TMP_Text runeenforceBtn;
        public TMP_Text runeDaBtn;
        public TMP_Text runeAllEnforceBtn;

        public void Init()
        {
            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;
            arrowObject.gameObject.SetActive(false);

            petTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Pet].StringToLocal;
            petTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Pet].StringToLocal;
            runeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Rune].StringToLocal;
            runeTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Rune].StringToLocal;
            equipeffectTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EquipEffect].StringToLocal;
            abilityTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Ability].StringToLocal;
            petskillTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_PetSkill].StringToLocal;
            equipTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equip].StringToLocal;
            enforceTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enforce].StringToLocal;
            decomposeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Decomposition].StringToLocal;
            awakeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ItemAwake].StringToLocal;
            allenforceTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AllEnforce].StringToLocal;
            decomposeDescTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_SkillAwakeDecomposition].StringToLocal;
        }
        void StartQuestPoint(QuestGuideTypeConfigure questConfig)
        {
            if (IsQuestGuide(questConfig) == false)
            {
                arrowObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }

            arrowObject.gameObject.SetActive(false);

            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }
        void EndTutorial()
        {
            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);
            arrowObject.gameObject.SetActive(false);
        }

        bool IsQuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            bool isDialogTuto = false;
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.EquipEquip_1)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        enum moveType
        {
            up, down
        }
        moveType movetype;
        float currenttime;
        private IEnumerator RoutineMoveArrow()
        {
            movetype = moveType.up;
            currenttime = 0;
            while (true)
            {
                switch (movetype)
                {
                    case moveType.up:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(downPos, upPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                        break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 2;
                        arrowObject.anchoredPosition = Vector2.Lerp(upPos, downPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.up;
                            currenttime = 0;
                        }
                        break;
                    default:
                        break;
                }
                yield return null;
            }
        }

        public void SetPetDetailInfo(Model.Player.Pet.PetCacheData _petcache)
        {
            isDetailSelected = true;

            currentFrame = 0;
            currentIndex = 0;

            spriteInfo = PetResourcesBundle.Loaded.petImage[_petcache.tabledata.index];

            for(int i=0; i< spriteInfo.animInfo.Length; i++)
            {
                if(spriteInfo.animInfo[i].spriteType==UnitAnimSprtieType.Move)
                {
                    currentAnimspriteInfo = spriteInfo.animInfo[i];
                    break;
                }
            }
            PetImage.gameObject.SetActive(true);
            PetImage.sprite = currentAnimspriteInfo.animInfo.spriteList[currentIndex];

            if (_petcache.IsUnlocked)
                PetImage.color = Color.white;
            else
            {
                PetImage.color = Color.black;
            }

            double equipabilValue_0 = _petcache._equipabilitycaches[_petcache.tabledata.equipabilitylist[0]];
            petDesc_equipAbil_0.text = 
                string.Format( StaticData.Wrapper.localizeddesclist[(int)_petcache.tabledata.equipabilitylmk[0]].StringToLocal, equipabilValue_0.ToNumberString());

            petDesc_equipAbil_1.gameObject.SetActive(false);
            if (_petcache.tabledata.equipabilitylist.Length>=2)
            {
                petDesc_equipAbil_1.gameObject.SetActive(true);
                double equipabilValue_1 = _petcache._equipabilitycaches[_petcache.tabledata.equipabilitylist[1]];
                petDesc_equipAbil_1.text =
                    string.Format(StaticData.Wrapper.localizeddesclist[(int)_petcache.tabledata.equipabilitylmk[1]].StringToLocal, equipabilValue_1.ToNumberString());
            }

            if(_petcache.tabledata.isMelee)
            {
                string localize = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_SwordAbilForPet].StringToLocal;
                petDesc_atkRate.text = string.Format(localize, _petcache.atkRate);
            }
            else
            {
                string localize = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_SwordAbilForPet].StringToLocal;
                petDesc_atkRate.text = string.Format(localize, _petcache.atkRate);
            }
            string localizemove = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_MoveSpeedForPet].StringToLocal;
            petDesc_moveRate.text = string.Format(localizemove, _petcache.tabledata.moveSpeed*100.0f);


            petlevel.text = _petcache.userPetdata.level.ToString();

            if (_petcache.IsMaxLevel())
            {
                currentAmountText.text = string.Format("{0}", _petcache.userPetdata.Obtaincount);
                currentAmount.value = 1;
                DisAssembleBtn.gameObject.SetActive(true);
                enforcePetBtn.gameObject.SetActive(false);
            }
            else
            {
                currentAmountText.text = string.Format("{0}/{1}", _petcache.userPetdata.Obtaincount, StaticData.Wrapper.petAmountTableData[_petcache.userPetdata.level].amountForLvUp);
                var floatvalue = Mathf.Clamp((float)(_petcache.userPetdata.Obtaincount / (float)(StaticData.Wrapper.petAmountTableData[_petcache.userPetdata.level].amountForLvUp)), 0.0f, 1.0f);
                currentAmount.value = floatvalue;

                DisAssembleBtn.gameObject.SetActive(false);
                enforcePetBtn.gameObject.SetActive(true);
            }

            if(_petcache.tabledata.petskillKey!=PetSkillKey.None)
            {
                string coolTime = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Cooltime].StringToLocal, _petcache.tabledata.skillCoolTime);
                string effectTime = string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EffectTime].StringToLocal, _petcache.tabledata.skillEffectTime);
                string skilldesc;
                if (_petcache.tabledata.skillEffectTime>0)
                {
                    skilldesc = StaticData.Wrapper.localizeddesclist[(int)_petcache.tabledata.petSkillDesc].StringToLocal+ effectTime+coolTime;
                }
                else
                {
                    skilldesc = StaticData.Wrapper.localizeddesclist[(int)_petcache.tabledata.petSkillDesc].StringToLocal + coolTime;
                }
                petSkillDesc.text =string.Format(skilldesc, _petcache.GetSkillPureValue());
                petSkillUnlockObj.SetActive(!_petcache.IsSkillUnlocked);
            }
            else
            {
                string localize = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_SkillNot].StringToLocal;
                petSkillDesc.text = localize;
                petSkillUnlockObj.SetActive(true);
            }

            if(_petcache.tabledata.petskillKey==PetSkillKey.None ||_petcache.IsUnlocked==false)
            {
                petSkillUnlockOpenBtn.enabled = false;
                petSkillUnlockOpenBtnIgnore.SetActive(true);
            }
            else
            {
                if (_petcache.userPetdata.unlockSkillLevel > 0)
                {
                    petSkillUnlockOpenBtn.enabled = false;
                    petSkillUnlockOpenBtnIgnore.SetActive(true);
                }
                else
                {
                    petSkillUnlockOpenBtn.enabled = true;
                    petSkillUnlockOpenBtnIgnore.SetActive(false);
                }
            }
         
        }

        
        public void SetRuneDetailInfo(Model.Player.Rune.RuneCacheData _runecache)
        {
            isDetailSelected = true;

            RuneImage.gameObject.SetActive(true);
            RuneFrameImage.sprite= InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[_runecache.tabledata.grade - 1];
            RuneImage.sprite = PetResourcesBundle.Loaded.runeImage[_runecache.tabledata.index];
            runeLockedObj.SetActive(!_runecache.IsUnlocked);

            double equipabilValue_0 = _runecache._equipabilitycaches[_runecache.tabledata.equipabilitylist[0]];

            string desc = RuneDesc(_runecache);

            runeDesc_equipAbil_0.text = desc;



            petlevel.text = _runecache.userRunedata.Obtainlv.ToString();

            runelevel.text = _runecache.userRunedata.Obtainlv.ToString();

            if (_runecache.IsMaxLevel())
            {
                currentAmountText_rune.text = string.Format("{0}", _runecache.userRunedata.Obtaincount);
                currentAmount_rune.value = 1;
                DisAssembleBtn_rune.gameObject.SetActive(true);
                enforceRuneBtn.gameObject.SetActive(false);
            }
            else
            {
                currentAmountText_rune.text = string.Format("{0}/{1}", _runecache.userRunedata.Obtaincount, StaticData.Wrapper.runeAmountTableData[_runecache.userRunedata.Obtainlv].amountForLvUp);
                var floatvalue = Mathf.Clamp((float)(_runecache.userRunedata.Obtaincount / (float)(StaticData.Wrapper.runeAmountTableData[_runecache.userRunedata.Obtainlv].amountForLvUp)), 0.0f, 1.0f);
                currentAmount_rune.value = floatvalue;

                DisAssembleBtn_rune.gameObject.SetActive(false);
                enforceRuneBtn.gameObject.SetActive(true);
            }
        }

        public string RuneDesc(Player.Rune.RuneCacheData _runecache)
        {
            double equipabilValue_0 = _runecache._equipabilitycaches[_runecache.tabledata.equipabilitylist[0]];
            string desc = string.Format(StaticData.Wrapper.localizeddesclist[(int)_runecache.tabledata.equipabilitylmk[0]].StringToLocal, equipabilValue_0);
         
            double calculValue = -1;
            RuneAbilityKey abiltype = _runecache.tabledata.equipabilitylist[0];
            var equipRune = _runecache;
            switch (abiltype)
            {
                case RuneAbilityKey.SwordAttackIncrease:
                case RuneAbilityKey.MagicAttackIncrease:
                case RuneAbilityKey.SkillAttackIncrease:
                case RuneAbilityKey.PetAttackIncrease:
                case RuneAbilityKey.HpIncrease:
                case RuneAbilityKey.ShieldIncrease:
                case RuneAbilityKey.ShieldRecoverIncrease:
                    break;
                case RuneAbilityKey.MATKIncreaseWhenUseGuideMissileSkill:
                    break;
                case RuneAbilityKey.SATKIncreaseWhenUseGodmodeSkill:
                    break;
                case RuneAbilityKey.SATKIncreaseWhenUseSummonSpawnSkill:
                    calculValue = Player.Rune.companioncount* equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.MATKIncreaseWhenUseLaserSkill:
                    break;
                case RuneAbilityKey.ThirdgradeSwordTotalLevelSwordtkIncrease:
                    calculValue = Player.EquipItem.weaponTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.ThirdgradeStaffTotalLevelMagictkIncrease:
                    calculValue = Player.EquipItem.staffTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.ThirdgradeArmorTotalLevelPettkIncrease:
                    calculValue = Player.EquipItem.armorTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.ThirdgradePetTotalLevelSkillAtkIncrease:
                    calculValue = Player.Pet.petTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FourthgradeSwordTotalLevelSwordtkIncrease:
                    calculValue = Player.EquipItem.weaponTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FourthgradeStaffTotalLevelMagictkIncrease:
                    calculValue = Player.EquipItem.staffTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FourthgradeArmorTotalLevelPettkIncrease:
                    calculValue = Player.EquipItem.armorTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FourthgradePetTotalLevelSkillAtkIncrease:
                    calculValue = Player.Pet.petTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FifthgradeSwordTotalLevelSwordtkIncrease:
                    calculValue = Player.EquipItem.weaponTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FifthgradeStaffTotalLevelMagictkIncrease:
                    calculValue = Player.EquipItem.staffTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FifthgradeArmorTotalLevelPettkIncrease:
                    calculValue = Player.EquipItem.armorTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FifthgradePetTotalLevelSkillAtkIncrease:
                    calculValue = Player.Pet.petTotalLevel[5] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.ThirdgradeSkillTotalLevelSwordtkIncrease:
                    calculValue = Player.Skill.skillTotalLevel[3] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.FourthgradeSkillTotalLevelMagictkIncrease:
                    calculValue = Player.Skill.skillTotalLevel[4] * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.OneToTwocountForSkillAwake_AtkIncrease:
                    calculValue = (Player.Skill.skillAwakeTotalLevel[1] + Player.Skill.skillAwakeTotalLevel[2]) * equipRune._equipabilitycaches[abiltype];
                    break;
                case RuneAbilityKey.END:
                    break;
                default:
                    break;
            }

            if(abiltype>= RuneAbilityKey.ThirdgradeSwordTotalLevelSwordtkIncrease ||abiltype== RuneAbilityKey.SATKIncreaseWhenUseSummonSpawnSkill)
            {
                if (calculValue > 0)
                {
                    string localize = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_TotalAbil].StringToLocal;
                    string addDesc = string.Format(localize, calculValue);
                    desc = desc + addDesc;
                }
                else
                {
                    string localize = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_TotalAbil].StringToLocal;
                    string addDesc = string.Format(localize,0);
                    desc = desc + addDesc;
                }

            }

            return desc;
        }


        private void Update()
        {
            if (!isDetailSelected)
                return;
            currentFrame++;
            if(currentFrame>=waitFrame)
            {
                currentFrame = 0;
                currentIndex++;
                if(currentIndex>= currentAnimspriteInfo.animInfo.spriteList.Length)
                {
                    currentIndex = 0;
                }
                PetImage.sprite = currentAnimspriteInfo.animInfo.spriteList[currentIndex];
       
             
            }
        }

    }
}
