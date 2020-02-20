using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpTime
{
    public partial class PasswordForm : Form
    {
        private OptimeControl opControl;

        public PasswordForm(OptimeControl opControl)
        {
            InitializeComponent();

            this.opControl = opControl;             
        }

        private void OK_Btn_Click(object sender, EventArgs e)
        {
            //if (Password_TB.Text == Program.CheckSum)
            if (!String.IsNullOrEmpty(Password_TB.Text))   // для демонстрации убрана проверка пароля  
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Введен неверный пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }           
        }

        private void Cancel_Btn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
