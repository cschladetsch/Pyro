using System;
using System.Windows.Forms;

using Pyro.Network;

namespace WinForms
{
    /// <summary>
    /// Dialog for connecting to another Pyro Peer
    /// </summary>
    public partial class NetworkConnect
        : Form
    {
        private IPeer _local;

        public NetworkConnect(IPeer local)
        {
            InitializeComponent();
            _local = local;
        }

        private void _OkPressed(object sender, EventArgs e)
        {
        }

        private void _CancelPressed(object sender, EventArgs e)
        {

        }
    }
}

