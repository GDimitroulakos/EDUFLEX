/*
MIT License

Copyright(c) [2016] [Grigoris Dimitroulakos]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Prints the AST Tree
/// </summary>
namespace Parser.ASTVisitor.Visitors {
    class ASTPrinter : CASTAbstractConcreteVisitor<int> {

        private StreamWriter m_outputStream;

        private string m_outputFile;

        private int ms_clusterCounter = 0;

        private bool flag = false;

        public ASTPrinter(string file) {
            m_outputFile = Path.GetFileNameWithoutExtension(file) + "AST.dot";
            m_outputStream = new StreamWriter(m_outputFile);
        }
        /*
        /* 27 #1# , CT_RANGE_MAX = 28,
            CT_NA*/

        /// <summary>
        /// Visits the node given as an argument and all of its contexts
        ///  and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitLexerDescription(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;

            m_outputStream.WriteLine("digraph {\n");

            // Visit context
            if (current.GetNumberOfContextElements(ContextType.CT_LEXERDESCRIPTION_BODY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_LEXERDESCRIPTION_BODY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_LEXERDESCRIPTION_BODY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitLexerDescription(currentNode);

            m_outputStream.WriteLine("}");
            m_outputStream.Close();

            if (true) {
                Process process = new Process();
                // Configure the process using the StartInfo properties.
                process.StartInfo.FileName = "dot.exe";
                process.StartInfo.Arguments = "-Tgif " + m_outputFile + " -o" +
                    Path.GetFileNameWithoutExtension(m_outputFile) + ".gif";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();// Waits here for the process to exit.
            }


            return 0;
        }
        /// <summary>
        /// Visits the node given as an argument and all of its contexts
        /// and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpStatement(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            
            // Visit regexpstatement_tokenname context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPSTATEMENT_TOKENNAME) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPSTATEMENT_TOKENNAME.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPSTATEMENT_TOKENNAME)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            // Visit regexpstatement_regexp context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPSTATEMENT_REGEXP) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPSTATEMENT_REGEXP.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPSTATEMENT_REGEXP)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            // Visit regexpstatement_actioncode context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPSTATEMENT_ACTIONCODE) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPSTATEMENT_ACTIONCODE.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPSTATEMENT_ACTIONCODE)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit regexpstatement_ID context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPSTATEMENT_TOKENNAME) > 0)
            {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPSTATEMENT_TOKENNAME.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                m_outputStream.WriteLine("}");
            }

            base.VisitRegexpStatement(currentNode);

            return 0;
        }
        /// <summary>
        /// Visits the node given as an argument and all of its contexts
        /// and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpAlternation(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit regexpalternation context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPALTERNATION_TERMS) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPALTERNATION_TERMS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPALTERNATION_TERMS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }


            base.VisitRegexpAlternation(currentNode);

            return 0;
        }
        /// <summary>
        /// Visits the node given as an argument and all of its contexts
        /// and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpConcatenation(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit concatenation context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPCONCATENATION_TERMS) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPCONCATENATION_TERMS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPCONCATENATION_TERMS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitRegexpConcatenation(currentNode);

            return 0;
        }
        /// <summary>
        /// Visits the node given as an argument and all of its contexts
        /// and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpClosure(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit closure's regexp context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPCLOSURE_REGEXP) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPCLOSURE_REGEXP.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPCLOSURE_REGEXP)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit closure's quantifier context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPCLOSURE_QUANTIFIER) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPCLOSURE_QUANTIFIER.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPCLOSURE_QUANTIFIER)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitRegexpClosure(currentNode);

            return 0;
        }

        public override int VisitClosureRange(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit closureRange's MIN context
            if (current.GetNumberOfContextElements(ContextType.CT_CLOSURERANGE_MIN) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_CLOSURERANGE_MIN.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_CLOSURERANGE_MIN)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit closure's quantifier context
            if (current.GetNumberOfContextElements(ContextType.CT_CLOSURERANGE_MAX) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_CLOSURERANGE_MAX.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_CLOSURERANGE_MAX)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitClosureRange(currentNode);
            return 0;
        }

        /// <summary>
        ///
        /// Visits the node given as an argument and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpbasicParen(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            // Visit regexpalternation context
            if (current.GetNumberOfContextElements(ContextType.CT_RGEXPBASIC_PAREN) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_RGEXPBASIC_PAREN.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                m_outputStream.WriteLine("}");
            }


            base.VisitRegexpbasicParen(currentNode);

            return 0;
        }

        /// <summary>
        /// Visits the node given as an argument and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">The node we want to print</param>
        /// <returns></returns>
        public override int VisitRegexpbasicSet(CASTElement currentNode) {
            CRegexpbasicSet current = currentNode as CRegexpbasicSet;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            //set negation
            if (current.M_IsSetNegation) {
                if (current.GetNumberOfContextElements(ContextType.CT_REGEXPBASIC_SETNEGATION) > 0) {
                    clusterName = "cluster" + ms_clusterCounter++;
                    contextName = ContextType.CT_REGEXPBASIC_SETNEGATION.ToString();
                    m_outputStream.WriteLine(
                        "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                        clusterName, contextName);
                    foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPBASIC_SETNEGATION)) {
                        m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                        if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                            m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                        }
                    }
                    m_outputStream.WriteLine("}");
                }
            }
            // Visit regexpbasic set context
            else if (current.GetNumberOfContextElements(ContextType.CT_REGEXPBASIC_SET) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPBASIC_SET.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPBASIC_SET)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitRegexpbasicSet(currentNode);

            return 0;
        }
        
        /// <summary>
        /// Its a leaf in the tree
        /// Visits the node given as an argument and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpbasicAnyexcepteol(CASTElement currentNode) {
            CASTLeaf<string> current = currentNode as CASTLeaf<string>;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            return 0;
        }
        /// <summary>
        /// Its a leaf in the tree
        /// Visits the node given as an argument and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpbasicChar(CASTElement currentNode) {
            CASTLeaf<string> current = currentNode as CASTLeaf<string>;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            return 0;
        }
        /// <summary>
        /// Its a leaf in the tree
        /// Visits the node given as an argument and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpbasicEndofline(CASTElement currentNode) {
            CASTLeaf<string> current = currentNode as CASTLeaf<string>;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            return 0;
        }
        /// <summary>
        /// Its a leaf in the tree
        /// Visits the node given as an argument and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpbasicStartofline(CASTElement currentNode) {
            CASTLeaf<string> current = currentNode as CASTLeaf<string>;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            return 0;
        }

        public override int VisitRegexpbasicAssertions(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit regexpbasic assertions context
            if (current.GetNumberOfContextElements(ContextType.CT_REGEXPBASIC_ASSERTIONS) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_REGEXPBASIC_ASSERTIONS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_REGEXPBASIC_ASSERTIONS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }


            base.VisitRegexpbasicAssertions(currentNode);

            return 0;
        }
        /// <summary>
        /// Its a leaf in the tree
        /// Visits the node given as an argument and prints the cluster affilliated to it
        /// </summary>
        /// <param name="currentNode">the node we want to print </param>
        /// 
        public override int VisitRegexpbasicString(CASTElement currentNode) {
            CASTLeaf<string> current = currentNode as CASTLeaf<string>;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            return 0;
        }

        public override int VisitAssertionFwdpos(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit assertionfwdpos context
            if (current.GetNumberOfContextElements(ContextType.CT_ASSERTION_FWDPOS) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_ASSERTION_FWDPOS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_ASSERTION_FWDPOS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }


            base.VisitAssertionFwdpos(currentNode);

            return 0;
        }

        public override int VisitAssertionFwdneg(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit assertionfwdneg context
            if (current.GetNumberOfContextElements(ContextType.CT_ASSERTION_FWDNEG) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_ASSERTION_FWDNEG.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_ASSERTION_FWDNEG)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }


            base.VisitAssertionFwdneg(currentNode);

            return 0;
        }

        public override int VisitAssertionBwdpos(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit assertionbwdpos context
            if (current.GetNumberOfContextElements(ContextType.CT_ASSERTION_BWDPOS) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_ASSERTION_BWDPOS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_ASSERTION_BWDPOS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }


            base.VisitAssertionBwdpos(currentNode);

            return 0;
        }

        public override int VisitAssertionBwdneg(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit assertionbwdneg context
            if (current.GetNumberOfContextElements(ContextType.CT_ASSERTION_BWDNEG) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_ASSERTION_BWDNEG.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_ASSERTION_BWDNEG)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }


            base.VisitAssertionBwdneg(currentNode);

            return 0;
        }


        public override int VisitRange(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit range_min context
            clusterName = "cluster" + ms_clusterCounter++;
            contextName = ContextType.CT_RANGE_MIN.ToString();
            m_outputStream.WriteLine(
                "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                clusterName, contextName);
            foreach (CASTElement element in current.GetContextChildren(ContextType.CT_RANGE_MIN)) {
                m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                    m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                }
            }

            m_outputStream.WriteLine("}");
            
            // Visit range_max context
            clusterName = "cluster" + ms_clusterCounter++;
            contextName = ContextType.CT_RANGE_MAX.ToString();
            m_outputStream.WriteLine(
                "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                clusterName, contextName);
            foreach (CASTElement element in current.GetContextChildren(ContextType.CT_RANGE_MAX)) {
                m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                    m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                }
            }
            m_outputStream.WriteLine("}");
            

            base.VisitRange(currentNode);

            return 0;
        }

        public override int VisitActionCode(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit range_min context
            if (current.GetNumberOfContextElements(ContextType.CT_ACTIONCODE) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_ACTIONCODE.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                m_outputStream.WriteLine("}");
           }
            return 0;
        }
        
        public override int VisitTerminal(CASTElement node) {
            CASTComposite current = node as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", node.M_Parent.M_Label, node.M_Label);

            // Visit range_min context
            return 0;
        }
        
    }
}
