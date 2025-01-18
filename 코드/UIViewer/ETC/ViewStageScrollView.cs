using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackTree.Bundles;
using BlackTree.Core;
using BlackTree.Model;

namespace BlackTree
{
    public class ViewStageScrollView : UIRecycleViewController<StageInfo_scrollview>
    {
        public void LoadData()
        {
            for (int i = 0; i < 100; i++)
            {
                int chapterindex = i;

                for (int j = 0; j < Battle.Field.stagecount; j++)
                {
                    int stageindex = j;
                    var stageslotdata = new StageInfo_scrollview();
                    stageslotdata.chapterIndex = chapterindex;
                    stageslotdata.stageIndex = stageindex;

                    tableData.Add(stageslotdata);
                }
            }

            InitializeTableView();
        }

        public void SetPhaseChapter(int phaseIndex)
        {
            int startindex = phaseIndex * 100;
            int endindex = ((phaseIndex + 1) * 100);

            for (int i = startindex; i < endindex; i++)
            {
                int chapterindex = i;

                for (int j = 0; j < Battle.Field.stagecount; j++)
                {
                    int stageindex = j;
                    var stageslotdata = new StageInfo_scrollview();
                    stageslotdata.chapterIndex = chapterindex;
                    stageslotdata.stageIndex = stageindex;

                    tableData[(chapterindex-(phaseIndex*100)) * Battle.Field.stagecount + stageindex].chapterIndex= chapterindex;
                    tableData[(chapterindex - (phaseIndex * 100)) * Battle.Field.stagecount + stageindex].stageIndex = stageindex;
                }
            }

            InitializeTableView();
        }

        protected override void Start()
        {
            base.Start();

        }
    }

}
