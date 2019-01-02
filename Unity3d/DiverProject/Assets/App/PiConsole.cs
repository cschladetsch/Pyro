using UIWidgets;
using UnityEngine;

namespace App
{
    public class PiConsole : MonoBehaviour
    {
        public TreeView TreeView;
        protected internal ObservableList<ListNode<TreeViewItem>> _treeViewDataSource;

        void Awake()
        {
            TreeView.Init();
            _treeViewDataSource = new ObservableList<ListNode<TreeViewItem>>();
            TreeView.DataSource = _treeViewDataSource;

        }
        public void Process(string text)
        {
            Debug.Log(text);
            var treeViewItem = new TreeViewItem(text);
            var treeNode = new TreeNode<TreeViewItem>(treeViewItem);
            _treeViewDataSource.Add(new ListNode<TreeViewItem>(treeNode, 0));

            //var item = new TreeNode<TreeViewItem>();
            //var node = new ListNode<TreeViewItem>(item, 0);
            //data.Add(node);
        }
    }
}
