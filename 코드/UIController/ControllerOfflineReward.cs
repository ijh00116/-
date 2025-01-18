using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using System;

namespace BlackTree.Core
{
   
    public class ControllerOfflineReward
    {
        public static Action OnReward;
        private readonly ViewCanvasOfflineReward _view;
        private readonly CancellationTokenSource _cts;

        private int _minute;
        private bool _isGetReward = true;

        private List<ViewGoodRewardSlot> viewRewardSlotList = new List<ViewGoodRewardSlot>();
        private List<ViewGoodRewardSlot> viewItemRewardSlotList = new List<ViewGoodRewardSlot>();
        private List<ItemrewardData> rewardDataList = new List<ItemrewardData>();

        public ControllerOfflineReward(Transform parent,CancellationTokenSource cts)
        {
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasOfflineReward>(parent);
            _view.SetLocalizedText();

            _view.getRewardBtn.onClick.AddListener(() =>
            {
                GetReward();
            });

            _view.getDoubleRewardBtn.onClick.AddListener(() =>
            {
                if (Player.Cloud.inAppPurchase.purchaseAds)
                {
                    GetReward(true);
                }
                else
                {
                    AdmobManager.Instance.ShowRewardedAd(()=> {
                        GetReward(true);
                    });
                }
                    
            });

            OnReward += () =>
            {
                if (!_isGetReward)
                    GetReward();
            };

            _view._textTitle.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_OfflineRewardTitle].StringToLocal;
            _view.offlineDesc.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Welcome].StringToLocal;
            _view.rewardTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Get].StringToLocal;

            Main().Forget();
        }

        private void UpdateReward()
        {
            double expValueForone = Battle.Field.GetRewardExpForEnemy(Player.Cloud.field.bestChapter, Player.Cloud.field.bestStage);
            double goldValueForone = Battle.Field.GetRewardGoldForEnemy(Player.Cloud.field.bestChapter);

            ViewGoodRewardSlot slot_gold;
            ViewGoodRewardSlot slot_exp;
            if (viewRewardSlotList.Count==0)
            {
                slot_gold = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab);
                viewRewardSlotList.Add(slot_gold);

                slot_exp = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab);
                viewRewardSlotList.Add(slot_exp);
            }
            else
            {
                slot_gold = viewRewardSlotList[0];
                slot_exp = viewRewardSlotList[1];
            }
            slot_gold.transform.SetParent(_view.slotParent, false);
            slot_gold.goodValue.text = $"{(goldValueForone * _minute * 20).ToNumberString()}";
            slot_gold.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Coin];

            slot_exp.transform.SetParent(_view.slotParent, false);
            slot_exp.goodValue.text = $"{(expValueForone * _minute * 20).ToNumberString()}";
            slot_exp.goodsIcon.sprite = GoodResourcesBundle.Loaded.rewardSprites[(int)RewardTypes.Exp];

            //otherReward
            int rewardCount = _minute / 70;
            ChapterRewardItemData itemdata = null;
            for (int i = 0; i < StaticData.Wrapper.chapterItemrewardData.Length; i++)
            {
                var data = StaticData.Wrapper.chapterItemrewardData[i];
                if (Player.Cloud.field.bestChapter < data.chapterIndex)
                {
                    itemdata = data;
                    break;
                }
            }
          
            for (int i=0; i<rewardCount; i++)
            {
                int randomindex = GetRandommItemrewardIndex(itemdata);
                ItemType _itemtype = itemdata.itemTypes[randomindex];
              
                ItemrewardData rewardData = new ItemrewardData();
                rewardData._itemType = _itemtype;
                rewardData.rewardIdx = itemdata.itemIds[randomindex];
                rewardData.equiptype = EquipType.END;
                rewardData.amount = itemdata.amount[randomindex];
                int equipRandom = UnityEngine.Random.Range(0, 3);
                if (rewardData._itemType==ItemType.Equip)
                {
                    rewardData.equiptype = (EquipType)equipRandom;
                }

                bool isExist = false;
                foreach (var item in rewardDataList)
                {
                    if (item.equiptype == rewardData.equiptype && item.rewardIdx == rewardData.rewardIdx && item._itemType == rewardData._itemType)
                    {
                        item.amount += rewardData.amount;
                        isExist = true;
                        break;
                    }
                }
                if (isExist == false)
                {
                    rewardDataList.Add(rewardData);
                }
            }

            for (int i = 0; i < rewardDataList.Count; i++)
            {
                int gradeIndex = 0;
                ViewGoodRewardSlot goodslot = null;

                switch (rewardDataList[i]._itemType)
                {
                    case ItemType.Equip:

                        goodslot = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab);
                        goodslot.transform.SetParent(_view.slotParent, false);
                        gradeIndex = StaticData.Wrapper.weapondatas[rewardDataList[i].rewardIdx].grade - 1;
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                        goodslot.goodValue.text = rewardDataList[i].amount.ToString();
                        if (rewardDataList[i].equiptype == EquipType.Weapon)
                        {
                            goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.weaponIcon[rewardDataList[i].rewardIdx];
                        }
                        else if (rewardDataList[i].equiptype == EquipType.Bow)
                        {
                            goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.bowIcon[rewardDataList[i].rewardIdx];
                        }
                        else if (rewardDataList[i].equiptype == EquipType.Armor)
                        {
                            goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.armorUIIcon[rewardDataList[i].rewardIdx];
                        }

                        break;
                    case ItemType.Skill:
                        goodslot = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab_skill);
                        goodslot.transform.SetParent(_view.slotParent, false);
                        gradeIndex = StaticData.Wrapper.skillDatas[rewardDataList[i].rewardIdx].grade - 1;
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                        goodslot.goodsIcon.sprite = InGameResourcesBundle.Loaded.skillIcon[rewardDataList[i].rewardIdx];
                        goodslot.goodValue.text = rewardDataList[i].amount.ToString();
                        break;
                    case ItemType.Pet:
                        goodslot = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab_pet);
                        goodslot.transform.SetParent(_view.slotParent, false);
                        gradeIndex = StaticData.Wrapper.weapondatas[rewardDataList[i].rewardIdx].grade - 1;
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[gradeIndex];
                        goodslot.goodsIcon.sprite = PetResourcesBundle.Loaded.petImage[rewardDataList[i].rewardIdx].slotIconsprite;
                        goodslot.goodValue.text = rewardDataList[i].amount.ToString();
                        break;
                    case ItemType.Goods:
                        goodslot = UnityEngine.Object.Instantiate(_view.rewardSlotPrefab);
                        goodslot.transform.SetParent(_view.slotParent, false);
                        goodslot.goodsFrame.sprite = InGameResourcesBundle.Loaded.weaponGradeInnerFrameSprite[0];
                        goodslot.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[rewardDataList[i].rewardIdx];
                        goodslot.goodValue.text = rewardDataList[i].amount.ToString();
                        break;
                    case ItemType.End:
                        break;
                    default:
                        break;
                }
                viewItemRewardSlotList.Add(goodslot);
            }
        }

        static int GetRandommItemrewardIndex(ChapterRewardItemData itemdata)
        {
            float random = UnityEngine.Random.Range(0.0f, 1.0f);

            float _data = 0;
            int index = 0;
            for (int i = 0; i < itemdata.rewardItemRate.Length; i++)
            {
                _data += itemdata.rewardItemRate[i];
                if (random <= _data)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        private async UniTaskVoid Main()
        {
            await UniTask.WaitUntil(() => BackEnd.Backend.IsInitialized, PlayerLoopTiming.Update, _cts.Token);

            if (Player.OfflineReward.IsGetableReward)
            {
                _isGetReward = false;
                _minute = Player.OfflineReward.RewardTimeToMinute;

                _view.SetOfflineTime(_minute);
                PopupWindowOn();
                UpdateReward();
            }

            while (true)
            {
                Player.OfflineReward.SetLastTime();
                await UniTask.Delay(60000, true, PlayerLoopTiming.Update, _cts.Token);
            }
        }

        private void GetReward(bool doubleValue = false)
        {
            double expValueForone=Battle.Field.GetRewardExpForEnemy(Player.Cloud.field.bestChapter,Player.Cloud.field.bestStage);
            double goldValueForone = Battle.Field.GetRewardGoldForEnemy(Player.Cloud.field.bestChapter);

            Player.Level.ExpUpAndLvUp(expValueForone*_minute*20*(doubleValue?2:1));
            Player.ControllerGood.Earn(GoodsKey.Coin, goldValueForone*_minute*20*(doubleValue?2:1));


            ChapterRewardItemData itemdata = null;
            for (int i = 0; i < StaticData.Wrapper.chapterItemrewardData.Length; i++)
            {
                var data = StaticData.Wrapper.chapterItemrewardData[i];
                if (Player.Cloud.field.bestChapter < data.chapterIndex)
                {
                    itemdata = data;
                    break;
                }
            }

            for (int i=0; i< rewardDataList.Count;i++)
            {
                switch (rewardDataList[i]._itemType)
                {
                    case ItemType.Equip:
                        int equipRandom = UnityEngine.Random.Range(0, 3);
                        Player.EquipItem.Obtain((EquipType)equipRandom, rewardDataList[i].rewardIdx, rewardDataList[i].amount);
                        break;
                    case ItemType.Skill:
                        Player.Skill.Obtain((SkillKey)rewardDataList[i].rewardIdx, rewardDataList[i].amount);
                        break;
                    case ItemType.Pet:
                        Player.Pet.Obtain(rewardDataList[i].rewardIdx, rewardDataList[i].amount);
                        break;
                    case ItemType.Goods:
                        Player.ControllerGood.Earn((GoodsKey)rewardDataList[i].rewardIdx, rewardDataList[i].amount);
                        break;
                    case ItemType.End:
                        break;
                    default:
                        break;
                }
            }

            Player.OfflineReward.SetLastTime();
            _isGetReward = true;
            PopupWindowOff();

            string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_GoodsRewarded].StringToLocal;
            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
        }

        void PopupWindowOn()
        {
            _view.SetVisible(true);
            _view.blackBG.PopupOpenColorFade();
            _view.popupRect.CommonPopupOpenAnimationDown(() => {

            });
        }

        void PopupWindowOff()
        {
            _view.blackBG.PopupCloseColorFade();
            _view.popupRect.CommonPopupCloseAnimationUp(() => {
                _view.SetVisible(false);
            });
        }
    }

}
