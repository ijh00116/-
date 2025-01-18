using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using BlackTree.Bundles;
using BlackTree.Model;
using LitJson;
using BackEnd;
using BlackTree.Definition;

namespace BlackTree.Core
{
    [System.Serializable]
    public class MailItemTable
    {
        public MailItem item;
        public int itemCount;//뒤끝 제공 itemCount
        public string chartName;
    }

    [System.Serializable]
    public class MailItem
    {
        public int index;
        public MailRewardType mailRewardType;
    }
   
    public class ControllerMail
    {
        ViewCanvasMail _viewMail;
        CancellationTokenSource _cts;
        Transform _parent;

        List<ViewMailSlot> mailList = new List<ViewMailSlot>();
        public ControllerMail(Transform parent, CancellationTokenSource cts)
        {
            _parent = parent;
            _cts = cts;
            _viewMail = ViewCanvas.Create<ViewCanvasMail>(_parent);

            for(int i= Player.Cloud.mailRecieveHistory.recieved.Count; i<StaticData.Wrapper.fixedMailData.Length ; i++)
            {
                Player.Cloud.mailRecieveHistory.recieved.Add(false);
            }

            foreach (var button in _viewMail.closeBtn)
            {
                button.onClick.AddListener(() =>
                {
                    _viewMail.blackBG.PopupCloseColorFade();
                    _viewMail.Wrapped.CommonPopupCloseAnimationUp(() =>
                    {
                        _viewMail.SetVisible(false);
                    });
                });
            }

            Player.Option.MailUpdate += GetMailList;

            _viewMail._title.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Mail].StringToLocal;

            GetMailList();
            Main().Forget();
            TableMailUpdate().Forget();
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                GetMailList();
                await UniTask.Delay(60000);
            }
        }

        async UniTaskVoid TableMailUpdate()
        {
            while (true)
            {
                if (IsTableMailExist())
                {
                    Player.Option.menuRedDotCallback?.Invoke(MenuType.Mail, true);
                }

                TableMailSlotSetting();
                await UniTask.Delay(60000);
            }
        }

        void GetMailList()
        {
            for(int i=0; i< mailList.Count;i++)
            {
                mailList[i].gameObject.SetActive(false);
            }
            var bro = Backend.UPost.GetPostList(PostType.Admin, 100);
            if(bro.GetStatusCode()=="200")
            {
                JsonData adminjson = bro.GetReturnValuetoJSON()["postList"];
                if (adminjson.Count > 0)
                {
                    Player.Option.menuRedDotCallback?.Invoke(MenuType.Mail, true);
                }
                MailSlotSetting(adminjson, PostType.Admin);
            }
            var rankingbro = Backend.UPost.GetPostList(PostType.Rank, 100);
            if(rankingbro.GetStatusCode()=="200")
            {
                JsonData rankingjson = rankingbro.GetReturnValuetoJSON()["postList"];
                if (rankingjson.Count > 0)
                {
                    Player.Option.menuRedDotCallback?.Invoke(MenuType.Mail, true);
                }
                MailSlotSetting(rankingjson, PostType.Rank);
            }
           
          
            if(IsTableMailExist())
            {
                Player.Option.menuRedDotCallback?.Invoke(MenuType.Mail, true);
            }
      
            
            TableMailSlotSetting();
        }

        void MailSlotSetting(JsonData json, PostType postType)
        {
            //mail slot
            for (int i = 0; i < json.Count; i++)//우편의 갯수
            {
                int index = i;

                string postTitle = json[index]["title"].ToString(); // 우편 제목
                string postContent = json[index]["content"].ToString(); // 우편 내용
                string postIndate = json[index]["inDate"].ToString();

                ViewMailSlot slot = mailList.Find(o => o.gameObject.activeInHierarchy == false);
                if (slot == null)
                {
                    slot = Object.Instantiate(_viewMail.mailSlot);
                    slot.transform.SetParent(_viewMail.slotParent, false);
                    mailList.Add(slot);
                }
                slot.raidMailIndex = 0;
                slot.mailTitle = postTitle;
                slot.mailIndate = postIndate;
                slot.gameObject.SetActive(true);
                slot.getBtn.onClick.RemoveAllListeners();
                slot.getBtn.onClick.AddListener(() => { ReceivePostItem(postType, postIndate, slot); });
                slot.getBtnTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Get].StringToLocal;
                if (postType==PostType.Rank)
                {
                    if (mailList[i].mailTitle.Contains("레이드"))
                    {
                        string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RaidRankingMailTitle].StringToLocal;
                        slot.title.text = localized;
                    }
                    else
                    {
                        if (mailList[i].mailTitle.Contains("레벨"))
                        {
                            string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_levelRankingMailTitle].StringToLocal;
                            slot.title.text = localized;
                        }
                        else
                        {
                            string localized = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_StageRankingMailTitle].StringToLocal;
                            slot.title.text = localized;
                        }
                            
                    }
                }
                else
                {
                    slot.title.text = postTitle;
                }
                

                int slotIndex = 0;
                foreach (LitJson.JsonData data in json[i]["items"])
                {
                    var mailitem = JsonUtility.FromJson<MailItemTable>(data.ToJson().ToString());

                    if(mailitem!=null)
                    {
                        var rewarddata = StaticData.Wrapper.mailRewardDatas[(int)mailitem.item.mailRewardType];

                        for (int j = 0; j < rewarddata.goodskeyList.Length; j++)
                        {
                            ViewGoodRewardSlot rewardslot = null;
                            if (slotIndex >= slot.rewardSlotList.Count)
                            {
                                rewardslot = Object.Instantiate(_viewMail.rewardSlot);
                                slot.rewardSlotList.Add(rewardslot);
                            }
                            else
                            {
                                rewardslot = slot.rewardSlotList[slotIndex];
                            }
                            slotIndex++;

                            rewardslot.transform.SetParent(slot.rewardSlotParent, false);
                            rewardslot.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)rewarddata.goodskeyList[j]];
                            rewardslot.goodValue.text = rewarddata.goodsAmountList[j].ToString();
                        }
                    }
                }
            }

            if(postType==PostType.Rank)
            {
                List<int> raidRewardmailIndex = new List<int>();
                for(int i=0; i< mailList.Count; i++)
                {
                    int index = i;
                    if(mailList[i].mailTitle.Contains("레이드"))
                    {
                        raidRewardmailIndex.Add(index);
                    }
                }

                if(raidRewardmailIndex.Count>1)
                {
                    for(int i=0; i<raidRewardmailIndex.Count; i++)
                    {
                        if(mailList[raidRewardmailIndex[i]].mailTitle.Contains("레이드 랭킹 보상 D"))
                        {
                            mailList[raidRewardmailIndex[i]].raidMailIndex = 4;
                        }
                        if (mailList[raidRewardmailIndex[i]].mailTitle.Contains("레이드 랭킹 보상 C"))
                        {
                            mailList[raidRewardmailIndex[i]].raidMailIndex = 3;
                        }
                        if (mailList[raidRewardmailIndex[i]].mailTitle.Contains("레이드 랭킹 보상 B"))
                        {
                            mailList[raidRewardmailIndex[i]].raidMailIndex = 2;
                        }
                        if (mailList[raidRewardmailIndex[i]].mailTitle.Contains("레이드 랭킹 보상 A"))
                        {
                            mailList[raidRewardmailIndex[i]].raidMailIndex = 1;
                        }
                        if (mailList[raidRewardmailIndex[i]].mailTitle.Contains("레이드 랭킹 보상 S"))
                        {
                            mailList[raidRewardmailIndex[i]].raidMailIndex = 0;
                        }
                    }

                    int survivedIndex = 5;
                    int survivedValue = 5;

                    for (int i = 0; i < raidRewardmailIndex.Count; i++)
                    {
                        if(mailList[raidRewardmailIndex[i]].raidMailIndex< survivedValue)
                        {
                            survivedIndex = raidRewardmailIndex[i];
                            survivedValue = mailList[raidRewardmailIndex[i]].raidMailIndex;
                        }
                    }
                    for (int i = 0; i < raidRewardmailIndex.Count; i++)
                    {
                        if(raidRewardmailIndex[i]!=survivedIndex)
                        {
                            var bro = Backend.UPost.ReceivePostItem(PostType.Rank, mailList[raidRewardmailIndex[i]].mailIndate);
                            mailList[raidRewardmailIndex[i]].gameObject.SetActive(false);
                        }
                    }

                }
            }
        }
        bool IsTableMailExist()
        {
            bool canRecieve = false;

            for (int i = 0; i < StaticData.Wrapper.fixedMailData.Length; i++)
            {
                int index = i;
                if (Player.Cloud.mailRecieveHistory.recieved[i])
                    continue;
                switch (StaticData.Wrapper.fixedMailData[index].lockType)
                {
                    case ContentLockType.UnitLevel:
                        if (Player.Cloud.userLevelData.currentLevel >= StaticData.Wrapper.fixedMailData[index].unLockLevel)
                        {
                            canRecieve = true;
                        }
                        else
                        {
                            canRecieve = false;
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (Player.Cloud.field.chapter >= StaticData.Wrapper.fixedMailData[index].unLockLevel)
                        {
                            canRecieve = true;
                        }
                        else
                        {
                            canRecieve = false;
                        }
                        break;
                    default:
                        break;
                }
                if (canRecieve)
                {
                    break;
                }
            }

            return canRecieve;
        }
        void TableMailSlotSetting()
        {
            for(int i=0; i<StaticData.Wrapper.fixedMailData.Length; i++)
            {
                int index = i;
                if (Player.Cloud.mailRecieveHistory.recieved[i])
                    continue;
                bool canRecieve = true;
                switch (StaticData.Wrapper.fixedMailData[index].lockType)
                {
                    case ContentLockType.UnitLevel:
                        if (Player.Cloud.userLevelData.currentLevel >= StaticData.Wrapper.fixedMailData[index].unLockLevel)
                        {
                            canRecieve = true;
                        }
                        else
                        {
                            canRecieve = false;
                        }
                        break;
                    case ContentLockType.ChapterLevel:
                        if (Player.Cloud.field.chapter >= StaticData.Wrapper.fixedMailData[index].unLockLevel)
                        {
                            canRecieve = true;
                        }
                        else
                        {
                            canRecieve = false;
                        }
                        break;
                    default:
                        break;
                }
                //if (canRecieve == false)
                //    continue;
                ViewMailSlot slot = mailList.Find(o => o.gameObject.activeInHierarchy == false);
                if (slot == null)
                {
                    slot = Object.Instantiate(_viewMail.mailSlot);
                    slot.transform.SetParent(_viewMail.slotParent, false);
                    mailList.Add(slot);
                }
                slot.gameObject.SetActive(true);
                slot.getBtn.onClick.RemoveAllListeners();
                slot.getBtn.onClick.AddListener(() => { RecieveFixeMailItem(StaticData.Wrapper.fixedMailData[index], slot); });
                LocalizeDescKeys key = StaticData.Wrapper.fixedMailData[index].desc;
                slot.title.text = StaticData.Wrapper.localizeddesclist[(int)key].StringToLocal;
                slot.getBtnTxt.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Get].StringToLocal;
                if (canRecieve==false)
                {
                    slot.lockObj.SetActive(true);
                    slot.getBtn.enabled = false;
                    string infoText = null;
                    
                    switch (StaticData.Wrapper.fixedMailData[index].lockType)
                    {
                        case ContentLockType.UnitLevel:
                            infoText = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_CanRewardLevel].StringToLocal, StaticData.Wrapper.fixedMailData[index].unLockLevel);
                            break;
                        case ContentLockType.ChapterLevel:
                            infoText = string.Format(StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_CanRewardChapter].StringToLocal, StaticData.Wrapper.fixedMailData[index].unLockLevel);
                            break;
                        default:
                            break;
                    }
                    slot.lockedText.text = infoText;
                }
                else
                {
                    slot.lockObj.SetActive(false);
                    slot.getBtn.enabled = true;
                }
                int slotIndex = 0;

                for (int j = 0; j < StaticData.Wrapper.fixedMailData[index].goodskeyList.Length; j++)
                {
                    ViewGoodRewardSlot rewardslot = null;
                    if (slotIndex >= slot.rewardSlotList.Count)
                    {
                        rewardslot = Object.Instantiate(_viewMail.rewardSlot);
                        slot.rewardSlotList.Add(rewardslot);
                    }
                    else
                    {
                        rewardslot = slot.rewardSlotList[slotIndex];
                    }
                    slotIndex++;

                    rewardslot.transform.SetParent(slot.rewardSlotParent, false);
                    rewardslot.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)StaticData.Wrapper.fixedMailData[index].goodskeyList[j]];
                    rewardslot.goodValue.text = StaticData.Wrapper.fixedMailData[index].goodsAmountList[j].ToString();
                }
              
            }
        }
        private void RecieveFixeMailItem(FixedMailData tableData,ViewMailSlot slotObject)
        {
            if (Player.Cloud.mailRecieveHistory.recieved[tableData.index])
                return;
            for(int i=0; i< tableData.goodskeyList.Length; i++)
            {
                Player.ControllerGood.Earn(tableData.goodskeyList[i], tableData.goodsAmountList[i]);
            }
            PopupRewardPopup(tableData.goodskeyList, tableData.goodsAmountList);

            Player.Cloud.mailRecieveHistory.recieved[tableData.index] = true;
            slotObject.gameObject.SetActive(false);
            Player.Cloud.mailRecieveHistory.UpdateHash().SetDirty(true);

            Player.SaveUserDataToFirebaseAndLocal().Forget();

            bool allRecieve = true;
            for (int i = 0; i < mailList.Count; i++)
            {
                if (mailList[i].gameObject.activeInHierarchy)
                {
                    if (mailList[i].lockObj.activeInHierarchy==false)
                    {
                        allRecieve = false;
                        break;
                    }
                }
            }

            if (allRecieve)
            {
                Player.Option.menuRedDotCallback?.Invoke(MenuType.Mail, false);
            }
        }

        public void ReceivePostItem(PostType postType,string postIndate, ViewMailSlot slotObject)
        {
            var bro = Backend.UPost.ReceivePostItem(postType, postIndate);
            JsonData postList = bro.GetReturnValuetoJSON()["postItems"];

            for (int i = 0; i < postList.Count; i++)
            {
                if (postList[i].Count <= 0)
                {
                    //Debug.Log("아이템이 없는 우편");
                    continue;
                }
                var mailitem = JsonUtility.FromJson<MailItem>(postList[i]["item"].ToJson().ToString());

                if(mailitem!=null)
                {
                    var rewarddata = StaticData.Wrapper.mailRewardDatas[(int)mailitem.mailRewardType];
                    for (int j = 0; j < rewarddata.goodskeyList.Length; j++)
                    {
                        Player.ControllerGood.Earn(rewarddata.goodskeyList[j], rewarddata.goodsAmountList[j]);
                    }
                    if(postList.Count>1)
                    {
                        var toastCanvas = ViewCanvas.Get<ViewCanvasToastMessage>();
                        string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_RewardSended].StringToLocal;
                        toastCanvas.ShowandFade(localizedvalue);

                    }
                    else
                    {
                        PopupRewardPopup(rewarddata.goodskeyList, rewarddata.goodsAmountList);
                    }
                    
                }
            }

            slotObject.gameObject.SetActive(false);

            bool allRecieve = true;
            for (int i = 0; i < mailList.Count; i++)
            {
                if (mailList[i].gameObject.activeInHierarchy)
                {
                    if (mailList[i].lockObj.activeInHierarchy == false)
                    {
                        allRecieve = false;
                        break;
                    }
                }
            }

            if (allRecieve)
            {
                Player.Option.menuRedDotCallback?.Invoke(MenuType.Mail, false);
            }
        }

        //재화 획득 팝업
        private void PopupRewardPopup(GoodsKey[] goodskeyList,int[] goodsAmountList)
        {
            var toastCanvas = ViewCanvas.Get<ViewCanvasToastMessage>();

            Dictionary<Definition.GoodsKey, double> goodsAmount = new Dictionary<Definition.GoodsKey, double>();

            for (int i = 0; i < goodskeyList.Length; i++)
            {
                if (goodsAmount.ContainsKey(goodskeyList[i]))
                {
                    goodsAmount[goodskeyList[i]] += goodsAmountList[i];
                }
                else
                {
                    goodsAmount.Add(goodskeyList[i], goodsAmountList[i]);
                }
            }

            for (int i = 0; i < toastCanvas.rewardSlotList.Count; i++)
            {
                toastCanvas.rewardSlotList[i].gameObject.SetActive(false);
            }
            int index = 0;
            foreach(var rewardData in goodsAmount)
            {
                ViewGoodRewardSlot slotObj = null;
                if (index < toastCanvas.rewardSlotList.Count)
                {
                    slotObj = toastCanvas.rewardSlotList[index];
                }
                else
                {
                    slotObj = UnityEngine.Object.Instantiate(toastCanvas.rewardSlotPrefab);
                    slotObj.transform.SetParent(toastCanvas.rewardParent, false);
                    toastCanvas.rewardSlotList.Add(slotObj);
                }
                slotObj.goodValue.text = rewardData.Value.ToNumberString();
                slotObj.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)rewardData.Key];

                slotObj.gameObject.SetActive(true);

                index++;
            }

            string localizedvalue = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_GoodsReward].StringToLocal;
            toastCanvas.titleDesc.text = localizedvalue;
            toastCanvas.RewardPopupShowandFadeAsync().Forget();
        }
    }

}
