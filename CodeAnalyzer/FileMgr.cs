///////////////////////////////////////////////////////////////////////
// FileMgr.cs -  fetches for the required files from the directory   //
//            based on the patterns and options provided by the user //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win8.1, SP1                    //
// Application: Code Analyzer                                        //
// Author:      Monisha Lakshmipathi,mlakshmi@syr.edu                //
//                Syracuse University                                //
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 */
//File Manager Module is responsible for retrieving file names from the given path that needs 
//to be analyzed. It provides support for retrieving files with a specific file name pattern in a 
//directory and its sub directories, if required recursively.
//
// *   
// * Build command:
// *   csc /D:TEST_PARSER Parser.cs IRulesAndActions.cs RulesAndActions.cs \
// *                      Semi.cs Toker.cs

//*/version 1.0 9th oct 2014
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeAnalysis
{
    public class FileMgr
    {
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
        private static bool recurse = false;
        string[] newFiles;
        public bool recursion
        {
            get
            {
                return recurse;
            }
            set
            {
                recurse = value;
            }
        }
        public void findFiles(string path)
        {
             if (patterns.Count == 0)
               addPattern("*.cs");
            foreach (string pattern in patterns)
            {
                newFiles = Directory.GetFiles(path, pattern);
                for (int i = 0; i < newFiles.Length; ++i)
                    newFiles[i] = Path.GetFullPath(newFiles[i]);
                files.AddRange(newFiles);
            }
            if (recurse)
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                    findFiles(dir);
            }
        }

        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }

        public List<string> getFile()
        {
            return files;
        }

        public string[] getFiles(string path, List<string> patterns)
        {
            FileMgr fm = new FileMgr();
            try
            {
                foreach (string pattern in patterns)
                    fm.addPattern(pattern);
                fm.findFiles(path);

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
            return fm.getFile().ToArray();
        }



#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            Console.Write("\n  Testing FileMgr Class");
            Console.Write("\n =======================\n");

            FileMgr fm = new FileMgr();
            fm.addPattern("*.cs");
            fm.findFiles("../../");
            List<string> files = fm.getFile();
            foreach (string file in files)
                Console.Write("\n  {0}", file);
            Console.Write("\n\n");
        
        }
#endif
    }
}
