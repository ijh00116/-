using BlackTree.Core;
using BlackTree.Definition;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class ViewCanvasGuideDialogue : ViewCanvas
    {
        [Header("NPC")]
        public GameObject NPCObject;
        public TMP_Text _dialogText;
        //public Febucci.UI.TextAnimator_TMP textAnimator;
        public Febucci.UI.TypewriterByCharacter textTypeWriter;
        public BTButton NextDialog;

        private Coroutine _routineNpcDialog = null;

        public Image SpeakerImage;
        public Sprite[] speakerImageList;
        int spriteIndex;
        float currentTime = 0;
        [SerializeField]float indexChangeTime;

        private void Awake()
        {
            Player.Guide.TutorialEndcallback += EndTutorial;
            Player.Guide.tutorialConfigAction += StartDialog;
            NextDialog.onClick.AddListener(GoNextDialog);

            textTypeWriter.onMessage.AddListener(TextTypeWritingEnd);
        }

        private void Update()
        {
            if (IsVisible == false)
                return;

            currentTime += Time.deltaTime;
            if(currentTime>=indexChangeTime)
            {
                currentTime = 0;
                SpeakerImage.sprite = speakerImageList[spriteIndex];
                spriteIndex++;
                if(spriteIndex>=speakerImageList.Length)
                {
                    spriteIndex = 0;
                }
            }
        }

        void TextTypeWritingEnd(Febucci.UI.Core.Parsing.EventMarker eventMarker)
        {
            switch (eventMarker.name)
            {
                case "endMsg":
                    //Debug.Log("¥Î»≠ ≥°!");
                    isTextUpdating = false;
                    break;
                default:
                    break;
            }
            
        }
        void EndTutorial()
        {
            SetVisible(false);
        }

        void GoNextDialog()
        {
            if (isTextUpdating)
            {
                //if (_routineNpcDialog != null)
                //    StopCoroutine(_routineNpcDialog);
                //SetDialogText(currentDialog);
                textTypeWriter.SkipTypewriter();
                isTextUpdating = false;
                return;
            }
            var dialogExist = Player.Guide.StartTutorial(Player.Guide.currentTutorial);
            if(dialogExist!=null)
            {
                if(dialogExist.descKey==LocalizeDescKeys.None)
                {
                    SetVisible(false);
                }
            }
            else
            {
                SetVisible(false);
               
            }
        }

        void StartDialog(Model.TutorialDescData tutoData)
        {
            if (tutoData.descKey == LocalizeDescKeys.None)
            {
                SetVisible(false);
                return;
            }
            if(IsDialogTutorial(tutoData)==false)
            {
                SetVisible(false);
                return;
            }
            else
            {
                //Time.timeScale = 0;
            }

            SetVisible(true);
            NPCObject.SetActive(true);

            //if (_routineNpcDialog != null)
            //    StopCoroutine(_routineNpcDialog);

            //_routineNpcDialog = StartCoroutine(RoutineNPCDialog(Player.Guide.GetDialog(tutoData)));

            var dialogs = Player.Guide.GetDialog(tutoData);
            currentDialog = StaticData.Wrapper.localizeddesclist[(int)dialogs].StringToLocal;
            textTypeWriter.ShowText(currentDialog+ "<?endMsg>");
            isTextUpdating = true;
        }

        

        bool IsDialogTutorial(Model.TutorialDescData tutoData)
        {
            bool isDialogTuto = false;
            if (tutoData.tutoConfig == TutorialTypeConfigure.FirstInGameDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.FirstInGameDialog_1 ||
                tutoData.tutoConfig == TutorialTypeConfigure.FirstInGameDialog_2||
                tutoData.tutoConfig == TutorialTypeConfigure.RPDungeonClearDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.ExpDungeonClearDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.GoldUpgradeDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.StatusUpgradeDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.AwakeDungeonClearDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.SummonSkillDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.SummonEquipDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.CharacterAwakeDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.AdBuffDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.AwakeUpgradeDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.SkillAwakeDalog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.RPContentClearDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.RaidStartDialog_0 ||
                tutoData.tutoConfig == TutorialTypeConfigure.SummonPetDialog_0||
                tutoData.tutoConfig == TutorialTypeConfigure.CharacterAwakeDialog_1||
                tutoData.tutoConfig == TutorialTypeConfigure.SkillAwakeDalog_1)
            {
                isDialogTuto = true;
            }

            return isDialogTuto;
        }

        WaitForSecondsRealtime dialogFrame = new WaitForSecondsRealtime(0.03f);

        bool isTextUpdating = false;
        string currentDialog = null;
        private IEnumerator RoutineNPCDialog(LocalizeDescKeys dialogs)
        {
            isTextUpdating = true;
            currentDialog = StaticData.Wrapper.localizeddesclist[(int)dialogs].StringToLocal;
            StringBuilder sb = new StringBuilder();
            foreach (var ch in StaticData.Wrapper.localizeddesclist[(int)dialogs].StringToLocal)
            {
                sb.Append(ch);
                //if(ch.Equals("<"))
                //{

                //}
                SetDialogText(sb.ToString());
                yield return dialogFrame;
            }

            isTextUpdating = false;

            yield break;
        }

        public void SetDialogText(string text)
        {
            _dialogText.text = text;
        }
    }

}
