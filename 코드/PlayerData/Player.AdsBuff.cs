using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Definition;
using BlackTree.Core;

namespace BlackTree.Model
{
    public enum AdsBuffType
    {
        AttackIncrease=0,
        ShieldInrease,
        CoinGetIncrease,
        ExpGetIncrease,

        End
    }
    public static partial class Player
    {
        public static class AdsBuff
        {
            //public static Dictionary<AdsBuffType, int> adbuffStatus = new Dictionary<AdsBuffType, int>();
            //public static Dictionary<AdsBuffType, float> adbuffTime= new Dictionary<AdsBuffType, float>();
            public const int defaultBuffValuedata = 1;
            public const int activatedBuffValuedata = 2;
            public const float startBuffTimedata = 1800;
            public static void Init()
            {
                for(int i=Cloud.adsBuffData.adsValudata.Count; i<(int)AdsBuffType.End; i++)
                {
                    Cloud.adsBuffData.adsValudata.Add(1);
                    Cloud.adsBuffData.adsLeftTimeData.Add(0);
                }

                for(int i=Cloud.adsBuffData.adsFreeBuffComplete.Count; i<(int)AdsBuffType.End; i++)
                {
                    Cloud.adsBuffData.adsFreeBuffComplete.Add(false);
                }
            }

            public static void ActivateBuff(AdsBuffType _key,bool active)
            {
                Cloud.adsBuffData.adsValudata[(int)_key]= active ? activatedBuffValuedata : defaultBuffValuedata;
                

                if(active)
                {
                    Cloud.adsBuffData.adsLeftTimeData[(int)_key] = startBuffTimedata;
                    Player.Unit.StatusSync();
                }

                Cloud.adsBuffData.UpdateHash().SetDirty(true);
                LocalSaveLoader.SaveUserCloudData();
            }

            public static int GetBuffValueData(AdsBuffType _key)
            {
                return Cloud.adsBuffData.adsValudata[(int)_key];
            }
        }
    }
}
