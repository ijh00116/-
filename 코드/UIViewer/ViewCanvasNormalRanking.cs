using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;


namespace BlackTree.Bundles
{
    public class ViewCanvasNormalRanking : ViewCanvas
    {
        [Header("Selectors")]
        public BTSelector topSelectors;
        public BTButton levelRankingBtn;
        public BTButton StageRankingBtn;
        public TMPro.TMP_Text timertxt;

        [Header("LevelRankingWindow")]
        public GameObject levelRankingWindow;
        public ScrollRect levelRankScrollview;
        public ViewNormalRankingSlot myLevelRank;

        [Header("StageRankingWindow")]
        public GameObject stageRankingWindow;
        public ScrollRect stageRankScrollview;
        public ViewNormalRankingSlot myStageRank;

        public ViewNormalRankingSlot rankslotPrefab;
        public BTButton[] closeBtn;

        [Header("rankRewardWindow")]
        public BTButton openRewardTableBtn;
        public GameObject rankingRewardWindow;
        public ScrollRect rewardParentScrollRect;
        public RankingRewardInfoSlot rankingRewardslotPrefab;
        public BTButton[] closeRewardTableBtns;

        public GameObject lockedObject;
        public TMP_Text locked_text;

        public TMP_Text titleTxt;
        public TMP_Text descTxt;
        public TMP_Text levelTxt;
        public TMP_Text levelTxt_on;
        public TMP_Text chapterTxt;
        public TMP_Text chapterTxt_on;
        public TMP_Text rewardTxt;

        public TMP_Text rewardTitleTxt;
        public TMP_Text rewardDescTxt;

    }

}
