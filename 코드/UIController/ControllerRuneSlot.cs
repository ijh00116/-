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
    public class ControllerRuneSlot
    {
        public readonly Player.Rune.RuneCacheData _runeCache;
        public readonly ViewRuneSlot _viewSlot;

        private readonly ViewCanvasPetInventory _petInventory;

        public ControllerRuneSlot(int _ID)
        {
            _runeCache = Player.Rune.Get(_ID);

            _petInventory = ViewCanvas.Get<ViewCanvasPetInventory>();
            _viewSlot = Object.Instantiate(_petInventory.runeSlotPrefab);
            _viewSlot.transform.SetParent(_petInventory.scrollRect_rune.content, false);

            _viewSlot.Init(_runeCache);
            _viewSlot.runeInfoBtn.onClick.AddListener(() =>
            {
                Player.Rune.SlotTouched?.Invoke(_runeCache.tabledata.index);
            });

            Player.Rune.onUpdateSync += SyncViewer;

            SyncViewer(_ID);
        }

        public void SyncViewer(int _ID)
        {
            if (_runeCache.tabledata.index != _ID)
                return;

            _viewSlot.SyncInfo(_runeCache);
        }
    }
}
