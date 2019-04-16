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

namespace DFASimulator {

    public interface IEDUFlexStream {
        int NextChar();

        int GetChar(int index);

        int SeekPosition(int index);

        int Size { get; }

        string SourceName { get; }
    }

    /// <summary>
    /// This class offers buffering for the EduFlex input file. StreamReader class is not
    /// seekable by design. Although it is possible to random access data in a streamreader through
    /// its embedding stream interface it inquires significant overhead .
    /// We need support efficient seeking operations on character data in our EduFlexStream class.
    /// StreamReader is the easiest way to access text data from a stream however it doen't support
    /// seeking. Thus to efficiently
    /// </summary>
    public class EDUFlexStream {
        private StreamReader m_inputStream;
        /// <summary>
        /// Buffered Stream characters
        /// </summary>
        private char[] m_bufferedCharData;
        /// <summary>
        /// Unicode of Buffered Stream Characters 
        /// </summary>
        private int[] m_bufferedData;
        /// <summary>
        /// Pointer into the EDUFlex character stream. Always point to
        /// the next character
        /// </summary>
        private int m_charBufferIndex;
        /// <summary>
        /// The size of content into the character buffer
        /// </summary>
        private int m_charBufferSize;
        /// <summary>
        /// The size of byte buffer
        /// </summary>
        private int m_byteBufferSize;
        /// <summary>
        /// The offset of buffer content into the stream
        /// </summary>
        private int m_streamOffset;
        /// <summary>
        /// EDUFlexStream Buffer size
        /// </summary>
        public readonly int mc_BUFFERSIZE;
        /// <summary>
        /// Indicates whether EOF reached in the underline stream
        /// </summary>
        private bool m_EOF=false;

        private Encoding m_streamEncoding;

        public bool MEof => m_EOF;

        public EDUFlexStream(StreamReader stream, int bufferSize = 4096) {
            mc_BUFFERSIZE = bufferSize;
            m_bufferedCharData = new char[bufferSize];
            // Depending on the character encoding the size of 
            // m_bufferedData array differs
            m_bufferedData = null;
            m_charBufferIndex = 0;
            m_charBufferSize = 0;
            m_streamOffset = 0;
            m_EOF = false;
            m_inputStream = stream;
        }

        public int NextChar() {
            byte[] byteUnicodeData;
            int nextChar;
            //1. Check if this index reached the end of the buffer
            // In this case request the next packet of data from the
            // StreamReader
            if (m_charBufferIndex == mc_BUFFERSIZE || m_charBufferSize == 0) {
                m_streamOffset += m_charBufferSize = m_inputStream.Read(m_bufferedCharData,
                    m_streamOffset + m_charBufferSize, mc_BUFFERSIZE);
                m_charBufferIndex = 0;
                // Input stream encoding is valid after the first read
                m_streamEncoding = m_inputStream.CurrentEncoding;
                byteUnicodeData = m_streamEncoding.GetBytes(m_bufferedCharData, 0, m_charBufferSize);
                m_bufferedData = new int[byteUnicodeData.Length+1];
                for (int i = 0; i < m_bufferedData.Length; i++) {
                    m_bufferedData[i] = (int) m_bufferedCharData[i];
                }
                m_byteBufferSize = m_bufferedData.Length;
            }
            nextChar = m_bufferedData[m_charBufferIndex++];

            if (m_bufferedCharData[m_charBufferIndex] == 0) {
                m_EOF = true;
            }

            return nextChar;
        }

    }

    public class DFASimulator {
        private EDUFlexStream m_inputCharStream;
        private FA m_dfa;
        Stack<CGraphNode> m_stateStack = new Stack<CGraphNode>();
        CGraphNode m_currentState;
        StringBuilder m_lexeme = new StringBuilder();
        private bool m_deadend;

        public DFASimulator(FA mDfa, EDUFlexStream inputStream) {
            m_dfa = mDfa;
            m_inputCharStream = inputStream;
        }

        public int yylex() {
            CGraphNode nextState;

            // Initialization
            int nextChar = -1;
            m_lexeme.Clear();
            m_currentState = m_dfa.M_Initial;


            while (!m_deadend && !m_inputCharStream.MEof) {
                if (m_dfa.MFinal.Contains(m_currentState)) {
                    m_stateStack.Clear();
                }
                m_stateStack.Push(m_currentState);
                nextChar = m_inputCharStream.NextChar();
                m_lexeme.Append((char)nextChar);
                m_currentState = m_dfa.GetTransitionTarget(m_currentState, nextChar);
             }

            while (true) {
                m_currentState = m_stateStack.Pop();
                m_lexeme.Remove(m_lexeme.Length - 1, 1);
                // Rollback();
            }

            if (m_dfa.MFinal.Contains(m_currentState)) {
                return 1;
            }
            else {
                return -1;
            }
        }
    }



    class Program {
        static void Main(string[] args) {
            EDUFlexStream istream = new EDUFlexStream(new StreamReader("source.txt"));

            Facade.VerifyRegExp(args);

            DFASimulator dfaSimulator = new DFASimulator(Facade.MsMinDfa,istream);
            dfaSimulator.yylex();

        }
    }
}
