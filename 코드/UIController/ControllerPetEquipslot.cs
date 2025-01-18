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
    public class ControllerPetEquipslot 
    {
        public Player.Pet.PetCacheData _petCache;
        public readonly ViewEquipPetSlot _viewslot;

        CancellationTokenSource _cts;

        ContentState contentState;
        int slotIndex;

        public ControllerPetEquipslot(int _petID,ViewEquipPetSlot view,int equipslotIndex,CancellationTokenSource cts)
        {
            _cts = cts;
            _viewslot = view;

            slotIndex = equipslotIndex;

            _viewslot.petInfoBtn.onClick.AddListener(()=> { 
                if(Player.Pet.currentEquipwaitPetID!=-1)
                {
                    ChangeForSelectedPet(Player.Pet.currentEquipwaitPetID);
                }
                else
                {
                    if(_petCache!=null)
                    {
                        Player.Pet.SlotTouched?.Invoke(_petCache.tabledata.index);
                    }
                    else
                    {
                        ContentState lockState = Player.Option.IsPetSlotUnlocked(slotIndex);
                        if (lockState == ContentState.Locked)
                        {
                            LocalizeDescKeys _desc = LocalizeDescKeys.Etc_UnlockAfterLevel;
                            var lockData = StaticData.Wrapper.petLockTabledata[slotIndex];
                            string localizedvalue = string.Format(StaticData.Wrapper.localizeddesclist[(int)_desc].StringToLocal, lockData.unlockLevel);
                            ViewCanvas.Get<ViewCanvasToastMessage>().ShowandFade(localizedvalue);
                        }
                    }

                }
            });

            ContentState lockState = Player.Option.IsPetSlotUnlocked(slotIndex);
            _petCache= Player.Pet.Get(_petID);
            _viewslot.Init(_petCache, lockState);

            Model.Player.Option.ContentUnlockUpdate += SlotUnlockUpdate;

        }

        void SlotUnlockUpdate()
        {
            ContentState lockState = Player.Option.IsPetSlotUnlocked(slotIndex);
            _viewslot.SyncInfo(_petCache, lockState);
        }

        void ChangeForSelectedPet(int _petID)
        {
            Player.Pet.UnEquip(_petCache.tabledata.index);

            Player.Pet.Equip(_petID);

            _petCache= Player.Pet.Get(_petID);

            ContentState lockState = Player.Option.IsPetSlotUnlocked(slotIndex);
            _viewslot.SyncInfo(_petCache, lockState);

            Player.Pet.equipslotEnduceOff?.Invoke();

            Player.Pet.currentEquipwaitPetID= -1;
        }

        public void SyncViewer(int _petID)
        {
            //if (_petCache == null)
            //{
            //    _petCache = Player.Pet.Get(_petID);
            //}
            _petCache = Player.Pet.Get(_petID);
            //if (_petCache.tabledata.index != _petID)
            //    return;

            ContentState lockState = Player.Option.IsPetSlotUnlocked(slotIndex);

            _viewslot.SyncInfo(_petCache, lockState);
        }

    }

}
