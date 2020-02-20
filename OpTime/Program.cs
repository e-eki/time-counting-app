using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpTime
{
    static class Program
    {
        public static readonly string Version;
        public static readonly string CheckSum;

        static Program()
        {
            Version = typeof(Program).Assembly.GetName().Version.ToString();
            CheckSum = Convert.ToString(CalcCS.CRC_mycalc.Calc(Application.ExecutablePath), 16).PadLeft(8, '0').ToUpper();
        }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
