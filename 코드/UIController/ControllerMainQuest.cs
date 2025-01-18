using BlackTree.Bundles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using BlackTree.Model;

namespace BlackTree.Core
{
    public class ControllerMainQuest
    {
        private ViewCanvasMainQuest _viewCanvasmainQuest;
        private CancellationTokenSource _cts;

        //private ViewCanvasReview _viewCanvasReview;
        public ControllerMainQuest(Transform parent, CancellationTokenSource cts)
        {
            _viewCanvasmainQuest = ViewCanvas.Create<ViewCanvasMainQuest>(parent);

            var questData = StaticData.Wrapper.mainRepeatQuest[Player.Cloud.playingRecord.mainQuest.id];

            _viewCanvasmainQuest.Init(questData, PlayingRecordType.MainRepeat);

            _viewCanvasmainQuest.SetVisible(true);

            Model.Player.Quest.otherUIActive += OtherUIActive;
        }

        Vector2 OtherWindowPopupedPos = new Vector2(0, -93);
        Vector2 NoramlPos = new Vector2(0, -290);
        int OtherWindowPopupedOrder = 305;
        int NormalOrder = 97;

        void OtherUIActive(bool active)
        {
            if(active)
            {
                _viewCanvasmainQuest.MyCanvas.sortingOrder = OtherWindowPopupedOrder;
                _viewCanvasmainQuest.mainQuestUI.anchoredPosition= OtherWindowPopupedPos;
            }
            else
            {
                _viewCanvasmainQuest.MyCanvas.sortingOrder = NormalOrder;
                _viewCanvasmainQuest.mainQuestUI.anchoredPosition = NoramlPos;

            }
        }
    }

}
