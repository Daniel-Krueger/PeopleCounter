using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PeopleCounter
{
    static class Program
    {
        private static PeopleCounterApplicationContext appContext;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try { 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            appContext = new PeopleCounterApplicationContext();

            Application.Run(appContext);
            }
            catch(Exception ex)
            {
                string errorMessage = "";
                Exception current = ex;
                while (current != null)
                {
                    errorMessage += current.Message+"\r\n";
                    current = current.InnerException;
                }
                System.Windows.Forms.MessageBox.Show($"{errorMessage}", "Fehler beim Ausführen des Besucherzählers",MessageBoxButtons.OK, MessageBoxIcon.Error,MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }
        public static void Quit()
        {
            appContext.ExitThread();
        }
    }
}
