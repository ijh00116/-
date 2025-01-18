using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerEquipInventory
    {
        private ViewCanvasEquipInventory _viewCanvasEquipInventory;
        private CancellationTokenSource _cts;

        public const int _index =1;

        Dictionary<EquipType, List<ViewEquipSlot>> itemviewerList = new Dictionary<EquipType,List<ViewEquipSlot>>();

        
        public ControllerEquipInventory(Transform parent,CancellationTokenSource cts)
        {
            _cts = cts;

            _viewCanvasEquipInventory = ViewCanvas.Create<ViewCanvasEquipInventory>(parent);
            _viewCanvasEquipInventory.BindOnChangeVisible(OnViewVisible);

            _viewCanvasEquipInventory.openSwordwindowBtn.onClick.AddListener(TouchWeaponBtn);
            _viewCanvasEquipInventory.openBowwindowBtn.onClick.AddListener(TouchBowBtn);
            _viewCanvasEquipInventory.openArmorwindowBtn.onClick.AddListener(TouchArmorBtn);

            _viewCanvasEquipInventory.Init();
            Init();

            Player.EquipItem.onUpdateSync += OnItemViewSync;
            MainNav.onChange += UpdateViewVisible;

            Player.EquipItem.TotalLevelCalculate();
        }

        void TouchWeaponBtn()
        {
            Player.Equip.currentselectEquipType = Definition.EquipType.Weapon;
            Player.Equip.currentSelectIdx = Player.EquipItem.currentEquipIndex[EquipType.Weapon];
           _viewCanvasEquipInventory.ActiveOffScrollRect();
            _viewCanvasEquipInventory.buttonSelector.Show(0);
            _viewCanvasEquipInventory.weaponscrollRect.gameObject.SetActive(true);

            var _itemdata = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
            _viewCanvasEquipInventory.DetailInfoUpdate(_itemdata);
        }

        void TouchBowBtn()
        {
            Player.Equip.currentselectEquipType = Definition.EquipType.Bow;
            Player.Equip.currentSelectIdx = Player.EquipItem.currentEquipIndex[EquipType.Bow];
            _viewCanvasEquipInventory.ActiveOffScrollRect();
            _viewCanvasEquipInventory.buttonSelector.Show(1);
            _viewCanvasEquipInventory.bowscrollRect.gameObject.SetActive(true);

            var _itemdata = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
            _viewCanvasEquipInventory.DetailInfoUpdate(_itemdata);
        }

        void TouchArmorBtn()
        {
            Player.Equip.currentselectEquipType = Definition.EquipType.Armor;
            Player.Equip.currentSelectIdx = Player.EquipItem.currentEquipIndex[EquipType.Armor];
            _viewCanvasEquipInventory.ActiveOffScrollRect();
            _viewCanvasEquipInventory.buttonSelector.Show(2);
            _viewCanvasEquipInventory.armorcrollRect.gameObject.SetActive(true);

            var _itemdata = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
            _viewCanvasEquipInventory.DetailInfoUpdate(_itemdata);
        }
     
        void OnViewVisible(bool active)
        {
            if(active)
            {
                var _itemdata = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
                _viewCanvasEquipInventory.DetailInfoUpdate(_itemdata);
            }
        }

        void Init()
        {
            bool isRedDot = false;
            bool isWeaponRedDot = false;
            bool isArmorRedDot = false;
            bool isStaffRedDot = false;

            foreach (var dataWeapon in StaticData.Wrapper.weapondatas)
            {
                var weaponslot = ViewBase.Create<ViewEquipSlot>(_viewCanvasEquipInventory.weaponscrollRect.content);
                weaponslot.gameObject.SetActive(true);

                var weapondata = Player.EquipItem.Get(EquipType.Weapon ,dataWeapon.index);

                weaponslot.SetupstaticData(weapondata)
                    .SetHideImage(weapondata.IsUnlocked)
                    .SetLevel(weapondata.userEquipdata.Obtainlv)
                    .SetMaxLevel(weapondata.tabledata.maxLevel)
                    .SetObtainCount(weapondata)
                    .SetEquip(weapondata.IsEquiped)
                    .SetGrade(weapondata.tabledata.grade)                   
                    .AddListenerSeeDetail(()=> _viewCanvasEquipInventory.DetailInfoUpdate(weapondata));

                if (weapondata.IsMaxLevel())
                {
                    if (weapondata.userEquipdata.Obtaincount >=Player.Equip.CountForCombine)
                    {
                        isRedDot = true;
                        isWeaponRedDot = true;
                    }
                }
                else
                {
                    if (weapondata.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[weapondata.userEquipdata.Obtainlv].amountForLvUp)
                    {
                        isRedDot = true;
                        isWeaponRedDot = true;
                    }
                }
                
                if(weapondata.tabledata.index>Player.Cloud.weapondata.currentEquipIndex && weapondata.userEquipdata.Obtaincount>0)
                {
                    isRedDot = true;
                    isWeaponRedDot = true;
                }

                if (itemviewerList.ContainsKey(EquipType.Weapon)==false)
                {
                    List<ViewEquipSlot> temp = new List<ViewEquipSlot>();
                    temp.Add(weaponslot);
                    itemviewerList.Add(EquipType.Weapon, temp);
                }
                else
                {
                    itemviewerList[EquipType.Weapon].Add(weaponslot);
                }
            }
            _viewCanvasEquipInventory.swordReddot.SetActive(isWeaponRedDot);


            foreach (var data in StaticData.Wrapper.armordatas)
            {
                var Armorslot = ViewBase.Create<ViewEquipSlot>(_viewCanvasEquipInventory.armorcrollRect.content);
                Armorslot.gameObject.SetActive(true);


                var Armordata = Player.EquipItem.Get( EquipType.Armor,data.index);

                Armorslot.SetupstaticData(Armordata)
                    .SetHideImage(Armordata.IsUnlocked)
                    .SetLevel(Armordata.userEquipdata.Obtainlv)
                    .SetMaxLevel(Armordata.tabledata.maxLevel)
                    .SetObtainCount(Armordata)
                    .SetEquip(Armordata.IsEquiped)
                    .SetGrade(Armordata.tabledata.grade)
                    .AddListenerSeeDetail(() => _viewCanvasEquipInventory.DetailInfoUpdate(Armordata));

                if (Armordata.IsMaxLevel())
                {
                    if (Armordata.userEquipdata.Obtaincount >= Player.Equip.CountForCombine)
                    {
                        isRedDot = true;
                        isArmorRedDot = true;
                    }
                }
                else
                {
                    if (Armordata.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[Armordata.userEquipdata.Obtainlv].amountForLvUp)
                    {
                        isRedDot = true;
                        isArmorRedDot = true;
                    }
                }

                
                if (Armordata.tabledata.index > Player.Cloud.armordata.currentEquipIndex && Armordata.userEquipdata.Obtaincount > 0)
                {
                    isRedDot = true;
                    isArmorRedDot = true;
                }

                if (itemviewerList.ContainsKey(EquipType.Armor)==false)
                {
                    List<ViewEquipSlot> temp = new List<ViewEquipSlot>();
                    temp.Add(Armorslot);
                    itemviewerList.Add(EquipType.Armor, temp);
                }
                else
                {
                    itemviewerList[EquipType.Armor].Add(Armorslot);
                }
            }
            _viewCanvasEquipInventory.armorReddot.SetActive(isArmorRedDot);


            foreach (var data in StaticData.Wrapper.staffdatas)
            {
                var bowslot = ViewBase.Create<ViewEquipSlot>(_viewCanvasEquipInventory.bowscrollRect.content);
                bowslot.gameObject.SetActive(true);

                var bowdata = Player.EquipItem.Get(EquipType.Bow,data.index);

                bowslot.SetupstaticData(bowdata)
                    .SetHideImage(bowdata.IsUnlocked)
                    .SetLevel(bowdata.userEquipdata.Obtainlv)
                    .SetMaxLevel(bowdata.tabledata.maxLevel)
                    .SetObtainCount(bowdata)
                    .SetEquip(bowdata.IsEquiped)
                    .SetGrade(bowdata.tabledata.grade)
                    .AddListenerSeeDetail(() => _viewCanvasEquipInventory.DetailInfoUpdate(bowdata));

                if (bowdata.IsMaxLevel())
                {
                    if (bowdata.userEquipdata.Obtaincount >= Player.Equip.CountForCombine)
                    {
                        isRedDot = true;
                        isStaffRedDot = true;
                    }
                }
                else
                {
                    if (bowdata.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[bowdata.userEquipdata.Obtainlv].amountForLvUp)
                    {
                        isRedDot = true;
                        isStaffRedDot = true;
                    }
                }
                if (bowdata.tabledata.index > Player.Cloud.staffdata.currentEquipIndex && bowdata.userEquipdata.Obtaincount > 0)
                {
                    isRedDot = true;
                    isStaffRedDot = true;
                }
                if (itemviewerList.ContainsKey(EquipType.Bow) == false)
                {
                    List<ViewEquipSlot> temp = new List<ViewEquipSlot>();
                    temp.Add(bowslot);
                    itemviewerList.Add(EquipType.Bow, temp);
                }
                else
                {
                    itemviewerList[EquipType.Bow].Add(bowslot);
                }
            }
            _viewCanvasEquipInventory.staffReddot.SetActive(isStaffRedDot);

            ///////////////////////////
            ViewCanvas.Get<ViewCanvasMainNav>().viewMainNavButtons[(int)MainNavigationType.Item].ActivateNotification(isRedDot);

            ///////////////////////////
            _viewCanvasEquipInventory.CombineBtn.onClick.AddListener(CombineToNext);
            _viewCanvasEquipInventory.allCombineBtn.onClick.AddListener(AllCombine);
            _viewCanvasEquipInventory.EnforceBtn.onClick.AddListener(EnforceCurrentEquip);
            _viewCanvasEquipInventory.EquipBtn.onClick.AddListener(EquipCurrentEquip);
            _viewCanvasEquipInventory.AwakeBtn.onClick.AddListener(AwakeCurrentEquip);

            TouchWeaponBtn();
            var _weapondata = Player.EquipItem.Get(EquipType.Weapon, Player.Equip.currentSelectIdx);
            _viewCanvasEquipInventory.DetailInfoUpdate(_weapondata);

            for(int i=0; i<_viewCanvasEquipInventory.closeBtn.Length; i++)
            {
                int index = i;
                _viewCanvasEquipInventory.closeBtn[index].onClick.AddListener(() => MainNav.CloseMainUIWindow());
            }

            _viewCanvasEquipInventory.BestEquipBtn.onClick.AddListener(BestEquip);

            _viewCanvasEquipInventory.combineDesc.text = StaticData.Wrapper.localizeddesclist[(int)LocalizeDescKeys.UI_CombineDesc].StringToLocal;
        }

        void CombineToNext()
        {
            if (Player.Equip.currentSelectIdx >= StaticData.Wrapper.weapondatas.Length - 1)
                return;
            
            Player.EquipData current=null;
            Player.EquipData next = null;

            current = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);

            var itemState = current.CurrentItemState();
            if (itemState!=ItemState.canCombine)
            {
                //Debug.Log("50레벨까지 강화하여야 합성이 가능합니다");
                return;
            }
            if(current.userEquipdata.Obtaincount< Player.Equip.CountForCombine)
            {
                //Debug.Log($"개수가 {Player.Equip.CountForCombine }보다 작으므로 땡");
                return;
            }

            next= Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx+1);

            current.userEquipdata.Obtaincount -= Player.Equip.CountForCombine;
            next.userEquipdata.Obtaincount++;

            Player.EquipItem.onUpdateSync?.Invoke(next.equipType, next.tabledata.index);
            Player.EquipItem.onUpdateSync?.Invoke(current.equipType,current.tabledata.index);
            

            LocalSaveLoader.SaveUserCloudData();
        }

        void AllCombine()
        {
            Definition.EquipTableData[] datas=null;

            switch (Player.Equip.currentselectEquipType)
            {
                case Definition.EquipType.Weapon:
                    datas = StaticData.Wrapper.weapondatas;
                    break;
                case Definition.EquipType.Armor:
                    datas = StaticData.Wrapper.armordatas;
                    break;
                case Definition.EquipType.Bow:
                    datas = StaticData.Wrapper.staffdatas;
                    break;
                default:
                    break;
            }

            foreach (var data in datas)
            {
               
                Player.EquipData current = Player.EquipItem.Get(Player.Equip.currentselectEquipType,data.index);
                var itemState = current.CurrentItemState();

                if (current.userEquipdata.Obtaincount >= Player.Equip.CountForCombine)
                {
                    switch (itemState)
                    {
                        case ItemState.normal:
                            {
                                if (current.IsUnlocked)
                                {
                                    while (true)
                                    {
                                        bool canEnforce = current.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[current.userEquipdata.Obtainlv].amountForLvUp;
                                        if (canEnforce && current.IsMaxLevel()==false)
                                            Player.EquipItem.Enforce(Player.Equip.currentselectEquipType, data.index);
                                        else
                                            break;
                                    }
                                    Player.EquipItem.UpdateAfterEnforce(Player.Equip.currentselectEquipType, data.index);
                                }
                            }
                            break;
                        case ItemState.canCombine:
                            if (data.index >= StaticData.Wrapper.weapondatas.Length - 1)
                                break;
                            Player.EquipData next = Player.EquipItem.Get(Player.Equip.currentselectEquipType, data.index + 1);
                            int combinecount = current.userEquipdata.Obtaincount / (Player.Equip.CountForCombine );
                            current.userEquipdata.Obtaincount -= combinecount * (Player.Equip.CountForCombine);
                            next.userEquipdata.Obtaincount += combinecount;

                            Player.EquipItem.onUpdateSync?.Invoke(next.equipType, next.tabledata.index);
                            Player.EquipItem.onUpdateSync?.Invoke(current.equipType, current.tabledata.index);
                            break;
                        default:
                            break;
                    }

                }
            }

            Player.EquipItem.TotalLevelCalculate();
            Player.EquipItem.onUpdateSync?.Invoke(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
            LocalSaveLoader.SaveUserCloudData();
        }

        void EnforceCurrentEquip()
        {
            var equipdata = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);

            var itemState = equipdata.CurrentItemState();
            if (itemState != ItemState.normal)
            {
                //Debug.Log("강화는불가");
                return;
            }

            bool canEnforce = equipdata.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[equipdata.userEquipdata.Obtainlv].amountForLvUp;
            if (canEnforce==false)
            {
                //Debug.Log($"개수가 {StaticData.Wrapper.itemAmountTableData[equipdata.userEquipdata.Obtainlv].amountForLvUp}보다 작으므로 땡");
                return;
            }
            if (equipdata.IsUnlocked)
            {
                Player.EquipItem.Enforce(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
                Player.EquipItem.UpdateAfterEnforce(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);

                Player.EquipItem.TotalLevelCalculate();
            }
        }

        void AwakeCurrentEquip()
        {
            //var equipdata = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);

            //var itemState = equipdata.CurrentItemState();
            //if (itemState != ItemState.havetoAwake)
            //{
            //    Debug.Log(" 현재 초월 불가");
            //    return;
            //}
            //if (equipdata.userEquipdata.Obtaincount < Player.Equip.CountForEnforce * (equipdata.userEquipdata.AwakeLv + 1))
            //{
            //    Debug.Log($"개수가 {Player.Equip.CountForEnforce * (equipdata.userEquipdata.AwakeLv + 1)}보다 작으므로 땡");
            //    return;
            //}
            //if (equipdata.IsUnlocked)
            //{
            //    Player.EquipItem.Awake(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
            //}
        }

        void EquipCurrentEquip()
        {
            var equipdata = Player.EquipItem.Get(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
            if(equipdata.IsUnlocked)
            {
                Player.EquipItem.Equip(Player.Equip.currentselectEquipType, Player.Equip.currentSelectIdx);
            }
            if (Player.Guide.currentGuideQuest == QuestGuideType.ItemEquip)
            {
                Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
            }
        }

        void OnItemViewSync(EquipType _type, int index)
        {
            for (int i = 0; i < itemviewerList[_type].Count; i++)
            {
                itemviewerList[_type][i].SetEquipMydata();
            }

            var equipslot = itemviewerList[_type][index];
            var equipdata = equipslot.equipCache;

            equipslot.SetHideImage(equipdata.IsUnlocked)
             .SetLevel(equipdata.userEquipdata.Obtainlv)
             .SetObtainCount(equipdata)
             .SetEquip(equipdata.IsEquiped)
             .SetGrade(equipdata.tabledata.grade);

            bool isRedDot = false;

            bool isWeaponRedDot = false;
            bool isArmorRedDot = false;
            bool isStaffRedDot = false;

            foreach (var data in StaticData.Wrapper.weapondatas)
            {
                var weaponData = Player.EquipItem.Get(EquipType.Weapon, data.index);

                if (weaponData.IsMaxLevel())
                {
                    if (weaponData.userEquipdata.Obtaincount >= Player.Equip.CountForCombine)
                    {
                        isRedDot = true;
                        isWeaponRedDot = true;
                    }
                }
                else
                {
                    if (weaponData.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[weaponData.userEquipdata.Obtainlv].amountForLvUp)
                    {
                        isRedDot = true;
                        isWeaponRedDot = true;
                    }
                }
              
                if (weaponData.tabledata.index > Player.Cloud.weapondata.currentEquipIndex && weaponData.userEquipdata.Obtaincount > 0)
                {
                    isRedDot = true;
                    isWeaponRedDot = true;
                }
            }
            foreach (var data in StaticData.Wrapper.staffdatas)
            {
                var staffData = Player.EquipItem.Get(EquipType.Bow, data.index);
                if (staffData.IsMaxLevel())
                {
                    if (staffData.userEquipdata.Obtaincount >= Player.Equip.CountForCombine)
                    {
                        isRedDot = true;
                        isStaffRedDot = true;
                    }
                }
                else
                {
                    if (staffData.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[staffData.userEquipdata.Obtainlv].amountForLvUp)
                    {
                        isRedDot = true;
                        isStaffRedDot = true;
                    }
                }
                if (staffData.tabledata.index > Player.Cloud.staffdata.currentEquipIndex && staffData.userEquipdata.Obtaincount > 0)
                {
                    isRedDot = true;
                    isStaffRedDot = true;
                }
            }
            foreach (var data in StaticData.Wrapper.armordatas)
            {
                var Armordata = Player.EquipItem.Get(EquipType.Armor, data.index);

                if (Armordata.IsMaxLevel())
                {
                    if (Armordata.userEquipdata.Obtaincount >= Player.Equip.CountForCombine)
                    {
                        isRedDot = true;
                        isArmorRedDot = true;
                    }
                }
                else
                {
                    if (Armordata.userEquipdata.Obtaincount >= StaticData.Wrapper.itemAmountTableData[Armordata.userEquipdata.Obtainlv].amountForLvUp)
                    {
                        isRedDot = true;
                        isArmorRedDot = true;
                    }
                }
                if (Armordata.tabledata.index > Player.Cloud.armordata.currentEquipIndex && Armordata.userEquipdata.Obtaincount > 0)
                {
                    isRedDot = true;
                    isArmorRedDot = true;
                }
            }
            _viewCanvasEquipInventory.swordReddot.SetActive(isWeaponRedDot);
            _viewCanvasEquipInventory.armorReddot.SetActive(isArmorRedDot);
            _viewCanvasEquipInventory.staffReddot.SetActive(isStaffRedDot);
            ViewCanvas.Get<ViewCanvasMainNav>().viewMainNavButtons[(int)MainNavigationType.Item].ActivateNotification(isRedDot);

            _viewCanvasEquipInventory.DetailAbilInfoUpdate(equipdata);
        }

        private void BestEquip()
        {
            bool isWeaponRedDot = false;
            bool isArmorRedDot = false;
            bool isStaffRedDot = false;

            int swordindex = 0;
            int armorindex = 0;
            int staffindex = 0;

            foreach (var data in StaticData.Wrapper.weapondatas)
            {
                var weaponData = Player.EquipItem.Get(EquipType.Weapon, data.index);
                if (weaponData.tabledata.index > Player.Cloud.weapondata.currentEquipIndex && (weaponData.userEquipdata.Obtaincount > 0|| weaponData.userEquipdata.Obtainlv > 0))
                {
                    swordindex = weaponData.tabledata.index;
                    isWeaponRedDot = true;
                }
            }
            foreach (var data in StaticData.Wrapper.staffdatas)
            {
                var staffData = Player.EquipItem.Get(EquipType.Bow, data.index);
                if (staffData.tabledata.index > Player.Cloud.staffdata.currentEquipIndex && (staffData.userEquipdata.Obtaincount > 0 || staffData.userEquipdata.Obtainlv > 0))
                {
                    staffindex = staffData.tabledata.index;
                    isStaffRedDot = true;
                }
            }
            foreach (var data in StaticData.Wrapper.armordatas)
            {
                var Armordata = Player.EquipItem.Get(EquipType.Armor, data.index);
                if (Armordata.tabledata.index > Player.Cloud.armordata.currentEquipIndex && (Armordata.userEquipdata.Obtaincount > 0 || Armordata.userEquipdata.Obtainlv > 0))
                {
                    armorindex = Armordata.tabledata.index;
                    isArmorRedDot = true;
                }
            }
            if(isWeaponRedDot)
            {
                Player.EquipItem.Equip(EquipType.Weapon, swordindex);
            }
            if (isStaffRedDot)
            {
                Player.EquipItem.Equip(EquipType.Bow, staffindex);
            }
            if (isArmorRedDot)
            {
                Player.EquipItem.Equip(EquipType.Armor, armorindex);
            }
            if (Player.Guide.currentGuideQuest == QuestGuideType.ItemEquip)
            {
                Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
            }
        }

        private void UpdateViewVisible()
        {
            if (MainNav.SelectTabIndex == _index)
            {
                if(_viewCanvasEquipInventory.IsVisible)
                {
                    _viewCanvasEquipInventory.blackBG.PopupCloseColorFade();
                    _viewCanvasEquipInventory.Wrapped.CommonPopupCloseAnimationDown(() => {
                        _viewCanvasEquipInventory.SetVisible(false);
                    });
                }
                else
                {
                    _viewCanvasEquipInventory.SetVisible(true);
                    _viewCanvasEquipInventory.weaponscrollRect.SetContentScrollOffsetToTop();

                    _viewCanvasEquipInventory.blackBG.PopupOpenColorFade();
                    _viewCanvasEquipInventory.Wrapped.CommonPopupOpenAnimationUp(()=> {

                        if (Player.Guide.currentGuideQuest == QuestGuideType.ItemEquip)
                        {
                            Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                        }
                    });

                }
            }
            else
            {
                _viewCanvasEquipInventory.blackBG.PopupCloseColorFade();
                _viewCanvasEquipInventory.Wrapped.CommonPopupCloseAnimationDown(()=> {
                    _viewCanvasEquipInventory.SetVisible(false);
                });
            }

        }
    }
}
