using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;


namespace AGTIAADDIN.Utility
{
    public static class XmlEdit
    {
       
        public static bool CreateCSV(string filePath)
        {
            try
            {
                var tagrecords = new List<TagRecord>();
                //Open document
                var document = new XmlDocument();
                document.Load(filePath);
                var nsmgr = new XmlNamespaceManager(document.NameTable);
                nsmgr.AddNamespace("ns", @"http://www.siemens.com/automation/Openness/SW/Interface/v4");
                var tags = document.SelectSingleNode("//SW.Blocks.GlobalDB");
                var dbNumber = document.SelectSingleNode("//SW.Blocks.GlobalDB/AttributeList/Number").InnerText;
                var sectionReturn = tags.SelectSingleNode(".//ns:Section[@Name='Static']", nsmgr);
                //var dbNumber = tags.SelectSingleNode("/AttributeList/InterfaceModifiedDate");
                var members = sectionReturn.SelectNodes("./ns:Member", nsmgr);
                foreach (XmlNode member in members)
                {
                    var name = member.Attributes.GetNamedItem("Name").Value;
                    var dataType = member.Attributes.GetNamedItem("Datatype").Value;
                    var offset = member.ChildNodes[0].ChildNodes[0].InnerText;
                    var byteOffset = int.Parse(offset) / 8;
                    var bitOffset = int.Parse(offset) % 8;
                    if (dataType == "Bool")
                    {
                        tagrecords.Add(new TagRecord { TagName = name, Address = "DB" + dbNumber + "," + "X" + byteOffset + "." + bitOffset });
                        //tagrecords.Add(new TagRecord { TagName = name, Address = "DB" + dbNumber + "," + dataType.ToUpper() + byteOffset });
                    }
                    else if (dataType.ToUpper() == "TIME")
                    {
                        tagrecords.Add(new TagRecord { TagName = name, Address = "DB" + dbNumber + "," + "REAL" + byteOffset });
                    }
                    else
                    {
                        tagrecords.Add(new TagRecord { TagName = name, Address = "DB" + dbNumber + "," + dataType.ToUpper() + byteOffset });
                    }
                }

                var csv = new StringBuilder();
                foreach (var item in tagrecords)
                {
                    //Suggestion made by KyleMit
                    var newLine = string.Format("\"{0}\";\"{1}\"", item.TagName, item.Address);
                    csv.AppendLine(newLine);
                }
                File.WriteAllText("C:\\Temp\\CSV\\" + dbNumber + ".csv", csv.ToString(),Encoding.UTF8);

                return true;
            }
            catch (Exception e)
            {
                
                throw e;
                
            }
            
        }
    }
}