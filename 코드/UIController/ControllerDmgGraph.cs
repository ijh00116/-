using UnityEngine;
using BlackTree.Definition;
using System.Threading;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;
using System.Collections.Generic;
using System.Linq;

namespace BlackTree.Core
{
    public class ControllerDmgGraph
    {
        ViewCanvasMainIcons _viewMainIcon;
        public List<ViewDmgSlot> dmgSlotList = new List<ViewDmgSlot>();
        public List<ViewDmgSlot> sortedList = new List<ViewDmgSlot>();

        CancellationTokenSource _cts;
        public ControllerDmgGraph(Transform parent, CancellationTokenSource cts)
        {
            _viewMainIcon = ViewCanvas.Get<ViewCanvasMainIcons>();
            _viewMainIcon.refresh.onClick.AddListener(RefreshAllList);
            _cts = cts;
           // Init();
        }

        void Init()
        {
            for(int i=0; i< (int)SkillKey.End; i++)
            {
                var slotObj = Object.Instantiate(_viewMainIcon.dmgSlotPrefab);
                slotObj.transform.SetParent(_viewMainIcon.dmgGraphParent, false);
                slotObj.Init((SkillKey)i);
                dmgSlotList.Add(slotObj);
            }

            var normalslotObj = Object.Instantiate(_viewMainIcon.dmgSlotPrefab);
            normalslotObj.transform.SetParent(_viewMainIcon.dmgGraphParent, false);
            normalslotObj.Init(SkillKey.End);
            dmgSlotList.Add(normalslotObj);

            Progress().Forget();
            MainTime().Forget();
        }

        float currentTime;

        async UniTaskVoid MainTime()
        {
            while (true)
            {
                currentTime += Time.deltaTime;
                _viewMainIcon.timeText.text = string.Format("<color=yellow>{0:F0}</color>√ ", currentTime);
                await UniTask.Yield();
            }
        }

        async UniTaskVoid Progress()
        {
            while (true)
            {
                UpdateSlotList();
                await UniTask.Delay(500);
            }
        }
        void RefreshAllList()
        {
            for (int i = 0; i < dmgSlotList.Count; i++)
            {
                dmgSlotList[i].Refresh();
                dmgSlotList[i].SetCurrentDmg();
            }
            currentTime = 0;
        }

        void UpdateSlotList()
        {
            dmgSlotList = dmgSlotList.OrderByDescending(o => o.GetDmg()).ToList();
            Player.Skill.currentBestDmgIngame = dmgSlotList[0].GetDmg();

            for (int i = 0; i < dmgSlotList.Count; i++)
            {
                dmgSlotList[i].SetCurrentDmg();
            }

            for (int i = 0; i < dmgSlotList.Count; i++)
            {
                dmgSlotList[i].transform.SetAsLastSibling();
            }
        }
    }

}
