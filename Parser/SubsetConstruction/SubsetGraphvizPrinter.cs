using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;

namespace Parser.SubsetConstruction {
    class SubsetGraphvizPrinter :CGraphVizPrinter {
        public SubsetGraphvizPrinter(CGraph graph, CGraphLabeling<CGraphNode> nodeLabeling = null, CGraphLabeling<CGraphEdge> edgeLabeling = null) : base(graph, nodeLabeling, edgeLabeling) {
        }

        public override StringBuilder Print() {

            return base.Print();
        }

        public override void Generate(string filepath, bool executeGenerator = true) {
            // Open a streamwriter
            using (StreamWriter fstream = new StreamWriter(filepath)) {
                fstream.WriteLine(Print());
                fstream.Close();
            }
            // Prepare the process dot to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = "-Tgif " +
                              Path.GetFileName(filepath) + " -o " +
                              Path.GetFileNameWithoutExtension(filepath) + ".gif";
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
