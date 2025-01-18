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
    public class ControllerSkillSlotEquiped
    {
        public Player.Skill.SkillCacheData _skillCache;
        public readonly ViewSkillEquipslot _viewslot;

        CancellationTokenSource _cts;

        ContentState contentState;
        int slotIndex;
        public ControllerSkillSlotEquiped(SkillKey key, ViewSkillEquipslot view,int equipslotIndex,CancellationTokenSource cts)
        {
            _cts = cts;
            _viewslot = view;

            Main().Forget();
            slotIndex = equipslotIndex;

            _viewslot.skillInfoBtn.onClick.AddListener(() => {

                ContentState lockState = Player.Option.IsSkillSlotUnlocked(slotIndex);
                if(lockState==ContentState.Locked)
                {
                    string warningmsg= Player.Option.SkillUnlockMessage(slotIndex);
                    var tabledata = StaticData.Wrapper.skillLockTabledata[slotIndex];
                    var toastmsg = ViewCanvas.Get<ViewCanvasToastMessage>();
                    toastmsg.ShowandFade(warningmsg);
                }
                else
                {
                    if (Player.Skill.currentEquipwaitSkill != SkillKey.None)
                    {
                        Player.Skill.Switch(_skillCache.tabledataSkill.skillKey);
                    }
                    else
                    {
                        UseSkill();
                    }
                }
            });

            ContentState lockState = Player.Option.IsSkillSlotUnlocked(slotIndex);
            _skillCache = Player.Skill.Get(key);
            _viewslot.Init(_skillCache, lockState);

            Model.Player.Option.ContentUnlockUpdate += SlotUnlockUpdate;

            Player.Skill.UpdateSkillCacheDataInEquipedSlot += ChangeForSelectedSkill;
        }

        void SlotUnlockUpdate()
        {
            ContentState lockState = Player.Option.IsSkillSlotUnlocked(slotIndex);
            _viewslot.SyncInfo(_skillCache, lockState);
        }

        void UseSkill()
        {
            if (Battle.Field.IsFightScene==false )
                return;
            ContentState lockState = Player.Option.IsSkillSlotUnlocked(slotIndex);
            if(lockState==ContentState.Locked)
            {
                //Debug.Log("잠금해제 곧 예정");
                return;
            }
            else
            {
                if(_skillCache==null)
                {
                    //Debug.Log("스킬 추가하세용");
                    if(Battle.Field.currentSceneState==eSceneState.MainIdle)
                    {
                        MainNav.SetTabIndex(ControllerSkillInventory._index);
                    }
                    return;
                }
            }
            if (_skillCache.tabledataSkill.skillType == SkillType.Passive)
                return;

            if (_skillCache.IsEquiped && Player.Unit.userUnit._state.stop == false)
            {
                if (_skillCache.tabledataSkill.skillType == SkillType.Passive)
                    return;
                if (_skillCache.leftCooltime > 0)
                {

                }
                else
                {
                    if (_skillCache.waitForuseSkill == false)
                    {
                        if (_skillCache.tabledataSkill.skillKey == Definition.SkillKey.SwordExplode || _skillCache.tabledataSkill.skillKey == Definition.SkillKey.MultipleFireball)
                        {
                            if (Player.Unit.userUnit._state.IsCurrentState(eActorState.Attack))
                            {
                                _skillCache.waitForuseSkill = true;
                                Player.Skill.RegisterSkillInput((int)_skillCache.tabledataSkill.skillKey);
                            }
                        }
                        else
                        {
                            _skillCache.waitForuseSkill = true;
                            Player.Skill.RegisterSkillInput((int)_skillCache.tabledataSkill.skillKey);
                        }

                    }
                }
            }
        }

        void ChangeForSelectedSkill(SkillKey before,SkillKey after)
        {
            if (_skillCache == null)
                return;
            if(_skillCache.tabledataSkill.skillKey!=before)
            {
                return;
            }
            _skillCache = Player.Skill.Get(after);

            ContentState lockState = Player.Option.IsSkillSlotUnlocked(slotIndex);
            _viewslot.SyncInfo(_skillCache, lockState);
        }

        public void SyncViewer(SkillKey key)
        {
           // if(_skillCache==null)
            {
                _skillCache = Player.Skill.Get(key);
            }
            ContentState lockState = Player.Option.IsSkillSlotUnlocked(slotIndex);
            if(key==SkillKey.None)
            {
                _viewslot.SyncInfo(_skillCache, lockState);
                return;
            }
            if (_skillCache == null)
            {
                return;
            }
            if (_skillCache.tabledataSkill.skillKey != key)
                return;

           

            _viewslot.SyncInfo(_skillCache, lockState);
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                OnUpdate();
                await UniTask.Yield(_cts.Token);
            }
        }

        void OnUpdate()
        {
            if (_skillCache == null)
                return;

            if(_skillCache.leftCooltime>0)
            {
                if (_skillCache.IsEquiped)
                {
                    var maxcooltime = StaticData.Wrapper.skillDatas[(int)_skillCache.tabledataSkill.skillKey].coolTime;
                    _viewslot.cooltimeImage.gameObject.SetActive(true);
                    _viewslot.cooltimeImage.fillAmount = (float)(_skillCache.leftCooltime / maxcooltime);
                    _viewslot.leftTimeText.gameObject.SetActive(true);
                    _viewslot.leftTimeText.text = string.Format("{0:F1}", _skillCache.leftCooltime);
                }
            }
            else
            {
                if (_skillCache.IsEquiped)
                {
                    _viewslot.leftTimeText.gameObject.SetActive(false);
                    _viewslot.cooltimeImage.gameObject.SetActive(false);
                    _viewslot.cooltimeImage.fillAmount = 0;
                }
            }
        }
    }

}
