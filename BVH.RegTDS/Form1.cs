using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BVH.RegTDS
{
    public partial class Form1 : Form
    {
        private List<AccountInfor> listAccountInfor;
        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            listAccountInfor = new List<AccountInfor>();
            gridAccInfor.AllowUserToAddRows = false;
            ReloadGrid();
        }

        private void ReloadGrid()
        {
            var bindingList = new SortableBindingList<AccountInfor>(listAccountInfor);
            var source = new BindingSource(bindingList, null);
            gridAccInfor.DataSource = source;
            gridAccInfor.Update();
            gridAccInfor.Refresh();
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string userName = txtUsername.Text;
            int from = int.Parse(txtUserNameFrom.Text);
            int to = int.Parse(txtUserNameTo.Text);
            string password = txtPassword.Text;

            for (int i = from; i <= to; i++)
            {
                var accInfo = new AccountInfor()
                {
                    Username = $"{userName}{i}",
                    Password = password
                };

                listAccountInfor.Add(accInfo);

                var tdsProxy = new TDSProxy();
                await tdsProxy.RegAcc(accInfo);
            }

            ReloadGrid();
        }
    }
}
