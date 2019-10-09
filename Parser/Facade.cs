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
using Parser.SubsetConstruction.Parser.SubsetConstruction;
using Parser.Thompson_Algorithm;
using Parser.UOPCore;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

//kefalaio 20 File Info klaseis Directory,DirectoryInfo , File, FileInfo
// IEnumerable<T> interface, IEnumerator<T>,  foreach sel 297-300 +google msdn
namespace Parser {

    public enum ParserOptionsEnum {
        PO_DEFAULT = 0,
        /// <summary>
        /// This option determines whether the lexical analyzer performs a
        /// simple input validity check or executes code upon pattern match
        /// recognition
        /// </summary>
        PO_OPERATION_SIMPLECHECK_VS_CODE = 1
    }

    public static class Facade {
        private static Options<ParserOptionsEnum> m_parserOptions=new Options<ParserOptionsEnum>();

        private static FA ms_globalMinDFA;

        public static FA MsMinDfa => ms_globalMinDFA;

        // Holds the Regular Expression record indexed by the line declared into the input grammar
        private static Dictionary<uint, RERecord> m_reRecords = new Dictionary<uint, RERecord>();

        public static Dictionary<uint, RERecord> M_ReRecords {
            get => m_reRecords;
        }

        static Facade() {
            SetOperationModeCode(true);
        }

        public static void SetOperationModeCode(bool code = true) {
            if (code) {
                m_parserOptions.Set(ParserOptionsEnum.PO_OPERATION_SIMPLECHECK_VS_CODE);
            }
            else {
                m_parserOptions.Reset(ParserOptionsEnum.PO_OPERATION_SIMPLECHECK_VS_CODE);
            }
        }

        public static bool GetOperationModeCode() {
            return m_parserOptions.IsSet(ParserOptionsEnum.PO_OPERATION_SIMPLECHECK_VS_CODE);
        }

          

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
            ASTGeneration astGeneration = new ASTGeneration(){M_ReRecords = m_reRecords};

            astGeneration.Visit(tree);
            ASTPrinter astPrinter = new ASTPrinter(loc);
            astPrinter.Visit(astGeneration.M_ASTRoot);

            if (m_parserOptions.IsSet(ParserOptionsEnum.PO_OPERATION_SIMPLECHECK_VS_CODE)) {
                ThompsonAlgorithmStructured thompson = new ThompsonAlgorithmStructured(ThompsonOptions.TO_STEPS | ThompsonOptions.TO_NFAGENERATION_FLATTEN_VS_STRUCTURED,m_reRecords);
                thompson.Visit(astGeneration.M_ASTRoot);
                
                CSubsetConstructionStructuredAlgorithm subsetcontruction = new CSubsetConstructionStructuredAlgorithm(m_reRecords);
                subsetcontruction.Start();

                CHopcroftAlgorithmStructured hopcroftAlgorithm = new CHopcroftAlgorithmStructured(m_reRecords);
                hopcroftAlgorithm.Start();

                SerializeEDUFLEXOutput(@"EDUFLEX.out");
            }
            else {
                m_reRecords[0] = new RERecord();
                ThompsonVisitorFlatten thompson = new ThompsonVisitorFlatten(ThompsonOptions.TO_STEPS |
                (~ThompsonOptions.TO_NFAGENERATION_FLATTEN_VS_STRUCTURED));
                thompson.Visit(astGeneration.M_ASTRoot);
                m_reRecords[0].M_Nfa = thompson.M_Nfa;

                CSubsetConstructionAlgorithm subsetconstruction = CSubsetConstructionAlgorithm.Init(thompson.M_Nfa);
                subsetconstruction.Start();
                m_reRecords[0].M_Dfa = subsetconstruction.Dfa;

                CHopcroftAlgorithm hopcroftAlgorithm = new CHopcroftAlgorithm(subsetconstruction.Dfa);
                hopcroftAlgorithm.Init();
                m_reRecords[0].M_MinDfa = hopcroftAlgorithm.MinimizedDfa;
                ms_globalMinDFA = hopcroftAlgorithm.MinimizedDfa;
            }
            return parser.NumberOfSyntaxErrors;
        }

        static void SerializeEDUFLEXOutput(string filename) {
            BinaryFormatter saver = new BinaryFormatter();

            using (Stream stream  = new FileStream(filename, FileMode.Create, FileAccess.Write)) {
                saver.Serialize(stream, m_reRecords);
            }
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