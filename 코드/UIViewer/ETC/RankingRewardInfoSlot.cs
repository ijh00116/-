using BlackTree.Core;
using BlackTree.Definition;
using BlackTree.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class RankingRewardInfoSlot : MonoBehaviour
    {
        public TMP_Text Place;
        public Transform rewardSlotParent;
        public ViewGoodRewardSlot rewardSlotPrefab;

        List<ViewGoodRewardSlot> rewardslotList = new List<ViewGoodRewardSlot>();
        public void Init(int place,MailRewardType rewardType)
        {
            int min = StaticData.Wrapper.rankRewardRange[place].min;
            int max= StaticData.Wrapper.rankRewardRange[place].max;

            string first= string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RankCount].StringToLocal, min);
            string second= string.Format(StaticData.Wrapper.localizednamelist[(int)LocalizeNameKeys.Etc_RankCount].StringToLocal, max);
            Place.text = string.Format(first+"-"+second);

            var mailGoodsList = StaticData.Wrapper.mailRewardDatas[(int)rewardType].goodskeyList;
            for(int i=0; i< rewardslotList.Count; i++)
            {
                rewardslotList[i].gameObject.SetActive(false);
            }
            for (int i=0; i< mailGoodsList.Length; i++ )
            {
                int index = i;
                ViewGoodRewardSlot obj = null;
                if (index>=rewardslotList.Count)
                {
                    obj = Instantiate(rewardSlotPrefab);
                    obj.transform.SetParent(rewardSlotParent, false);
                    rewardslotList.Add(obj);
                }
                else
                {
                    obj = rewardslotList[index];
                }
                obj.gameObject.SetActive(true);
                obj.goodsIcon.sprite = GoodResourcesBundle.Loaded.goodSprites[(int)mailGoodsList[index]];
                obj.goodValue.text = StaticData.Wrapper.mailRewardDatas[(int)rewardType].goodsAmountList[index].ToString();
                obj.goodsDesc.text = "";
            }
        }
    }
}
