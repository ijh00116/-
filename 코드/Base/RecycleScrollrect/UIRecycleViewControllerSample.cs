using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTree
{
    public class UIRecycleViewControllerSample : UIRecycleViewController<UICellSampleData>
    {
        // ����Ʈ �׸��� �����͸� �о� ���̴� �޼���
        private void LoadData()
        {
            // �Ϲ����� �����ʹ� ������ �ҽ��κ��� �������µ� ���⼭�� �ϵ� �ڵ带 ������Ͽ� �����Ѵ�
            tableData = new List<UICellSampleData>() {
            new UICellSampleData { index=1, name="One"},
            new UICellSampleData { index=2, name="Two" },
            new UICellSampleData { index=3, name="Three" },
            new UICellSampleData { index=4, name="Four" },
            new UICellSampleData { index=5, name="Five" },
            new UICellSampleData { index=6, name="Six" },
            new UICellSampleData { index=7, name="Seven" },
            new UICellSampleData { index=8, name="Eight" },
            new UICellSampleData { index=9, name="Nine" },
            new UICellSampleData { index=10,name="Ten" }
        };

            // ��ũ�ѽ�ų ������ ũ�⸦ �����Ѵ�
            InitializeTableView();
        }

        // �ν��Ͻ��� �ε��� �� Awake �޼��尡 ó���� ������ ȣ��ȴ�
        protected override void Start()
        {
            // ��� Ŭ������ Start �޼��带 ȣ���Ѵ�
            base.Start();

            // ����Ʈ �׸��� �����͸� �о� ���δ�
            LoadData();

        }

        // ���� ���õ��� �� ȣ��Ǵ� �޼���
        public void OnPressCell(UIRecycleViewCellSample cell)
        {
            //Debug.Log("Cell Click");
            //Debug.Log(tableData[cell.Index].name);
        }
    }
}