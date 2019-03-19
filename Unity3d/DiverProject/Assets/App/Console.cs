using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

using Pyro.Network;
using Pyro.ExecutionContext;

namespace App
{
    public class Console : MonoBehaviour
    {
        public TreeView TreeView;
        public TmpDropShadowText IpAddress;
        public TMPro.TMP_InputField PiScript;
        public TMPro.TMP_InputField RhoScript;
        public int ListenPort;

        protected internal ObservableList<ListNode<TreeViewItem>> _treeViewDataSource;

        private IPeer _peer;
        private Context _context;

        private void Awake()
        {
            _context = new Context();
            _peer = Pyro.Network.Create.NewPeer(ListenPort);
            _peer.Start();
            _peer.OnReceivedResponse += NetworkResponse;
            IpAddress.Text = $"{_peer.LocalHostName.Replace('.', '-')}:{ListenPort}";

            SetupStackView();
        }

        private void NetworkResponse(IServer server, IClient client, string text)
        {
            AddOutputItem($"Net: {text}");
        }

        private void SetupStackView()
        {
            TreeView.Init();
            _treeViewDataSource = new ObservableList<ListNode<TreeViewItem>>();
            TreeView.DataSource = _treeViewDataSource;
        }

        private void Start()
        {
            PiScript.onEndEdit.AddListener(PiScriptSubmit);
            RhoScript.onEndEdit.AddListener(RhoScriptSubmit);
        }

        private void RhoScriptSubmit(string text)
        {
            Process(Pyro.Language.ELanguage.Rho, text);
        }

        private void PiScriptSubmit(string text)
        {
            Process(Pyro.Language.ELanguage.Pi, text);
        }

        public void Process(Pyro.Language.ELanguage lang, string text)
        {
            try
            {
                if (!_context.Exec(lang, text))
                    _context.Executor.Push($"Error: {_context.Error}");
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
            foreach (var obj in _context.Executor.DataStack)
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
