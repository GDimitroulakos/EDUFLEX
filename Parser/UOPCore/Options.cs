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
    public class Options<T> where T : struct {
        private int m_options;

        public void Set(T option) {
            m_options = ToInt<T>(option) | m_options;
        }
        public void Reset(T option) {
            m_options = ~ToInt<T>(option) & m_options;
        }

        public bool IsSet(T option) {
            int temp;
            temp=(m_options & ToInt<T>(option)) & ToInt<T>(option);
            return Convert.ToBoolean(temp);
        }

        private int ToInt<T>(T options) {
            return Convert.ToInt32(options);
        }
    }
}
