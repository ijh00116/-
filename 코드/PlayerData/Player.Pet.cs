using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using UnityEngine.Events;
using System;
using BlackTree.Core;
using BlackTree.Bundles;

namespace BlackTree.Model
{
    [System.Serializable]
    public class PetSpriteInfo
    {
        public Sprite slotIconsprite;
        public AnimSpriteInfo[] animInfo;
    }
    public static partial class Player
    {
       
        public class Pet
        {
            public class PetCacheData
            {
                public readonly PetTableData tabledata;
                public readonly UserPetInfo userPetdata;

                public Dictionary<EquipAbilityKey, double> _equipabilitycaches;

                public bool IsEquiped => currentEquipContainsPet(tabledata.index);
                public bool IsUnlocked => userPetdata.Obtaincount > 0 || userPetdata.level > 0;

                public bool IsSkillUnlocked => userPetdata.unlockSkillLevel > 0;
                public int EquipedIndex = -1;


                public float atkRate;

                public Dictionary<PetSkillAbilityKey, double> _passiveSkillAbilitycaches;
                public PetCacheData(PetTableData _tabledata, UserPetInfo userCloudData)
                {
                    tabledata = _tabledata;
                    userPetdata = userCloudData;
                 
                    _equipabilitycaches = new Dictionary<EquipAbilityKey, double>();

                    for (int i = 0; i < tabledata.equipabilitylist.Length; i++)
                    {
                        _equipabilitycaches.Add(tabledata.equipabilitylist[i], 0);
                    }

                    _passiveSkillAbilitycaches = new Dictionary<PetSkillAbilityKey, double>();
                    _passiveSkillAbilitycaches.Add(tabledata.passiveSkillAbil, 0);

                    SyncAbilityValue();
                }

                public double GetSkillValue()
                {
                    double dmg=0;
                    dmg = (atkRate * 0.01f) * Player.Unit.SwordAtk;
                    dmg = dmg * (Player.Unit.IncreasePetAtk);
                    dmg = dmg * (tabledata.startValue + tabledata.deltaValue * userPetdata.level) * 0.01f;
                    return dmg;
                }
                public double GetSkillPureValue()
                {
                    double _value = (tabledata.startValue + tabledata.deltaValue * userPetdata.level);
                    return _value;
                }

                public void SetPassiveAbil(double skillValue)
                {
                    _passiveSkillAbilitycaches[tabledata.passiveSkillAbil] = skillValue;
                    Player.Pet.UpdatePassiveAbility();
                }

                public float elapsedCooltime
                {
                    get
                    {
                        return userPetdata.elapsedCooltime;
                    }
                    set
                    {
                        userPetdata.elapsedCooltime = value;
                    }
                }

                public float leftCooltime
                {
                    get
                    {
                        float lefttime = 0;
                        lefttime =tabledata.skillCoolTime- elapsedCooltime;
                        return lefttime;
                    }
                }
                public bool IsMaxLevel()
                {
                    return userPetdata.level >= tabledata.maxLevel;
                }

                public void SyncAbilityValue()
                {
                    //for (int i = 0; i < tabledata.equipabilitylist.Length; i++)
                    //{
                    //    if (_equipabilitycaches.ContainsKey(tabledata.equipabilitylist[i]))
                    //    {
                    //        _equipabilitycaches[tabledata.equipabilitylist[i]] = 0;
                    //    }
                    //}
                    for (int i = 0; i < tabledata.equipabilitylist.Length; i++)
                    {
                        if (_equipabilitycaches.ContainsKey(tabledata.equipabilitylist[i]))
                        {
                            double abilValue = tabledata.equipabilitystartvalue[i] + tabledata.equipabilitydeltavalue[i] * userPetdata.level;

                            _equipabilitycaches[tabledata.equipabilitylist[i]] = abilValue;
                        }
                    }

                    atkRate= tabledata.attackStartValue+ tabledata.attackDeltaValue * userPetdata.level;

                }
            }

            public static Dictionary<int, PetCacheData> petCaches = new Dictionary<int, PetCacheData>();

            public static Dictionary<EquipAbilityKey, double> _equipAbilitycaches; //key: 장비 토탈 능력치, value: 장비 능력치 값
            

            public static System.Action<int> onAfterEquip;
            public static System.Action<int> onAfterUnEquip;
            public static System.Action<int> onUpdateSync;

            public static UnityAction<int> SlotTouched;

            //deck
            public static int CurrentPetInvenindex = 0;

            //pet장착
            public static int currentEquipwaitPetID;
            public static UnityAction<int> equipslotEnduceForchange;
            public static UnityAction equipslotEnduceOff;

            public const int PetSlotLength = 6;
            public const int EnforceMaxLv = 49;

            public const int petAmountForLvup = 5;

            public static Dictionary<int, int> petTotalLevel = new Dictionary<int, int>();

            public static System.Action changePetPresetUpdate;

            //pet skill
            public static HashSet<int> _inputSkillSlotIndexes = new HashSet<int>();
            public static List<PetSkillKey> playingskillList;
            public static Dictionary<SkillAbilityKey, double> _activeSkillAbilitycaches;
            public static Dictionary<PetSkillAbilityKey, double> _passiveSkillAbilitycaches;

            public static System.Action<PetSkillKey, bool> SkillActivate;
            public static void Init()
            {
                CurrentPetInvenindex = Cloud.userPetEquipinfo.currentEquipPetDeckIndex;

                for (int i = 0; i < Constraints.petEquipPresetCount; i++)
                {
                    if (Cloud.userPetEquipinfo.deckList.Count <= i)
                    {
                        EquipedPetList petEquipSet = new EquipedPetList();
                        Cloud.userPetEquipinfo.deckList.Add(petEquipSet);
                    }
                }

                for (int i = Cloud.petdata.collection.Count; i < StaticData.Wrapper.petdatas.Length; i++)
                {
                    Cloud.petdata.collection.Add(new UserPetInfo());
                }

                for (int i = Cloud.petdata.petUnlockState.Count; i < StaticData.Wrapper.petLockTabledata.Length; i++)
                {
                    Cloud.petdata.petUnlockState.Add(false);
                }

                for (int i = 0; i < Cloud.petdata.collection.Count; i++)
                {
                    int index = i;
                    var data = StaticData.Wrapper.petdatas[i];
                    petCaches.Add(index, new PetCacheData(data, Cloud.petdata.collection[i]));
                }

                int invenIndex = 0;
                foreach (var _petdata in Cloud.userPetEquipinfo.deckList)
                {
                    for (int i = 0; i < Constraints.petEquipmax; i++)
                    {
                        int id = 0;
                        if (i >= _petdata.petIDlist.Count)
                        {
                            _petdata.petIDlist.Add(-1);
                            id = -1;
                        }
                        else
                        {
                            id = _petdata.petIDlist[i];
                        }
                        if (invenIndex == Cloud.userPetEquipinfo.currentEquipPetDeckIndex)
                        {
                            if (id!=-1)
                            {
                                Get(id).EquipedIndex = i;
                            }
                        }
                    }
                    invenIndex++;
                }

                _equipAbilitycaches = new Dictionary<EquipAbilityKey, double>();
                for (int i = 0; i < (int)EquipAbilityKey.END; i++)
                {
                    _equipAbilitycaches.Add((EquipAbilityKey)i, 0);
                }
              
                var currentEquipPetList = currentPetContainer();
                foreach (var _equipedID in currentEquipPetList)
                {
                    if (_equipedID == -1)
                        continue;
                    var equipPet = Get(_equipedID);
                    for (int i = 0; i < equipPet.tabledata.equipabilitylist.Length; i++)
                    {
                        if(equipPet.IsEquiped)
                        {
                            var abiltype = equipPet.tabledata.equipabilitylist[i];
                            _equipAbilitycaches[abiltype] += equipPet._equipabilitycaches[abiltype];
                        }
                        
                    }
                }

                for (int i = 0; i < StaticData.Wrapper.petdatas.Length; i++)
                {
                    if (petTotalLevel.ContainsKey(StaticData.Wrapper.petdatas[i].grade) == false)
                    {
                        petTotalLevel.Add(StaticData.Wrapper.petdatas[i].grade, 0);
                    }
                }

                playingskillList = new List<PetSkillKey>();
                _activeSkillAbilitycaches = new Dictionary<SkillAbilityKey, double>();
                _passiveSkillAbilitycaches = new Dictionary<PetSkillAbilityKey, double>();
                SetPetPassiveAbility();
                //TotalLevelCalculate();
            }

            public static void SetPetPassiveAbility()
            {
                for (int i = 0; i < (int)PetSkillAbilityKey.End; i++)
                {
                    if (_passiveSkillAbilitycaches.ContainsKey((PetSkillAbilityKey)i))
                    {
                        _passiveSkillAbilitycaches[(PetSkillAbilityKey)i] = 0;
                    }
                    else
                    {
                        _passiveSkillAbilitycaches.Add((PetSkillAbilityKey)i, 0);
                    }
                }
            }

            public static void UpdatePassiveAbility()
            {
                for (int i = 0; i < (int)PetSkillAbilityKey.End; i++)
                {
                    if(_passiveSkillAbilitycaches.ContainsKey((PetSkillAbilityKey)i))
                    {
                        _passiveSkillAbilitycaches[(PetSkillAbilityKey)i] = 0;
                    }
                }


                for (int i = 0; i < StaticData.Wrapper.petdatas.Length; i++)
                {
                    if (StaticData.Wrapper.petdatas[i].isActive == false)
                    {
                        var petCache = Get(StaticData.Wrapper.petdatas[i].index);

                        _passiveSkillAbilitycaches[petCache.tabledata.passiveSkillAbil] += petCache._passiveSkillAbilitycaches[petCache.tabledata.passiveSkillAbil];
                    }
                }
            }
            public static void SkillActiveUpdate(PetSkillKey _key, bool use = true)
            {
                if (use)
                {
                    playingskillList.Add(_key);
                }
                else
                {
                    playingskillList.Remove(_key);
                }
                SkillValueUpdate(_key);
            }
            static void SkillValueUpdate(PetSkillKey _key)
            {

            }

            public static bool IsSkillActive(PetSkillKey _key)
            {
                return playingskillList.Contains(_key);
            }

            public static void RegisterSkillInput(int index)
            {
                if (!_inputSkillSlotIndexes.Contains(index))
                {
                    _inputSkillSlotIndexes.Add(index);
                }
            }
            public static HashSet<int> GetAllRegisterSkillInput()
            {
                return _inputSkillSlotIndexes;
            }

            public static void RemoveSkillInput(int key)
            {
                _inputSkillSlotIndexes.Remove(key);
            }







            public static void TotalLevelCalculate()
            {
                for (int i = 0; i < StaticData.Wrapper.petdatas.Length; i++)
                {
                    if (petTotalLevel.ContainsKey(StaticData.Wrapper.petdatas[i].grade) == false)
                    {
                        petTotalLevel.Add(StaticData.Wrapper.petdatas[i].grade, 0);
                    }
                    else
                    {
                        petTotalLevel[StaticData.Wrapper.petdatas[i].grade] = 0;
                    }
                }
                foreach (var _obtainpet in petCaches)
                {
                    var item = _obtainpet.Value;
                    if (item.IsUnlocked)
                    {
                        if (item.userPetdata.level > 0)
                        {
                            petTotalLevel[item.tabledata.grade] += item.userPetdata.level;
                        }
                    }
                }

                Player.Rune.SyncAllData();
            }

            public static List<int> currentPetContainer()
            {
                return Cloud.userPetEquipinfo.deckList[Cloud.userPetEquipinfo.currentEquipPetDeckIndex].petIDlist;
            }

            public static bool currentEquipContainsPet(int id)
            {
                return currentPetContainer().Contains(id);
            }

            public static PetCacheData Get(int id)
            {
                if (id == -1)
                    return null;
                return petCaches[id];
            }

            public static void Obtain(int _petID,int count)
            {
                var cache = Get(_petID);
                var obtainCount = cache.userPetdata.Obtaincount;

                cache.userPetdata.Obtaincount += count;

                Cloud.petdata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                SyncData(_petID);

                onUpdateSync?.Invoke(_petID);
            }

            public static void Equip(int _petID)
            {
                if (Player.Pet.Get(_petID).IsEquiped)
                    return;
                int index = 0;
                bool empty = false;
                foreach (var _slot in currentPetContainer())
                {
                    if (_slot == -1)
                    {
                        empty = true;
                        break;
                    }
                    index++;
                }

                if (HasEmptySlot() == false)
                {
                    //Debug.Log("<color=red>빈 스킬 슬롯 없음</color>");
                    currentEquipwaitPetID = _petID;
                    equipslotEnduceForchange?.Invoke(index);
                    return;
                }

                var petdata = Get(_petID);

                if (index < Constraints.petEquipmax && empty)
                {
                    var currentskilllist = currentPetContainer();
                    currentskilllist[index] = _petID;
                    petdata.EquipedIndex = index;
                    onAfterEquip?.Invoke(_petID);
                }
                else
                {
                    currentEquipwaitPetID = _petID;
                    equipslotEnduceForchange?.Invoke(index);
                }

                SyncData(_petID);

                Player.Quest.TryCountUp(QuestType.EquipPet, 1);

                onUpdateSync(_petID);

                Cloud.petdata.UpdateHash().SetDirty(true);
                Cloud.userPetEquipinfo.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            public static void UnEquip(int _petID)
            {
                var petdata = Get(_petID);

                int index = 0;
                foreach (var _slot in currentPetContainer())
                {
                    if (_slot == _petID)
                    {
                        break;
                    }
                    index++;
                }
                petdata.EquipedIndex = -1;
                Cloud.userPetEquipinfo.deckList[Cloud.userPetEquipinfo.currentEquipPetDeckIndex].petIDlist[index] = -1;

                SkillActivate?.Invoke(petdata.tabledata.petskillKey, false);
             
                Cloud.userPetEquipinfo.UpdateHash().SetDirty(true);
                Cloud.petdata.UpdateHash().SetDirty(true);

                onUpdateSync(_petID);
            }

            public static void Enforce(int _petID)
            {
                var petdata = Get(_petID);

                int amountForLvUP = StaticData.Wrapper.petAmountTableData[petdata.userPetdata.level].amountForLvUp;

                petdata.userPetdata.Obtaincount -= amountForLvUP;
                petdata.userPetdata.level++;

              

       
            }

            public static void Disassemble(int _petID)
            {
                var petdata = Get(_petID);

                int eachRewardAmount = StaticData.Wrapper.petDisassembleDatas[petdata.tabledata.grade - 1].rewardAmount;
                int amount = petdata.userPetdata.Obtaincount;

                double awakeAmount = eachRewardAmount * amount;

                Player.ControllerGood.Earn(GoodsKey.AwakeStone, awakeAmount);
                petdata.userPetdata.Obtaincount = 0;

                Cloud.petdata.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();

                SyncData(_petID);

                onUpdateSync?.Invoke(_petID);
            }

            //public static void Combine(int _petID,int _nexPetID)
            //{
            //    var current = Get(_petID);

            //    if (current.userPetdata.level< EnforceMaxLv)
            //    {
            //        //Debug.Log("50레벨까지 강화하여야 합성이 가능합니다");
            //        return;
            //    }
            //    if (current.userPetdata.Obtaincount < 5)
            //    {
            //        //Debug.Log("개수가 5보다 작으므로 땡");
            //        return;
            //    }

            //    var next = Get(_nexPetID);

            //    if (current.userPetdata.Obtaincount >= 5)
            //    {
            //        current.userPetdata.Obtaincount -= 5;
            //        next.userPetdata.Obtaincount++;
            //    }

            //    onUpdateSync?.Invoke(_petID);
            //    onUpdateSync?.Invoke(_nexPetID);

            //    Cloud.petdata.UpdateHash().SetDirty(true);
            //    LocalSaveLoader.SaveUserCloudData();
            //}

            public static bool HasEmptySlot()
            {
                var container = currentPetContainer();
                int index = 0;

                bool Empty = false;
                for (int i = 0; i < container.Count; i++)
                {
                    index = i;
                    var isunlock = Player.Option.IsPetSlotUnlocked(index);
                    bool isEmpty = container[i] == -1;
                    if (isunlock == ContentState.UnLocked && isEmpty)
                    {
                        Empty = true;
                        break;
                    }
                }
                return Empty;
            }

            public static void SyncTotalView()
            {
                foreach(var _petData in petCaches)
                {
                    onUpdateSync(_petData.Key);
                }
            }

            public static void SyncData(int index)
            {
                Get(index).SyncAbilityValue();
               
                SyncPetAbility();
            }

            public static void SyncPetAbility()
            {
                for (int i = 0; i < (int)EquipAbilityKey.END; i++)
                {
                    _equipAbilitycaches[(EquipAbilityKey)i] = 0;
                }
              

                var currentEquipPetList = currentPetContainer();
                foreach (var _equipedID in currentEquipPetList)
                {
                    if (_equipedID == -1)
                        continue;
                    var equipPet = Get(_equipedID);
                    for (int i = 0; i < equipPet.tabledata.equipabilitylist.Length; i++)
                    {
                        if(equipPet.IsEquiped)
                        {
                            var abiltype = equipPet.tabledata.equipabilitylist[i];
                            _equipAbilitycaches[abiltype] += equipPet._equipabilitycaches[abiltype];
                        }
                        
                    }
                }

                Player.Unit.StatusSync();
            }

            static float[] randomHeroChance = new float[] { 0.5f, 0.3f, 0.15f, 0.05f };

            static int GetindexForChance(int arraylength)
            {
                float max = 0;
                for (int i = 0; i < arraylength; i++)
                {
                    max += randomHeroChance[i];
                }
                float x = UnityEngine.Random.Range(0f, max);

                float totalvalue = 0;
                int index = -1;
                for (int i = 0; i < randomHeroChance.Length; i++)
                {
                    totalvalue += randomHeroChance[i];
                    if (x < totalvalue)
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
            public static int GetRandomindex()
            {
                int index = 0;
                int randomGrade = 0;
                int[] options;
                int itemindex = -1;
                int itemgradeIndex = 0;

                index = Cloud.petdata.summonLevel - 1;
                randomGrade = StaticData.Wrapper.dataChance[index].GetindexForChance();
                
                options = StaticData.Wrapper.petdatas.Where(o => o.grade == randomGrade + 1).Select(o => o.index).ToArray();

                Array.Sort(options);
                itemgradeIndex = GetindexForChance(options.Length);
                itemindex = options[itemgradeIndex];

                return itemindex;
            }

            public static void UpdateExp(int exp)
            {
                if (Cloud.petdata.summonLevel >= StaticData.Wrapper.dataChance.Length)
                {
                    return;
                }
                Cloud.petdata.currentSummonExp += exp;
                var maxexp = StaticData.Wrapper.dataMaxExp[Cloud.petdata.summonLevel - 1].maxExp;
                if (Cloud.petdata.currentSummonExp >= maxexp)
                {
                    if (Cloud.petdata.summonLevel <= StaticData.Wrapper.dataMaxExp.Length)
                    {
                        Cloud.petdata.summonLevel++;
                        Cloud.petdata.currentSummonExp = Cloud.petdata.currentSummonExp - maxexp;
                    }
                }

                if(Cloud.petdata.summonLevel>= StaticData.Wrapper.dataChance.Length)
                {
                    Cloud.petdata.currentSummonExp = 0;
                }
                Cloud.petdata.UpdateHash().SetDirty(true);
            }

            public static void ChangeCurrentPetPreset(int index)
            {
                if (index >= Constraints.petEquipPresetCount)
                    return;

                Cloud.userPetEquipinfo.currentEquipPetDeckIndex= index;

                for (int i = 0; i < (int)PetSkillKey.End; i++)
                {
                    Player.Pet.SkillActivate?.Invoke((PetSkillKey)i, false);
                }

                changePetPresetUpdate?.Invoke();

                SyncPetAbility();
            }
        }
    }
}