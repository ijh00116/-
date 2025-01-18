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
    public class ControllerPetSlot
    {
        public readonly Player.Pet.PetCacheData _petCache;
        public readonly ViewPetSlot _viewSlot;

        private readonly ViewCanvasPetInventory _petInventory;

        public ControllerPetSlot(int _petID)
        {
            _petCache = Player.Pet.Get(_petID);

            _petInventory = ViewCanvas.Get<ViewCanvasPetInventory>();
            _viewSlot = ViewBase.Create<ViewPetSlot>(_petInventory.scrollRect.content);

            _viewSlot.Init(_petCache);
            _viewSlot.petInfoBtn.onClick.AddListener(() =>
            {
                Player.Pet.SlotTouched?.Invoke(_petCache.tabledata.index);
            });

            Player.Pet.onUpdateSync += SyncViewer;

            SyncViewer(_petID);
        }

        public void SyncViewer(int _petID)
        {
            if (_petCache.tabledata.index != _petID)
                return;

            _viewSlot.SyncInfo(_petCache);
        }
    }
}

