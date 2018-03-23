using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LeApiSnippetGenerator {
    class Program {

        const string baseUrl = "https://www.leadwerks.com/documentation/";
        static List<Object> topics = new List<Object>();

        static void Main(string[] args) {
            Task<string> tocData = GetData();
            tocData.Wait();

            GetApiReferenceObjects(tocData.Result);

            GenerateSnippets();
            Console.WriteLine(topics.Count);
            Console.ReadLine();
        }

        static async Task<string> GetData() {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl + "toc.xml"))
            using (HttpContent content = res.Content) {
                string data = await content.ReadAsStringAsync();
                if (data != null) {
                    Console.WriteLine(data);
                    return data;
                }
                return null;
            }
        }

        private static void GetApiReferenceObjects(string data) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);

            List<XmlNode> apiObjects = GetApiObjectNodes(xml);
            GetApiDataObjects(apiObjects);
        }

        private static List<XmlNode> GetApiObjectNodes(XmlDocument xml) {
            XmlNodeList xnList = xml.SelectNodes("/contents/topics/topic/title");
            foreach (XmlNode xn in xnList) {
                if (xn.InnerText == "API Reference") {
                    XmlNode apiObjects = xn.ParentNode.LastChild.LastChild.LastChild;
                    return apiObjects.ChildNodes.Cast<XmlNode>().ToList();
                }
            }
            return null;
        }

        private static void GetApiDataObjects(List<XmlNode> apiObjects) {
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
                    Console.WriteLine("Some weird object or comment.");
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
                            Console.WriteLine(string.Concat(new String('-', level * 2), output, " There are no functions for this class."));
                        }
                        else {
                            Console.WriteLine(string.Concat(new String('-', level * 2), output));

                            TraverseObject(subClass, level + 1);
                        }
                    }
                    else {
                        Task topicData = GetObjectData(fileName);
                        topicData.Wait();
                    }
                }
            }
        }

        static async Task GetObjectData(string fileName) {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(baseUrl + fileName + ".xml"))
            using (HttpContent content = res.Content) {
                string data = await content.ReadAsStringAsync();

                if (data != null) {
                    var topic = new Topic();
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(data);
                    Object obj = ObjectToXML(data.Replace("<?xml version=\"1.0\"?>", ""), topic.GetType());
                    if (obj != null) {
                        topics.Add(obj);
                    }
                }
            }
        }

        public static Object ObjectToXML(string xml, Type objectType) {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            Object obj = null;
            try {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp) {
                Console.WriteLine(exp.ToString());
                //Handle Exception Code
            }
            finally {
                if (xmlReader != null) {
                    xmlReader.Close();
                }
                if (strReader != null) {
                    strReader.Close();
                }
            }
            return obj;
        }

        private static XmlNode GetTopicsXmlNode(XmlNode apiObject) {
            foreach (XmlNode xn in apiObject.ChildNodes) {
                if (xn.Name == "topics" && xn.FirstChild != null) {
                    return xn;
                }
            }
            return null;
        }

        private static async Task GenerateSnippets() {
            //await Task.Delay(3000);

            StringBuilder allApiSnippets = new StringBuilder("{");
            using (StreamWriter file = File.CreateText("../../../../snippets/leadwerksapisnippets.json")) {
                foreach (var topicObject in topics) {
                    try {
                        Topic topic = (Topic)topicObject;
                        foreach (var luaSyntax in topic.Luasyntaxes.Syntax) {
                            allApiSnippets.Append(CreateSnippet(topicObject, luaSyntax));
                        }
                    }
                    catch (Exception) {
                        Console.WriteLine("Something wrong with: " + ((Topic)topicObject).Title.ToString());
                    }
                }
                allApiSnippets.Remove(allApiSnippets.Length-2,1);
                allApiSnippets.Append("}");
                file.Write(allApiSnippets.ToString()); ;
            }
        }

        private static string CreateSnippet(Object topicObject, string luaSyntax) {
            Topic topic = (Topic)topicObject;
            StringBuilder snippet = new StringBuilder();
            snippet.Append("\""+ topic.Title + "\": {\n");
            snippet.Append("\"prefix\": \"" + topic.Title + "\",\n");
            snippet.Append(string.Concat("\"description\": \"", GetCleanXmlString(topic), "\",\n"));
            snippet.Append("\"body\": [\"" + luaSyntax + "\"]\n");
            snippet.Append("},\n");
            return snippet.ToString();
        }

        private static string GetCleanXmlString(Topic topic) {
            if (topic.Description.Contains('"'))
                Console.WriteLine();
            string clean = topic.Description.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\'");
            Console.WriteLine(clean);
            return clean;
        }
    }
}
