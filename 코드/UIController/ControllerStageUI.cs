using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerStageUI
    {
        public ViewCanvasStage viewcanvasStage;
        CancellationTokenSource _cts;

        int currentChapter;
        int currentStage;

        int phaseIndex;
        public ControllerStageUI(Transform parent, CancellationTokenSource cts)
        {
            viewcanvasStage = ViewCanvas.Create<ViewCanvasStage>(parent);
            _cts = cts;

            for(int i=0; i< viewcanvasStage.closeBtn.Length; i++)
            {
                int index = i;
                viewcanvasStage.closeBtn[index].onClick.AddListener(CloseStageWindow);
            }

            int phaseMaxIndex = (int)(StaticData.Wrapper.chapterRewardTableDatas.Length / 100);
            viewcanvasStage.phaseSelector.panels = new GameObject[phaseMaxIndex];
            viewcanvasStage.phaseSelector.inactivepanels = new GameObject[phaseMaxIndex];
            for (int i = 0; i < phaseMaxIndex; i++)
            {
                int index = i;
                var obj = Object.Instantiate(viewcanvasStage.phasePrefab);
                obj.transform.SetParent(viewcanvasStage.phaseScroll.content, false);

                string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Phase].StringToLocal;
                obj.phaseName_0.text = string.Format(localized, index + 1);
                obj.phaseName_1.text = string.Format(localized, index + 1);

                viewcanvasStage.phaseSelector.panels[index] = obj.onObj;
                viewcanvasStage.phaseSelector.inactivepanels[index] = obj.offObj;

                obj.seePhaseBtn.onClick.AddListener(() => {
                    viewcanvasStage.stageScrollview.SetPhaseChapter(index);
                    viewcanvasStage.phaseSelector.Show(index);
                    viewcanvasStage.scroll.SetContentScrollOffsetToTop();
                    phaseIndex = index;
                });
            }

            viewcanvasStage.stageScrollview.LoadData();

            viewcanvasStage.nextChapter.onClick.AddListener(()=> {
                if (isButtonMoving == false)
                {
                    currentChapter += 10;
                    int leftvalue = currentChapter % 10;
                    currentChapter = currentChapter - leftvalue;

                    if (currentChapter >= 100)
                        currentChapter = 98;
                    MoveStageSlot();
                }
           
            });
            viewcanvasStage.prevChapter.onClick.AddListener(() => {
                if(isButtonMoving==false)
                {
                    currentChapter -= 10;
                    if (currentChapter < 0)
                        currentChapter = 0;
                    int leftvalue = currentChapter % 10;
                    currentChapter = currentChapter - leftvalue;
                    MoveStageSlot();
                }
             
            });

          
            viewcanvasStage.BindOnChangeVisible(o =>
            {
                if (o)
                {
                    MoveStageSlot();
                    //viewcanvasStage.stageScrollview.SetPhaseChapter(phaseIndex);
                    //MoveStageSlot(Player.Cloud.field.chapter);
                }
            });

            Player.Option.stageUIOff += CloseStageWindow;

            viewcanvasStage.allRewardBtn.onClick.AddListener(()=> {

                int startIndex = Player.Cloud.chapterRewardedData.LastRewardIndex + 1;
                int lastIndex = (Player.Cloud.field.bestChapter * 5 + Player.Cloud.field.bestStage);

                if(lastIndex>= startIndex)
                {
                    Player.Option.stageAllReward?.Invoke();

                    string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
                    ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);

                    Player.Quest.TryCountUp(QuestType.GetStageReward, 1);
                }
            });

            currentChapter = Player.Cloud.field.bestChapter % 100;
            phaseIndex = Player.Cloud.field.bestChapter / 100;


            viewcanvasStage.rewardItemWindowBtn.onClick.AddListener(()=> {
                viewcanvasStage.chapterItemTableWindow.SetActive(true);
            });

            for(int i=0; i<viewcanvasStage.rewardItemWindowCloseBtns.Length; i++)
            {
                viewcanvasStage.rewardItemWindowCloseBtns[i].onClick.AddListener(() => {
                    viewcanvasStage.chapterItemTableWindow.SetActive(false);
                });
            }

            for(int i=0; i<StaticData.Wrapper.chapterItemrewardData.Length; i++)
            {
                var slotObj = UnityEngine.Object.Instantiate(viewcanvasStage.chapterRewardSlotPrefab);
                slotObj.transform.SetParent(viewcanvasStage.rewardSlotParent.content, false);

                var rewardData = StaticData.Wrapper.chapterItemrewardData[i];

                string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;
                if (i>0)
                {
                    slotObj.title.text = string.Format(localized+" {0}~{1}", StaticData.Wrapper.chapterItemrewardData[i - 1].chapterIndex, rewardData.chapterIndex-1);
                }
                else
                {
                    slotObj.title.text = string.Format(localized+" 1~{0}", rewardData.chapterIndex-1);
                }
            
                for (int j=0; j< rewardData.itemTypes.Length; j++)
                {
                    ViewGoodRewardSlot goodslot = null; 
                   
                    int gradeIndex = 0;
                    switch (rewardData.itemTypes[j])
                    {
                        case ItemType.Equip:
                            ViewGoodRewardSlot goodslot_weapon =  UnityEngine.Object.Instantiate(viewcanvasStage.rewardslotPrefab);
                            ViewGoodRewardSlot goodslot_staff = UnityEngine.Object.Instantiate(viewcanvasStage.rewardslotPrefab);
                            ViewGoodRewardSlot goodslot_armor = UnityEngine.Object.Instantiate(viewcanvasStage.rewardslotPrefab);
                            goodslot_weapon.transform.SetParent(slotObj.rewardSlotParent, false);
                            goodslot_staff.transform.SetParent(slotObj.rewardSlotParent, false);
                            goodslot_armor.transform.SetParent(slotObj.rewardSlotParent, false);

                            gradeIndex = StaticData.Wrapper.weapondatas[rewardData.itemIds[j]].grade - 1;
                            goodslot_weapon.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                            goodslot_weapon.goodsIcon.sprite = InGameResourcesBundle.Loaded.weaponIcon[rewardData.itemIds[j]];
                            goodslot_weapon.goodValue.text = rewardData.amount[j].ToString();

                            gradeIndex = StaticData.Wrapper.weapondatas[rewardData.itemIds[j]].grade - 1;
                            goodslot_staff.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                            goodslot_staff.goodsIcon.sprite = InGameResourcesBundle.Loaded.bowIcon[rewardData.itemIds[j]];
                            goodslot_staff.goodValue.text = rewardData.amount[j].ToString();

                            gradeIndex = StaticData.Wrapper.weapondatas[rewardData.itemIds[j]].grade - 1;
                            goodslot_armor.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                            goodslot_armor.goodsIcon.sprite = InGameResourcesBundle.Loaded.armorUIIcon[rewardData.itemIds[j]];
                            goodslot_armor.goodValue.text = rewardData.amount[j].ToString();
                         
                            break;
                        case ItemType.Skill:
                            goodslot = UnityEngine.Object.Instantiate(viewcanvasStage.rewardslotPrefab_forSkill);
                            goodslot.transform.SetParent(slotObj.rewardSlotParent, false);
                            gradeIndex = StaticData.Wrapper.skillDatas[rewardData.itemIds[j]].grade - 1;
                            goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                            goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[rewardData.itemIds[j]];
                            goodslot.goodValue.text = rewardData.amount[j].ToString();
                            break;
                        case ItemType.Pet:
                            goodslot = UnityEngine.Object.Instantiate(viewcanvasStage.rewardslotPrefab_forPet);
                            goodslot.transform.SetParent(slotObj.rewardSlotParent, false);
                            gradeIndex = StaticData.Wrapper.petdatas[rewardData.itemIds[j]].grade - 1;
                            goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                            goodslot.goodsIcon.sprite = PetResourcesBundle.Loaded.petImage[rewardData.itemIds[j]].slotIconsprite;
                            goodslot.goodValue.text = rewardData.amount[j].ToString();
                            break;
                        case ItemType.Goods:
                            goodslot = UnityEngine.Object.Instantiate(viewcanvasStage.rewardslotPrefab);
                            goodslot.transform.SetParent(slotObj.rewardSlotParent, false);
                            goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[0];
                            goodslot.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[rewardData.itemIds[j]];
                            goodslot.goodValue.text = rewardData.amount[j].ToString();
                            break;
                        case ItemType.End:
                            break;
                        default:
                            break;
                    }
                }
                
            }
            
            //MoveStageSlot();
            Main().Forget();

            Player.Option.stageAllReward += AllReward;

            viewcanvasStage.BindOnChangeVisible(active=> { 
                if(active)
                {
                    if (Player.Guide.currentGuideQuest == QuestGuideType.StageReward)
                    {
                        Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                    }
                }
            });

            viewcanvasStage.titleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;
            viewcanvasStage.rewardTableTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AddReward].StringToLocal;
            viewcanvasStage.allrewardTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AllReward].StringToLocal;
            viewcanvasStage.rewardWindowTitleTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_MonsReward].StringToLocal;
        }

      

        void AllReward()
        {
            int startChapter;
            int startStage;

            int startIndex = Player.Cloud.chapterRewardedData.LastRewardIndex + 1;
            int lastIndex = (Player.Cloud.field.bestChapter * 5 + Player.Cloud.field.bestStage);
            startChapter = (startIndex / 5);
            startStage = startIndex % 5;

            for(int i=startIndex; i<= lastIndex; i++)
            {
                int index = i;

                int chapterIndex = index / 5;
                int stageIndex = index / 5;
                var chapterRewardData = StaticData.Wrapper.chapterRewardTableDatas[chapterIndex];

                Player.ControllerGood.Earn(chapterRewardData.rewardGood, chapterRewardData.rewardValue);
            }
            Player.Cloud.chapterRewardedData.LastRewardIndex = (Player.Cloud.field.bestChapter * 5 + Player.Cloud.field.bestStage);
            
            //for (int i = 0; i < StaticData.Wrapper.chapterRewardTableDatas.Length; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //    {
            //        int chapterindex = i;
            //        int stageindex = j;

            //        bool canRecieve = false;
            //        if (chapterindex < Player.Cloud.field.bestChapter)
            //        {
            //            canRecieve = true;
            //        }
            //        else if (chapterindex == Player.Cloud.field.bestChapter)
            //        {
            //            canRecieve = stageindex <= Player.Cloud.field.bestStage;
            //        }

            //        if (canRecieve == false)
            //            continue;

            //        int rewardIndex = chapterindex * 5 + stageindex;

            //        if (Player.Cloud.chapterRewardedData.rewardedList[rewardIndex])
            //        {
            //            continue;
            //        }
            //        Player.Cloud.chapterRewardedData.rewardedList[rewardIndex] = true;

            //        var chapterRewardData = StaticData.Wrapper.chapterRewardTableDatas[chapterindex];

            //        Player.ControllerGood.Earn(chapterRewardData.rewardGood, chapterRewardData.rewardValue);
            //    }
            //}

            //Player.Cloud.chapterRewardedData.LastRewardIndex = Player.Cloud.field.bestChapter * 5 + Player.Cloud.field.bestStage;
            
          
            viewcanvasStage.stageScrollview.InitializeTableView();
            //viewcanvasStage.stageScrollview.SlotUpdate();
            LocalSaveLoader.SaveUserCloudData();
            Player.Cloud.chapterRewardedData.UpdateHash().SetDirty(true);
        }
        async UniTaskVoid Main()
        {
            while (true)
            {
                if(viewcanvasStage.gameObject.activeInHierarchy)
                {
                    float contentY = viewcanvasStage.scroll.content.anchoredPosition.y;

                    currentChapter = (int)((contentY - paddingTop + slotHeight * 2) / (slotHeight * 5));

                    string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;

                    viewcanvasStage.selectedChapter.text = string.Format("{0}"+ localized, phaseIndex*100+ currentChapter + 1);
                }

             
                await UniTask.Yield(_cts.Token);
            }
        }

        void CloseStageWindow()
        {
            viewcanvasStage.blackBG.PopupCloseColorFade();
            viewcanvasStage.Wrapped.CommonPopupCloseAnimationUp(() => {
                viewcanvasStage.SetVisible(false);
            });
        }

        const int paddingTop = 30;
        const int slotHeight = 175;

        bool isButtonMoving = false;
        void MoveStageSlot()
        {
            isButtonMoving = true;

            viewcanvasStage.stageScrollview.SetPhaseChapter(phaseIndex);
            viewcanvasStage.phaseSelector.Show(phaseIndex);
      
            //isButtonMoving = true;
            int contentY=paddingTop + slotHeight * currentChapter * 5 - slotHeight * 2;
            if (contentY < 0)
                contentY = 0;
            //viewcanvasStage.scroll.content.anchoredPosition = new Vector2(viewcanvasStage.scroll.content.anchoredPosition.x,contentY);
            viewcanvasStage.scroll.content.DOAnchorPos(new Vector2(viewcanvasStage.scroll.content.anchoredPosition.x, contentY),0.2f)
                .SetEase(Ease.InQuart).OnComplete(() => {
                    isButtonMoving = false;
            });

            string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Chapter].StringToLocal;
            viewcanvasStage.selectedChapter.text = string.Format("{0}"+ localized, phaseIndex * 100 + currentChapter + 1);
        }


    }
}

