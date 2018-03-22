using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LeApiSnippetGenerator {
    class Program {

        const string baseUrl = "https://www.leadwerks.com/documentation/";
        static List<Snippet> snippets = new List<Snippet>();

        static void Main(string[] args) {
            GetData();
            Console.ReadLine();
            Console.ReadKey();
        }

        static async void GetData() {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl+"toc.xml"))
            using (HttpContent content = res.Content) {
                string data = await content.ReadAsStringAsync();
                if (data != null) {
                    //Console.WriteLine(data);
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
                if (firstChild != null) {
                    if (apiObject.FirstChild.NextSibling.InnerText == "class") {
                        Console.WriteLine("main classes: " + apiObject.FirstChild.InnerText);

                        if (apiObject.FirstChild.InnerText == "Entity") {
                            TraverseObject(apiObject, 1);
                        }
                    }
                    else if (apiObject.FirstChild.NextSibling.InnerText == "function") {
                        Console.WriteLine("Core function: " + apiObject.FirstChild.InnerText);
                    }
                }
                else {
                    Console.WriteLine("Some weird object or comment: ");
                }
            }
        }

        private static void TraverseObject(XmlNode apiObject, int level) {
            string currentClass = apiObject.FirstChild.InnerXml;
            XmlNode topicsNode = GetTopicsXmlNode(apiObject);

            if (topicsNode != null) {
                foreach (XmlNode topicNode in topicsNode.ChildNodes) {
                    bool isClass = false;
                    string fileName = "";
                    XmlNode subClass = null;

                    foreach (XmlNode topicSubNode in topicNode.ChildNodes) {
                        if (topicSubNode.Name == "title")
                            Console.WriteLine(string.Concat(new String('-', level), topicSubNode.InnerText));
                        if (topicSubNode.Name == "type" && topicSubNode.InnerText == "class")
                            isClass = true;
                        if (topicSubNode.Name == "filename")
                            fileName = topicSubNode.InnerText;

                        //Are there subclasses? Find with <topics>
                        if (isClass && topicSubNode.Name == "topics") {
                            subClass = topicSubNode.FirstChild != null ? topicSubNode : null;
                            break;
                        }
                    }

                    if (isClass) {
                        string output = "ApiObject '" + currentClass + "' contains subclass '" + topicNode.FirstChild.InnerText + "'.";
                        if (subClass == null) {
                            Console.WriteLine(string.Concat(new String('-', level), output, " There are no functions for this class."));
                        }
                        else {
                            Console.WriteLine(string.Concat(new String('-', level), output));

                            TraverseObject(subClass, level + 1);
                        }
                    }
                    else {
                        GetObjectData(fileName);
                    }
                }
            }
        }

        static async void GetObjectData(string fileName) {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl + fileName +".xml"))
            using (HttpContent content = res.Content) {
                string data = await content.ReadAsStringAsync();

                if (data != null) {
                    var snippet = new Snippet();
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(data);

                    XmlNodeList topicNode = xml.SelectNodes("/topic");
                    foreach (XmlNode node in topicNode) {
                        switch (node.Name) {
                            case "title":
                                snippet.title = node.InnerText;
                                break;
                            case "description":
                                snippet.title = node.InnerText;
                                break;
                            case "syntaxes":
                                snippet.title = node.InnerText;
                                break;
                            case "returns":
                                snippet.title = node.InnerText;
                                break;
                            case "remarks":
                                snippet.title = node.InnerText;
                                break;
                            default:
                                break;
                        }
                    }


                    snippets.Add(snippet);
                    //Console.WriteLine(data);
                    //ConvertToLeObjects(data);
                }
            }
        }


        private static XmlNode GetTopicsXmlNode(XmlNode apiObject) {
            foreach (XmlNode xn in apiObject.ChildNodes) {
                if (xn.Name == "topics" && xn.FirstChild != null) {
                    return xn;
                }
            }
            return null;
        }
    }
}
