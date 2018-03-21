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

            XmlNode apiReference = GetApiReferenceNode(xml);

            Console.WriteLine();
        }

        private static XmlNode GetApiReferenceNode(XmlDocument xml) {
            XmlNodeList xnList = xml.SelectNodes("/contents/topics/topic/title");
            foreach (XmlNode xn in xnList) {
                if (xn.InnerText == "API Reference")
                    return xn;
            }
            return null;
        }
    }
}
