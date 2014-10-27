using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HtmlLinksTreeEditor
{
    public partial class MainForm : Form
    {
        private LinkTreeDocument Document = new LinkTreeDocument();

        public MainForm()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Document != null && Document.Modified)
            {
                Document.Save();
            }

            Document = new LinkTreeDocument();
            Document.View = listBox1;
            RefreshItems();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Document != null && Document.Modified)
            {
                Document.Save();
            }

            Document = new LinkTreeDocument();
            Document.View = listBox1;
            Document.Open();
            RefreshItems();

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Document != null)
                Document.Save();

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Document != null)
                Document.SaveAs();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Document != null && Document.Modified)
            {
                Document.Save();
            }
            Close();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i >= 0 && i > 0)
            {
                LinkTreeItem li = Document.Items[i] as LinkTreeItem;
                Document.Items.RemoveAt(i);
                Document.Items.Insert(i - 1, li);
                Document.Modified = true;
                listBox1.SelectedIndex = i - 1;
                Document.ValidateLevels();
                RefreshItems();
            }
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            LinkTreeItem li = SelectedItem;
            if (li != null && li.Level > 0)
            {
                li.Level--;
                Document.Modified = true;
                Document.ValidateLevels();
                RefreshItems();
            }
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            LinkTreeItem li = SelectedItem;
            if (li != null)
            {
                li.Level++;
                Document.Modified = true;
                Document.ValidateLevels();
                RefreshItems();
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i >= 0 && i < listBox1.Items.Count - 1)
            {
                LinkTreeItem li = Document.Items[i] as LinkTreeItem;
                Document.Items.RemoveAt(i);
                Document.Items.Insert(i + 1, li);
                Document.Modified = true;
                listBox1.SelectedIndex = i + 1;
                Document.ValidateLevels();
                RefreshItems();
            }
        }

        private void ValidateButtons()
        {
            int i = listBox1.SelectedIndex;
            int count = listBox1.Items.Count;
            if (i < 0 || i >= count)
            {
                buttonDown.Enabled = false;
                buttonLeft.Enabled = false;
                buttonRight.Enabled = false;
                buttonUp.Enabled = false;
            }
            else
            {
                LinkTreeItem li = listBox1.Items[i] as LinkTreeItem;
                buttonUp.Enabled = (i > 0);
                buttonDown.Enabled = (i < (count - 1));
                if (li != null)
                {
                    buttonLeft.Enabled = (li.Level > 0);
                    buttonRight.Enabled = true;
                }
                else
                {
                    buttonRight.Enabled = false;
                    buttonLeft.Enabled = false;
                }
            }
        }

        private LinkTreeItem SelectedItem
        {
            get
            {
                try
                {
                    return listBox1.Items[listBox1.SelectedIndex] as LinkTreeItem;
                }
                catch
                {
                    return null;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateButtons();
            LinkTreeItem li = SelectedItem;
            if (li != null)
            {
                textBox1.Text = li.Text;
                textBox2.Text = li.Target;
                richTextBox1.Text = li.Descr;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LinkTreeItem li = SelectedItem;
            if (li != null)
            {
                li.Text = textBox1.Text;
                Document.Modified = true;
                RefreshItems();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LinkTreeItem li = SelectedItem;
            if (li != null)
            {
                Document.Modified = true;
                li.Target = textBox2.Text;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            LinkTreeItem li = SelectedItem;
            if (li != null)
            {
                Document.Modified = true;
                li.Descr = richTextBox1.Text;
            }
        }

        private void newItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkTreeItem li = new LinkTreeItem();
            li.Text = "<new>";
            Document.Modified = true;
            Document.Items.Add(li);
            RefreshItems();
        }

        private void RefreshItems()
        {
            listBox1.BeginUpdate();
            int i = listBox1.TopIndex;
            int s = listBox1.SelectedIndex;
            listBox1.Items.Clear();
            foreach(object item in Document.Items)
            {
                listBox1.Items.Add(item);
            }
            if (i < Document.Items.Count)
                listBox1.TopIndex = i;
            if (s < Document.Items.Count)
                listBox1.SelectedIndex = s;
            listBox1.EndUpdate();
        }

        /// <summary>
        /// Generate HTML code from items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = Document.HtmlCode;
            
        }

        private void insertBeforeSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i >= 0)
            {
                LinkTreeItem li = new LinkTreeItem();
                li.Text = "<new>";
                Document.Modified = true;
                Document.Items.Insert(i,li);
                RefreshItems();
            }
        }

        private void insertAfterSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i >= 0)
            {
                LinkTreeItem li = new LinkTreeItem();
                li.Text = "<new>";
                Document.Modified = true;
                Document.Items.Insert(i+1, li);
                RefreshItems();
            }
        }

    }
}
