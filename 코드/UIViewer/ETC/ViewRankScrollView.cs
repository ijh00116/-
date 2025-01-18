using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree
{
    public class ViewRankScrollView : UIRecycleViewController<Player.BackendData.BackendRankData>
    {
        public void LoadData(Player.BackendData.NormalRankingType _type)
        {
            switch (_type)
            {
                case Player.BackendData.NormalRankingType.LevelRanking:
                    tableData = Player.BackendData.levelRankList;
                    break;
                case Player.BackendData.NormalRankingType.StageRanking:
                    tableData = Player.BackendData.stageRankList;
                    break;
                case Player.BackendData.NormalRankingType.RaidRanking:
                    tableData = Player.BackendData.raidRankList;
                    break;
                default:
                    break;
            }
           
            InitializeTableView();
        }

    }

}
