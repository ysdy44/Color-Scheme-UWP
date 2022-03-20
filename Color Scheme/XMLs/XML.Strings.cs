using Color_Scheme.Model;
using Color_Scheme.Strings;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Color_Scheme
{
    public static partial class XML
    {

        public static XElement SaveStrings(string elementName, StringList strings)
        {
            return new XElement(elementName,
            (
                from item
                in strings
                select new XElement("String", new XAttribute("Key", item.Key.ToString()), new XAttribute("Value", item.Value))
            ));
        }

        public static StringList LoadStrings(XElement element)
        {
            return new StringList(element.Elements("String").ToDictionary
            (
                item => Enum.TryParse(item.Attribute("Key").Value, out LangCode code) ? code : default,
                item => item.Attribute("Value").Value
            ));
        }

    }
}