using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using System;
using BlackTree.Definition;

namespace BlackTree.Core
{

    public class ControllerSleepMode
    {
        private readonly ViewCanvasSleepMode _viewCanvasSleepMode;
        private readonly CancellationTokenSource _cts;

        int animindex_0;
        int animindex_1;
        int currentFrame = 0;

        float currentTime = 0;
        const float startAutoSleepModeTime = 120;


        List<ItemrewardData> itemRewardList = new List<ItemrewardData>();
        List<ViewGoodRewardSlot> rewardSlotList = new List<ViewGoodRewardSlot>();
        public ControllerSleepMode(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewCanvasSleepMode = ViewCanvas.Create<ViewCanvasSleepMode>(parent);

            _viewCanvasSleepMode.Init();

            Main().Forget();
            SetTime().Forget();

            Player.sleepMode += ActiveUIInSleepMode;
            Battle.Field.onChangeStage+= UpdateStageText;

            Battle.Field.itemRewardCallback += AddRewardItem;

            _viewCanvasSleepMode.BindOnChangeVisible((active)=> { 
                if(active)
                {
                    for(int i=0; i< rewardSlotList.Count; i++)
                    {
                        rewardSlotList[i].gameObject.SetActive(false);
                    }
                    for(int i=0; i< itemRewardList.Count; i++)
                    {
                        itemRewardList[i].amount = 0;
                    }

                    _viewCanvasSleepMode.earnItemObj.SetActive(false);
                }
            
            });

            UpdateStageText();
        }

        void AddRewardItem(ItemrewardData _data)
        {
            if (_viewCanvasSleepMode.IsVisible == false)
                return;
            bool isExist = false;
            foreach (var item in itemRewardList)
            {
                if(item.equiptype==_data.equiptype&& item.rewardIdx == _data.rewardIdx&& item._itemType == _data._itemType)
                {
                    item.amount += _data.amount;
                    isExist = true;
                    break;
                }
              
            }
            if(isExist==false)
            {
                ItemrewardData rewarddata = new ItemrewardData();
                rewarddata._itemType= _data._itemType;
                rewarddata.equiptype = _data.equiptype;
                rewarddata.rewardIdx = _data.rewardIdx;
                rewarddata.amount = _data.amount;
                itemRewardList.Add(rewarddata);
            }

            for(int i=0; i< itemRewardList.Count; i++)
            {
                ViewGoodRewardSlot goodslot = null;
                if (i< rewardSlotList.Count)
                {
                    goodslot = rewardSlotList[i];
                }
                else
                {
                    switch (itemRewardList[i]._itemType)
                    {
                        case Definition.ItemType.Goods:
                        case Definition.ItemType.Equip:
                            goodslot = UnityEngine.Object.Instantiate(_viewCanvasSleepMode.rewardSlotPrefab);
                            break;
                        case Definition.ItemType.Skill:
                            goodslot = UnityEngine.Object.Instantiate(_viewCanvasSleepMode.rewardSlotPrefab_skill);
                            break;
                        case Definition.ItemType.Pet:
                            goodslot = UnityEngine.Object.Instantiate(_viewCanvasSleepMode.rewardSlotPrefab_pet);
                            break;
                        default:
                            break;
                    }
                    goodslot.transform.SetParent(_viewCanvasSleepMode.slotParent,false);
                    rewardSlotList.Add(goodslot);
                }
                if(itemRewardList[i].amount>0)
                {
                    goodslot.gameObject.SetActive(true);
                }
                
                int gradeIndex = 0;

                switch (itemRewardList[i]._itemType)
                {
                    case ItemType.Equip:
                        gradeIndex = StaticData.Wrapper.weapondatas[itemRewardList[i].rewardIdx].grade - 1;
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                        goodslot.goodValue.text = itemRewardList[i].amount.ToString();
                        if (itemRewardList[i].equiptype == EquipType.Weapon)
                        {
                            goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.weaponIcon[itemRewardList[i].rewardIdx];
                        }
                        else if (itemRewardList[i].equiptype == EquipType.Bow)
                        {
                            goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.bowIcon[itemRewardList[i].rewardIdx];
                        }
                        else if (itemRewardList[i].equiptype == EquipType.Armor)
                        {
                            goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.armorUIIcon[itemRewardList[i].rewardIdx];
                        }

                        break;
                    case ItemType.Skill:
                        gradeIndex = StaticData.Wrapper.skillDatas[itemRewardList[i].rewardIdx].grade - 1;
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                        goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[itemRewardList[i].rewardIdx];
                        goodslot.goodValue.text = itemRewardList[i].amount.ToString();
                        break;
                    case ItemType.Pet:
                        gradeIndex = StaticData.Wrapper.weapondatas[itemRewardList[i].rewardIdx].grade - 1;
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                        goodslot.goodsIcon.sprite = PetResourcesBundle.Loaded.petImage[itemRewardList[i].rewardIdx].slotIconsprite;
                        goodslot.goodValue.text = itemRewardList[i].amount.ToString();
                        break;
                    case ItemType.Goods:
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[0];
                        goodslot.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[itemRewardList[i].rewardIdx];
                        goodslot.goodValue.text = itemRewardList[i].amount.ToString();
                        break;
                    case ItemType.End:
                        break;
                    default:
                        break;
                }
            }

            if(_viewCanvasSleepMode.earnItemObj.activeInHierarchy==false)
            {
                _viewCanvasSleepMode.earnItemObj.SetActive(true);
            }
        }
        private async UniTaskVoid Main()
        {
            PointerEventData eventCurrentPosition = new PointerEventData(EventSystem.current);
            bool isMouseDrag = false;

            animindex_0 = 0;
            animindex_1 =0;
            currentFrame = 0;
            while (true)
            {
                if (_viewCanvasSleepMode.IsVisible)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        eventCurrentPosition.position = Input.mousePosition;
                        isMouseDrag = true;
                    }

                    if (isMouseDrag)
                    {
                        float pos = Vector2.Distance(eventCurrentPosition.position * 0.005f, Input.mousePosition * 0.005f);
                        _viewCanvasSleepMode.SetCanvasAlpha(1f - Mathf.Clamp01(pos));

                        if (Input.GetMouseButtonUp(0))
                        {
                            isMouseDrag = false;
                            if (pos < 1)
                                _viewCanvasSleepMode.SetOrigin();
                            else
                            {
                                _viewCanvasSleepMode.SetInit();
                                Player.sleepMode?.Invoke(false);
                            }
                        }
                    }
                    currentFrame++;
                    if (currentFrame >= 3)
                    {
                        currentFrame = 0;
                        _viewCanvasSleepMode._character_0.sprite = _viewCanvasSleepMode.character_0Images[animindex_0];
                        _viewCanvasSleepMode._character_1.sprite = _viewCanvasSleepMode.character_1Images[animindex_1];

                        animindex_0++;
                        animindex_1++;

                        if (animindex_0 >= _viewCanvasSleepMode.character_0Images.Length)
                        {
                            animindex_0 = 0;
                        }
                        if (animindex_1 >= _viewCanvasSleepMode.character_1Images.Length)
                        {
                            animindex_1 = 0;
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        currentTime = 0;
                    }
                    if (Player.Cloud.optiondata.autoSaveMode)
                    {
                        if (currentTime < startAutoSleepModeTime)
                        {
                            currentTime += Time.deltaTime;
                        }
                        else
                        {
                            currentTime = 0;
                            Player.sleepMode?.Invoke(true);
                        }
                    }
                }

                _viewCanvasSleepMode.SetGoldText(Battle.Field.currentGetGoldAfterSleep.ToNumberString());
                _viewCanvasSleepMode.SetExpText(Battle.Field.currentGetExpAfterSleep.ToNumberString());

                await UniTask.Yield(_cts.Token);
            }
        }

   

        void ActiveUIInSleepMode(bool active)
        {
            //무조건 켜져 있어야 할 것들만 활성 비활성 체크함
            ViewCanvas.Get<ViewCanvasMainNav>().SetVisible(!active);
            ViewCanvas.Get<ViewCanvasMainQuest>().SetVisible(!active);
            ViewCanvas.Get<ViewCanvasMainTop>().SetVisible(!active);
            _viewCanvasSleepMode.SetVisible(active);

            if(active)
            {
                AudioManager.Instance.SetVolume(true, 0);
                AudioManager.Instance.SetVolume(false, 0);
            }
            else
            {
                AudioManager.Instance.SetVolume(true, Player.Cloud.optiondata.bgmSound);
                AudioManager.Instance.SetVolume(false, Player.Cloud.optiondata.effectSound);
            }

            Battle.Field.currentGetExpAfterSleep = 0;
            Battle.Field.currentGetGoldAfterSleep = 0;

            if(active)
            {
                Application.targetFrameRate = 30;
            }
            else
            {
                Application.targetFrameRate = 60;
            }
        }

        private async UniTaskVoid SetTime()
        {
            while (true)
            {
                _viewCanvasSleepMode.SetTimeText(DateTime.Now.ToString("HH:mm"));
                await UniTask.Delay(60000, false, PlayerLoopTiming.Update, _cts.Token);
            }
        }

        private void UpdateStageText()
        {
            string stageTxt=string.Format("{0}-{1}", Battle.Field.CurrentFieldChapter+1, Battle.Field.CurrentFieldStage+1);
            _viewCanvasSleepMode.SetWaveInStageText(stageTxt);
        }
    }
}
