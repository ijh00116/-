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
    public class ViewCanvasSkillInventory : ViewCanvas
    {
        public ScrollRect scrollRect;
        public ViewSkillEquipslot[] equipedSlotList;

        [Header("detail view")]
        public BTButton equipSkillBtn;
        public BTButton enforceSkillBtn;
        public BTButton totalSkillBtn;
        public GameObject equipInActive;
        public GameObject enforceInActive;
        public BTButton awakeSkillBtn;
        public GameObject awakeInActive;

        public BTButton disassembleBtn;
        public GameObject disassembleInActive;
        //public RawImage skilldetailImage;
        public TMP_Text skillName;

        //current select skillslot
        public Image slotBg;
        public Image skillImage;
        public TMP_Text skillLv;
        public Slider skillAmount;
        public TMP_Text skillAmountText;

        public AbilityDescSlot abilitydescSlot;
        private List<AbilityDescSlot> abilslotList = new List<AbilityDescSlot>();
        public Transform descParent;
        [Header("LOCK")]
        public Image skilllockImage;
        public GameObject skillAwakeLockObj;
        public BTButton[] closeBtn;

        [Header("Money")]
        [SerializeField] public ObtainGood obtainSkillAwakeGood;

        [Header("AwakeDetail")]
        public GameObject AwakeDetailWindow;
        public Image skillIcon;
        public Image skillIconBG;
        public TMP_Text awakeDesc;
        public BTButton awakeConfirmBtninDetail;
        public GameObject cantawakeBtnImageinDetail;
        public TMP_Text awakeStoneCost;
        public BTButton[] closeAwakeDetailWindowBtn;


        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;
        Vector2 normalArrowPos = new Vector2(0, 74);
        Vector2 upPos = new Vector2(0, 64);
        Vector2 downPos = new Vector2(0, 84);

        Vector2 arrowObjectPos = new Vector2(0, 74);

        [Header("skill disassemble")]
        public GameObject disAssembleWindow;
        public Image skillIconImage_da;
        public TMP_Text skillDisassembleDesc;
        public TMP_Text obtainSkillawakeStoneAmount;
        public BTButton disassembleBtnInWindow;
        public BTButton[] closeDisassembleWindowBtn;

        [Header("skill preset")]
        public BTButton presetBtn;
        public List<GameObject> PresetOnList;

        public BTButton goodsBtn;

        public TMP_Text equipTxt;
        public TMP_Text enforceTxt;
        public TMP_Text decomposeTxt;
        public TMP_Text awakeTxt;
        public TMP_Text allenforceTxt;
        public TMP_Text decomposeDescTxt;

        public TMP_Text skillawakeTitle;
        public TMP_Text skillawakeBtnTitle;

        public TMP_Text skilldisassembleTitle;
        public TMP_Text skilldisassembleBtnTitle;
        public void SetSkillDetailInfo(Model.Player.Skill.SkillCacheData _skillcache)
        {
            skillName.text =string.Format("{0}<color=white>(max.{1})</color>",
                StaticData.Wrapper.localizednamelist[(int)_skillcache.tabledataSkill.SkillName].StringToLocal, _skillcache.tabledataSkill.maxLevel);

            for(int i=0; i<abilslotList.Count;i++)
            {
                abilslotList[i].gameObject.SetActive(false);
            }
            for(int i=0; i< _skillcache.tabledataSkill.SkillDesc.Length; i++)
            {
                AbilityDescSlot slot;
                if(i>= abilslotList.Count)
                {
                    slot = Instantiate(abilitydescSlot);
                    slot.transform.SetParent(descParent, false);
                    abilslotList.Add(slot);
                }
                else
                {
                    slot = abilslotList[i];
                }
                int index = i;
                slot.gameObject.SetActive(true);
                slot.SetSkillAbilityDesc(_skillcache, index);
            }

            slotBg.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[(int)_skillcache.tabledataSkill.grade - 1];
            skillImage.sprite = InGameResourcesBundle.Loaded.skillIcon[(int)_skillcache.tabledataSkill.skillKey];
            skillLv.text = _skillcache.userSkilldata.level.ToString();
            skillAmount.value = (float)_skillcache.userSkilldata.Obtaincount / (float)(_skillcache.GetAmountForLevelUp());
            skillAmountText.text = string.Format("{0}/{1}", _skillcache.userSkilldata.Obtaincount, _skillcache.GetAmountForLevelUp());

            skilllockImage.gameObject.SetActive(!_skillcache.userSkilldata.Unlock);
        }

        public void Init()
        {
            Player.Guide.tutorialConfigAction += StartPoint;
            Player.Guide.TutorialEndcallback += EndTutorial;

            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;

            equipTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equip].StringToLocal; 
            enforceTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enforce].StringToLocal;
            decomposeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Decomposition].StringToLocal;
            awakeTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ItemAwake].StringToLocal;
            allenforceTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AllEnforce].StringToLocal;
            decomposeDescTxt.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_SkillAwakeDecomposition].StringToLocal;

            arrowObject.gameObject.SetActive(false);
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

            arrowObject.transform.SetParent(equipSkillBtn.transform, false);
            arrowObject.anchoredPosition = normalArrowPos;

            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }

        bool IsQuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            bool isDialogTuto = false;
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.EquipSkill_1)
            {
                isDialogTuto = true;
            }
            return isDialogTuto;
        }

        void StartPoint(Model.TutorialDescData tutoData)
        {
            if (tutoData.descKey != LocalizeDescKeys.None)
            {
                arrowObject.gameObject.SetActive(false);
                return;
            }
            if (IsPointTutorial(tutoData) == false)
            {
                arrowObject.gameObject.SetActive(false);
                return;
            }

            SetSkillToThreeAtkSkill();
            SetArrowObjectPos(tutoData);
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

        bool IsPointTutorial(Model.TutorialDescData tutoData)
        {
            bool isPointTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.SkillAwakePoint_2)
            {
                isPointTuto = true;
            }

            return isPointTuto;
        }

        private void SetArrowObjectPos(Model.TutorialDescData tutoData)
        {
            if (tutoData.tutoConfig == TutorialTypeConfigure.SkillAwakePoint_2)
            {
                arrowObject.transform.SetParent(awakeSkillBtn.transform, false);
                arrowObject.anchoredPosition = arrowObjectPos;
            }
        }

        public void SetSkillToThreeAtkSkill()
        {
            Player.Skill.SlotTouched?.Invoke(SkillKey.GuidedMissile);
        }

        enum moveType
        {
            up, down
        }
        moveType movetype;
        float currenttime;
        private IEnumerator RoutineMoveArrow()
        {
            arrowObject.gameObject.SetActive(true);
            movetype = moveType.up;
            currenttime = 0;
            while (true)
            {
                if(Player.Skill.currentTouchedSkill.tabledataSkill.skillKey==SkillKey.GuidedMissile)
                {
                    if (arrowObject.gameObject.activeInHierarchy==false)
                    {
                        arrowObject.gameObject.SetActive(true);
                    }
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
                }
                else
                {
                    if (arrowObject.gameObject.activeInHierarchy)
                    {
                        arrowObject.gameObject.SetActive(false);
                    }
                }
                
                yield return null;
            }
        }
    }
}
