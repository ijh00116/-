using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerRuneEquipSlot
    {
        public Player.Rune.RuneCacheData _runeCache;
        public readonly ViewRuneEquipSlot _viewslot;

        CancellationTokenSource _cts;

        ContentState contentState;
        int slotIndex;

        public ControllerRuneEquipSlot(int _ID, ViewRuneEquipSlot view, int equipslotIndex, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewslot = view;

            slotIndex = equipslotIndex;

            _viewslot.runeInfoBtn.onClick.AddListener(() => {
                if (Player.Rune.currentEquipwaitRuneID != -1)
                {
                    ChangeForSelectedRune(Player.Rune.currentEquipwaitRuneID);
                }
                else
                {
                    if (_runeCache != null)
                    {
                        Player.Rune.SlotTouched?.Invoke(_runeCache.tabledata.index);
                    }
                    else
                    {
                        ContentState lockState = Player.Option.IsRuneSlotUnlocked(slotIndex);
                        if(lockState==ContentState.Locked)
                        {
                            LocalizeDescKeys _desc = LocalizeDescKeys.Etc_UnlockAfterLevel;
                            var lockData = StaticData.Wrapper.runeLockTabledata[slotIndex];
                            string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)_desc].StringToLocal, lockData.unlockLevel);
                            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                        }
                    }

                }
            });

            ContentState lockState = Player.Option.IsRuneSlotUnlocked(slotIndex);
            _runeCache = Player.Rune.Get(_ID);
            _viewslot.Init(_runeCache, lockState);

            Model.Player.Option.ContentUnlockUpdate += SlotUnlockUpdate;

        }

        void SlotUnlockUpdate()
        {
            ContentState lockState = Player.Option.IsRuneSlotUnlocked(slotIndex);
            _viewslot.SyncInfo(_runeCache, lockState);
        }

        void ChangeForSelectedRune(int _ID)
        {
            Player.Rune.UnEquip(_runeCache.tabledata.index);

            Player.Rune.Equip(_ID);

            _runeCache = Player.Rune.Get(_ID);

            ContentState lockState = Player.Option.IsRuneSlotUnlocked(slotIndex);
            _viewslot.SyncInfo(_runeCache, lockState);

            Player.Rune.equipslotEnduceOff?.Invoke();

            Player.Rune.currentEquipwaitRuneID = -1;
        }

        public void SyncViewer(int _ID)
        {
            if (_runeCache == null)
            {
                _runeCache = Player.Rune.Get(_ID);
            }
            if (_runeCache == null)
                return;

            if (_runeCache.tabledata.index != _ID)
                return;

            ContentState lockState = Player.Option.IsRuneSlotUnlocked(slotIndex);

            _viewslot.SyncInfo(_runeCache, lockState);
        }
    }

}
