using SkinSharp;
using System;
using System.Windows.Forms;

namespace ASCIIArt
{
    public static class Program
    {
        private static SkinH_Net skin;

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                skin = new SkinH_Net();
                skin.AttachEx("Skin.she", "");
            }
            catch
            {
            }

            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}