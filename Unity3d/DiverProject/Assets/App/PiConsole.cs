using System;
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
            _exec = new Executor(_registry);

            TreeView.Init();
            _treeViewDataSource = new ObservableList<ListNode<TreeViewItem>>();
            TreeView.DataSource = _treeViewDataSource;
        }

        public void Process(string text)
        {
            try
            {
                if (_pi.Translate(text))
                    _exec.Continue(_pi.Result());
                else
                    AddOutputItem(_pi.Error);
            }
            catch (Exception e)
            {
                AddOutputItem(e.Message);
                return;
            }

            WriteDataStack();
        }

        private void WriteDataStack()
        {
            var index = 0;
            _treeViewDataSource.Clear();
            foreach (var obj in _exec.DataStack)
                AddOutputItem($"[{index++}]: {obj}");
        }

        private void AddOutputItem(string output)
        {
            var treeViewItem = new TreeViewItem(output);
            var treeNode = new TreeNode<TreeViewItem>(treeViewItem);
            _treeViewDataSource.Add(new ListNode<TreeViewItem>(treeNode, 0));
        }
    }
}
