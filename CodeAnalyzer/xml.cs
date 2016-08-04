/////////////////////////////////////////////////////////////////////////
// xml.cs - Displays the type and relationship summary in the xml format//
// ver 1.0                                                              //
// Language:    C#, 2008, .Net Framework 4.0                            //
// Platform:    Dell Precision T7400, Win7, SP1                         //
// Application: Code Analyzer                                           // 
// Author:        Monisha Lakshmipathi,Syracuse University              //
//                  mlakshmi@syr.edu                                    //
//                                                                      //
/////////////////////////////////////////////////////////////////////////
/* Required Files:
 * , Parser.cs, Analyser.cs
 *   
 * 
 *   
 * Maintenance History:
 * ver 1.0 9 October 2014
 * - first release
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CStoker;
using System.Xml;

namespace CodeAnalysis
{
   public class xml_class
    {
        public void display(List<Elem> types,List<relationelem> relations)
        {
            
          XmlWriter xmlWriter = XmlWriter.Create("test_types.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("TypesCheck");
            foreach (Elem e in types)
            {
                int size = e.end - e.begin;
                xmlWriter.WriteStartElement("Output");
                xmlWriter.WriteAttributeString("Type", e.type);
                xmlWriter.WriteAttributeString("Name", e.name);
                String begin = Convert.ToString(e.begin);
                xmlWriter.WriteAttributeString("Begin", begin);
                String end = Convert.ToString(e.end);
                xmlWriter.WriteAttributeString("End", end);
                String typSize = Convert.ToString(size);
                xmlWriter.WriteAttributeString("Size", typSize);
                xmlWriter.WriteEndElement();
            }
          

            foreach (relationelem r in relations)
            {
                //Console.Write("\n  {0,10}, {1,25}, {2,5}, {3,5},{4,5},{5,5},{6,5}", r.isInheritance, r.isUsingType, r.a_class, r.b_class, r.isAggregation, r.isComposition);
                xmlWriter.WriteStartElement("RelOutput");
                string inhert = Convert.ToString(r.isInheritance);
                string rusing = Convert.ToString(r.isUsingType);
                string Aggre = Convert.ToString(r.isAggregation);
                string Compo = Convert.ToString(r.isComposition);
                xmlWriter.WriteAttributeString("IsInheritance", inhert);
                xmlWriter.WriteAttributeString("IsUsingType", rusing);
                xmlWriter.WriteAttributeString("classa", r.a_class);
                xmlWriter.WriteAttributeString("classb", r.b_class);
                xmlWriter.WriteAttributeString("a_class", Aggre);
                xmlWriter.WriteAttributeString("b_class", Compo);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

#if(TEST_XML_CLASS)
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
               xml_class xml_obj = new xml_class();
               xml_obj.display(List<Elem> typesFiles,List<relationelem> relFiles);
                 
        }
#endif


    }
               
}
