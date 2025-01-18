using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;
using System.Collections;

namespace BlackTree.Bundles
{
    public class ViewCanvasEquipInventory : ViewCanvas
    {
        public BTSelector buttonSelector;

        public BTButton openSwordwindowBtn;
        public BTButton openBowwindowBtn;
        public BTButton openArmorwindowBtn;

        public GameObject swordReddot;
        public GameObject staffReddot;
        public GameObject armorReddot;

        public ScrollRect weaponscrollRect;
        public ScrollRect bowscrollRect;
        public ScrollRect armorcrollRect;

        [Header("디테일")]
        //합성
        public Image currentObjectIcon;

        //아이템 세부 설명
       
        public TMP_Text currentItemlevel;
        public TMP_Text currentItemAmount;
        public Slider currentItemAmountSlider;

        public BTButton CombineBtn;
        public GameObject combineIgnoreBtn;
        public BTButton EnforceBtn;
        public GameObject enforceIgnoreBtn;
        public BTButton AwakeBtn;
        public GameObject awakeIgnoreBtn;
        public BTButton EquipBtn;
        public GameObject equipIgnoreBtn;
        public BTButton BestEquipBtn;

        public TMP_Text combineDesc;

        public BTButton allCombineBtn;

        public BTButton[] closeBtn;

        public AbilityDescSlot abilitySlot;
        private System.Collections.Generic.List<AbilityDescSlot> equipAbilslotList = new System.Collections.Generic.List<AbilityDescSlot>();
        private System.Collections.Generic.List<AbilityDescSlot> possessAbilslotList = new System.Collections.Generic.List<AbilityDescSlot>();
        public Transform equipDescParent;
        public Transform possesDescParent;


        [Header("guideQuest")]
        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;

        Vector2 upPos = new Vector2(0, 105);
        Vector2 downPos = new Vector2(0, 72);

        [Header("localizing")]
        public TMP_Text swordTxt;
        public TMP_Text swordTxt_on;
        public TMP_Text staffTxt;
        public TMP_Text staffTxt_on;
        public TMP_Text armorTxt;
        public TMP_Text armorTxt_on;
        public TMP_Text equipEffectTxt;
        public TMP_Text possessEffectTxt;
        public TMP_Text equipTxt;
        public TMP_Text equipignoreTxt;
        public TMP_Text enforceTxt;
        public TMP_Text enforceignoreTxt;
        public TMP_Text combineTxt;
        public TMP_Text combineIgnoreTxt;
        public TMP_Text allEnforceTxt;
        public TMP_Text recommendEquipTxt;
        public TMP_Text awakeBtnTxt;
        public TMP_Text awakeIgnoreTxt;

        public void Init()
        {
            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;

            swordTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Sword].StringToLocal;
            staffTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Staff].StringToLocal;
            armorTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Armor].StringToLocal;
            swordTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Sword].StringToLocal;
            staffTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Staff].StringToLocal;
            armorTxt_on.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Armor].StringToLocal;
            equipEffectTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_EquipEffect].StringToLocal;
            possessEffectTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_PossessEffect].StringToLocal;
            equipTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equip].StringToLocal;
            enforceTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enforce].StringToLocal;
            combineTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Combine].StringToLocal;
            allEnforceTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AllEnforce].StringToLocal;
            recommendEquipTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RecommendEquip].StringToLocal;
            awakeBtnTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ItemAwake].StringToLocal;

            equipignoreTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equip].StringToLocal;
            enforceignoreTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Enforce].StringToLocal;
            combineIgnoreTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Combine].StringToLocal;
            awakeIgnoreTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ItemAwake].StringToLocal;
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


        public void ActiveOffScrollRect()
        {
            weaponscrollRect.gameObject.SetActive(false);
            bowscrollRect.gameObject.SetActive(false);
            armorcrollRect.gameObject.SetActive(false);
        }

        Color black = Color.black;
        public void DetailInfoUpdate(Model.Player.EquipData equipCache)
        {
            Model.Player.Equip.currentSelectIdx = equipCache.tabledata.index;

            switch (equipCache.equipType)
            {
                case Definition.EquipType.Weapon:
                    currentObjectIcon.sprite = InGameResourcesBundle.Loaded.weaponIcon[equipCache.tabledata.index];
                    break;
                case Definition.EquipType.Armor:
                    currentObjectIcon.sprite = InGameResourcesBundle.Loaded.armorUIIcon[equipCache.tabledata.index];
                    break;
                case Definition.EquipType.Bow:
                    currentObjectIcon.sprite = InGameResourcesBundle.Loaded.bowIcon[equipCache.tabledata.index];
                    break;
                default:
                    break;
            }
            currentObjectIcon.color = equipCache.IsUnlocked ? Color.white : Color.black;
            
            DetailAbilInfoUpdate(equipCache);

            Player.EquipItem.EquipedSlotTouched?.Invoke(equipCache.equipType, equipCache.tabledata.index);
        }

        public void DetailAbilInfoUpdate(Model.Player.EquipData equipCache)
        {
            for(int i=0;i< equipAbilslotList.Count;i++)
            {
                equipAbilslotList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < possessAbilslotList.Count; i++)
            {
                possessAbilslotList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < equipCache.tabledata.equipabilitylmk.Length; i++)
            {
                AbilityDescSlot tempslot;
                int index = i;
                if(index >= equipAbilslotList.Count)
                {
                    tempslot = Instantiate(abilitySlot);
                    tempslot.transform.SetParent(equipDescParent, false);
                    equipAbilslotList.Add(tempslot);
                }
                else
                {
                    tempslot = equipAbilslotList[index];
                }
                tempslot.gameObject.SetActive(true);
                tempslot.SetAbilityDesc(equipCache, index,true);
            }

            for (int i = 0; i < equipCache.tabledata.possessabilitylmk.Length; i++)
            {
                AbilityDescSlot tempslot;
                int index = i;
                if (index >= possessAbilslotList.Count)
                {
                    tempslot = Instantiate(abilitySlot);
                    tempslot.transform.SetParent(possesDescParent, false);
                    possessAbilslotList.Add(tempslot);
                }
                else
                {
                    tempslot = possessAbilslotList[index];
                }
                tempslot.gameObject.SetActive(true);
                tempslot.SetAbilityDesc(equipCache, index,false);
            }
            currentItemlevel.text=string.Format("{0}", equipCache.userEquipdata.Obtainlv);

            if (equipCache.IsMaxLevel())
            {
                currentItemAmount.text = string.Format("{0}/{1}", equipCache.userEquipdata.Obtaincount, Player.Equip.CountForCombine);
                var floatvalue = Mathf.Clamp((float)(equipCache.userEquipdata.Obtaincount / (float)(Player.Equip.CountForCombine)), 0.0f, 1.0f);
                currentItemAmountSlider.value = floatvalue;
            }
            else
            {
                currentItemAmount.text = string.Format("{0}/{1}", equipCache.userEquipdata.Obtaincount, StaticData.Wrapper.itemAmountTableData[equipCache.userEquipdata.Obtainlv].amountForLvUp);
                var floatvalue = Mathf.Clamp((float)(equipCache.userEquipdata.Obtaincount / (float)(StaticData.Wrapper.itemAmountTableData[equipCache.userEquipdata.Obtainlv].amountForLvUp)), 0.0f, 1.0f);
                currentItemAmountSlider.value = floatvalue;
            }
           



            if(equipCache.IsUnlocked)
            {
                EquipBtn.enabled = !equipCache.IsEquiped;
                equipIgnoreBtn.SetActive(equipCache.IsEquiped);
            }
            else
            {
                EquipBtn.enabled = false;
                equipIgnoreBtn.SetActive(true);
            }

            var itemState = equipCache.CurrentItemState();

            switch (itemState)
            {
                case Definition.ItemState.normal:
                    EnforceBtn.gameObject.SetActive(true);
                    AwakeBtn.gameObject.SetActive(false);

                    combineIgnoreBtn.SetActive(true);
                    enforceIgnoreBtn.SetActive(false);
                    break;
                case Definition.ItemState.canCombine:
                    EnforceBtn.gameObject.SetActive(true);
                    AwakeBtn.gameObject.SetActive(false);

                    enforceIgnoreBtn.SetActive(true);
                    combineIgnoreBtn.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
