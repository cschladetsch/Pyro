using System.Text;
using Diver;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;
using UIWidgets;
using UnityEngine;

namespace App
{
    public class PiConsole : MonoBehaviour
    {
        public TreeView TreeView;
        protected internal ObservableList<ListNode<TreeViewItem>> _treeViewDataSource;
        private PiTranslator _pi;
        private IRegistry _registry;
        private Executor _exec;

        void Awake()
        {
            _registry = new Registry();
            _pi = new PiTranslator(_registry);
            _exec = new Executor();

            TreeView.Init();
            _treeViewDataSource = new ObservableList<ListNode<TreeViewItem>>();
            TreeView.DataSource = _treeViewDataSource;

            //_pi.Translate("1 2 +");
            //_exec.Continue(_pi.Continuation);
            //Debug.Log(_exec.DataStack.Peek());
        }

        public void Process(string text)
        {
            Debug.Log(text);
            //AddOutputItem(text);
            //return;

            if (_pi.Translate(text))
            {
                _exec.Continue(_pi.Continuation);
                var index = 0;
                foreach (var obj in _exec.DataStack)
                {
                    AddOutputItem($"{index++}: {obj}");
                }

                return;
            }

            AddOutputItem(_pi.Error);
        }

        private void AddOutputItem(string output)
        {
            var treeViewItem = new TreeViewItem(output);
            var treeNode = new TreeNode<TreeViewItem>(treeViewItem);
            _treeViewDataSource.Add(new ListNode<TreeViewItem>(treeNode, 0));
        }
    }
}
