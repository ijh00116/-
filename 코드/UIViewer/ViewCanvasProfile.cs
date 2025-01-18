using BlackTree.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlackTree.Model;
using BlackTree.Definition;
using System.Collections;

namespace BlackTree.Bundles
{
    public class ViewCanvasProfile : ViewCanvas
    {
        public BTButton[] closeButton;

        public Image unitImage;
        public TMP_Text level;
        public TMP_Text exp;
        public TMP_Text swordAtk;
        public TMP_Text bowAtk;
        public TMP_Text stage;

        public ViewEquipSlot[] equiplist;

        public ViewSkillSlot[] skillslotList;

        public BTButton openNicknameChangeBtn;
        public GameObject nicknameChangeWindow;
        public TMP_InputField inputfield;

        public BTButton[] closeNicknamewindowBtn;

        public BTButton confirmNicknamewindowBtn;
        public GameObject cantNicknameChangeObj;

        public TMP_Text leftTime;

        public TMP_Text uiNickname;


        [Header("detail profile")]
        public BTButton openDetailWindowBtn;
        public GameObject detailAbilityWindow;
        public BTButton[] closeDetailWindowBtn;
        public ScrollRect descContent;

        public Transform[] AbilityDescScrollArray;
       

        public ViewAbilityDesc abilSlotPrefab;

        [Header("arrow")]
        public RectTransform arrowTotalObject;
        public RectTransform arrowObject;
        private Coroutine _guidedArrowCo = null;
        Vector2 upPos = new Vector2(0, 50);
        Vector2 downPos = new Vector2(0, 0);

        public TMP_Text titleTxt;
        public TMP_Text detailTxt;

        public TMP_Text equipTxt;
        public TMP_Text skillTxt;


        public TMP_Text detailabilTxt;
        public TMP_Text totalAbil;
        public TMP_Text goldup_1;
        public TMP_Text goldup_2;
        public TMP_Text statup;
        public TMP_Text awakeUp;
        public TMP_Text equipAbil;
        public TMP_Text petAbil;
        public TMP_Text advanceAbil;
        public TMP_Text rpUp_1;
        public TMP_Text rpUp_2;
        public TMP_Text etcAbil;

        [Header("local")]
        public TMP_Text levelTitle;
        public TMP_Text expTitle;
        public TMP_Text swordAtkTitle;
        public TMP_Text magicAtkTitle;
        public TMP_Text stageTitle;

        public TMP_Text nickChangeTitle;
        public TMP_Text nickChangeBtnText;
        private void Awake()
        {
            Player.Guide.questGuideAction += StartQuestPoint;
            Player.Guide.QuestGuideEndcallback += EndTutorial;

            titleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_profile].StringToLocal;
            detailTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_profileDetail].StringToLocal;
            equipTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equip].StringToLocal;
            skillTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Skill].StringToLocal;

            detailabilTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Ability].StringToLocal;
            totalAbil.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_TotalAbil].StringToLocal;
            goldup_1.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoldUpgrade_1].StringToLocal;
            goldup_2.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GOldUpgrade_2].StringToLocal;
            statup.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_StatUp].StringToLocal;
            awakeUp.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AwakeUp].StringToLocal;
            equipAbil.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Equipment].StringToLocal;
            petAbil.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Pet].StringToLocal;
            advanceAbil.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Advance].StringToLocal;
            rpUp_1.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ResearchUp_1].StringToLocal;
            rpUp_2.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_ResearchUp_2].StringToLocal;
            etcAbil.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Etc].StringToLocal;

            levelTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Level].StringToLocal;
            expTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Exp].StringToLocal;
            swordAtkTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_SwordAtk].StringToLocal;
            magicAtkTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_WitchAtk].StringToLocal;
            stageTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Stage].StringToLocal;

            nickChangeTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_NickNameChange].StringToLocal;
            nickChangeBtnText.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Change].StringToLocal;
        }

        void StartQuestPoint(QuestGuideTypeConfigure questConfig)
        {
            if (IsQuestGuide(questConfig) == false)
            {
                arrowTotalObject.gameObject.SetActive(false);
                if (_guidedArrowCo != null)
                    StopCoroutine(_guidedArrowCo);
                return;
            }

            arrowTotalObject.gameObject.SetActive(true);

            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);

            _guidedArrowCo = StartCoroutine(RoutineMoveArrow());
        }

        void EndTutorial()
        {
            if (_guidedArrowCo != null)
                StopCoroutine(_guidedArrowCo);
            arrowTotalObject.gameObject.SetActive(false);
        }
       
        bool IsQuestGuide(QuestGuideTypeConfigure questGuideTypeConfigure)
        {
            bool isDialogTuto = false;
            if (questGuideTypeConfigure == QuestGuideTypeConfigure.OpenProfileDetail_1)
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
                        currenttime += Time.deltaTime * 1.5f;
                        arrowObject.anchoredPosition = Vector2.Lerp(downPos, upPos, currenttime);
                        if (currenttime >= 1)
                        {
                            movetype = moveType.down;
                            currenttime = 0;
                        }
                        break;
                    case moveType.down:
                        currenttime += Time.deltaTime * 1.5f;
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

    }
}
