using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;
using System;

namespace BlackTree.Model
{
    public static partial class Player
    {
        public static class OfflineReward
        {
            public const int maxMinute = 480;
            public static int RewardTimeToMinute
            {
                get
                {
                    TimeSpan time = new TimeSpan(Extension.GetServerTime().Ticks- Extension.GetDateTimeByIsoString(Cloud.offlineReward.lastReceiveRewardTime).Ticks);
                    return Mathf.Clamp((int)time.TotalMinutes, 0, maxMinute);
                }
            }

            public static bool IsGetableReward => RewardTimeToMinute > 10 && !Cloud.offlineReward.lastReceiveRewardTime.Equals("");

            public static void SetLastTime()
            {
                Cloud.offlineReward.lastReceiveRewardTime = Extension.GetServerTime().ToIsoString();
                Cloud.offlineReward.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }
        }
    }
}