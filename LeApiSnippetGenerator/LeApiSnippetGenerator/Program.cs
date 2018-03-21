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
            CreateSnippets(apiObjects);
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

        private static void CreateSnippets(List<XmlNode> apiObjects) { 
            foreach (var apiObject in apiObjects) {
                var firstChild = apiObject.FirstChild;
                if(firstChild!= null){
                    if (apiObject.FirstChild.NextSibling.InnerText == "class") {
                        Console.WriteLine("main classes: " + apiObject.FirstChild.InnerText);

                        if(apiObject.FirstChild.InnerText == "Entity"){
                            CreateSnippet(apiObject, 1);
                        }
                    }
                    else if (apiObject.FirstChild.NextSibling.InnerText == "function") {
                        Console.WriteLine("Core function: " + apiObject.FirstChild.InnerText);
                    }
                }
                else {
                    Console.WriteLine("Some weird object or comment: " );
                }
            }
        }

        private static void CreateSnippet(XmlNode apiObject, int level) {
            foreach (XmlNode xn in apiObject.ChildNodes) {

                //Are there subclasses? Find with <topics>
                if (xn.Name == "topics" && xn.FirstChild != null) {
                    string output = "ApiObject '" + apiObject.FirstChild.InnerXml + " contains subclass '" + xn.FirstChild.FirstChild.InnerXml + "'.";
                    Console.WriteLine(string.Concat(new String('-', level), output));
                    
                    XmlNodeList subClasses = xn.ChildNodes;
                    foreach (XmlNode subClass in subClasses) {
                        CreateSnippet(subClass, level + 1);
                    }
                }
            }
            //var firstChild = apiObject.FirstChild;
            //    if (firstChild != null) {
            //        if (apiObject.FirstChild.NextSibling.InnerText == "class") {
            //            Console.WriteLine("main classes: " + apiObject.FirstChild.InnerText);
            //        }
            //        else if (apiObject.FirstChild.NextSibling.InnerText == "function") {
            //            Console.WriteLine("Core function: " + apiObject.FirstChild.InnerText);
            //            CreateSnippet(apiObject);
            //        }
            //    }
            //    else {
            //        Console.WriteLine("Some weird object or comment: ");
            //    }
            //}
        }
    }
}
