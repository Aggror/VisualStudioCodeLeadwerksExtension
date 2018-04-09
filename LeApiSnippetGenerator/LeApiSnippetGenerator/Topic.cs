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
        public string x { get; set; }
        [XmlElement(ElementName = "y")]
        public string Y { get; set; }
        [XmlElement(ElementName = "z")]
        public string z { get; set; }
        [XmlElement(ElementName = "force")]
        public string Force { get; set; }
        [XmlElement(ElementName = "fx")]
        public string fx { get; set; }
        [XmlElement(ElementName = "fy")]
        public string fy { get; set; }
        [XmlElement(ElementName = "fz")]
        public string fz { get; set; }
        [XmlElement(ElementName = "px")]
        public string px { get; set; }
        [XmlElement(ElementName = "py")]
        public string py { get; set; }
        [XmlElement(ElementName = "pz")]
        public string pz { get; set; }
        [XmlElement(ElementName = "position")]
        public string position { get; set; }
        [XmlElement(ElementName = "torque")]
        public string torque { get; set; }
        [XmlElement(ElementName = "axis")]
        public string axis { get; set; }
        [XmlElement(ElementName = "rate")]
        public string rate { get; set; }
        [XmlElement(ElementName = "sound")]
        public string sound { get; set; }
        [XmlElement(ElementName = "range")]
        public string range { get; set; }
        [XmlElement(ElementName = "volume")]
        public string volume { get; set; }
        [XmlElement(ElementName = "pitch")]
        public string pitch { get; set; }
        [XmlElement(ElementName = "loopmode")]
        public string loopmode { get; set; }
        [XmlElement(ElementName = "index")]
        public string index { get; set; }
        [XmlElement(ElementName = "entity")]
        public string entity { get; set; }
        [XmlElement(ElementName = "p0")]
        public string p0 { get; set; }
        [XmlElement(ElementName = "p1")]
        public string p1 { get; set; }
        [XmlElement(ElementName = "pick")]
        public string pick { get; set; }
        [XmlElement(ElementName = "radius")]
        public string radius { get; set; }
        [XmlElement(ElementName = "closest")]
        public string closest { get; set; }
        [XmlElement(ElementName = "recursive")]
        public string recursive { get; set; }
        [XmlElement(ElementName = "sequence")]
        public string sequence { get; set; }
        [XmlElement(ElementName = "speed")]
        public string speed { get; set; }
        [XmlElement(ElementName = "blend")]
        public string blend { get; set; }
        [XmlElement(ElementName = "time")]
        public string time { get; set; }
        [XmlElement(ElementName = "name")]
        public string name { get; set; }
        [XmlElement(ElementName = "collisiontype")]
        public string collisiontype { get; set; }
        [XmlElement(ElementName = "r")]
        public string r { get; set; }
        [XmlElement(ElementName = "g")]
        public string g { get; set; }
        [XmlElement(ElementName = "b")]
        public string b { get; set; }
        [XmlElement(ElementName = "a")]
        public string a { get; set; }
        [XmlElement(ElementName = "color")]
        public string color { get; set; }
        [XmlElement(ElementName = "mode")]
        public string mode { get; set; }
        [XmlElement(ElementName = "Color")]
        public string Color { get; set; }
        [XmlElement(ElementName = "angle")]
        public string angle { get; set; }
        [XmlElement(ElementName = "move")]
        public string move { get; set; }
        [XmlElement(ElementName = "strafe")]
        public string strafe { get; set; }
        [XmlElement(ElementName = "jump")]
        public string jump { get; set; }
        [XmlElement(ElementName = "crouch")]
        public string crouch { get; set; }
        [XmlElement(ElementName = "maxacceleration")]
        public string maxacceleration { get; set; }
        [XmlElement(ElementName = "maxdecelleration")]
        public string maxdecelleration { get; set; }
        [XmlElement(ElementName = "detailed")]
        public string detailed { get; set; }
        [XmlElement(ElementName = "intensity")]
        public string intensity { get; set; }
        [XmlElement(ElementName = "mass")]
        public string mass { get; set; }
        [XmlElement(ElementName = "cx")]
        public string cx { get; set; }
        [XmlElement(ElementName = "cy")]
        public string cy { get; set; }
        [XmlElement(ElementName = "cz")]
        public string cz { get; set; }
        [XmlElement(ElementName = "ixx")]
        public string ixx { get; set; }
        [XmlElement(ElementName = "iyy")]
        public string iyy { get; set; }
        [XmlElement(ElementName = "material")]
        public string material { get; set; }
        [XmlElement(ElementName = "omega")]
        public string omega { get; set; }
        [XmlElement(ElementName = "shape")]
        public string shape { get; set; }
        [XmlElement(ElementName = "velocity")]
        public string velocity { get; set; }
        [XmlElement(ElementName = "yaw")]
        public string yaw { get; set; }
        [XmlElement(ElementName = "roll")]
        public string roll { get; set; }
    }

    [XmlRoot(ElementName = "topic")]
    public class Topic {
        public string classType;
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