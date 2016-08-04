///////////////////////////////////////////////////////////////////////
// Executive.cs - Executive module acts as a global controller,      //
//                which simply directs the control to various modules//
//                based on the actions to be performed.              //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win7, SP1                      //
// Application: code Analyzer                                        // 
// Author:        Monisha Lakshmipathi,Syracuse University           //
//                  mlakshmi@syr.edu                                 //
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * This module  directs the control to various modules 
based on the actions to be performed.
 *   
 */
/* Required Files:
 *   Analyzer.cs,FileMgs.cs,Display.cs
 *   
 * Build command:
 *   csc /D:TEST_PARSER Parser.cs IRulesAndActions.cs RulesAndActions.cs \
 *                      Semi.cs Toker.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 9 October 2014
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class Executive
    {
        static void Main(string[] args)
        {
            string path = null;
            List<string> patterns = new List<string>();
            List<string> options = null;
            FileMgr fileManager = new FileMgr();
            CommandLineParser arguments = new CommandLineParser();
            bool x_display = false;
            bool r_display = false;
            //****************fetches for the path,patterns and options given by the user***************//
            path = arguments.processPath(args);
            patterns = arguments.processPattern(args);
            options = arguments.processOptions(args);



            if ((options.Contains("/x")) || (options.Contains("/X"))) //Checks if the user wants the display in XML format
            {
                x_display = true;
            }
            if ((options.Contains("/s")) || (options.Contains("/S"))) //Checks if the user wants to search for the files in the sub directories 
            {
                fileManager.recursion = true;
            }
            if ((options.Contains("/r")) || (options.Contains("/R"))) //Checks if the user wants to view the relationships
            {
                r_display = true;
            }
            Analyzer anal = new Analyzer();

            Display DispObj = new Display();
            string[] files = fileManager.getFiles(path, patterns); //returns all the files
            anal.doAnalysis(files);
            anal.reldoAnalysis(files);

            xml_class xml_obj = new xml_class();
            if ((r_display != true) && (x_display != true))
            {
                DispObj.DisplayReport(anal.typesBetweenFiles);
                DispObj.complete(anal.rel_BetweenFiles);
               
            }
            if (r_display)
            {
                DispObj.complete(anal.rel_BetweenFiles);
            }

            if (x_display)
            {
                xml_obj.display(anal.typesBetweenFiles, anal.rel_BetweenFiles);
            }

        }

    }
}
