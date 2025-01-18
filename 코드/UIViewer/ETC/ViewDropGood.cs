using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using BlackTree.Definition;

namespace BlackTree.Bundles
{
    public class ViewDropGood : ViewBase
    {
        [SerializeField] private Image _goodIcon;
        [SerializeField] private AnimationCurve _curve;

        [SerializeField] private float _dropSpeed;
        [SerializeField] private float _moveSpeed;

        private Vector2 _dropPosition;
        private Vector2 _endPosition;

        public ViewDropGood SetVisible(bool flag)
        {
            gameObject.SetActive(flag);
            if(flag)
            {
                _goodIcon.color = original;
            }
            return this;
        }

        public ViewDropGood SetPosition(Vector2 start, Vector2 end)
        {
            transform.position= start;
            _dropPosition = transform.position;
            _dropPosition.x = Random.Range(_dropPosition.x - 1f, _dropPosition.x + 1f);
            _dropPosition.y = Random.Range(_dropPosition.y - 0.3f, _dropPosition.y + 0.3f);
            _endPosition = end;

            return this;
        }

        public ViewDropGood SetScale(float value)
        {
            transform.localScale = Vector3.one * value;
            return this;
        }

        public ViewDropGood SetIcon(Sprite sprite)
        {
            _goodIcon.sprite = sprite;
            return this;
        }

        public async UniTask Drop(CancellationTokenSource cts)
        {
            float time = 0.0f;

            Vector2 start = transform.position;
            float heightRange = Vector2.Distance(_dropPosition, start) * Random.Range(3f, 3.5f);

            while (time < _dropSpeed)
            {
                time += Time.unscaledDeltaTime;
                float linearT = time / _dropSpeed;
                float heightT = _curve.Evaluate(linearT);
                float height = Mathf.Lerp(0.0f, heightRange, heightT);

                transform.position = Vector2.Lerp(start, _dropPosition, linearT) + new Vector2(0.0f, height);

                await UniTask.Yield(cts.Token);
            }

            await UniTask.Delay(500);
            await GoGoods(cts);

            SetVisible(false);
        }

        public async UniTask DropRound(float range, CancellationTokenSource cts)
        {
            Vector2 endPosition = (Vector2)transform.position + (Random.insideUnitCircle * range);

            float time = 0.0f;
            while (Vector2.Distance(transform.position, endPosition) >= 0.05f)
            {
                time += Time.deltaTime;
                transform.position = Vector2.Lerp(transform.position, endPosition, _moveSpeed * Time.unscaledDeltaTime);

                await UniTask.Yield(cts.Token);
            }
            await UniTask.Delay(500, true, PlayerLoopTiming.Update, cts.Token);
            await GoGoods(cts);

            SetVisible(false);
        }
        Color original = new Color(1, 1, 1, 1);
        Color fade= new Color(1, 1, 1, 0);
        private async UniTask GoGoods(CancellationTokenSource cts)
        {
            float time = 0.0f;
            while (time<1)//Vector2.Distance(transform.position, _endPosition) >= 0.05f)
            {
                time += Time.unscaledDeltaTime*5;
                _goodIcon.color = Color.Lerp(original, fade, time);

                //transform.position = Vector2.Lerp(transform.position
                //    , _endPosition, _moveSpeed * Time.unscaledDeltaTime);

                await UniTask.Yield(cts.Token);
            }
        }
    }
}