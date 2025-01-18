using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/* ����� ������ �ش� Ŭ���� ��ӹ޾� ��� */
namespace BlackTree
{
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(RectTransform))]
    public class UIRecycleViewController<T> : MonoBehaviour
    {
        protected List<T> tableData = new List<T>(); // ����Ʈ �׸��� �����͸� ����
        [SerializeField]
        protected GameObject cellBase = null;   // ���� ���� ��
        [SerializeField]
        private RectOffset padding; // ��ũ���� ������ �е�
        [SerializeField]
        private float spacingHeight = 4.0f; // �� ���� ����
        [SerializeField]
        private RectOffset visibleRectPadding = null; // visibleRect�� �е�

        private LinkedList<UIRecycleViewCell<T>> cells = new LinkedList<UIRecycleViewCell<T>>(); // �� ���� ����Ʈ

        private Rect visibleRect; // ����Ʈ �׸��� ���� ���·� ǥ���ϴ� ������ ��Ÿ���� �簢��

        private Vector2 prevScrollPos; // �ٷ� ���� ��ũ�� ��ġ�� ����

        public RectTransform CachedRectTransform => GetComponent<RectTransform>();
        public ScrollRect CachedScrollRect => GetComponent<ScrollRect>();

        protected virtual void Start()
        {
            // ���� ���� ���� ��Ȱ��ȭ�صд�
            cellBase.SetActive(false);

            // Scroll Rect ������Ʈ�� On Value Changed �̺�Ʈ�� �̺�Ʈ �����ʸ� �����Ѵ�
            CachedScrollRect.onValueChanged.AddListener(OnScrollPosChanged);
        }

        /// <summary>
        /// ���̺�並 �ʱ�ȭ �ϴ� �Լ�
        /// </summary>
        public void InitializeTableView()
        {
            UpdateScrollViewSize(); // ��ũ���� ������ ũ�⸦ �����Ѵ�
            UpdateVisibleRect();    // visibleRect�� �����Ѵ�

            if (cells.Count < 1)
            {
                // ���� �ϳ��� ���� ���� visibleRect�� ������ ���� ù ��° ����Ʈ �׸��� ã�Ƽ�
                // �׿� �����ϴ� ���� �ۼ��Ѵ�
                Vector2 cellTop = new Vector2(0.0f, -padding.top);
                for (int i = 0; i < tableData.Count; i++)
                {
                    float cellHeight = GetCellHeightAtIndex(i);
                    Vector2 cellBottom = cellTop + new Vector2(0.0f, -cellHeight);
                    if ((cellTop.y <= visibleRect.y && cellTop.y >= visibleRect.y - visibleRect.height) ||
                       (cellBottom.y <= visibleRect.y && cellBottom.y >= visibleRect.y - visibleRect.height))
                    {
                        UIRecycleViewCell<T> cell = CreateCellForIndex(i);
                        cell.Top = cellTop;
                        break;
                    }
                    cellTop = cellBottom + new Vector2(0.0f, spacingHeight);
                }

                // visibleRect�� ������ �� ���� ������ ���� �ۼ��Ѵ�
                SetFillVisibleRectWithCells();
            }
            else
            {
                // �̹� ���� ���� ���� ù ��° ������ ������� �����ϴ� ����Ʈ �׸���
                // �ε����� �ٽ� �����ϰ� ��ġ�� ������ �����Ѵ�
                LinkedListNode<UIRecycleViewCell<T>> node = cells.First;
                UpdateCellForIndex(node.Value, node.Value.Index);
                node = node.Next;

                while (node != null)
                {
                    UpdateCellForIndex(node.Value, node.Previous.Value.Index + 1);
                    node.Value.Top = node.Previous.Value.Bottom + new Vector2(0.0f, -spacingHeight);
                    node = node.Next;
                }

                // visibleRect�� ������ �� ���� ������ ���� �ۼ��Ѵ�
                SetFillVisibleRectWithCells();
            }
        }

        /// <summary>
        /// ���� ���̰��� �����ϴ� �Լ�
        /// </summary>
        /// <returns>The cell height at index.</returns>
        /// <param name="index">Index.</param>
        protected virtual float GetCellHeightAtIndex(int index)
        {
            // ���� ���� ��ȯ�ϴ� ó���� ����� Ŭ�������� �����Ѵ�
            // ������ ũ�Ⱑ �ٸ� ��� ��ӹ��� Ŭ�������� �� �����Ѵ�
            return cellBase.GetComponent<RectTransform>().sizeDelta.y;
        }

        /// <summary>
        /// ��ũ���� ���� ��ü�� ���̸� �����ϴ� �Լ�
        /// </summary>
        protected void UpdateScrollViewSize()
        {
            // ��ũ���� ���� ��ü�� ���̸� ����Ѵ�
            float contentHeight = 0.0f;
            for (int i = 0; i < tableData.Count; i++)
            {
                contentHeight += GetCellHeightAtIndex(i);

                if (i > 0)
                {
                    contentHeight += spacingHeight;
                }
            }

            // ��ũ���� ������ ���̸� �����Ѵ�
            Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
            sizeDelta.y = padding.top + contentHeight + padding.bottom;
            CachedScrollRect.content.sizeDelta = sizeDelta;
        }


        /// <summary>
        /// ���� �����ϴ� �Լ�
        /// </summary>
        /// <returns>The cell for index.</returns>
        /// <param name="index">Index.</param>
        private UIRecycleViewCell<T> CreateCellForIndex(int index)
        {
            // ���� ���� ���� �̿��� ���ο� ���� �����Ѵ�
            GameObject obj = Instantiate(cellBase) as GameObject;
            obj.SetActive(true);
            UIRecycleViewCell<T> cell = obj.GetComponent<UIRecycleViewCell<T>>();

            // �θ� ��Ҹ� �ٲٸ� �������̳� ũ�⸦ �Ҿ�����Ƿ� ������ �����صд�
            Vector3 scale = cell.transform.localScale;
            Vector2 sizeDelta = cell.CachedRectTransform.sizeDelta;
            Vector2 offsetMin = cell.CachedRectTransform.offsetMin;
            Vector2 offsetMax = cell.CachedRectTransform.offsetMax;

            cell.transform.SetParent(cellBase.transform.parent,false);

            // ���� �����ϰ� ũ�⸦ �����Ѵ�
            cell.transform.localScale = scale;
            cell.CachedRectTransform.sizeDelta = sizeDelta;
            cell.CachedRectTransform.offsetMin = offsetMin;
            cell.CachedRectTransform.offsetMax = offsetMax;

            // ������ �ε����� ���� ����Ʈ �׸� �����ϴ� ���� ������ �����Ѵ�
            UpdateCellForIndex(cell, index);

            cells.AddLast(cell);
           
            return cell;
        }

        /// <summary>
        /// ���� ������ �����ϴ� �Լ�
        /// </summary>
        /// <param name="cell">Cell.</param>
        /// <param name="index">Index.</param>
        private void UpdateCellForIndex(UIRecycleViewCell<T> cell, int index)
        {
            // ���� �����ϴ� ����Ʈ �׸��� �ε����� �����Ѵ�
            cell.Index = index;

            if (cell.Index >= 0 && cell.Index <= tableData.Count - 1)
            {
                // ���� �����ϴ� ����Ʈ �׸��� �ִٸ� ���� Ȱ��ȭ�ؼ� ������ �����ϰ� ���̸� �����Ѵ�
                cell.gameObject.SetActive(true);
                cell.UpdateContent(tableData[cell.Index]);
                cell.Height = GetCellHeightAtIndex(cell.Index);
            }
            else
            {
                // ���� �����ϴ� ����Ʈ �׸��� ���ٸ� ���� ��Ȱ��ȭ���� ǥ�õ��� �ʰ� �Ѵ�
                cell.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// VisibleRect�� �����ϱ� ���� �Լ�
        /// </summary>
        private void UpdateVisibleRect()
        {
            // visibleRect�� ��ġ�� ��ũ���� ������ �������κ��� ������� ��ġ��
            visibleRect.x = CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
            visibleRect.y = -CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

            // visibleRect�� ũ��� ��ũ�� ���� ũ�� + �е�
            visibleRect.width = CachedRectTransform.rect.width + visibleRectPadding.left + visibleRectPadding.right;
            visibleRect.height = CachedRectTransform.rect.height + visibleRectPadding.top + visibleRectPadding.bottom;
        }




        /// <summary>
        /// VisubleRect ������ ǥ�õ� ��ŭ�� ���� �����Ͽ� ��ġ�ϴ� �Լ�
        /// </summary>
        private void SetFillVisibleRectWithCells()
        {
            // ���� ���ٸ� �ƹ� �ϵ� ���� �ʴ´�
            if (cells.Count < 1)
            {
                return;
            }

            // ǥ�õ� ������ ���� �����ϴ� ����Ʈ �׸��� ���� ����Ʈ �׸��� �ְ�
            // ���� �� ���� visibleRect�� ������ ���´ٸ� �����ϴ� ���� �ۼ��Ѵ�
            UIRecycleViewCell<T> lastCell = cells.Last.Value;
            int nextCellDataIndex = lastCell.Index + 1;
            Vector2 nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

            while (nextCellDataIndex < tableData.Count && nextCellTop.y >= visibleRect.y - visibleRect.height)
            {
                UIRecycleViewCell<T> cell = CreateCellForIndex(nextCellDataIndex);
                cell.Top = nextCellTop;

                lastCell = cell;
                nextCellDataIndex = lastCell.Index + 1;
                nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);
            }
        }


        /// <summary>
        /// ��ũ�Ѻ䰡 �������� �� ȣ��Ǵ� �Լ�
        /// </summary>
        /// <param name="scrollPos">Scroll position.</param>
        public void OnScrollPosChanged(Vector2 scrollPos)
        {
            // visibleRect�� �����Ѵ�
            UpdateVisibleRect();
            // ���� �����Ѵ�
            UpdateCells((scrollPos.y < prevScrollPos.y) ? 1 : -1);

            prevScrollPos = scrollPos;
        }

        public void SlotUpdate()
        {
            // visibleRect�� �����Ѵ�
            UpdateVisibleRect();
            // ���� �����Ѵ�
            UpdateCells(1);

            //prevScrollPos = prevScrollPos;
        }
        /// <summary>
        /// ���� �����Ͽ� ǥ�ø� �����ϴ� �Լ�
        /// </summary>
        /// <param name="scrollDirection">Scroll direction.</param>
        private void UpdateCells(int scrollDirection)
        {
            if (cells.Count < 1)
            {
                return;
            }

            if (scrollDirection > 0)
            {
                // ���� ��ũ���ϰ� ���� ���� visibleRect�� ������ �������� ���� �ִ� ����
                // �Ʒ��� ���� ������� �̵����� ������ �����Ѵ�
                UIRecycleViewCell<T> firstCell = cells.First.Value;
                while (firstCell.Bottom.y > visibleRect.y)
                {
                    UIRecycleViewCell<T> lastCell = cells.Last.Value;
                    UpdateCellForIndex(firstCell, lastCell.Index + 1);
                    firstCell.Top = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

                    cells.AddLast(firstCell);
                    cells.RemoveFirst();
                    firstCell = cells.First.Value;
                }

                // visibleRect�� ������ ���� �ȿ� �� ���� ������ ���� �ۼ��Ѵ�
                SetFillVisibleRectWithCells();
            }
            else if (scrollDirection < 0)
            {
                // �Ʒ��� ��ũ���ϰ� ���� ���� visibleRect�� ������ �������� �Ʒ��� �ִ� ����
                // ���� ���� ������� �̵����� ������ �����Ѵ�
                UIRecycleViewCell<T> lastCell = cells.Last.Value;
                while (lastCell.Top.y < visibleRect.y - visibleRect.height)
                {
                    UIRecycleViewCell<T> firstCell = cells.First.Value;
                    UpdateCellForIndex(lastCell, firstCell.Index - 1);
                    lastCell.Bottom = firstCell.Top + new Vector2(0.0f, spacingHeight);

                    cells.AddFirst(lastCell);
                    cells.RemoveLast();
                    lastCell = cells.Last.Value;
                }
            }
        }
    }
}