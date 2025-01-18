using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;

namespace BlackTree.Core
{
    public class ControllerMainEquipSkill
    {
        private ViewCanvasMainEquipSkill _view;
        private CancellationTokenSource _cts;

        ControllerSkillSlotEquiped[] equipedskillSlotList;
        int currentPresetIndex;
        public ControllerMainEquipSkill(Transform parent, CancellationTokenSource cts)
        {
            currentPresetIndex = Player.Cloud.userSkillEquipinfo.currentEquipSkillIndex;
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasMainEquipSkill>(parent);
            _view.SetVisible(true);

            equipedskillSlotList = new ControllerSkillSlotEquiped[_view.equipedSlotList.Length];

            int equipslotindex = 0;
            foreach (var _key in Player.Cloud.userSkillEquipinfo.deckList[Player.Cloud.userSkillEquipinfo.currentEquipSkillIndex].skills)
            {
                equipedskillSlotList[equipslotindex] = new ControllerSkillSlotEquiped(_key, _view.equipedSlotList[equipslotindex], equipslotindex, _cts);
                equipslotindex++;
            }

            Player.Skill.onAfterEquip += AfterEquipSync;
            Player.Skill.changePresetUpdate += AllSlotUpdateSync;

            //skill view init
            int slotindex = 0;

            foreach (var _key in Player.Skill.currentSkillContainer())
            {
                var equipedslotview = equipedskillSlotList[slotindex];
                equipedslotview.SyncViewer(_key);
                slotindex++;
            }

            _view.SkillAutoBtn.onClick.AddListener(SkillAutoActive);

            _view.skillAutoOffImage.SetActive(!Model.Player.Cloud.skilldata.isAutoSkill);
            _view.skillAutoOnImage.SetActive(Model.Player.Cloud.skilldata.isAutoSkill);


            for (int i = 0; i < _view.PresetOnList.Count; i++)
            {
                _view.PresetOnList[i].SetActive(false);
            }
            _view.PresetOnList[currentPresetIndex].SetActive(true);

            _view.presetBtn.onClick.AddListener(ChangeSkillPresetIndex);

            _view.eliteAtkBuffBtn.onClick.AddListener(()=> {
                _view.SetOnDesc();
            });

            UpdateEliteBuff().Forget();
        }

        void AfterEquipSync(Definition.SkillKey skillkey)
        {
            int index = 0;
            foreach (var _key in Player.Skill.currentSkillContainer())
            {
                var equipedslotview = equipedskillSlotList[index];
                if (skillkey == _key)
                {
                    equipedslotview.SyncViewer(_key);
                }
                index++;
            }
        }

        void ChangeSkillPresetIndex()
        {
            currentPresetIndex++;
            if (currentPresetIndex >= Constraints.skillEquipPresetCount)
            {
                currentPresetIndex = 0;
            }


            Player.Skill.ChangeCurrentSkillPreset(currentPresetIndex);
        }


        async UniTaskVoid UpdateEliteBuff()
        {
            while (true)
            {
                if(Player.Unit.isEliteAtkBuff)
                {
                    _view.eliteAtkBuffObj.SetActive(true);
               
                }
                else
                {
                    _view.eliteAtkBuffObj.SetActive(false);
                }
                await UniTask.Yield(_cts.Token);
            }
        }
        void AllSlotUpdateSync()
        {
            int index = 0;
            foreach (var _key in Player.Skill.currentSkillContainer())
            {
                var equipedslotview = equipedskillSlotList[index];
                equipedslotview.SyncViewer(_key);
                index++;
            }
            currentPresetIndex = Player.Cloud.userSkillEquipinfo.currentEquipSkillIndex;

            for (int i = 0; i < _view.PresetOnList.Count; i++)
            {
                _view.PresetOnList[i].SetActive(false);
            }
            _view.PresetOnList[currentPresetIndex].SetActive(true);

        }

        void SkillAutoActive()
        {
            Player.Guide.StartTutorial(Definition.TutorialType.PushAutoSkill);

            Model.Player.Cloud.skilldata.isAutoSkill = !Model.Player.Cloud.skilldata.isAutoSkill;

            
            if (Model.Player.Cloud.skilldata.isAutoSkill)
            {
                Player.Quest.TryCountUp(QuestType.ActivateAutoSkill, 1);
            }

            _view.skillAutoOffImage.SetActive(!Model.Player.Cloud.skilldata.isAutoSkill);
            _view.skillAutoOnImage.SetActive(Model.Player.Cloud.skilldata.isAutoSkill);
        }
    }
}
