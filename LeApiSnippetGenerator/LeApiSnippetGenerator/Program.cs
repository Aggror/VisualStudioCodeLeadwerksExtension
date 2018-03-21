using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LeApiSnippetGenerator {
    class Program {

        const string tocUrl = "https://www.leadwerks.com/documentation/toc.xml";

        static void Main(string[] args) {
            GetData();
            Console.ReadLine();
            Console.ReadKey();
        }

        static async void GetData() {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(tocUrl))
            using (HttpContent content = res.Content) {
                string data = await content.ReadAsStringAsync();
                if (data != null) {
                    Console.WriteLine(data);
                    ConvertToLeObjects(data);
                }
            }
        }

        private static void ConvertToLeObjects(string data) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);

            List<XmlNode> apiObjects = GetApiReferenceObjects(xml);
           
            CreateGlobalFunctions(apiObjects);
            //ProcessClasses();
            Console.WriteLine();
        }

        private static void CreateGlobalFunctions(List<XmlNode> apiObjects) {
            List<XmlNode> globalObjects = new List<XmlNode>();
            foreach (var apiObject in apiObjects) {
                var firstChild = apiObject.FirstChild;
                if(firstChild!= null){
                    if (apiObject.FirstChild.NextSibling.InnerText == "class") {
                        Console.WriteLine("class: " + apiObject.FirstChild.InnerText);
                    }
                    else if (apiObject.FirstChild.NextSibling.InnerText == "function") {
                        Console.WriteLine("function: " + apiObject.FirstChild.InnerText);
                    }
                }
                else {
                    Console.WriteLine("Some weird object or comment: " );
                }
            }
            //List<XmlNode> globalObjects = apiObjects.Where(a => a.FirstChild.NextSibling.InnerText == "function").ToList();

            //foreach (var globalFunctionNode in globalFunctionNodes) {
            //    Console.WriteLine(globalFunctionNode.LastChild.InnerText);
            //}
        }

        private static List<XmlNode> GetApiReferenceObjects(XmlDocument xml) {
            XmlNodeList xnList = xml.SelectNodes("/contents/topics/topic/title");
            foreach (XmlNode xn in xnList) {
                if (xn.InnerText == "API Reference") {
                    XmlNode apiObjects = xn.ParentNode.LastChild.LastChild.LastChild;
                    return apiObjects.ChildNodes.Cast<XmlNode>().ToList();
                }
            }
            return null;
        }
    }
}
