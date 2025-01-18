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
    public class ControllerSkillSlot
    {
        public readonly Player.Skill.SkillCacheData _skillCache;
        public readonly ViewSkillSlot _viewslot;

        private readonly ViewCanvasSkillInventory _skillInventory;

  
        public ControllerSkillSlot(SkillKey key)
        {
            _skillCache = Player.Skill.Get(key);

            _skillInventory = ViewCanvas.Get<ViewCanvasSkillInventory>();
            _viewslot = ViewBase.Create<ViewSkillSlot>(_skillInventory.scrollRect.content);

            _viewslot.Init(_skillCache);

            _viewslot.skillInfoBtn.onClick.AddListener(()=> {
                Player.Skill.SlotTouched?.Invoke(_skillCache.tabledataSkill.skillKey);
                
            });

            Player.Skill.onUpdateSync += SyncViewer;

            SyncViewer(key);
        }

        public void SyncViewer(SkillKey key)
        {
            if (_skillCache.tabledataSkill.skillKey != key)
                return;

            _viewslot.SyncInfo(_skillCache);
        }

        
    }
}
