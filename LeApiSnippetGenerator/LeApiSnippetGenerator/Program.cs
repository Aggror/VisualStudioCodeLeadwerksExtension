using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static List<Topic> topics = new List<Topic>();
        static List<string> functions = new List<string>();

        static void Main(string[] args) {
            Task<string> tocData = GetData();
            tocData.Wait();

            GetApiReferenceObjects(tocData.Result);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            GenerateSnippets();
            sw.Stop();

            Console.WriteLine("Elapsed= {0}", sw.Elapsed);
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

                        //if (apiObject.FirstChild.InnerText == "Entity") {
                            TraverseObject(apiObject, 1);
                        //}
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
            Console.WriteLine();
            string currentClass = apiObject.FirstChild.InnerXml;
            XmlNode topicsNode = GetTopicsXmlNode(apiObject);

            if (topicsNode != null) {
                foreach (XmlNode topicNode in topicsNode.ChildNodes) {
                    bool isClass = false;
                    string fileName = "";
                    XmlNode subClass = null;

                    foreach (XmlNode topicSubNode in topicNode.ChildNodes) {
                        if (topicSubNode.Name == "title")
                            Console.WriteLine(string.Concat(new String('-', level * 2), topicSubNode.InnerText));
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

                            TraverseObject(topicNode, level + 1);
                        }
                    }
                    else {
                        Task topicData = GetObjectData(fileName, currentClass);
                        topicData.Wait();
                    }
                }
            }
        }

        static async Task GetObjectData(string fileName, string currentClass) {
            string url = baseUrl + fileName + ".xml";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage res = await client.GetAsync(url))
            using (HttpContent content = res.Content) {
                string data = await content.ReadAsStringAsync();

                if (res.StatusCode != System.Net.HttpStatusCode.OK) {
                    Console.WriteLine("URL error: " + url, ". CurrentClass: " + currentClass);
                }
                else {
                    if (!string.IsNullOrEmpty(data)) {
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(data);
                        Topic topic = new Topic();
                        Object obj = ObjectToXML(data.Replace("<?xml version=\"1.0\"?>", ""), topic.GetType());
                        if (obj != null) {
                            topic = (Topic)obj;
                            topic.classType = currentClass;
                            topics.Add(topic);
                        }
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
            StringBuilder allApiSnippets = new StringBuilder("{");
            using (StreamWriter file = File.CreateText("../../../../snippets/leadwerksapisnippets.json")) {
                foreach (var topic in topics) {
                    try {
                        foreach (var luaSyntax in topic.Luasyntaxes.Syntax) {
                            allApiSnippets.Append(CreateSnippet(topic, luaSyntax, true));
                            allApiSnippets.Append(CreateSnippet(topic, luaSyntax, false));
                        }
                    }
                    catch (Exception) {
                        Console.WriteLine("Something wrong with: " + (topic).Title.ToString());
                    }
                }
                allApiSnippets.Remove(allApiSnippets.Length - 2, 1);
                allApiSnippets.Append("}");
                file.Write(allApiSnippets.ToString()); ;
            }
        }

        private static string CreateSnippet(Topic topic, string luaSyntax, bool includeClass) {
            StringBuilder snippet = new StringBuilder();
            string function = (includeClass ? topic.classType + ":" : "") + topic.Title;
            function = ConvertToUniqueFunction(function);
            functions.Add(function);

            snippet.Append(string.Concat("\"", function, "\": {\n"));
            snippet.Append(string.Concat("\"prefix\": \"", function, "\",\n"));
            snippet.Append(string.Concat("\"description\": \"", GetCleanXmlString(topic.Description), "\",\n"));
            snippet.Append("\"body\": [\"" + GetCleanXmlString(luaSyntax) + "\"]\n");
            snippet.Append("},\n");
            return snippet.ToString();
        }

        private static string ConvertToUniqueFunction(string function) {
            if (functions.Contains(function)) 
                function = ConvertToUniqueFunction(function + " ");
            
            return function;
        }

        private static string GetCleanXmlString(string xmlToClean) {
            if (xmlToClean.Contains('"'))
                Console.WriteLine();
            string clean = xmlToClean.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("\"", "'").Replace("  ", " ").Trim();
            Console.WriteLine(clean);
            return clean;
        }
    }
}
