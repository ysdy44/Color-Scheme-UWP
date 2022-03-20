using Color_Scheme.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Windows.UI;

namespace Color_Scheme
{
    public static partial class XML
    {

        public static XDocument SaveItems(IEnumerable<Item> items)
        {
            return new XDocument
            (
                // Set the document definition for xml.
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement
                (
                    "Root",
                     from item
                     in items
                     select XML.SaveItem("Item", item)
                )
            );
        }

        public static IEnumerable<Item> LoadItems(XDocument document)
        {
            if (document.Element("Root") is XElement root)
            {
                if (root.Elements("Item") is IEnumerable<XElement> items)
                {
                    return
                    (
                        from item
                        in items
                        select XML.LoadItem(item)
                    );
                }
            }

            return null;
        }

    }
}