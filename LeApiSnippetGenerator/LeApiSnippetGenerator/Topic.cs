using System.Collections.Generic;
using System.Xml.Serialization;

namespace LeApiSnippetGenerator {
	[XmlRoot(ElementName = "cppsyntaxes")]
    public class Cppsyntaxes {
        [XmlElement(ElementName = "syntax")]
        public List<string> Syntax { get; set; }
    }

    [XmlRoot(ElementName = "luasyntaxes")]
    public class Luasyntaxes {
        [XmlElement(ElementName = "syntax")]
        public List<string> Syntax { get; set; }
    }

    [XmlRoot(ElementName = "parameters")]
    public class Parameters {
        [XmlElement(ElementName = "x")]
        public string X { get; set; }
        [XmlElement(ElementName = "y")]
        public string Y { get; set; }
        [XmlElement(ElementName = "z")]
        public string Z { get; set; }
        [XmlElement(ElementName = "position")]
        public string Position { get; set; }
        [XmlElement(ElementName = "global")]
        public string Global { get; set; }
    }

    [XmlRoot(ElementName = "topic")]
    public class Topic {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "cppsyntaxes")]
        public Cppsyntaxes Cppsyntaxes { get; set; }
        [XmlElement(ElementName = "luasyntaxes")]
        public Luasyntaxes Luasyntaxes { get; set; }
        [XmlElement(ElementName = "parameters")]
        public Parameters Parameters { get; set; }
        [XmlElement(ElementName = "remarks")]
        public string Remarks { get; set; }
        [XmlElement(ElementName = "luaexample")]
        public string Luaexample { get; set; }
        [XmlElement(ElementName = "cppexample")]
        public string Cppexample { get; set; }
    }
}