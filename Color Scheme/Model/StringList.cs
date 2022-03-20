using Color_Scheme.Strings;
using System.Collections.Generic;
using System.Linq;

namespace Color_Scheme.Model
{
    public class StringList : Dictionary<LangCode, string>
    {
        public StringList() { }
        public StringList(IDictionary<LangCode, string> dictionary) : base(dictionary) { }

        public StringList Clone() => new StringList(this.ToDictionary
        (
            item => item.Key,
            item => item.Value
        ));
    }
}