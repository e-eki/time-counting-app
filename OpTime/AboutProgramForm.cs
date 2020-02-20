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
    public partial class AboutProgramForm : Form
    {
        public AboutProgramForm()
        {
            InitializeComponent();

            ProgramName_LBL.Text = "Программа подсчета времени наработки" /*this.GetType().Assembly.GetName().Name.ToString()*/;
            Developer_LBL.Text = "Дремина В.А.";
            Version_LBL.Text = Program.Version;
            Summ_LBL.Text = Program.CheckSum;
        }
    }
}
