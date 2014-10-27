using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace HtmlLinksTreeEditor
{
    public class LinkTreeDocument
    {
        public string FileName = string.Empty;
        public bool Modified = false;
        public ListBox View = null;
        public ArrayList Items = new ArrayList();
        public int BlockIdCounter = 1;

        public void Save()
        {
            if (FileName.Length == 0 || File.Exists(FileName) == false)
            {
                SaveAs();
            }
            else
            {
                DoSave();
            }
        }

        public void SaveAs()
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Filter = "LinkTreeEditor files (*.ltex)|*.ltex|All Files (*.*)|*.*";
            svd.DefaultExt = ".ltex";
            if (svd.ShowDialog() == DialogResult.OK)
            {
                FileName = svd.FileName;
                DoSave();
            }   
        }

        public void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LinkTreeEditor files (*.ltex)|*.ltex|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileName = ofd.FileName;
                DoOpen();
            }
        }

        private void DoOpen()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(FileName);

            XmlNode e = doc.ChildNodes[0];
            foreach (XmlElement item in e.ChildNodes)
            {
                LinkTreeItem li = new LinkTreeItem();

                if (item.HasAttribute("text"))
                    li.Text = item.GetAttribute("text");
                if (item.HasAttribute("descr"))
                    li.Descr = item.GetAttribute("descr");
                if (item.HasAttribute("target"))
                    li.Target = item.GetAttribute("target");
                if (item.HasAttribute("level"))
                    int.TryParse(item.GetAttribute("level"), out li.Level);

                Items.Add(li);
            }
        }

        private void DoSave()
        {
            XmlDocument doc = new XmlDocument();

            XmlElement main = doc.CreateElement("LinkTreeDocument");

            doc.AppendChild(main);

            XmlElement item = null;

            foreach (LinkTreeItem e in Items)
            {
                item = doc.CreateElement("LinkTreeItem");
                item.SetAttribute("text", e.Text);
                item.SetAttribute("target", e.Target);
                item.SetAttribute("descr", e.Descr);
                item.SetAttribute("level", e.Level.ToString());
                main.AppendChild(item);
            }

            Modified = false;

            doc.Save(FileName);
        }

        public void AddItem(LinkTreeItem li)
        {
            Modified = true;
            Items.Add(li);
        }

        public void ValidateLevels()
        {
            int last = -1;
            foreach (LinkTreeItem li in Items)
            {
                if (li.Level - last > 1)
                    li.Level = last + 1;
                last = li.Level;
            }
        }

        public string HtmlCode
        {
            get
            {
                int LEFT_INDENT = 40;
                StringBuilder sb = new StringBuilder();


                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<script type=\"text/javascript\">");
                sb.AppendLine("<!--");
                sb.AppendLine("function toogleElement(id)");
                sb.AppendLine("{");
                sb.AppendLine("   el = document.getElementById(id);");
                sb.AppendLine("   if (el.style.display == 'block')");
                sb.AppendLine("       el.style.display = 'none';");
                sb.AppendLine("   else");
                sb.AppendLine("       el.style.display = 'block';");
                sb.AppendLine("}");
                sb.AppendLine("-->");
                sb.AppendLine("</script>");
                sb.AppendLine("<style>");
                sb.AppendLine(".dirlink { cursor:pointer; color:#004444;text-decoration:underline;}");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                LinkTreeItem ni = null;
                int lastLevel = -1;

                for(int i = 0; i < Items.Count; i++)
                {
                    LinkTreeItem li = Items[i] as LinkTreeItem;
                    while (lastLevel > li.Level)
                    {
                        sb.AppendLine("</div>");
                        lastLevel--;
                    }
                    if (i < Items.Count - 1)
                        ni = Items[i + 1] as LinkTreeItem;
                    else ni = null;
                    if (ni != null && ni.Level > li.Level)
                    {
                        li.NextBlockID = BlockIdCounter;
                        BlockIdCounter++;
                    }
                    sb.AppendFormat("<p style='margin-left: {0}pt;'>", li.Level*LEFT_INDENT);
                    /*for (int k = 0; k < li.Level; k++)
                    {
                        sb.Append("&nbsp;&nbsp; ");
                    }*/
                    if (li.NextBlockID > 0)
                    {
                        sb.Append("<span class='dirlink' onClick=\"toogleElement('cont" + li.NextBlockID + "')\">");
                        sb.Append(li.XmlText);
                        sb.Append("</span>");
                        if (li.Target.Length > 0)
                        {
                            sb.AppendFormat(" &nbsp;&nbsp;.&nbsp;.&nbsp;.&nbsp; <a href=\"{0}\">[READ FULL]</a>", li.Target);
                        }
                    }
                    else
                    {
                        if (li.Target.Length > 0)
                            sb.AppendFormat("<a href=\"{0}\">{1}</a>", li.Target, li.XmlText);
                        else
                            sb.Append(li.XmlText);
                        if (li.Descr.Length > 0)
                        {
                            sb.AppendFormat("</p><p style='margin-left:{0}pt;font-style:italic;'>", li.Level*LEFT_INDENT + 15);
                            sb.AppendLine(li.XmlDescr);
                        }
                    }
                    sb.AppendLine("</p>");
                    if (li.NextBlockID > 0)
                    {
                        sb.AppendLine(string.Format("<div id=\"cont{0}\" style='display:none;line-height:5pt;'>", li.NextBlockID));
                    }
                    lastLevel = li.Level;
                }
                while (lastLevel > 0)
                {
                    sb.AppendLine("</div>");
                    lastLevel--;
                }

                sb.AppendLine();
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");

                return sb.ToString();
            }
        }
    }
}
