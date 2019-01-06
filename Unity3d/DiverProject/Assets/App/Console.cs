using System;
using System.Collections;
using System.Collections.Generic;
using Diver;
using Diver.Exec;
using Diver.Impl;
using Diver.Language;
using UIWidgets;
using UnityEngine;

namespace App
{
    public class Console : MonoBehaviour
    {
        public TreeView TreeView;
        public TMPro.TMP_InputField PiScript;
        public TMPro.TMP_InputField RhoScript;

        protected internal ObservableList<ListNode<TreeViewItem>> _treeViewDataSource;

        private void Awake()
        {
            _reg = new Registry();
            _rho = new RhoTranslator(_reg);
            _pi = new PiTranslator(_reg);
            _exec = new Executor(_reg);

            SetupStackView();
        }

        private void SetupStackView()
        {
            TreeView.Init();
            _treeViewDataSource = new ObservableList<ListNode<TreeViewItem>>();
            TreeView.DataSource = _treeViewDataSource;
        }

        private void Start()
        {
        }

        private void Update()
        {
            ProcessScrpipts();
        }

        private void ProcessScrpipts()
        {
            var enter = Input.GetKeyDown(KeyCode.Return);
            var control = Input.GetKey(KeyCode.LeftControl);
            if (!enter) 
                return;

            if (PiScript.isFocused)
            {
                Process(_pi, PiScript.text);
            }
            else if (RhoScript.isFocused && control)
            {
                Process(_rho, RhoScript.text);
            }
        }

        public void Process(TranslatorCommon trans, string text)
        {
            try
            {
                if (trans.Translate(text))
                    _exec.Continue(trans.Result());
                else
                    AddOutputItem(trans.Error);
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

        private RhoTranslator _rho;
        private PiTranslator _pi;
        private IRegistry _reg;
        private Executor _exec;
    }
}
