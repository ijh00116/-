using UnityEngine;
using BlackTree.Definition;
using Cysharp.Threading.Tasks;
using BlackTree.Bundles;
using BlackTree.Model;

namespace BlackTree.Core
{
    public class ControllerUnitUpgrade
    {
        public const int _index = 0;
        private readonly ViewCanvasUnitUpgrade _viewCanvasUnitUpgrade;
        ViewCanvasGoldUpgrade goldWindow;
        ViewCanvasStatusUpgrade statusUpgradeWindow;
        ViewCanvasAwakeUpgrade awakeWindow;

        ViewCanvas currentView;
        public ControllerUnitUpgrade(Transform parent)
        {
            _viewCanvasUnitUpgrade = ViewCanvas.Create<ViewCanvasUnitUpgrade>(parent);

            goldWindow = ViewCanvas.Get<ViewCanvasGoldUpgrade>();
            statusUpgradeWindow = ViewCanvas.Get<ViewCanvasStatusUpgrade>();
            awakeWindow = ViewCanvas.Get<ViewCanvasAwakeUpgrade>();

            _viewCanvasUnitUpgrade.GoldupBtn.onClick.AddListener(() => {
                Player.Guide.StartTutorial(TutorialType.GoldUpgrade);
                Player.Guide.QuestGuideProgress(QuestGuideType.GoldUpgrade);
                VisibleGoldUpWindow();
            });
            _viewCanvasUnitUpgrade.StatUpBtn.onClick.AddListener(() => {
                Player.Guide.StartTutorial(TutorialType.StatusUpgrade);
                Player.Guide.QuestGuideProgress(QuestGuideType.StatUpgrade);
                VisibleStatUpWindow();
            });
            _viewCanvasUnitUpgrade.awakeUpBtn.onClick.AddListener(() => {
                Player.Guide.StartTutorial(TutorialType.AwakeUpgrade);
                VisibleAwakeUpWindow();
            });

            for(int i=0; i< _viewCanvasUnitUpgrade.closeButton.Length; i++)
            {
                _viewCanvasUnitUpgrade.closeButton[i].onClick.AddListener(()=>MainNav.CloseMainUIWindow());
            }

            MainNav.onChange += UpdateViewVisible;

            _viewCanvasUnitUpgrade.goldRedDot.SetActive(false);
            _viewCanvasUnitUpgrade.statusRedDot.SetActive(false);
            _viewCanvasUnitUpgrade.awakeRedDot.SetActive(false);
            Main().Forget();
        }

        async UniTaskVoid Main()
        {
            while (true)
            {
                _viewCanvasUnitUpgrade.goldRedDot.SetActive(Model.Player.GoldUpgrade.canUpgrade);
                _viewCanvasUnitUpgrade.statusRedDot.SetActive(Model.Player.StatusUpgrade.canUpgrade);
                _viewCanvasUnitUpgrade.awakeRedDot.SetActive(Model.Player.AwakeUpgrade.canUpgrade);

                await UniTask.DelayFrame(60);
            }
        }

        private void UpdateViewVisible()
        {
            if (MainNav.SelectTabIndex == _index)
            {
                if(_viewCanvasUnitUpgrade.IsVisible)
                {
                    currentView.Wrapped.CommonPopupCloseAnimationDown();
                    _viewCanvasUnitUpgrade.blackBG.PopupCloseColorFade();
                    _viewCanvasUnitUpgrade.Wrapped.CommonPopupCloseAnimationDown(() => {
                        currentView.SetVisible(false);
                        _viewCanvasUnitUpgrade.SetVisible(false);
                    });
                }
                else
                {
                    _viewCanvasUnitUpgrade.SetVisible(true);
                    if (currentView == null)
                    {
                        VisibleGoldUpWindow();
                    }
                    currentView.SetVisible(true);

                    goldWindow.Wrapped.CommonPopupOpenAnimationUp();
                    statusUpgradeWindow.Wrapped.CommonPopupOpenAnimationUp();
                    awakeWindow.Wrapped.CommonPopupOpenAnimationUp();
                    _viewCanvasUnitUpgrade.Wrapped.CommonPopupOpenAnimationUp(()=> {
                        if(Player.Guide.currentTutorial==TutorialType.StatusUpgrade|| Player.Guide.currentTutorial==TutorialType.GoldUpgrade ||
                        Player.Guide.currentTutorial == TutorialType.AwakeUpgrade)
                        {
                            Player.Guide.StartTutorial(Player.Guide.currentTutorial);
                        }
                        if (Player.Guide.currentGuideQuest == QuestGuideType.GoldUpgrade|| Player.Guide.currentGuideQuest == QuestGuideType.StatUpgrade)
                        {
                            Player.Guide.QuestGuideProgress(Player.Guide.currentGuideQuest);
                        }
                    });
                    _viewCanvasUnitUpgrade.blackBG.PopupOpenColorFade();
                }
            }
            else
            {   _viewCanvasUnitUpgrade.blackBG.PopupCloseColorFade();
                goldWindow.Wrapped.CommonPopupCloseAnimationDown();
                statusUpgradeWindow.Wrapped.CommonPopupCloseAnimationDown();
                awakeWindow.Wrapped.CommonPopupCloseAnimationDown();
                _viewCanvasUnitUpgrade.blackBG.PopupCloseColorFade();
                _viewCanvasUnitUpgrade.Wrapped.CommonPopupCloseAnimationDown(()=>{
                    _viewCanvasUnitUpgrade.SetVisible(false);
                    goldWindow.SetVisible(false);
                    statusUpgradeWindow.SetVisible(false);
                    awakeWindow.SetVisible(false);
                });
            }
        }

        public void VisibleGoldUpWindow()
        {
         
            _viewCanvasUnitUpgrade.goldSelector.Show(0);
            _viewCanvasUnitUpgrade.awakeSelector.Show(1);
            _viewCanvasUnitUpgrade.statSelector.Show(1);
            
            statusUpgradeWindow.SetVisible(false);
            awakeWindow.SetVisible(false);

            currentView = goldWindow;
            goldWindow.SetVisible(true);
        }

        public void VisibleStatUpWindow()
        {
            _viewCanvasUnitUpgrade.awakeSelector.Show(1);
            _viewCanvasUnitUpgrade.statSelector.Show(0);
            _viewCanvasUnitUpgrade.goldSelector.Show(1);

            goldWindow.SetVisible(false);
            awakeWindow.SetVisible(false);

            currentView = statusUpgradeWindow;
            statusUpgradeWindow.SetVisible(true);

            statusUpgradeWindow.Show();
            statusUpgradeWindow.scrollRects.SetContentScrollOffsetToTop();
        }

        public void VisibleAwakeUpWindow()
        {
            _viewCanvasUnitUpgrade.goldSelector.Show(1);
            _viewCanvasUnitUpgrade.statSelector.Show(1);
            _viewCanvasUnitUpgrade.awakeSelector.Show(0);

            goldWindow.SetVisible(false);
            statusUpgradeWindow.SetVisible(false);

            currentView = awakeWindow;
            awakeWindow.SetVisible(true);
        }


    }
}
