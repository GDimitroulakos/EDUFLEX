using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphLibrary;
using Parser;
using Parser.UOPCore;
using SeekableStreamReader;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace DFASimulator {


    /// <summary>
    /// This class offers buffering for the EduFlex input file. StreamReader class is not
    /// seekable by design. Although it is possible to random access data in a streamreader through
    /// its embedding stream interface it inquires significant overhead .
    /// We need support efficient seeking operations on character data in our EduFlexStream class.
    /// StreamReader is the easiest way to access text data from a stream however it doen't support
    /// seeking. Thus to efficiently
    /// </summary>
    public class EDUFlexStream : BufferedStreamTextReader {
        public EDUFlexStream(Stream mIstream, int bufferSize = 4096,
                             Encoding mStreamEncoding = null) : base(mIstream, bufferSize, mStreamEncoding) {

        }

    }
    
    public partial class DFASimulatorFinite {
        private Dictionary<uint, RERecord> m_reRecords;
        private Dictionary<uint, DFAStateSingleton> m_dfaMultiStates = new Dictionary<uint, DFAStateSingleton>();

        private int m_streamPointer = 0;
        private EDUFlexStream m_inputCharStream;

        public DFASimulatorFinite(Dictionary<uint, RERecord> reRecords, EDUFlexStream inputCharStream) {
            m_reRecords = DeSerializeEDUFLEXOutput("EDUFLEX.out");
            m_inputCharStream = inputCharStream;

            /*foreach (KeyValuePair<uint, RERecord> pair in reRecords) {
                m_dfaMultiStates[pair.Key] = new DFAState();
                ResetDFASimulatorState(pair.Key);
            }*/
        }

        public Dictionary<uint, RERecord> DeSerializeEDUFLEXOutput(string filename) {

            BinaryFormatter res = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                return (Dictionary<uint, RERecord>)(res.Deserialize(stream));
            }
        }

        public int yylex() {
            uint? renum;
            int nextChar = 0;
            m_streamPointer = 0;
            while (!m_inputCharStream.M_EOF) {

            }
            return 0;
        }
    }

    class Program {
        static void Main(string[] args) {
            EDUFlexStream istream = new EDUFlexStream(new FileStream("source.txt", FileMode.Open));

            Facade.VerifyRegExp(args);

            if (Facade.GetOperationModeCode()) {
                LexerMulti dfaSimulator = new LexerMulti(Facade.M_ReRecords, istream);
                dfaSimulator.Continue((_, __) => {
                    var edustream = (EDUFlexStream) __;
                    LexerState state = (LexerState) _;
                    return !edustream.M_EOF && state.M_Match;
                });
            } else {
                DFASimulator dfaSimulator = new DFASimulator(Facade.M_ReRecords[0].M_MinDfa, istream);
                dfaSimulator.Step();
            }



        }
    }
}
