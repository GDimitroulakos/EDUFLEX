using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Parser {
    class Program {
        static void Main(string[] args) {
            Facade.VerifyRegExp(args);
        }
    }
}
