using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;
using TMPro;
using UnityEngine.UI;
namespace BlackTree
{
    public class ViewNormalRankingSlot : MonoBehaviour
    {
        public TMP_Text rank;
        public TMP_Text nickName;
        public TMP_Text userLevel;
        public TMP_Text userScore;

        public GameObject[] topRankImages;
        public void UpdateContent(Player.BackendData.BackendRankData itemData)
        {
            rank.gameObject.SetActive(true);
            int rankindex = (int)itemData.rank.N;
            if (rankindex <= 3)
            {
                for (int i = 0; i < topRankImages.Length; i++)
                {
                    topRankImages[i].gameObject.SetActive(false);
                }
                topRankImages[rankindex - 1].gameObject.SetActive(true);
                rank.gameObject.SetActive(false);
            }
            else
            {
                rank.text = ((int)itemData.rank.N).ToString();
            }
            nickName.text = itemData.nickname.S;
            userLevel.text = ((int)itemData.UserLevel.N).ToString();

            int scorevalue = (int)itemData.score.N;
            if (itemData.rankType == Player.BackendData.NormalRankingType.StageRanking)
            {
                userScore.text = string.Format("{0}-{1}", (scorevalue / 100) + 1, (scorevalue % 100) + 1);
            }
            else
            {
                userScore.text = string.Format("{0}Level", ((int)itemData.score.N).ToString());
            }

        }
    }

}