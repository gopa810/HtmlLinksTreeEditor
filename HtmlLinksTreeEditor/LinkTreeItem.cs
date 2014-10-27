using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlLinksTreeEditor
{
    public class LinkTreeItem
    {
        public int Level = 0;
        public string Text = string.Empty;
        public string Target = string.Empty;
        public string Descr = string.Empty;
        public int NextBlockID = -1;

        public string XmlText
        {
            get
            {
                return PlainToXml(Text);
            }
            set
            {
                Text = XmlToPlain(value);
            }
        }

        public string XmlTarget
        {
            get { return Target; }
            set { Target = value; }
        }

        public string XmlDescr
        {
            get { return PlainToXml(Descr); }
            set { Descr = XmlToPlain(value); }
        }

        public static string PlainToXml(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string XmlToPlain(string s)
        {
            return s.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&amp;", "&");
        }

        public override string ToString()
        {
            return Text.PadLeft(Text.Length + Level, ' ');
        }
    }
}
