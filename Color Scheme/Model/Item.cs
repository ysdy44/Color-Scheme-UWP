using Color_Scheme.Strings;
using System;
using Windows.UI;

namespace Color_Scheme.Model
{
    public struct Item
    {
        public string Title;
        public StringList Strings { get; set; }
        public Color Color { get; set; }

        public string Summny => "A:" + this.Color.A + " " + "R:" + this.Color.R + " " + "G:" + this.Color.G + " " + "B:" + this.Color.B;
        public Color Foreground => this.Color.A > 64 ? this.Color.R + this.Color.G + this.Color.B < 640 || Math.Abs(this.Color.R - this.Color.G) + Math.Abs(this.Color.G - this.Color.B) + Math.Abs(this.Color.B - this.Color.R) > 100 ? Colors.White : Colors.Black : Colors.Gray;
        public string ActualTitle => this.GetTitle(LanguageResourceDictionary.Code);

        public string GetTitle(LangCode code)
        {
            if (this.Strings != null) return this.Strings[code];
            else if (this.Title == null) return string.Empty;
            else return this.Title;
        }
    }
}