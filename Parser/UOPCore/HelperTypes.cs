using System;
using System.Collections.Generic;
using System.Text;
using Parser.UOPCore;
using RangeIntervals;

namespace Parser {
    public class CCharRange : Range<Int32> {
        private int m_minChar;
        private int m_maxChar;

        public CCharRange() {
        }

        public char MCharLower {
            get { return Convert.ToChar(m_minChar); }
        }

        public char MCharUpper {
            get { return Convert.ToChar(m_maxChar); }
        }

        public string M_RangeString {
            get {
                StringBuilder b = new StringBuilder();
                for (int i = m_minChar; i <= m_maxChar; i++) {
                    b.Append(Convert.ToChar(i));
                }
                return b.ToString();
            }
        }

        public CCharRange(char min, char max) : base(Convert.ToInt32(min), Convert.ToInt32(max) + 1) {
            m_minChar = Convert.ToInt32(min);
            m_maxChar = Convert.ToInt32(max);
        }
        public CCharRange(int min, int max) : base(min, max + 1) {
            m_minChar = min;
            m_maxChar = max;
        }

        public bool IsCharInRange(Int32 ch) {
            if (ch <= m_maxChar && ch >= m_minChar) {
                return true;
            }
            return false;
        }

        public override int Next(int x) {
            return x + 1;
        }

        public override int Prev(int x) {
            return x - 1;
        }

        public override string ToString() {
            if (m_maxChar != m_minChar) {
                return Convert.ToChar(m_minChar).ToString() + "-" + Convert.ToChar(m_maxChar).ToString();
            }
            else {
                return Convert.ToChar(m_minChar).ToString();
            }

        }

        public override int Min {
            get { return m_minChar; }
            set {
                base.Min = value;
                m_minChar = value;
            }
        }

        public override int Max {
            get { return m_maxChar; }
            set {
                base.Max = value;
                m_maxChar = value;
            }
        }



        public int MMinChar {
            get { return m_minChar; }
        }

        public int MMaxChar {
            get { return m_maxChar; }
        }
    }

    public class CCharRangeSet : RangeSetO<CCharRange, Int32> {
        private bool isNegation;

        public CCharRangeSet(char c, bool isNegation = false) : base(true) {
            AddRange(new CCharRange(c, c));
        }

        public CCharRangeSet(CCharRange range, bool isNegation = false) : base(true) {
            AddRange(range);
        }

        public static explicit operator CCharRangeSet(CCharRange range) {
            return new CCharRangeSet(range);
        }

        public static explicit operator CCharRangeSet(char c) {
            return new CCharRangeSet(c);
        }
        public CCharRangeSet(bool isNegation) : base(true) {
            this.isNegation = isNegation;
        }

        public static explicit operator System.String(CCharRangeSet set) {
            return set.ToString();
        }

        public bool IsCharInSet(Int32 ch) {
            foreach (CCharRange range in this) {
                if (range.IsCharInRange(ch)) {
                    return true;
                }
            }
            return false;
        }

        public override string ToString() {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("[");
            if (isNegation) {
                strBuild.Append("^");
            }
            foreach (var range in this) {
                strBuild.Append((range as CCharRange).ToString());
            }
            strBuild.Append("]");
            return strBuild.ToString();
        }
    }

    public struct TextSpan {
        private UInt32 m_startLine;
        private UInt32 m_endLine;
        private UInt32 m_startColumn;
        private UInt32 m_endColumn;

        public uint M_StartLine {
            get => m_startLine;
            set => m_startLine = value;
        }

        public uint M_EndLine {
            get => m_endLine;
            set => m_endLine = value;
        }

        public uint M_StartColumn {
            get => m_startColumn;
            set => m_startColumn = value;
        }

        public uint M_EndColumn {
            get => m_endColumn;
            set => m_endColumn = value;
        }
    }

    
}
