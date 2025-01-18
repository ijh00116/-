using BlackTree.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BlackTree.Bundles
{
    public class BTButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("Button Animation")]
        [SerializeField] private float downSize = 0.95f;
        [SerializeField] private float upSize = 1.05f;

        [Header("Button Image & Text")]
        [SerializeField] private Image[] images = null;
        [SerializeField] private Text textTitle;
        [SerializeField] private Text[] texts = null;

        [Header("Button Select&UnSelect")]
        [SerializeField] bool haveselectobj = false;
        [SerializeField] private GameObject SelectObj;
        [SerializeField] private GameObject UnSelectObj;


        [FormerlySerializedAs("onClick")]
        // [SerializeField]
        // private ButtonClickedEvent _OnClick = new ButtonClickedEvent();
        // [Serializable]
        // public class ButtonClickedEvent : UnityEvent
        // {
        // }

        // public ButtonClickedEvent onClick
        // {
        //     get => this._OnClick; 
        //     set => this._OnClick = value;
        // }

        public UnityEvent onClick = null;
        [SerializeField] private AudioSource clickAudio = null;
        [SerializeField] private bool isRepeatInvokeOnDown = false;


        private Vector2 downPosition;
        private bool isDown = false;
        private int refeatClickCount = 0;
        private float lastClickTime;
        public UnityEvent onClickDown;
        public UnityEvent onClickUp;
        public UnityAction<Vector2> onTouch;
        public UnityAction onTouchMoreThanMax;

        Coroutine downUpdate;
        private Vector2 GetSafeInputMousePos()
        {
            return Input.mousePosition;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (isDown) return; // �� �� ������ ����Ǿ��� ������ ������ �ִٸ� �ٽ� ������� ����.
            downPosition = GetSafeInputMousePos();
            isDown = true;
            if (clickAudio)
                clickAudio.Play();
           
            AudioManager.Instance.Play(AudioSourceKey.Click);

            //�ִϸ��̼�
            StopAllCoroutines();
            StartCoroutine(IeAnim(downSize));
            if (isRepeatInvokeOnDown)
            {
                refeatClickCount = 0;
                downUpdate =StartCoroutine(IeRepeatInvoke());
            }
        }

        public void UnSelect()
        {
            if (haveselectobj == false)
                return;
            SelectObj.SetActive(false);
            UnSelectObj.SetActive(true);
        }
        public void Select()
        {
            if (haveselectobj == false)
                return;
            SelectObj.SetActive(true);
            UnSelectObj.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDown) return;
            isDown = false;

            if (haveselectobj)
            {
                SelectObj.SetActive(false);
                UnSelectObj.SetActive(true);
            }

            //�ִϸ��̼�
            StopAllCoroutines();
            StartCoroutine(IeAnim(upSize));

            if(downUpdate!=null)
            {
                StopCoroutine(downUpdate);
                downUpdate = null;
                onClickUp?.Invoke();
            }
        }

        void TouchEffect(Vector2 pos)
        {
            onTouch?.Invoke(pos);


        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
         
            TouchEffect(eventData.position);

            //AudioManager.Instance.Play(AudioSourceKey.Click);

            //if (isRepeatInvokeOnDown)
            //{
            //    //0.35�� �̳��� �� �����ٸ�
            //    if (Time.time - lastClickTime <= 0.35f)
            //    {
            //        //�� ���� Ƚ���� 3�� �̻��̶��
            //        if (refeatClickCount >= 3)
            //        {
            //            onTouchMoreThanMax?.Invoke();
            //            //��ġ���� ����

            //            refeatClickCount = 0;
            //        }
            //        else
            //        {
            //            refeatClickCount++;
            //        }
            //    }
            //    else
            //    {
            //        refeatClickCount = 0;
            //    }

            //    //�ð� ����
            //    lastClickTime = Time.time;
            //}
        }

        void PointerExit()
        {
            if (!isDown) return;
            isDown = false;

            if (downUpdate != null)
            {
                StopCoroutine(downUpdate);
                downUpdate = null;
                onClickUp?.Invoke();
            }

            StopAllCoroutines();
            StartCoroutine(IeAnim(1f));
        }


        void OnDisable()
        {
            transform.localScale = Vector3.one;
            isDown = false;
        }

        public void SetTitleText(string text)
        {
            textTitle.text = text;
        }

        public void SetText(string text)
        {
            foreach (var txt in texts)
            {
                txt.text = text;
            }
        }

        public void SetTextSame(string str)
        {
            foreach (var text in texts)
            {
                text.text = str;
            }
        }

        public void SetTextColor(Color color)
        {
            texts[0].color = color;
        }


        //�ݺ� ����
        IEnumerator IeRepeatInvoke()
        {
            var delay = new WaitForSeconds(0.02f);
            yield return new WaitForSeconds(0.3f); //0.5�����Ŀ� �ݺ����� �ߵ�.
            while (true)
            {
                TouchEffect(GetSafeInputMousePos());
                onClickDown?.Invoke();
                
                if (refeatClickCount>40)
                {
                    for(int i=0; i<19; i++)
                    {
                        onClickDown?.Invoke();
                    }
                    yield return null; //�����Ӽӵ�
                }
                else if (refeatClickCount > 20)
                    yield return null; //�����Ӽӵ�
                else if (refeatClickCount > 10)
                    yield return delay;
                else
                    yield return delay;
                refeatClickCount++;

            }
        }

        //��ư ũ�� �ٲ�� �ִϸ��̼�
        IEnumerator IeAnim(float size)
        {
            while (Mathf.Abs(transform.localScale.x - size) > 0.01f)
            {
                transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, size, 0.45f);
                yield return null;
            }

            transform.localScale = Vector3.one * size;
            while (size > 1.01f)
            {
                transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, 1f, 0.45f);
                yield return null;
            }

            if (size > 1.01f)
                transform.localScale = Vector3.one;


            while (true)
            {
                if (Vector2.SqrMagnitude((Vector2)GetSafeInputMousePos() - downPosition) > 100000f)
                {
                    PointerExit();
                }

                yield return null;
            }
        }

        public void SetBackgroundAlpha(float alpha)
        {
            texts[0].color = new Color(texts[0].color.r, texts[0].color.g, texts[0].color.b, alpha);
            images[0].color = new Color(images[0].color.r, images[0].color.g, images[0].color.b, alpha);
        }


        public void SetImage(Sprite sprite)
        {
            foreach (var image in images)
                image.sprite = sprite;
        }
    }
}
