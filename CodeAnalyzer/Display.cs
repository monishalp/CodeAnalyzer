///////////////////////////////////////////////////////////////////////
// Display.cs - Displays the types and relationship analysis output  //
// ver 1.0                                                          //
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
 * Displays the code analysis output done by the executive package
 */
/* Required Files:
 *   IRulesAndActions.cs, RulesAndActions.cs, Executive.cs
 *   
 * Build command:
 *   csc /D:TEST_DISPLAY Parser.cs IRulesAndActions.cs RulesAndActions.cs \
 *                      Semi.cs Toker.cs
 *   
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodeAnalysis
{
   public class Display
    {
      public  Display()
       {
        }
   
       /// /////////// Displays the type information //////////////
      
      public void DisplayReport(List<Elem> list_elem)
      {
          Repository rep = Repository.getInstance();
          Console.WriteLine("PROCESSING DISLAY PACKAGE");
          Console.WriteLine("--------------------------------------------------------------------");
          string type = "type";
          string name = "name";
          string begin = "start#";
          string end = "end#";
          string size = "size";
          string complex = "function complexity";
           Console.WriteLine("\n  {0,-9}  {1,-24}  {2,-4}  {3,-4}  {4,-4}  {5,4} ", type, name, begin, end, size, complex);
           Console.WriteLine("--------------------------------------------------------------------");
          foreach (Elem e in list_elem)
          {
              Console.Write("\n  {0,-10}  {1,-25}  {2,-5}  {3,-5}  {4,-5}  {5,5} ", e.type, e.name, e.begin, e.end, e.size, e.prog_complexity);
             
          }

      }

       
       /// /////Displays the relationship info of the types displayed during the first parse/////////////////
       
      public void complete(List<relationelem> list_relations)
        {

           Repository_types rep = Repository_types.getInstance();

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
#if(TEST_DISPLAY)
        static void Main(string[] args)
        {        
               List<Elem> typesFiles = new  List<Elem>();
               Elem el = new Elem();
               el.type="class";
               el.name= "A_class";
               el.begin = 1;
               el.end = 10;
               el.size = 9;
               el.prog_complexit =1;
               typesFiles.Add(el);
               List<relationelem> relFiles = new  List<relationelem>();
               relationelem r_el = new relationelem();
               r_el.a_class = "A_class";
               r_el.b_class = "B_class";
               r_el.isInheritance = true;
               r_el.isAggregation = false;
               r_el.isComposition = false;
                relFiles.Add(r_el)
               Display dis = new Display();
               dis.DisplayReport(typesFiles);
               dis.complete(relFiles);
        }
#endif
    }
    }

