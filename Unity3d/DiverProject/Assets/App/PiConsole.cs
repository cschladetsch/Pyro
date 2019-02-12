//using System;
//using UIWidgets;
//using UnityEngine;

//using Pyro.Network;

//namespace App
//{
//    public class PiConsole : MonoBehaviour
//    {
//        public TreeView TreeView;
//        public int ListenPort;

//        protected internal ObservableList<ListNode<TreeViewItem>> _treeViewDataSource;
//        private IPeer _peer;
//        private Pyro.ExecutionContext.Context _context;

//        void Awake()
//        {
//            _context = new Pyro.ExecutionContext.Context();
//            _peer = Pyro.Network.Create.NewPeer(ListenPort);
//            _peer.Start();

//            TreeView.Init();
//            _treeViewDataSource = new ObservableList<ListNode<TreeViewItem>>();
//            TreeView.DataSource = _treeViewDataSource;
//        }

//        public void Process(string text)
//        {
//            try
//            {
//                //_context.
//                //if (_pi.Translate(text))
//                //    _exec.Continue(_pi.Result());
//                //else
//                //    AddOutputItem(_pi.Error);
//            }
//            catch (Exception e)
//            {
//                AddOutputItem(e.Message);
//                return;
//            }

//            WriteDataStack();
//        }

//        private void WriteDataStack()
//        {
//            var index = 0;
//            _treeViewDataSource.Clear();
//            foreach (var obj in _exec.DataStack)
//                AddOutputItem($"[{index++}]: {obj}");
//        }

//        private void AddOutputItem(string output)
//        {
//            var treeViewItem = new TreeViewItem(output);
//            var treeNode = new TreeNode<TreeViewItem>(treeViewItem);
//            _treeViewDataSource.Add(new ListNode<TreeViewItem>(treeNode, 0));
//        }
//    }
//}
