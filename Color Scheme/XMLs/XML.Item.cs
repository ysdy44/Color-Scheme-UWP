using Color_Scheme.Model;
using System.Xml.Linq;
using Windows.UI;

namespace Color_Scheme
{
    public static partial class XML
    {

        public static XElement SaveItem(string elementName, Item item)
        {
            XElement element = new XElement(elementName);
            if (item.Strings != null) element.Add(XML.SaveStrings("Strings", item.Strings));
            element.Add(new XAttribute("A", item.Color.A));
            element.Add(new XAttribute("R", item.Color.R));
            element.Add(new XAttribute("G", item.Color.G));
            element.Add(new XAttribute("B", item.Color.B));
            if (item.Title != null) element.Add(new XAttribute("Title", item.Title));

            return element;
        }

        public static Item LoadItem(XElement element)
        {
            Color color = Colors.White;
            if (element.Attribute("A") is XAttribute a) color.A = (byte)(int)a;
            if (element.Attribute("R") is XAttribute r) color.R = (byte)(int)r;
            if (element.Attribute("G") is XAttribute g) color.G = (byte)(int)g;
            if (element.Attribute("B") is XAttribute b) color.B = (byte)(int)b;

            Item item = new Item
            {
                Color = color,
            };
            if (element.Attribute("Title") is XAttribute title) item.Title = title.Value;
            if (element.Element("Strings") is XElement strings) item.Strings = XML.LoadStrings(strings);

            return item;
        }

    }
}