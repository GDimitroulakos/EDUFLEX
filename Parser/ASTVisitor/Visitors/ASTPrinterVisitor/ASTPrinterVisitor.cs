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
using System.Text;
using System.Threading.Tasks;

namespace MiniC1.ASTVisitor.Visitors {
    class ASTPrinter : CASTAbstractConcreteVisitor<int> {

        private StreamWriter m_outputStream;

        private string m_outputFile;

        private int ms_clusterCounter = 0;



        public ASTPrinter(string file) {
            m_outputFile = Path.GetFileNameWithoutExtension(file) + "AST.dot";
            m_outputStream = new StreamWriter(m_outputFile);
        }

        public override int VisitCompileUnit(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;

            m_outputStream.WriteLine("digraph {\n");

            // Visit fundef context
            if (current.GetNumberOfContextElements(ContextType.CT_COMPILEUNIT_FUNDEF) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_COMPILEUNIT_FUNDEF.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_COMPILEUNIT_FUNDEF)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT ) {
                        m_outputStream.WriteLine(" [fillcolor = "+ CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName+ "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit statements context
            if (current.GetNumberOfContextElements(ContextType.CT_COMPILEUNIT_STAT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_COMPILEUNIT_STAT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_COMPILEUNIT_STAT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitCompileUnit(currentNode);

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

        public override int VisitExpressionStatement(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit fundef context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_EXPRESSION) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_EXPRESSION.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_EXPRESSION)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitExpressionStatement(currentNode);

            return 0;
        }

        public override int VisitForStatement(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit init context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_FOR_INIT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_FOR_INIT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_FOR_INIT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.Write(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit fin context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_FOR_FIN) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_FOR_FIN.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_FOR_FIN)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit step context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_FOR_STEP) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_FOR_STEP.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_FOR_STEP)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit body context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_FOR_BODY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_FOR_BODY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_FOR_BODY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitForStatement(currentNode);

            return 0;
        }

        public override int VisitWhileStatement(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit condition context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_WHILE_CONDITION) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_WHILE_CONDITION.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_WHILE_CONDITION)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit body context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_WHILE_BODY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_WHILE_BODY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_WHILE_BODY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitWhileStatement(currentNode);

            return 0;
        }

        public override int VisitIfStatement(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit if context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_IF_CONDITION) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_IF_CONDITION.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_IF_CONDITION)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit then body context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_IF_THENBODY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_IF_THENBODY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_IF_THENBODY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit else body context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_IF_ELSEBODY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_IF_ELSEBODY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_IF_ELSEBODY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitIfStatement(currentNode);

            return 0;
        }

        public override int VisitCompoundStatement(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit body context
            if (current.GetNumberOfContextElements(ContextType.CT_STATEMENT_COMPOUND) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_STATEMENT_COMPOUND.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_STATEMENT_COMPOUND)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitCompoundStatement(currentNode);

            return 0;
        }

        public override int VisitFunctionDefinition(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;

            m_outputStream.WriteLine("\"{0}\"->\"{1}\"",currentNode.M_Parent.M_Label,currentNode.M_Label);

            // Visit type spec context
            if (current.GetNumberOfContextElements(ContextType.CT_FUNCTIONDEF_TYPESPEC) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_FUNCTIONDEF_TYPESPEC.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_FUNCTIONDEF_TYPESPEC)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit fun id context
            if (current.GetNumberOfContextElements(ContextType.CT_FUNCTIONDEF_FUNID) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_FUNCTIONDEF_FUNID.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_FUNCTIONDEF_FUNID)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            // Visit params context
            if (current.GetNumberOfContextElements(ContextType.CT_FUNCTIONDEF_PARAMS) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_FUNCTIONDEF_PARAMS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_FUNCTIONDEF_PARAMS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit body context
            if (current.GetNumberOfContextElements(ContextType.CT_FUNCTIONDEF_BODY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_FUNCTIONDEF_BODY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_FUNCTIONDEF_BODY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitFunctionDefinition(currentNode);

            return 0;
        }

        public override int VisitAssignmentExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit left context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_ASSIGNMENT_LEFT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_ASSIGNMENT_LEFT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_ASSIGNMENT_LEFT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit right context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_ASSIGNMENT_RIGHT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_ASSIGNMENT_RIGHT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_ASSIGNMENT_RIGHT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitAssignmentExpresssion(currentNode);

            return 0;
        }

        public override int VisitArrayMulti(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit array context
            if (current.GetNumberOfContextElements(ContextType.CT_MDARRAY_ARRAY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_MDARRAY_ARRAY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_MDARRAY_ARRAY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitArrayMulti(currentNode);

            return 0;
        }

        public override int VisitArray1D(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit array context
            if (current.GetNumberOfContextElements(ContextType.CT_1DARRAY_ARRAY) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_1DARRAY_ARRAY.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_1DARRAY_ARRAY)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }

                }
                m_outputStream.WriteLine("}");
            }
            base.VisitArrayMulti(currentNode);

            return 0;
        }

        public override int VisitFunctionCallExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit funid context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_FUNCTIONCALL_FUNID) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_FUNCTIONCALL_FUNID.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_FUNCTIONCALL_FUNID)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit params context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_FUNCTIONCALL_PARAMS) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_FUNCTIONCALL_PARAMS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_FUNCTIONCALL_PARAMS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitFunctionCallExpresssion(currentNode);

            return 0;
        }

        public override int VisitArrayReferenceExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit array base context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_ARRREF_ARRAYBASE) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_ARRREF_ARRAYBASE.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_ARRREF_ARRAYBASE)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit array offset context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_ARRREF_INDEΧ) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_ARRREF_INDEΧ.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_ARRREF_INDEΧ)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitArrayReferenceExpresssion(currentNode);

            return 0;
        }

        public override int VisitLogicalNotExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit expression context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_LOGICALNOT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_LOGICALNOT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_LOGICALNOT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitLogicalNotExpresssion(currentNode);

            return 0;
        }

        public override int VisitPostfixExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit expression context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_POSTFIX) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_POSTFIX.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_POSTFIX)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitPostfixExpresssion(currentNode);

            return 0;
        }

        public override int VisitPrefixExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit expression context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_PREFIX) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_PREFIX.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_PREFIX)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitPrefixExpresssion(currentNode);

            return 0;
        }

        public override int VisitMulDivExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit left context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_MULDIV_LEFT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_MULDIV_LEFT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_MULDIV_LEFT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit right context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_MULDIV_RIGHT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_MULDIV_RIGHT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_MULDIV_RIGHT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitMulDivExpresssion(currentNode);

            return 0;
        }

        public override int VisitAddSubExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit left context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_ADDSUB_LEFT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_ADDSUB_LEFT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_ADDSUB_LEFT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit right context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_ADDSUB_RIGHT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_ADDSUB_RIGHT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_ADDSUB_RIGHT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitAddSubExpresssion(currentNode);

            return 0;
        }

        public override int VisitLogicalANDExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit left context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_LOGICALAND_LEFT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_LOGICALAND_LEFT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_LOGICALAND_LEFT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit right context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_LOGICALAND_RIGHT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_LOGICALAND_RIGHT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_LOGICALAND_RIGHT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitLogicalANDExpresssion(currentNode);

            return 0;
        }

        public override int VisitLogicalORExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit left context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_LOGICALOR_LEFT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_LOGICALOR_LEFT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_LOGICALOR_LEFT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit right context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_LOGICALOR_RIGHT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_LOGICALOR_RIGHT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_LOGICALOR_RIGHT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }
            base.VisitLogicalORExpresssion(currentNode);

            return 0;
        }

        public override int VisitComparisonExpresssion(CASTElement currentNode) {
            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit left context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_COMPARISON_LEFT) > 0) { // if not a leaf
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_COMPARISON_LEFT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_COMPARISON_LEFT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            // Visit right context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_COMPARISON_RIGHT) > 0) {
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_COMPARISON_RIGHT.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_COMPARISON_RIGHT)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }

                }
                m_outputStream.WriteLine("}");
            }
            base.VisitComparisonExpresssion(currentNode);

            return 0;
        }

        public override int VisitParenthesizedExpresssion(CASTElement currentNode) {

            CASTComposite current = currentNode as CASTComposite;
            string clusterName;
            string contextName;
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);

            // Visit expression context
            if (current.GetNumberOfContextElements(ContextType.CT_EXPRESSION_PARENTHESIS) > 0) { // if not a leaf
                clusterName = "cluster" + ms_clusterCounter++;
                contextName = ContextType.CT_EXPRESSION_PARENTHESIS.ToString();
                m_outputStream.WriteLine(
                    "subgraph {0} {{\n node [style=filled,color=white];\n style=filled;\n color=lightgrey;\n label = \"{1}\";\n",
                    clusterName, contextName);
                foreach (CASTElement element in current.GetContextChildren(ContextType.CT_EXPRESSION_PARENTHESIS)) {
                    m_outputStream.WriteLine("\"{0}\"", element.M_Label);
                    if (CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_Color != Color.C_DEFAULT) {
                        m_outputStream.WriteLine(" [fillcolor = " + CConfigurationSettings.m_nodeTypeConfiguration[element.M_NodeType].M_ColorName + "]");
                    }
                }
                m_outputStream.WriteLine("}");
            }

            base.VisitParenthesizedExpresssion(currentNode);

            return 0;
        }

        public override int VisitSTRING(CASTElement currentNode) {
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            return base.VisitSTRING(currentNode);
        }

        public override int VisitINTEGER(CASTElement currentNode) {
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            return base.VisitINTEGER(currentNode);
        }

        public override int VisitFLOAT(CASTElement currentNode) {
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            return base.VisitFLOAT(currentNode);
        }

        public override int VisitIDENTIFIER(CASTElement currentNode) {
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            return base.VisitIDENTIFIER(currentNode);
        }

        public override int VisitTYPESPECIFIER(CASTElement currentNode) {
            m_outputStream.WriteLine("\"{0}\"->\"{1}\"", currentNode.M_Parent.M_Label, currentNode.M_Label);
            return base.VisitTYPESPECIFIER(currentNode);
        }
    }
}
