///////////////////////////////////////////////////////////////////////
// Analyser.cs -  Handles all the analysis                           //    
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Precision T7400, Win8.1, SP1                    //
// Application: Code analyzer                                        //
// Source:     Jim Fawcett, CST 4-187, Syracuse University           //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
//Author:      Monisha Lakshmipathi, Syracuse University             //
//                                                                   //
///////////////////////////////////////////////////////////////////////
/*
 * Code Analyzer- responsible for all the type and relationship analysis of the code.
This Code Analyzer uses the services of fileMgr,Rules and Actions package
 */
/* Required Files:
 *    RulesAndActions.cs,IRulesAndActions.cs.
 *   
 * Build command:
 *   csc /D:TEST_ANALYZER Parser.cs IRulesAndActions.cs RulesAndActions.cs \
 *                      Semi.cs Toker.cs
 *   
 * Maintenance History:
 * 
 * ver 1.0 : 9 Oct 2014
 * - first release
 */
/*
* Analyzer.cs - Manages Code Analysis
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class Analyzer
    {
       
        public List<relationelem> rel_BetweenFiles = new List<relationelem>(); //list to store info between files
        public List<Elem> typesBetweenFiles = new List<Elem>(); //list to store info between files
        public void doAnalysis(string[] files)
        {
            Console.Write("\n  Demonstrating Parser");
            Console.Write("\n ======================\n");

            //**processes it for every file one after another*********

            foreach (object file in files)
            {
                Console.Write("\n  Processing file {0}\n", file as string);

                CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                semi.displayNewLines = false;
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                    return;
                }

                Console.Write("\n  Type and Function Analysis");
                Console.Write("\n ----------------------------\n");

                CodeAnalysis.BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
                Parser parser = builder.build();

                try
                {
                    while (semi.getSemi())
                        parser.parse(semi); //parses every semi expression
                   
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                Repository rep = Repository.getInstance();
                List<Elem> table = rep.locations;
                typesBetweenFiles = rep.locations;
                foreach (Elem e in table)
                {
                    Console.Write("\n  {0,10}, {1,25}, {2,5}, {3,5}", e.type, e.name, e.begin, e.end);
                }
                Console.WriteLine();
                Console.Write("\n\n  That's all folks!\n\n");
                semi.close();
            }
        }

        //*****second parse************//
        public void reldoAnalysis(string[] files)
        {
            foreach (object file in files)
            {
                Console.Write("\n  Processing file {0}\n", file as string);

                CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
                semi.displayNewLines = false;
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                    return;
                }

                
                Repository rep_inst = Repository.getInstance();
                Repository.Copy_buffer = typesBetweenFiles;

                BuildCodeAnalyzer_types builder_ty = new BuildCodeAnalyzer_types(semi);
                Parser parser = builder_ty.build();
                // sends every semi again for relationship analysis
                try
                {
                    while (semi.getSemi())
                        parser.parse(semi);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }

                //stores all the updation to the type and relationship list back to the repository
                Repository_types rep = Repository_types.getInstance();
                List<relationelem> table = rep.classrelations;
                rel_BetweenFiles = (rep.classrelations);
              
            }
        }




#if(TEST_ANALYZER)
        
        void Main(string[] args)
        {
            FileMgr file = new FileMgr();
            List<string> patterns = new List<string>();
            string path = ("../../");
            patterns.Add("*.*");
            string[] files = file.getFiles(path, patterns);
            Analyzer analyzer = new Analyzer();
            analyzer.doAnalysis(files);
            analyzer.reldoAnalysis(files);
             foreach (Elem e in typesBetweenFiles)
          {
              Console.Write("\n  {0,-10}  {1,-25}  {2,-5}  {3,-5}  {4,-5}  {5,5} ", e.type, e.name, e.begin, e.end, e.size, e.prog_complexity);
             
          }
              
         Console.WriteLine("\n \n The relationships between the types above are as follows\n");
           foreach (relationelem e in list_relations)
           {
               if (e.isInheritance == true)
               {
                   Console.WriteLine("\n  {0} is inheriting from {1}", e.b_class,e.a_class);
               }

           }


           foreach (relationelem e in list_relations)
           {
               if (e.isAggregation == true)
               {
                   Console.Write("\n  {0} is aggregating  {1}", e.a_class, e.b_class);
               }

           }



           foreach (relationelem e in list_relations)
           {
               if (e.isComposition == true)
               {
                   Console.Write("\n  {0} Composition  {1}", e.a_class, e.b_class);
               }

           }
        }
#endif
    }
}


