using System;
using System.IO;
using System.Windows.Forms;
using vc.Forms;
using vc.Database;

namespace vcl
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            string dataPath = Path.Combine(Application.StartupPath, "Data");
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            // формируем путь к папке Data в директории приложения, иесли ее не существует, создаем ее



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}