using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using BlackTree.Bundles;
using BlackTree.Model;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class ControllerAttendance
    {
        private static readonly int _index = 13;
        ViewCanvasAttendance _viewAttend;
        CancellationTokenSource _cts;

        private int possibleAttendDay;
        private int rewardedIndexOfAttendance;

        private int TotalRewardAttendanceCount =29;

        List<ViewAttendSlot> attendSlotList = new List<ViewAttendSlot>();

        public ControllerAttendance(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _viewAttend = ViewCanvas.Create<ViewCanvasAttendance>(parent);
            foreach (var button in _viewAttend.closeBtn)
            {
                button.onClick.AddListener(() =>
                {
                    CloseWindow();
                });
            }
            _viewAttend.SetLocalizeText();

            _viewAttend._title.text = StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_AttentionCheck].StringToLocal;
            Main().Forget();
        }

        void CloseWindow()
        {
            _viewAttend.blackBG.PopupCloseColorFade();
            _viewAttend.Wrapped.CommonPopupCloseAnimationUp(() => {
                _viewAttend.SetVisible(false);
            });
        }
   
        async UniTaskVoid Main()
        {
            await UniTask.Yield(_cts.Token);

            var menuview = ViewCanvas.Get<ViewCanvasMainTop>();
            menuview.attendBtn.gameObject.SetActive(false);
            
            //출석 아이콘 활성 비활성
            if(Player.Cloud.playingRecord.mainQuest.id >= Player.Quest.attendQuestIndex)
            {
                menuview.attendBtn.gameObject.SetActive(true);
            }
            //Player.Cloud.attendData.lastRewardIndex = 28;
            //Player.Cloud.attendData.lastRewardedDay = 2;

            TimeCheck();
            CreateAttendanceWindow();
            if (IsNextDay())
            {
                if (Player.Cloud.playingRecord.mainQuest.id >= Player.Quest.attendQuestIndex)
                {
                    _viewAttend.SetVisible(true);
                    _viewAttend.Wrapped.CommonPopupOpenAnimation(() => {
                        //DVA2.Bundles.MainMenu.SetRedDot?.Invoke(_index, IsNextDay());
                    });
                }
            }

            while (true)
            {
                await UniTask.Delay(10000);
                bool isNextDayUpdate = IsNextDay();
                //DVA2.Bundles.MainMenu.SetRedDot?.Invoke(_index, isNextDayUpdate);
                if (isNextDayUpdate)
                {
                    TimeCheck();
                    CreateAttendanceWindow();
                }

                if (Player.Cloud.playingRecord.mainQuest.id >= Player.Quest.attendQuestIndex)
                {
                    menuview.attendBtn.gameObject.SetActive(true);
                }
            }
        }
      
        bool TimeCheck()
        {
            possibleAttendDay =Extension.GetServerTime().DayOfYear;
            if (possibleAttendDay <= Player.Cloud.attendData.lastRewardedDay)
            {
                return false;
            }
            //처음
            if (Player.Cloud.attendData.lastRewardIndex < 0)
            {
                rewardedIndexOfAttendance = 0;
                return true;
            }
            else if (Player.Cloud.attendData.lastRewardIndex >= TotalRewardAttendanceCount)//다받음
            {
                Player.Cloud.attendData.lastRewardedDay = -1;
                Player.Cloud.attendData.lastRewardIndex = -1;
                rewardedIndexOfAttendance = 0;
                return true;
            }
            else
            {
                bool canBuy = possibleAttendDay > Player.Cloud.attendData.lastRewardedDay;
                if (canBuy)
                {
                    rewardedIndexOfAttendance = Player.Cloud.attendData.lastRewardIndex + 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        int attendPossibleSlot;
        void CreateAttendanceWindow()
        {
            for (int i = 0; i < StaticData.Wrapper.attendanceTableData.Length; i++)
            {
                if(i< attendSlotList.Count)
                {
                    Object.Destroy(attendSlotList[i].gameObject);
                }
                
            }
            attendSlotList.Clear();

            //Debug.Log("�⼮���� üũ");
            for (int i = 0; i < StaticData.Wrapper.attendanceTableData.Length; i++)
            {
                ViewAttendSlot slotObj = null;
                slotObj = UnityEngine.Object.Instantiate(_viewAttend.attendSlotPrefab);
                slotObj.transform.SetParent(_viewAttend.slotParent, false);

                int slotindex = i;
                slotObj.SetButtonData(StaticData.Wrapper.attendanceTableData[i], slotindex);

                if ((rewardedIndexOfAttendance == i) && IsNextDay())
                {
                    attendPossibleSlot = slotindex;
                    slotObj.attendTouch.onClick.AddListener(() =>
                    {
                        TouchAttend(possibleAttendDay, rewardedIndexOfAttendance, slotObj);
                    });
                }
                else
                {
                    slotObj.attendTouch.enabled = false;
                }

                attendSlotList.Add(slotObj);
            }
        }

        bool IsNextDay()
        {
            //Debug.Log("��Ʈ ����" + Player.Cloud.attendData.lastRewardedDay.ToString() + "////" + "����:" + Extension.GetServerTime().DayOfYear.ToString());
            //if (Player.Cloud.attendData.lastRewardIndex >= TotalRewardAttendanceCount)
            //{
            //    return false;
            //}
            return Player.Cloud.attendData.lastRewardedDay < Extension.GetServerTime().DayOfYear;
        }

        void TouchAttend(int day, int lastrewardedIndex, ViewAttendSlot slotObj)
        {
            Player.Cloud.attendData.lastRewardedDay = day;
            Player.Cloud.attendData.lastRewardIndex = lastrewardedIndex;

            int index = Player.Cloud.attendData.lastRewardIndex;
            var attenddata = StaticData.Wrapper.attendanceTableData[index];

            Player.ControllerGood.Earn(attenddata.goodsType, attenddata.amount);

            Player.Cloud.attendData.UpdateHash().SetDirty(true);
            LocalSaveLoader.SaveUserCloudData();

            slotObj.StartConfirmCheck(null);

        }
    }
}
