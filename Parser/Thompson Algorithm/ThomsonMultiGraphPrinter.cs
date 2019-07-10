using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using GraphLibrary.Generics;
using Parser.UOPCore;

namespace Parser.Thompson_Algorithm {
    public class ThomsonMultiGraphGraphVizPrinter : CGraphMultiPrinter {

        public ThomsonMultiGraphGraphVizPrinter(string filePath) : base(filePath) {

        }

        /// <summary>
        /// The method calls the base generate method and invokes the
        /// graphviz tool
        /// </summary>
        public override void Generate() {
            base.Generate();

            // Prepare the process dot to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = "-Tgif " +
                              Path.GetFileName(m_filePath) + " -o " +
                              Path.GetFileNameWithoutExtension(m_filePath) + ".gif";
            // Enter the executable to run, including the complete path
            start.FileName = "dot";
            // Do you want to show a console window?
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            int exitCode;

            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start)) {
                proc.WaitForExit();

                // Retrieve the app's exit code
                exitCode = proc.ExitCode;
            }
        }
    }
}
