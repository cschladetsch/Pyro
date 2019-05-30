using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pyro.Core.Network.Interfaces;

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

