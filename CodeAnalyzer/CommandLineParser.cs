///////////////////////////////////////////////////////////////////////
// CommandLineParser.cs - parses the command line for various options// 
//and patterns of the input files                                    //
// ver 1.0                                                          //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: Code Analyzer                                        //
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
//Author:       Monisha Lakshmipathi, Syracuse University            //
//               mlakshmi@syr.edu                                    //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *   CommandLineParser-To parse the command line arguments
 */
/* Required Files:
 *   Executive.cs
 *   
 * Build command:
 *   csc /D:TEST_COMMANDLINEPARSER Parser.cs IRulesAndActions.cs RulesAndActions.cs \
 *                      Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 09 Oct 2014
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CodeAnalysis
{
    public class CommandLineParser 
    {
        public string processPath(string[] arg) // fetches for path information
        {

            string path = null;
            if(arg.Length > 0)
            {
                for (int i = 0; i < arg.Length; i++)
                {
                    if (arg[i].Contains("../") || arg[i].Contains("./") || arg[i].Contains(":\\"))
                    {
                        path = arg[i];
                        return path;
                    }
                    else
                    {
                        path = "../../";
                        return path;
                    }
                }
             }
             else
             {
                  path = "../../";
                  return path;
             }
             return path;
        }

        public List<string> processPattern(string[] arg)   //*****fetches for pattern information**********
        {

            List<string> patterns = new List<string>();
            if(arg.Length >0)
            {
                for (int i = 0; i < arg.Length; i++)
                {

                    string input = arg[i];
                    bool isMatch = Regex.IsMatch(input, @"[A-Za-z0-9\*]+\.[A-Za-z\*]+$");
                    if (isMatch == true)
                    {
                        patterns.Add(arg[i]);
                    }
                }
            }
            else
            {
                patterns.Add("*.cs");
            }
            return patterns;
        }

        public List<string> processOptions(string[] arg)  //*fetches for options info***********
        {

            List<string> options = new List<string>();

            for (int i = 0; i < arg.Length; i++)
            {
                if (arg[i].Contains("/X") || arg[i].Contains("/R") || arg[i].Contains("/S") || arg[i].Contains("/x") || arg[i].Contains("/r") || arg[i].Contains("/s"))
                {
                    options.Add(arg[i]);
                }


            }
            return options;
        }

     
#if(TEST_COMMANDLINEPARSER)

        static void Main(string[] args)
        {
            string path = null;
            List<string> patterns = new List<string>();
            List<string> options = new List<string>();
            string [] args= "../../program.*";
            CommandLineParsing passArguments = new CommandLineParsing();
            path = passArguments.processPath(args);
            patterns = passArguments.processPattern(args);
            options = passArguments.processOptions(args);
        }
#endif

    }
}
