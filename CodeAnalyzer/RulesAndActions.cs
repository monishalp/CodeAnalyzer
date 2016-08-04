///////////////////////////////////////////////////////////////////////////
// RulesAndActions.cs -  it contains all the rules and the corresponding //
//actions for a given set of grammar constructs                          //
// ver 2.3                                                               //
// Language:    C#, 2008, .Net Framework 4.0                             //
// Platform:    Dell Precision T7400, Win7, SP1                          //
// Application: Code Analyzer                                            // 
// Author:        Monisha Lakshmipathi,Syracuse University               //
//                  mlakshmi@syr.edu                                     //
// Source:      Jim Fawcett, CST 4-187, Syracuse University              //
//              (315) 443-3948, jfawcett@twcny.rr.com                    //
///////////////////////////////////////////////////////////////////////////
/*
/*
 * Package Operations:
 * -------------------
 * RulesAndActions package contains all of the Application specific
 * code required for most analysis tools.
 *
 * It defines the following Four rules which each have a
 * grammar construct detector and also a collection of IActions:
 *   - DetectNameSpace rule
 *   - DetectClass rule
 *   - DetectFunction rule
 *   - DetectScopeChange
 *   -DetectEnum
 *    -DetectDelegate
 *    Calculate Complexity
 *   Three actions - some are specific to a parent rule:
 *   - Print
 *   - PrintFunction
 *   - PrintScope
 * 
 * The package also defines a Repository class for passing data between
 * actions and uses the services of a ScopeStack, defined in a package
 * of that name.
 *
 * Note:
 * This package does not have a test stub since it cannot execute
 * without requests from Parser.
 *  
 */
/* Required Files:
 *   IRuleAndAction.cs, Parser.cs, ScopeStack.cs.
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRuleAndAction.cs RulesAndActions.cs \
 *                      ScopeStack.cs Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 2.3 : 9 Oct 2014
 * -Added rules to detect enums,delegates and complexity
 * Added rules to detect inheritance and aggregation relationships.
 * ver 2.2 : 24 Sep 2011
 * - modified Semi package to extract compile directives (statements with #)
 *   as semiExpressions
 * - strengthened and simplified DetectFunction
 * - the previous changes fixed a bug, reported by Yu-Chi Jen, resulting in
 * - failure to properly handle a couple of special cases in DetectFunction
 * - fixed bug in PopStack, reported by Weimin Huang, that resulted in
 *   overloaded functions all being reported as ending on the same line
 * - fixed bug in isSpecialToken, in the DetectFunction class, found and
 *   solved by Zuowei Yuan, by adding "using" to the special tokens list.
 * - There is a remaining bug in Toker caused by using the @ just before
 *   quotes to allow using \ as characters so they are not interpreted as
 *   escape sequences.  You will have to avoid using this construct, e.g.,
 *   use "\\xyz" instead of @"\xyz".  Too many changes and subsequent testing
 *   are required to fix this immediately.
 * ver 2.1 : 13 Sep 2011
 * - made BuildCodeAnalyzer a public class
 * ver 2.0 : 05 Sep 2011
 * - removed old stack and added scope stack
 * - added Repository class that allows actions to save and 
 *   retrieve application specific data
 * - added rules and actions specific to Project #2, Fall 2010
 * ver 1.1 : 05 Sep 11
 * - added Repository and references to ScopeStack
 * - revised actions
 * - thought about added folding rules
 * ver 1.0 : 28 Aug 2011
 * - first release
 *
 * Planned Modifications:
 * --------------------------------------------------
 * - add folding rules:
 *   - CSemiExp returns for(int i=0; i<len; ++i) { as three semi-expressions, e.g.:
 *       for(int i=0;
 *       i<len;
 *       ++i) {
 *     The first folding rule folds these three semi-expression into one,
 *     passed to parser. 
 *   - CToker returns operator[]( as four distinct tokens, e.g.: operator, [, ], (.
 *     The second folding rule coalesces the first three into one token so we get:
 *     operator[], ( 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace CodeAnalysis
{
    public class Elem  // holds scope information
    {
        private string type_;
        private string name_;
        private int begin_;
        private int end_;
        private int size_;
        private int prog_complexity_;

        public string type
        {
            get
            {
                return type_;
            }
            set
            {
                type_ = value;
            }
        }


        public string name
        {
            get
            {
                return name_;
            }

            set
            {
                name_ = value;
            }
        }


        public int begin
        {
            get
            {
                return begin_;
            }
            set
            {
                begin_ = value;
            }
        }

        public int end
        {
            get
            {
                return end_;
            }
            set
            {
                end_ = value;
            }
        }


        public int size
        {
            get
            {
                return size_;
            }

            set
            {
                size_ = value;
            }
        }


        public int prog_complexity
        {
            get
            {
                return prog_complexity_;
            }
            set
            {
                prog_complexity_ = value;
            }
        }


        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-5}", begin.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", end.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }
    }
    public class relationelem  // holds relationship information
    {

        private bool isInheritance_;// checks if inheritance relationship
        private bool isUsingType_;//checks if using relationship
        private string a_class_;// tells about which 2 classes is involved
        private string b_class_;// tells about which 2 classes is involved
        private bool isAggregation_;//checks if aggregation relationship
        private bool isComposition_;//checks if composition relationship

        public bool isInheritance
        {
            get
            {
                return isInheritance_;
            }
            set
            {
                isInheritance_ = value;
            }

        }

        public bool isUsingType
        {
            get
            {
                return isUsingType_;
            }

            set
            {
                isUsingType_ = value;
            }
        }

        public string a_class
        {
            get
            {
                return a_class_;
            }
            set
            {
                a_class_ = value;
            }
        }

        public string b_class
        {
            get
            {
                return b_class_;
            }
            set
            {
                b_class_ = value;
            }
        }

        public bool isAggregation
        {
            get
            {
                return isAggregation_;
            }
            set
            {
                isAggregation_ = value;
            }
        }


        public bool isComposition
        {
            get
            {
                return isComposition_;
            }
            set
            {
                isComposition_ = value;
            }
        }


        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", a_class));
            temp.Append(String.Format("{0,-10}", b_class));
            temp.Append(String.Format("{0,-5}", isInheritance.ToString()));  // line of scope start
            temp.Append(String.Format("{0,-5}", isComposition.ToString()));
            temp.Append(String.Format("{0,-5}", isAggregation.ToString()));
            temp.Append(String.Format("{0,-5}", isUsingType.ToString()));// line of scope end
            temp.Append("}");
            return temp.ToString();
        }

    }

    ///************************Repository *****************/////////////////


    public class Repository
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> locations_ = new List<Elem>(); //holds types
        static Repository instance;
        List<Elem> class_copy = new List<Elem>(); //holds the info about all the classes
        static List<Elem> Copy_buffer_ = new List<Elem>(); //used to pass values from the betweetfiles list to 2nd parse list 
        private int prog_comp_count_ = 0;  //used to calculate complexity
        public int prog_comp_count
        {
            get
            {
                return prog_comp_count_;
            }
            set
            {
                prog_comp_count_ = value;
            }
        }

        public Repository()
        {
            instance = this;
        }

        public static Repository getInstance()
        {
            return instance;
        }

        // provides all actions access to current semiExp
        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        //provides all actions access to current list of Copy_buffer which contains all the information about type analysis
        public static List<Elem> Copy_buffer
        {
            get
            {
                return Copy_buffer_;
            }

            set
            {
                Copy_buffer_ = value;
            }
        }




        public List<Elem> classLocations // to store all the class information in one list
        {
            get { return class_copy; }
        }

        // semi gets line count from toker who counts lines
        // while reading from its source
        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }

        // the locations table is the result returned by parser's actions of type analysis

        public List<Elem> locations
        {
            get { return locations_; }
        }


    }
    //************************************Repository to store the relationship info****************//
    public class Repository_types
    {
        ScopeStack<Elem> stack_ = new ScopeStack<Elem>();
        List<Elem> locations_ = new List<Elem>(); //holds types
        static Repository_types instance;
        //List<Elem> class_copy = new List<Elem>(); //holds the info about all the classes
        //static List<Elem> Copy_buffer_ = new List<Elem>(); //used to pass values from the betweetfiles list to 2nd parse list 
        List<relationelem> classrelations_ = new List<relationelem>(); //holds all the relationship info


        public Repository_types()
        {
            instance = this;
        }

        public static Repository_types getInstance()
        {
            return instance;
        }
        // provides all actions access to current semiExp

        public CSsemi.CSemiExp semi
        {
            get;
            set;
        }

        // semi gets line count from toker who counts lines
        // while reading from its source

        public int lineCount  // saved by newline rule's action
        {
            get { return semi.lineCount; }
        }
        public int prevLineCount  // not used in this demo
        {
            get;
            set;
        }
        // enables recursively tracking entry and exit from scopes

        public ScopeStack<Elem> stack  // pushed and popped by scope rule's action
        {
            get { return stack_; }
        }


        // the locations table is the result returned by parser's actions


        public List<relationelem> classrelations
        {
            get { return classrelations_; }
        }


    }




    /////////////////////////////////////////////////////////
    // pushes type info into the stack when entering scope


    public class PushStack : AAction
    {
        Repository repo_;

        public PushStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            elem.type = semi[0];  // expects type
            elem.name = semi[1];  // expects name
            elem.begin = repo_.semi.lineCount - 1;

            elem.prog_complexity = 0; ///CHANGED TO 0

            if (elem.type == "control" || elem.name == "anonymous")
                return;
            else if (elem.type == "delegate")
            {
                elem.end = repo_.semi.lineCount - 1;
            }
            else
            {
                elem.end = 0;
                repo_.stack.push(elem);
            }
            repo_.locations.Add(elem);
            if ((elem.type == "class") || (elem.type == "struct") || (elem.type == "enum") || (elem.type == "interface"))
                repo_.classLocations.Add(elem);

            if (AAction.displaySemi)
            {
                Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                Console.Write("entering ");
                string indent = new string(' ', 2 * repo_.stack.count);
                Console.Write("{0}", indent);
                this.display(semi); // defined in abstract action
            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }
    /////////////////////////////////////////////////////////
    // pushes relationship info into stack when entering scope

    public class PushStack_rel : AAction
    {
        Repository_types repo_;
        Repository rep;

        relationelem temp_relation = new relationelem();

        public PushStack_rel(Repository_types repo)
        {
            repo_ = repo;

        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Repository_types reporel_ = Repository_types.getInstance();

            //************************checks for inheritance***********************
            if (semi.Contains("inheritance") != -1)
            {
                int loc = semi.Contains("inheritance");
                if (loc != -1)
                {
                    Elem e = new Elem();
                    e.type = semi[loc + 3];
                    e.name = semi[loc + 1];
                    repo_.stack.push(e);
                    relationelem r_elem = new relationelem();
                    r_elem.a_class = semi[loc + 1].ToString();
                    r_elem.b_class = semi[loc + 2].ToString();
                    r_elem.isInheritance = true;
                    repo_.classrelations.Add(r_elem);
                }
            }

            //**************************** check for aggregation**************//
            if (semi.Contains("aggregation") != -1)
            {

                int loc = semi.Contains("aggregation");
                if (loc != -1)
                {
                    Elem e = new Elem();
                    e.type = semi[loc + 2];
                    e.name = semi[loc + 1];
                    repo_.stack.push(e);
                    int i = rep.locations.Count;
                    Elem temp = rep.locations[i - 1];
                    if (semi[1] != temp.name)
                    {
                        relationelem r_elem = new relationelem();
                        r_elem.a_class = temp.name;
                        r_elem.b_class = semi[1].ToString();
                        r_elem.isAggregation = true;
                        repo_.classrelations.Add(r_elem);
                    }
                }
            }


            if (AAction.displaySemi)
            {
                //Console.Write("\n  line# {0,-5}", repo_.semi.lineCount - 1);
                //Console.Write("entering ");
                //string indent = new string(' ', 2 * repo_.stack.count);
                //Console.Write("{0}", indent);
                //this.display(semi); // defined in abstract action

            }
            if (AAction.displayStack)
                repo_.stack.display();
        }
    }




    //***************************pops it out of the stack after encountering the end of scope/semi expression*************//


    public class PopStack : AAction
    {
        static Repository repo_;

        public PopStack(Repository repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Elem elem = new Elem();
            bool isFunc = false;

            try
            {
                elem = repo_.stack.pop();
                if (elem.type == "delegate")
                    return;
                if (semi[0] == "control" || semi[0] == "anonymous")
                {
                    elem.prog_complexity = elem.prog_complexity + 1;
                }
                if (elem.type == "function")
                {
                    elem.prog_complexity = repo_.prog_comp_count + 1;
                    repo_.prog_comp_count = 0;
                    Console.WriteLine("complexity: {0}", elem.prog_complexity);
                    isFunc = false;


                }

                // calculates the line count and sets the line count in the repository//
                for (int i = 0; i < repo_.locations.Count; ++i)
                {
                    Elem temp = repo_.locations[i];
                    if (elem.type == temp.type)
                    {
                        if (elem.name == temp.name)
                        {
                            if ((repo_.locations[i]).end == 0)
                            {
                                (repo_.locations[i]).end = repo_.semi.lineCount;
                                if (isFunc)
                                    (repo_.locations[i]).prog_complexity = elem.prog_complexity;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.Write("popped empty stack on semiExp: ");
                semi.display();
                return;
            }
            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
            local.Add(elem.type).Add(elem.name);
            if (local[0] == "control")
                return;

            if (AAction.displaySemi)
            {

                elem.end = repo_.semi.lineCount;
                elem.size = elem.end - elem.begin;
                string indent = new string(' ', 2 * (repo_.stack.count + 1));
                this.display(local); // defined in abstract action

            }
        }
    }




    ///////////////////////////////////////////////////////
    // action to print function signatures - not used in demo

    public class PrintFunction : AAction
    {
        Repository repo_;

        public PrintFunction(Repository repo)
        {
            repo_ = repo;
        }
        public override void display(CSsemi.CSemiExp semi)
        {
            for (int i = 0; i < semi.count; ++i)
                if (semi[i] != "\n" && !semi.isComment(semi[i]))
                    Console.Write("{0} ", semi[i]);
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // concrete printing action, useful for debugging

    public class Print : AAction
    {
        Repository_types repo_;

        public Print(Repository_types repo)
        {
            repo_ = repo;
        }
        public override void doAction(CSsemi.CSemiExp semi)
        {
            Console.Write("\n  line# {0}", repo_.semi.lineCount - 1);
            this.display(semi);
        }
    }
    /////////////////////////////////////////////////////////
    // rule to detect namespace declarations

    public class DetectNamespace : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("namespace");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    //**********rule to detect class****************
    public class DetectClass : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int indexCL = semi.Contains("class");
            int indexIF = semi.Contains("interface");
            int indexST = semi.Contains("struct");

            int index = Math.Max(indexCL, indexIF);
            index = Math.Max(index, indexST);
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }




    //**************** rule to detect enum declarations**************

    public class DetectEnum : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("enum");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }
    /////////////////////////////////////////////////////////
    // rule to dectect delegate*************


    public class DetectDelegate : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("delegate");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add(semi[index]).Add(semi[index + 2]);
                doActions(local);
                return true;
            }
            return false;
        }
    }

    /////////////////////////////////////////////////////////
    // rule to dectect function definitions

    public class DetectFunction : ARule
    {
        public static bool isSpecialToken(string token)
        {
            string[] SpecialToken = { "if", "for", "foreach", "while", "catch", "using" };
            foreach (string stoken in SpecialToken)
                if (stoken == token)
                    return true;
            return false;
        }
        public override bool test(CSsemi.CSemiExp semi)
        {
            if (semi[semi.count - 1] != "{")
                return false;

            int index = semi.FindFirst("(");
            if (index > 0 && !isSpecialToken(semi[index - 1]))
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.Add("function").Add(semi[index - 1]);
                doActions(local);
                return true;
            }
            return false;
        }
    }


    /////////////////////////////////////////////////////////
    // rule to dectect Complexity*******************


    public class DetectComp : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            string[] SplToks = { "for", "if", "else", "while", "foreach" };

            foreach (string toks in SplToks)
            {
                int index = semi.Contains(toks);
                if (index != -1)
                {
                    CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                    // create local semiExp with tokens for type and name
                    local.displayNewLines = false;
                    local.Add("control").Add("anonymous");
                    doActions(local);
                    return true;
                }
            }
            return false;
        }
    }





    //**************to detect inheritance****************
    public class DetectInheritance : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int loc = semi.Contains("class");
            if (loc != -1)
            {
                Repository rep = new Repository();
                if (semi[loc + 2] == ":")
                {
                    foreach (Elem e in Repository.Copy_buffer)
                    {
                        if ((e.name == semi[loc + 3]) && (e.type == "class"))
                        {
                            CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                            local.displayNewLines = false;
                            local.Add("inheritance").Add(semi[loc + 1]).Add(semi[loc + 3]).Add("class");
                            //  local.Add(semi[loc]).Add(semi[loc + 1]);
                            doActions(local);
                            return true;
                        }
                    }
                }
            } return false;
        }
    }


    //*****************to detect composition***************

    public class DetectComposition : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int loc = semi.Contains("new");
            if (loc != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                local.displayNewLines = false;
                local.Add(semi[loc]).Add(semi[loc + 1]);
                doActions(local);
                return true;
            }
            return false;/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////
        }
    }
    /// <summary>
    /// /////////////////******detect Aggregation***************
    /// </summary>

    public class DetectAggregation : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("new");
            Repository rep = new Repository();
            List<Elem> e = Repository.Copy_buffer;

            if (index != -1)
            {
                foreach (Elem e1 in e)
                {
                    if ((semi[index - 3] == e1.name) || (semi[index + 1] == e1.name))
                    {
                        CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                        // create local semiExp with tokens for type and name
                        local.displayNewLines = false;
                        local.Add("Aggregation").Add(semi[index + 1]);
                        doActions(local);
                        return true;
                    }
                }
            }
            return false;


        }
    }

    //******************to detect anonymous scope******************
    public class DetectAnonymousScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("{");
            if (index != -1)
            {
                CSsemi.CSemiExp local = new CSsemi.CSemiExp();
                // create local semiExp with tokens for type and name
                local.displayNewLines = false;
                local.Add("control").Add("anonymous");
                doActions(local);
                return true;
            }
            return false;
        }
    }




    /////////////////////////////////////////////////////////
    // detect leaving scope

    public class DetectLeavingScope : ARule
    {
        public override bool test(CSsemi.CSemiExp semi)
        {
            int index = semi.Contains("}");
            if (index != -1)
            {
                doActions(semi);

                return true;
            }
            return false;
        }
    }



    /// <summary>
    /// //////////////////*************build code analyser for type detection***********
    /// </summary>

    public class BuildCodeAnalyzer
    {
        static Repository repo = new Repository();

        public BuildCodeAnalyzer(CSsemi.CSemiExp semi)
        {
            repo.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = true;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack push = new PushStack(repo);

            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            //capture delegate
            DetectDelegate detectDL = new DetectDelegate();
            detectDL.add(push);
            parser.add(detectDL);


            // capture function info
            DetectFunction detectFN = new DetectFunction();
            detectFN.add(push);
            parser.add(detectFN);

            DetectEnum detectEN = new DetectEnum();
            detectEN.add(push);
            parser.add(detectEN);

            //capture complexity
            DetectComp complex = new DetectComp();
            complex.add(push);
            parser.add(complex);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);

            // handle leaving scopes
            DetectLeavingScope leave = new DetectLeavingScope();
            PopStack pop = new PopStack(repo);
            leave.add(pop);
            parser.add(leave);

            // parser configured
            return parser;
        }
    }
    //***************************build code analyser for relationship detection************
    public class BuildCodeAnalyzer_types
    {
        static Repository_types repo_rel = new Repository_types();

        public BuildCodeAnalyzer_types(CSsemi.CSemiExp semi)
        {
            repo_rel.semi = semi;
        }
        public virtual Parser build()
        {
            Parser parser = new Parser();

            // decide what to show
            AAction.displaySemi = true;
            AAction.displayStack = false;  // this is default so redundant

            // action used for namespaces, classes, and functions
            PushStack_rel push = new PushStack_rel(repo_rel);

            //detect Inheritance elem info
            DetectInheritance detectIn = new DetectInheritance();
            detectIn.add(push);
            parser.add(detectIn);
            // capture namespace info
            DetectNamespace detectNS = new DetectNamespace();
            detectNS.add(push);
            parser.add(detectNS);

            // capture class info
            DetectClass detectCl = new DetectClass();
            detectCl.add(push);
            parser.add(detectCl);

            // capture aggregation info
            DetectAggregation detectAg = new DetectAggregation();
            detectAg.add(push);
            parser.add(detectAg);

            DetectComposition detectComp = new DetectComposition();
            detectComp.add(push);
            parser.add(detectComp);

            // handle entering anonymous scopes, e.g., if, while, etc.
            DetectAnonymousScope anon = new DetectAnonymousScope();
            anon.add(push);
            parser.add(anon);
            return parser;
        }
    }
}
