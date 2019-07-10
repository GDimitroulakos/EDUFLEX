using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Parser.ASTVisitor.ConcreteVisitors;
using Parser.ASTVisitor.Visitors;
using Parser.Hopcroft;
using Parser.SubsetConstruction;
using Parser.Thompson_Algorithm;
using Parser.UOPCore;

//kefalaio 20 File Info klaseis Directory,DirectoryInfo , File, FileInfo
// IEnumerable<T> interface, IEnumerator<T>,  foreach sel 297-300 +google msdn
namespace Parser {
    public static class Facade {
        private static FA ms_minDFA;

        public static FA MsMinDfa => ms_minDFA;

        public static void VerifyRegExp(string[] args)//Validate the reg exp
        {
            int i = 0;
            int fileerror;
            int errors = 0;

            foreach (string loc in args) {   // 1. Check if the path refers to a file or directory
                // 2a. File : Initiate parsing for that file
                // 2a.1 Parse each file and record the number of errors per file and the total errors

                if (File.Exists(loc)) {
                    Console.WriteLine("Parsing {0}", args[i]);
                    errors += fileerror = Parse(loc);
                    Console.Write(":{0} errors", fileerror);
                }
                // 2b. Directory : Search for grammar files in current directory or subdirectories
                // 2b.1 Parse each file and record the number of errors per file and the total errors

                else if (Directory.Exists(loc)) {
                    errors += ParseSubDirectories(loc);
                }
                else {
                    Console.WriteLine("File or Directory with the given name <{0}> doesn't exist", loc);
                }
                i++;
            }
            Console.WriteLine("\n\nTotal {0} errors", errors);
        }

        // Parses the given file
        public static int Parse(string loc) {
            StreamReader reader = new StreamReader(loc);

            AntlrInputStream iStream = new AntlrInputStream(reader);
            RegExpLexer lexer = new RegExpLexer(iStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            RegExpParser parser = new RegExpParser(tokens);
            IParseTree tree = parser.lexerDescription();

            PTPrinter PTvisitor = new PTPrinter(loc);

            PTvisitor.Visit(tree);
            ASTGeneration astGeneration = new ASTGeneration();

             astGeneration.Visit(tree);
             ASTPrinter astPrinter = new ASTPrinter(loc);
             astPrinter.Visit(astGeneration.M_ASTRoot);

            ThompsonVisitor thompson = new ThompsonVisitor(ThompsonOptions.TO_STEPS);
            thompson.Visit(astGeneration.M_ASTRoot);

            CSubsetConstructionAlgorithm subsetcontruction = CSubsetConstructionAlgorithm.Init(thompson.M_Nfa);
            subsetcontruction.Start();

            CHopcroftAlgorithm hopcroftAlgorithm = new CHopcroftAlgorithm(subsetcontruction.Dfa);
            hopcroftAlgorithm.Init();
            ms_minDFA = hopcroftAlgorithm.MinimizedDfa;
            

            return parser.NumberOfSyntaxErrors;
        }
        static int ParseSubDirectories(string directory) {

            int errors = 0; // Errors per file
            int fileerror;  // Total Errors


            Console.WriteLine("Processing directory {0}...", directory);
            foreach (string file in Directory.GetFiles(directory)) {
                Console.WriteLine("Parsing {0}", Path.GetFileName(file));
                errors += fileerror = Parse(file);
                Console.WriteLine(":{0} errors\n", fileerror);
            }
            foreach (string dir in Directory.GetDirectories(directory)) {
                errors += ParseSubDirectories(dir);
            }
            return errors;

        }
    }
}