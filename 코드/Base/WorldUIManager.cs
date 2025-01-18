using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using BlackTree.Core;
using DG.Tweening;
using BlackTree.Definition;
using DamageNumbersPro;
using BlackTree.Model;

namespace BlackTree.Bundles
{
    public class WorldUIManager : Monosingleton<WorldUIManager>
    {
        private CancellationTokenSource _cts;
        private ViewCanvasWorldUI _view;

        private Queue<ViewFont> _viewFonts = new Queue<ViewFont>();
        private Queue<ViewDropGood> _goods = new Queue<ViewDropGood>();

        [SerializeField] DamageNumber golddmp;
        [SerializeField] DamageNumber dmp;
        [SerializeField] DamageNumber cridmp;
        [SerializeField] DamageNumber superdmp;
        [SerializeField] DamageNumber megadmp;
        [SerializeField]public GameObject introBlackBG;

        Color megacolor = new Color(109f / 255f, 195f / 255f, 1, 1);
        protected override void Init()
        {
            base.Init();
        }

        public void SetUp(Transform parent, CancellationTokenSource cts)
        {
            _cts = cts;
            _view = ViewCanvas.Create<ViewCanvasWorldUI>(parent);
            _view.SetVisible(true);

            golddmp.PrewarmPool();
            dmp.PrewarmPool();
            cridmp.PrewarmPool();
            superdmp.PrewarmPool();

            StartCoroutine(AnotherDayUpdate());
        }

        WaitForSeconds wait = new WaitForSeconds(5.0f);
        IEnumerator AnotherDayUpdate()
        {
            while (true)
            {
                if (Player.Option.isAnotherDay())
                {
                    Player.Option.AnotherDaySetting?.Invoke();
                    Player.Option.SetLoginedDay();
                }

                yield return wait;
            }
        }
     

        public void InstatiateFont(Vector3 positionV, double doubleValue, bool isCri,bool isSuper, Color _color, float fontSize = 34)
        {
            if (Model.Player.Cloud.optiondata.appearDmg == false)
                return;

            if(isCri)
            {
                cridmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), _color);
            }
            else if(isSuper)
            {
                superdmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), _color);
            }
            else
            {
                dmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), _color);
            }
            
        }


        public void InstatiateCriticalFont(Vector3 positionV, double doubleValue,CriticalType criType, Color _color, float fontSize = 34)
        {
            if (Model.Player.Cloud.optiondata.appearDmg == false)
                return;
            
            switch (criType)
            {
                case CriticalType.None:
                    dmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), _color);
                    break;
                case CriticalType.Cri:
                    cridmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), _color);
                    break;
                case CriticalType.Super:
                    superdmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), _color);
                    break;
                case CriticalType.Mega:
                    megadmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), megacolor);
                    break;
                default:
                    break;
            }

        }

        public void InstatiateGoldFont(Vector3 positionV, double doubleValue, bool isCri, Color _color, float fontSize = 34)
        {
            if (Model.Player.Cloud.optiondata.appearDmg == false)
                return;
           var golddn= golddmp.Spawn(positionV + (Vector3.up * 1.5f), doubleValue.ToNumberString(), _color);

        }

        private ViewFont GetViewFont()
        {
            return _viewFonts.Count > 0 ? _viewFonts.Dequeue() : ViewBase.Create<ViewFont>(InGameObjectManager.Instance.transform);
        }

        private async UniTaskVoid FontAnimation(ViewFont font)
        {
            font.RectTr.DOKill();
            font.Text.DOKill();

            //font.RectTr.DOScale(0.65f, 0.2f);
            font.RectTr.DOLocalMoveY(4 + font.RectTr.localPosition.y, 0.5f);
            await UniTask.Delay(300, false, PlayerLoopTiming.Update, _cts.Token);
            font.Text.DOFade(0, 0.5f)
                .OnComplete(() =>
                {
                    _viewFonts.Enqueue(font);
                    font.gameObject.SetActive(false);
                });
        }

        public ViewDropGood DropGoodEvent(GoodsKey goodkey,int count,Vector2 start,Vector2 end)
        {
            ViewDropGood viewDropGoods;
            Sprite sprite;

            if (_goods.Count == 0)
                viewDropGoods = ViewBase.Create<ViewDropGood>(InGameObjectManager.Instance.transform);
            else
                viewDropGoods = _goods.Dequeue();

           sprite = GoodResourcesBundle.Loaded.goodSprites[(int)goodkey];

            var startpos = InGameObjectManager.Instance.ingamecamera.WorldToScreenPoint(start);
            //Vector2 endpos = new Vector2(10, 2130);
            Vector2 endpos = InGameObjectManager.Instance.ingamecamera.ScreenToWorldPoint(new Vector2(0,0));
            viewDropGoods
                .SetIcon(sprite)
                .SetVisible(true)
                .SetPosition(start, endpos)
                .Drop(_cts).Forget();

            return viewDropGoods;
        }
    }

}
