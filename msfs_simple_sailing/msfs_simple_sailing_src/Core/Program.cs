using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using msfs_simple_sail_core.Core;
using msfs_simple_sail_core.UI;

namespace msfs_simple_sail
{
    static class Program
    {
        static void Main(string[] args)
        {
            var logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log");
            var fileStream = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            var writer = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
            Console.SetOut(writer);
            Console.SetError(writer);

            ApplicationConfiguration.Initialize();
            var config = Config.GetInstance();

            Controller controller = new Controller();
            controller.Init();
            Task controllerTask = Task.Run(() =>
            {
                controller.Run(); // Run the loop in the background
            });

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (config.darkModeEnabled)
            {
                if (config.darkModeSystem)
                {
                    Application.SetColorMode(SystemColorMode.System);
                }
                else
                {
                    Application.SetColorMode(SystemColorMode.Dark);
                }
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            var form = new FormUI(controller);
            controller.SetUI(form);

            Application.Run(form);
        }
    }
}


