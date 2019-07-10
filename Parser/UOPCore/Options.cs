using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.UOPCore
{
    /// <summary>
    /// option codes must be powers of 2. Objects of this class
    /// can host at most 32 options
    /// </summary>
    public class Options<T> {
        private int m_options = 0;

        public Options(int initOptions=0) {
            m_options = initOptions;
        }

        public Options(T initOptions) {
            Set(initOptions);
        }

        public int M_Options => m_options;
        

        public void Set(T option) {
            m_options = ToInt(option) | m_options;
        }
        public void Reset(T option) {
            m_options = ~ToInt(option) & m_options;
        }

        public bool IsSet(T option) {
            int temp;
            temp = (m_options & ToInt(option)) & ToInt(option);
            return Convert.ToBoolean(temp);
        }

        private int ToInt(T options) {
            return Convert.ToInt32(options);
        }
    }
}
