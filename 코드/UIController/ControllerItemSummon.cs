using UnityEngine;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlackTree.Core
{
    public class ControllerItemSummon
    {
        public const int _index = 4;
        private ViewCanvasItemSummon _viewItemSummon;

        List<ViewGachaSlot> gachaslotPool = new List<ViewGachaSlot>();
        List<ViewGachaSlot> gachaslotPool_forPet = new List<ViewGachaSlot>();

        int rateTableCurrentIndex=0;

        bool isSummonEventPlaying = false;
        CancellationTokenSource summonToken;

        bool isAutoSpawn = false;
        int summonCount = 10;

        const int summonDelay = 40;
        public ControllerItemSummon(Transform parent)
        {
            _viewItemSummon = ViewCanvas.Create<ViewCanvasItemSummon>(parent);
            _viewItemSummon.Init();

            if (Player.Option.isAnotherDay())
            {
                Player.Cloud.weapondata.adsummonCount = 0;
                Player.Cloud.skilldata.adsummonCount = 0;
                Player.Cloud.petdata.adsummonCount = 0;
                Player.Cloud.runeData.adsummonCount = 0;
            }

            for (int i = 0; i < _viewItemSummon.buttonTabs.Length; ++i)
            {
                var tabIndex = i;
                _viewItemSummon.buttonTabs[i].onClick.AddListener(() =>
                {
                    _viewItemSummon.Show(tabIndex);
                    if(tabIndex==0)
                    {
                        Player.Guide.StartTutorial(TutorialType.SummonEquip);
                        Player.Guide.QuestGuideProgress(QuestGuideType.SummonEquip);
                    }
                    if (tabIndex == 1)
                    {
                        Player.Guide.StartTutorial(TutorialType.SummonSkill);
                        Player.Guide.QuestGuideProgress(QuestGuideType.SummonSkill);
                    }
                    if (tabIndex == 2)
                    {
                        Player.Guide.StartTutorial(TutorialType.SummonPet);
                        Player.Guide.QuestGuideProgress(QuestGuideType.SummonPet);
                    }

                });
            }

            _viewItemSummon.BindOnChangeVisible((o)=> {
                _viewItemSummon.GachaResultPopup.SetActive(false);
            });

            //weapon
            _viewItemSummon.weaponSummonslots.adButton.onClick.AddListener(() =>
            {
                if(Player.Cloud.weapondata.adsummonCount< Player.Summon.adSummonMaxCount)
                {
                    if (Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        SummonItem(20, true);
                    }
                    else
                    {
                        AdmobManager.Instance.ShowRewardedAd(() => { SummonItem(20, true); });
                    }
                }
            });
            _viewItemSummon.weaponSummonslots.summon_10.onClick.AddListener(() => {
                SummonItem(10);
            });
            _viewItemSummon.weaponSummonslots.summon_100.onClick.AddListener(() => {
                SummonItem(100);
            });

            //Skill
            _viewItemSummon.skillSummonslots.adButton.onClick.AddListener(()=> {
                if (Player.Cloud.skilldata.adsummonCount < Player.Summon.adSummonMaxCount)
                {
                    if (Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        SummonSkill(20, true);
                    }
                    else
                    {
                        AdmobManager.Instance.ShowRewardedAd(() => { SummonSkill(20, true); });
                    }
                }
                
            });
            _viewItemSummon.skillSummonslots.summon_10.onClick.AddListener(() => {
                SummonSkill(10);
            });
            _viewItemSummon.skillSummonslots.summon_100.onClick.AddListener(() => {
                SummonSkill(100);
            });

            //pet
            _viewItemSummon.petSummonslots.adButton.onClick.AddListener(() => {
                if (Player.Cloud.petdata.adsummonCount < Player.Summon.adSummonMaxCount)
                {
                    if (Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        SummonPet(20, true);
                    }
                    else
                    {
                        AdmobManager.Instance.ShowRewardedAd(() => { SummonPet(20, true); });
                    }
                }
            
            });
            _viewItemSummon.petSummonslots.summon_10.onClick.AddListener(() => {
                SummonPet(10);
            });
            _viewItemSummon.petSummonslots.summon_100.onClick.AddListener(() => {
                SummonPet(100);
            });

            _viewItemSummon.runeSummonslots.adButton.onClick.AddListener(() => {
                if (Player.Cloud.runeData.adsummonCount < Player.Summon.adSummonMaxCount)
                {
                    if (Player.Cloud.inAppPurchase.purchaseAds)
                    {
                        SummonRune(20, true);
                    }
                    else
                    {
                        AdmobManager.Instance.ShowRewardedAd(() => { SummonRune(20, true); });
                    }
                }

            });
            _viewItemSummon.runeSummonslots.summon_10.onClick.AddListener(() => {
                SummonRune(10);
            });
            _viewItemSummon.runeSummonslots.summon_100.onClick.AddListener(() => {
                SummonRune(100);
            });

            _viewItemSummon.summon10BtnInGacharesult.onClick.AddListener(()=> {
   
                if (isSummonEventPlaying == false)
                {
                    Player.Summon.summonInGachaResult?.Invoke(10,false);
                }
                else
                {
                    SummonItemEventSkip();
                }
                    
            });
            _viewItemSummon.summon100BtnInGacharesult.onClick.AddListener(()=> {
         
                if (isSummonEventPlaying == false)
                {
                    Player.Summon.summonInGachaResult?.Invoke(100, false);
                }
                else
                {
                    SummonItemEventSkip();
                }
            });

            for(int i=0; i < _viewItemSummon.closeGachaBtn.Length; i++)
            {
                int index = i;
                _viewItemSummon.closeGachaBtn[i].onClick.AddListener(()=> {
                    if(isAutoSpawn)
                    {
                        _viewItemSummon.autoSpawnCheckObj.SetActive(false);
                        _viewItemSummon.summon10BtnInGacharesult.enabled = true;
                        _viewItemSummon.summon100BtnInGacharesult.enabled = true;
                        isAutoSpawn = false;
                        _viewItemSummon.autoSpawnToggle.isOn = false;
                        SummonItemEventSkip();
                    }
                    else
                    {
                        if (isSummonEventPlaying == false)
                        {
                            _viewItemSummon.GachaResultPopup.SetActive(false);
                        }
                        else
                        {
                            SummonItemEventSkip();
                        }
                    }
                   
                });
            }

            rateTableCurrentIndex = 0;
            _viewItemSummon.rateTableOpenBtn.onClick.AddListener(OpenRateTableWindow);
            _viewItemSummon.rateTableOffBtn.onClick.AddListener(() => { _viewItemSummon.rateTableWindow.SetActive(false); });
            _viewItemSummon.nextSummonLevelBtn.onClick.AddListener(()=> {
                rateTableCurrentIndex++;
                if(rateTableCurrentIndex>= StaticData.Wrapper.dataChance.Length)
                {
                    rateTableCurrentIndex = StaticData.Wrapper.dataChance.Length - 1;
                }
                SetRateTableInfo(rateTableCurrentIndex);
            });

            _viewItemSummon.prevSummonLevelBtn.onClick.AddListener(() => {
                rateTableCurrentIndex--;
                if (rateTableCurrentIndex <0)
                {
                    rateTableCurrentIndex = 0;
                }
                SetRateTableInfo(rateTableCurrentIndex);
            });

            MainNav.onChange += UpdateViewVisible;

            for (int i = 0; i < _viewItemSummon.closeBtn.Length; i++)
            {
                int index = i;
                _viewItemSummon.closeBtn[index].onClick.AddListener(() => MainNav.CloseMainUIWindow());
            }

            _viewItemSummon.GachaResultPopup.SetActive(false);

            Player.Option.ContentUnlockUpdate += LockUpdate;
            LockUpdate();

            isAutoSpawn = false;

            _viewItemSummon.autoSpawnToggle.onValueChanged.AddListener((isOn)=> {
                _viewItemSummon.autoSpawnCheckObj.SetActive(isOn);
                isAutoSpawn = isOn;
                if(isOn)
                {
                    _viewItemSummon.summon10BtnInGacharesult.enabled=true;
                    _viewItemSummon.summon100BtnInGacharesult.enabled = true;
                }
            });

            Player.Option.AnotherDaySetting += AnotherDaySetting;

#if UNITY_EDITOR
            spawn().Forget();
#endif
        }

        async UniTaskVoid spawn()
        {
            while (true)
            {
                if(BTETC.eang)
                {
                    _viewItemSummon.summon100BtnInGacharesult.onClick.Invoke();
                }
                await UniTask.Delay(10);
            }
        }

        void AnotherDaySetting()
        {
            Player.Cloud.weapondata.adsummonCount = 0;
            Player.Cloud.skilldata.adsummonCount = 0;
            Player.Cloud.petdata.adsummonCount = 0;
            Player.Cloud.runeData.adsummonCount = 0;

            _viewItemSummon.SyncItemLevel();
        }

        void LockUpdate()
        {
            int summonPetLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.PetSummonUnlock].unLockLevel;
            int summonSkillLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.SkillSummonUnlock].unLockLevel;
            int summonRuneLv = StaticData.Wrapper.ingameLockData[(int)LockedUIType.RuneSummonUnlock].unLockLevel;
            if (Player.Quest.mainQuestCurrentId >= summonPetLv)
            {
                _viewItemSummon.petLocked.gameObject.SetActive(false);
            }
            else
            {
                _viewItemSummon.petLocked.gameObject.SetActive(true);
            }

            if (Player.Quest.mainQuestCurrentId >= summonSkillLv)
            {
                _viewItemSummon.skillLocked.gameObject.SetActive(false);
            }
            else
            {
                _viewItemSummon.skillLocked.gameObject.SetActive(true);
            }

            if (Player.Cloud.userLevelData.currentLevel >= summonRuneLv)
            {
                _viewItemSummon.runeLocked.gameObject.SetActive(false);
            }
            else
            {
                _viewItemSummon.runeLocked.gameObject.SetActive(true);
            }
        }

        void OpenRateTableWindow()
        {
            rateTableCurrentIndex = 0;
            if(Player.Summon.currentPopupIndex==0)
            {
                rateTableCurrentIndex = Player.Cloud.weapondata.summonLevel - 1;
            }
            else if (Player.Summon.currentPopupIndex == 1)
            {
                rateTableCurrentIndex = Player.Cloud.skilldata.summonLevel - 1;
            }
            else if (Player.Summon.currentPopupIndex == 2)
            {
                rateTableCurrentIndex = Player.Cloud.petdata.summonLevel - 1;
            }
            else
            {
                rateTableCurrentIndex = Player.Cloud.runeData.summonLevel - 1;
            }
            SetRateTableInfo(rateTableCurrentIndex);
        }

        void SetRateTableInfo(int summonLevel)
        {
            _viewItemSummon.rateTableWindow.SetActive(true);
            string leveltxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Level].StringToLocal;
            _viewItemSummon.currentSummonLevelInTablewindow.text = string.Format("{0}"+ leveltxt, summonLevel+1);

            for(int i=0; i< _viewItemSummon.ratetableList.Length; i++)
            {
                double rate = 0;
                if (Player.Summon.currentPopupIndex == 0 || Player.Summon.currentPopupIndex == 2)
                {
                    rate = StaticData.Wrapper.dataChance[summonLevel].randomHeroChance[i];
                }
                else if (Player.Summon.currentPopupIndex == 1)
                {
                    rate = StaticData.Wrapper.skilldataChance[summonLevel].randomHeroChance[i];
                }
                else
                {
                    rate = StaticData.Wrapper.runedataChance[summonLevel].randomHeroChance[i];
                }
                string gradeTxt = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_Grade].StringToLocal;
                _viewItemSummon.ratetableList[i].gradeText.text = string.Format("{0}"+ gradeTxt, i + 1);
                _viewItemSummon.ratetableList[i].rateText.text = string.Format("{0}%", rate.ToNumberString(3));
            }
        }
        void SummonItem(int count,bool isAD=false)
        {
            if (isAutoSpawn)
            {
                _viewItemSummon.summon10BtnInGacharesult.enabled = false;
                _viewItemSummon.summon100BtnInGacharesult.enabled = false;
            }
            else
            {
                _viewItemSummon.autoSpawnToggle.gameObject.SetActive(false);
            }
            summonCount = count;
            var key = GoodsKey.Dia;

            int cost = StaticData.Wrapper.summonTableData.needDiaForItem100;
            if (count == 10)
            {
                cost = StaticData.Wrapper.summonTableData.needDiaForItem10;
            }
           
            if (isAD|| Player.ControllerGood.IsCanBuy(key, cost) ||Player.Cloud.tutorialData.isFirstSummonItems)
            {
                if(isAD==false)
                {
                    if(Player.Cloud.tutorialData.isFirstSummonItems==false)
                    {
                        Player.ControllerGood.Consume(key, cost);
                    }
                    else
                    {
                        Player.Cloud.tutorialData.isFirstSummonItems = false;
                    }
                }
                else
                {
                    Player.Cloud.weapondata.adsummonCount++;
                }
                
                isSummonEventPlaying = true;
                summonItemlist.Clear();
                for (int i = 0; i < count; i++)
                {
                    int _index = 0;
                    int randomType = UnityEngine.Random.Range(0, 3);
                    _index = Player.EquipItem.GetRandomindex((EquipType)randomType);

                    if (summonItemlist.ContainsKey((EquipType)randomType))
                    {
                        var itemindexs = summonItemlist[(EquipType)randomType];
                        if (itemindexs.ContainsKey(_index))
                        {
                            itemindexs[_index]++;
                        }
                        else
                        {
                            itemindexs.Add(_index, 1);
                        }
                    }
                    else
                    {
                        summonItemlist.Add((EquipType)randomType, new Dictionary<int, int>());
                        summonItemlist[(EquipType)randomType].Add(_index, 1);
                    }
                }
                foreach (var items in summonItemlist)
                {
                    foreach (var iteminfo in items.Value)
                    {
                        Player.EquipItem.Obtain(items.Key, iteminfo.Key, iteminfo.Value);
                    }
                }
                if(summonToken!=null)
                {
                    summonToken.Cancel();
                    summonToken = null;
                }
                
                summonToken = new CancellationTokenSource();
                RandomSummonItem(summonToken).Forget();
                Player.EquipItem.UpdateExp(count);

                Player.Summon.summonInGachaResult = null;
                Player.Summon.summonInGachaResult = SummonItem;
            
                _viewItemSummon.textIn10summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForItem10}";
                _viewItemSummon.textIn100summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForItem100}";
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DiaNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewItemSummon.SyncItemLevel();

            Player.Quest.TryCountUp(QuestType.SummonEquip, count);

            LocalSaveLoader.SaveUserCloudData();
        }

        void SummonItemEventSkip()
        {
            if (isAutoSpawn)
                return;
            summonToken.Cancel();
            int instantiateCount = 0;
            if (Player.Summon.currentPopupIndex == 0)
            {
                foreach (var items in summonItemlist)
                {
                    foreach (var iteminfo in items.Value)
                    {
                        if (instantiateCount < gachaslotPool.Count)
                        {
                            gachaslotPool[instantiateCount].gameObject.SetActive(true);
                        }
                        else
                        {
                            var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot);
                            instance.gameObject.SetActive(true);
                            instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);

                            gachaslotPool.Add(instance);
                        }
                        gachaslotPool[instantiateCount].SetData(Player.EquipItem.Get(items.Key, iteminfo.Key), iteminfo.Value);
                        instantiateCount++;
                    }
                }
            }
            else if (Player.Summon.currentPopupIndex == 1)
            {
                foreach (var index in summonSkillindexs)
                {
                    var skilldata = Player.Skill.Get((SkillKey)index.Key);
                    Sprite sp = InGameResourcesBundle.Loaded.skillIcon[index.Key];

                    if (instantiateCount < gachaslotPool.Count)
                    {
                        gachaslotPool[instantiateCount].gameObject.SetActive(true);
                        gachaslotPool[instantiateCount].SetData(sp, skilldata.tabledataSkill.grade, index.Value);
                    }
                    else
                    {
                        var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot);
                        instance.SetData(sp, skilldata.tabledataSkill.grade, index.Value);
                        instance.gameObject.SetActive(true);
                        instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);
                        gachaslotPool.Add(instance);
                    }
                    instantiateCount++;
                }
            }
            else if (Player.Summon.currentPopupIndex == 2)
            {
               
                foreach (var index in summonPetindexs)
                {
                    var petdata = Player.Pet.Get(index.Key);
                    Sprite sp = PetResourcesBundle.Loaded.petImage[index.Key].slotIconsprite;
                    if (instantiateCount < gachaslotPool_forPet.Count)
                    {
                        gachaslotPool_forPet[instantiateCount].gameObject.SetActive(true);

                        gachaslotPool_forPet[instantiateCount].SetData(sp, petdata.tabledata.grade, index.Value);
                    }
                    else
                    {
                        var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot_forPet);
                        instance.SetData(sp, petdata.tabledata.grade, index.Value);
                        instance.gameObject.SetActive(true);
                        instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);
                        gachaslotPool_forPet.Add(instance);
                    }
                    instantiateCount++;
                }
            }
            else if (Player.Summon.currentPopupIndex == 3)
            {

                foreach (var index in summonRuneindexs)
                {
                    var petdata = Player.Rune.Get(index.Key);
                    Sprite sp = PetResourcesBundle.Loaded.runeImage[index.Key];
                    if (instantiateCount < gachaslotPool.Count)
                    {
                        gachaslotPool[instantiateCount].gameObject.SetActive(true);

                        gachaslotPool[instantiateCount].SetData(sp, petdata.tabledata.grade, index.Value);
                    }
                    else
                    {
                        var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot);
                        instance.SetData(sp, petdata.tabledata.grade, index.Value);
                        instance.gameObject.SetActive(true);
                        instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);
                        gachaslotPool.Add(instance);
                    }
                    instantiateCount++;
                }
            }
            isSummonEventPlaying = false;
            _viewItemSummon.autoSpawnToggle.gameObject.SetActive(true);
            _viewItemSummon.summon10BtnInGacharesult.enabled = true;
            _viewItemSummon.summon100BtnInGacharesult.enabled = true;
        }

  
        void SummonSkill(int count, bool isAD = false)
        {
            if (isAutoSpawn)
            {
                _viewItemSummon.summon10BtnInGacharesult.enabled = false;
                _viewItemSummon.summon100BtnInGacharesult.enabled = false;
            }
            else
            {
                _viewItemSummon.autoSpawnToggle.gameObject.SetActive(false);
            }
            summonCount = count;
            var key = GoodsKey.Dia;
            int cost = StaticData.Wrapper.summonTableData.needDiaForSkill100;
            if (count == 10)
            {
                cost = StaticData.Wrapper.summonTableData.needDiaForSkill10;
            }
            bool isFreeSummon=false;
            if (isAD||Player.ControllerGood.IsCanBuy(key, cost) || Player.Cloud.tutorialData.isFirstSummonSkills)
            {
                if(isAD==false)
                {
                    if(Player.Cloud.tutorialData.isFirstSummonSkills)
                    {
                        Player.Cloud.tutorialData.isFirstSummonSkills = false;
                        isFreeSummon = true;
                    }
                    else
                    {
                        Player.ControllerGood.Consume(key, cost);
                    }
                }
                else
                {
                    Player.Cloud.skilldata.adsummonCount++;
                }
                
                isSummonEventPlaying = true;
                summonSkillindexs.Clear();

                if(isFreeSummon)
                {
                    summonSkillindexs.Add(2,1);
                    summonSkillindexs.Add(15, 1);
                    count -= 1;
                }

                var guideMissileInfo = Player.Skill.Get(SkillKey.GuidedMissile);
                int amountForLevel = 0;
                int pureAmount = 0;
                if(guideMissileInfo.userSkilldata.level>1)
                {
                    for(int i=2; i< guideMissileInfo.userSkilldata.level; i++)
                    {
                        amountForLevel += StaticData.Wrapper.skillAmountTableData[i - 2].amountForLvUp;
                    }
                    pureAmount = guideMissileInfo.userSkilldata.Obtaincount;
                }
                int guideMissiletotalAmount = pureAmount + amountForLevel;

                if(guideMissiletotalAmount < 70)
                {
                    if (count < 30)
                    {
                        int random = UnityEngine.Random.Range(4, 7);
                        summonSkillindexs.Add(13, random);
                        count -= random;
                    }
                    else
                    {
                        int random = UnityEngine.Random.Range(20,30);
                        summonSkillindexs.Add(13, random);
                        count -= random;
                    }
                }
              

                for (int i = 0; i < count; i++)
                {
                    int _index = 0;
                    _index = Player.Skill.GetRandomindex();

                    if (summonSkillindexs.ContainsKey(_index))
                    {
                        summonSkillindexs[_index]++;
                    }
                    else
                    {
                        summonSkillindexs.Add(_index, 1);
                    }
                }
                foreach (var index in summonSkillindexs)
                {
                    Player.Skill.Obtain((SkillKey)index.Key, index.Value);
                }
                if (summonToken != null)
                {
                    summonToken.Cancel();
                    summonToken = null;
                }
                summonToken = new CancellationTokenSource();
                RandomSummonSkill(summonToken).Forget();
                if (isFreeSummon)
                {
                    Player.Skill.UpdateExp(10);
                }
                Player.Skill.UpdateExp(summonCount);

                Player.Summon.summonInGachaResult = null;
                Player.Summon.summonInGachaResult = SummonSkill;

                _viewItemSummon.textIn10summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForSkill10}";
                _viewItemSummon.textIn100summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForSkill100}";

            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DiaNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewItemSummon.SyncItemLevel();

            if (isFreeSummon)
            {
                Player.Quest.TryCountUp(QuestType.SummonSkill, summonCount);
            }
            else
            {
                Player.Quest.TryCountUp(QuestType.SummonSkill, summonCount);
            }
                

            LocalSaveLoader.SaveUserCloudData();
        }

        void SummonPet(int count,bool isAD=false)
        {
            if (isAutoSpawn)
            {
                _viewItemSummon.summon10BtnInGacharesult.enabled = false;
                _viewItemSummon.summon100BtnInGacharesult.enabled = false;
            }
            else
            {
                _viewItemSummon.autoSpawnToggle.gameObject.SetActive(false);
            }

            summonCount = count;
            var key = GoodsKey.Dia;
            int cost= StaticData.Wrapper.summonTableData.needDiaForPet100;
            if (count == 10)
            {
                cost = StaticData.Wrapper.summonTableData.needDiaForPet10;
            }
           
            if (isAD || Player.ControllerGood.IsCanBuy(key, cost))
            {
                if(isAD==false)
                {
                    Player.ControllerGood.Consume(key, cost);
                }
                else
                {
                    Player.Cloud.petdata.adsummonCount++;
                }
                
                isSummonEventPlaying = true;
                summonPetindexs.Clear();
                for (int i = 0; i < count; i++)
                {
                    int _index = 0;
                    _index = Player.Pet.GetRandomindex();

                    if (summonPetindexs.ContainsKey(_index))
                    {
                        summonPetindexs[_index]++;
                    }
                    else
                    {
                        summonPetindexs.Add(_index, 1);
                    }
                }
                foreach (var index in summonPetindexs)
                {
                    var petdata = Player.Pet.Get(index.Key);
                    Player.Pet.Obtain(index.Key, index.Value);
                }
                if (summonToken != null)
                {
                    summonToken.Cancel();
                    summonToken = null;
                }
                summonToken = new CancellationTokenSource();
                RandomSummonPet(summonToken).Forget();
                Player.Pet.UpdateExp(count);

                Player.Summon.summonInGachaResult = null;
                Player.Summon.summonInGachaResult = SummonPet;

                _viewItemSummon.textIn10summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForPet10}";
                _viewItemSummon.textIn100summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForPet100}";
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DiaNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewItemSummon.SyncItemLevel();

            Player.Quest.TryCountUp(QuestType.SummonPet, count);

            LocalSaveLoader.SaveUserCloudData();
        }

        void SummonRune(int count, bool isAD = false)
        {
            if (isAutoSpawn)
            {
                _viewItemSummon.summon10BtnInGacharesult.enabled = false;
                _viewItemSummon.summon100BtnInGacharesult.enabled = false;
            }
            else
            {
                _viewItemSummon.autoSpawnToggle.gameObject.SetActive(false);
            }

            summonCount = count;
            var key = GoodsKey.Dia;
            int cost = StaticData.Wrapper.summonTableData.needDiaForRune100;
            if (count == 10)
            {
                cost = StaticData.Wrapper.summonTableData.needDiaForRune10;
            }

            if (isAD || Player.ControllerGood.IsCanBuy(key, cost))
            {
                if (isAD == false)
                {
                    Player.ControllerGood.Consume(key, cost);
                }
                else
                {
                    Player.Cloud.runeData.adsummonCount++;
                }

                isSummonEventPlaying = true;
                summonRuneindexs.Clear();
                for (int i = 0; i < count; i++)
                {
                    int _index = 0;
                    _index = Player.Rune.GetRandomindex();

                    if (summonRuneindexs.ContainsKey(_index))
                    {
                        summonRuneindexs[_index]++;
                    }
                    else
                    {
                        summonRuneindexs.Add(_index, 1);
                    }
                }
                foreach (var index in summonRuneindexs)
                {
                    Player.Rune.Obtain(index.Key, index.Value);
                }
                if (summonToken != null)
                {
                    summonToken.Cancel();
                    summonToken = null;
                }
                summonToken = new CancellationTokenSource();
                RandomSummonRune(summonToken).Forget();
                Player.Rune.UpdateExp(count);

                Player.Summon.summonInGachaResult = null;
                Player.Summon.summonInGachaResult = SummonRune;

                _viewItemSummon.textIn10summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForRune10}";
                _viewItemSummon.textIn100summonBtninResult.text = $"{StaticData.Wrapper.summonTableData.needDiaForRune100}";
            }
            else
            {
                string localizedvalue = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.Etc_DiaNotEnough].StringToLocal;
                ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                return;
            }
            _viewItemSummon.SyncItemLevel();

            LocalSaveLoader.SaveUserCloudData();
        }

        Dictionary<int, int> summonSkillindexs = new Dictionary<int, int>();
        Dictionary<EquipType, Dictionary<int, int>> summonItemlist = new Dictionary<EquipType, Dictionary<int, int>>();
        Dictionary<int, int> summonPetindexs = new Dictionary<int, int>();
        Dictionary<int, int> summonRuneindexs = new Dictionary<int, int>();
        async UniTaskVoid RandomSummonSkill(CancellationTokenSource cts)
        {
            for (int i = 0; i < gachaslotPool.Count; i++)
            {
                gachaslotPool[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < gachaslotPool_forPet.Count; i++)
            {
                gachaslotPool_forPet[i].gameObject.SetActive(false);
            }
            _viewItemSummon.GachaResultPopup.SetActive(true);

            int instantiateCount = 0;
            foreach (var index in summonSkillindexs)
            {
                var skilldata = Player.Skill.Get((SkillKey)index.Key);
                Sprite sp = InGameResourcesBundle.Loaded.skillIcon[index.Key];

                if (instantiateCount < gachaslotPool.Count)
                {
                    gachaslotPool[instantiateCount].gameObject.SetActive(true);
                    gachaslotPool[instantiateCount].SetData(sp, skilldata.tabledataSkill.grade, index.Value);
                }
                else
                {
                    var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot);
                    instance.SetData(sp, skilldata.tabledataSkill.grade, index.Value);
                    instance.gameObject.SetActive(true);
                    instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);
                    gachaslotPool.Add(instance);
                }

                if (skilldata.tabledataSkill.grade >= 4)
                {
                    AudioManager.Instance.Play(AudioSourceKey.Summon_Good);
                }
                else
                {
                    AudioManager.Instance.Play(AudioSourceKey.Summon_Normal);
                }
                instantiateCount++;

                await UniTask.Delay(summonDelay, cancellationToken: cts.Token);
            }

            isSummonEventPlaying = false;

            if(isAutoSpawn)
            {
                SummonSkill(summonCount);
            }
            _viewItemSummon.autoSpawnToggle.gameObject.SetActive(true);
            _viewItemSummon.summon10BtnInGacharesult.enabled = true;
            _viewItemSummon.summon100BtnInGacharesult.enabled = true;
        }

        async UniTaskVoid RandomSummonItem(CancellationTokenSource cts)
        {
            for (int i = 0; i < gachaslotPool.Count; i++)
            {
                gachaslotPool[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < gachaslotPool_forPet.Count; i++)
            {
                gachaslotPool_forPet[i].gameObject.SetActive(false);
            }
            _viewItemSummon.GachaResultPopup.SetActive(true);

            int instantiateCount = 0;
            foreach(var items in summonItemlist)
            {
                foreach(var iteminfo in items.Value)
                {
                    if (instantiateCount < gachaslotPool.Count)
                    {
                        gachaslotPool[instantiateCount].gameObject.SetActive(true);
                    }
                    else
                    {
                        var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot);
                        instance.gameObject.SetActive(true);
                        instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);
                        
                        gachaslotPool.Add(instance);
                    }
                    var cacheData = Player.EquipItem.Get(items.Key, iteminfo.Key);
                    gachaslotPool[instantiateCount].SetData(cacheData, iteminfo.Value);
                    if(cacheData.tabledata.grade>=4)
                    {
                        AudioManager.Instance.Play(AudioSourceKey.Summon_Good);
                    }
                    else
                    {
                        AudioManager.Instance.Play(AudioSourceKey.Summon_Normal);
                    }
                    instantiateCount++;
                    await UniTask.Delay(summonDelay, cancellationToken: cts.Token);
                }
            }
            isSummonEventPlaying = false;

            if (isAutoSpawn)
            {
                SummonItem(summonCount);
            }
            _viewItemSummon.autoSpawnToggle.gameObject.SetActive(true);
            _viewItemSummon.summon10BtnInGacharesult.enabled = true;
            _viewItemSummon.summon100BtnInGacharesult.enabled = true;
        }

        async UniTaskVoid RandomSummonPet(CancellationTokenSource cts)
        {
            for (int i = 0; i < gachaslotPool.Count; i++)
            {
                gachaslotPool[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < gachaslotPool_forPet.Count; i++)
            {
                gachaslotPool_forPet[i].gameObject.SetActive(false);
            }
            _viewItemSummon.GachaResultPopup.SetActive(true);

            int instantiateCount = 0;
            foreach (var index in summonPetindexs)
            {
                var petdata = Player.Pet.Get(index.Key);
                Sprite sp = PetResourcesBundle.Loaded.petImage[index.Key].slotIconsprite;
                if (instantiateCount < gachaslotPool_forPet.Count)
                {
                    gachaslotPool_forPet[instantiateCount].gameObject.SetActive(true);

                    gachaslotPool_forPet[instantiateCount].SetData(sp, petdata.tabledata.grade, index.Value);
                }
                else
                {
                    var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot_forPet);
                    instance.SetData(sp, petdata.tabledata.grade, index.Value);
                    instance.gameObject.SetActive(true);
                    instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);
                    gachaslotPool_forPet.Add(instance);
                }

                if (petdata.tabledata.grade >= 4)
                {
                    AudioManager.Instance.Play(AudioSourceKey.Summon_Good);
                }
                else
                {
                    AudioManager.Instance.Play(AudioSourceKey.Summon_Normal);
                }

                instantiateCount++;

                await UniTask.Delay(summonDelay, cancellationToken: cts.Token);
            }

            isSummonEventPlaying = false;

            if (isAutoSpawn)
            {
                SummonPet(summonCount);
            }
            _viewItemSummon.autoSpawnToggle.gameObject.SetActive(true);
            _viewItemSummon.summon10BtnInGacharesult.enabled = true;
            _viewItemSummon.summon100BtnInGacharesult.enabled = true;
        }

        async UniTaskVoid RandomSummonRune(CancellationTokenSource cts)
        {
            for (int i = 0; i < gachaslotPool.Count; i++)
            {
                gachaslotPool[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < gachaslotPool_forPet.Count; i++)
            {
                gachaslotPool_forPet[i].gameObject.SetActive(false);
            }
            _viewItemSummon.GachaResultPopup.SetActive(true);

            int instantiateCount = 0;
            foreach (var index in summonRuneindexs)
            {
                var runedata = Player.Rune.Get(index.Key);
                Sprite sp = PetResourcesBundle.Loaded.runeImage[index.Key];
                if (instantiateCount < gachaslotPool.Count)
                {
                    gachaslotPool[instantiateCount].gameObject.SetActive(true);

                    gachaslotPool[instantiateCount].SetData(sp, runedata.tabledata.grade, index.Value);
                }
                else
                {
                    var instance = UnityEngine.Object.Instantiate(_viewItemSummon.gachaSlot);
                    instance.SetData(sp, runedata.tabledata.grade, index.Value);
                    instance.gameObject.SetActive(true);
                    instance.transform.SetParent(_viewItemSummon.gachaContent.content, false);
                    gachaslotPool.Add(instance);
                }

                if (runedata.tabledata.grade >= 4)
                {
                    AudioManager.Instance.Play(AudioSourceKey.Summon_Good);
                }
                else
                {
                    AudioManager.Instance.Play(AudioSourceKey.Summon_Normal);
                }

                instantiateCount++;

                await UniTask.Delay(summonDelay, cancellationToken: cts.Token);
            }

            isSummonEventPlaying = false;

            if (isAutoSpawn)
            {
                SummonRune(summonCount);
            }
            _viewItemSummon.autoSpawnToggle.gameObject.SetActive(true);
            _viewItemSummon.summon10BtnInGacharesult.enabled = true;
            _viewItemSummon.summon100BtnInGacharesult.enabled = true;
        }

        private void UpdateViewVisible()
        {
            if (MainNav.SelectTabIndex == _index)
            {
                if (_viewItemSummon.IsVisible)
                {
                    _viewItemSummon.blackBG.PopupCloseColorFade();
                    _viewItemSummon.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewItemSummon.SetVisible(false);
                    });
                }
                else
                {
                    _viewItemSummon.SetVisible(true);
                    if (Player.Summon.currentPopupIndex < 0)
                    {
                        _viewItemSummon.Show(0);
                    }
                    else
                    {
                        _viewItemSummon.Show(Player.Summon.currentPopupIndex);
                    }
                    _viewItemSummon.blackBG.PopupOpenColorFade();
                    _viewItemSummon.Wrapped.CommonPopupOpenAnimationUp(()=> {
                        if (Player.Guide.currentTutorial == TutorialType.SummonEquip || Player.Guide.currentTutorial == TutorialType.SummonSkill || Player.Guide.currentTutorial == TutorialType.SummonPet)
                        {
                            Player.Guide.StartTutorial(Player.Guide.currentTutorial);
                        }
                        if (Player.Guide.currentGuideQuest == QuestGuideType.SummonEquip || Player.Guide.currentGuideQuest == QuestGuideType.SummonPet
                        || Player.Guide.currentGuideQuest == QuestGuideType.SummonSkill)
                        {
                            Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                        }
                    });
                }
            }
            else
            {
                _viewItemSummon.blackBG.PopupCloseColorFade();
                _viewItemSummon.Wrapped.CommonPopupCloseAnimationDown(() => {
                    _viewItemSummon.SetVisible(false);
                });
            }
        }
    }
}
