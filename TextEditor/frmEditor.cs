//^(?([^\r\n])\s)*\r?$\r?\n

using System.IO;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Security.AccessControl;
using System.Drawing;

namespace TextEditor
{
    public partial class frmEditor : Form
    {
        string[] args = Environment.GetCommandLineArgs();

        public frmEditor()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewDocument();
        }

        int index = -1;
        public void AddNewDocument(string MyFile = "")
        {
            index++;
            StanderedForm frm = new StanderedForm();
            frm.MdiParent = this;
            frm.IsOpened = false;
            frm.Index = index;
            frm.Text += (index + 1) + "";
            if (!string.IsNullOrEmpty(MyFile.Trim()))
            {
                //angeProps(MyFile);
                frm.IsOpened = true;
                frm.FilePath = MyFile;
                frm.FileName = Path.GetFileNameWithoutExtension(MyFile);
                frm.Text = frm.FileName;
                if (index == 0 && args.Count() > 1)
                    frm.Controls[0].Text= File.ReadAllText(MyFile);
                ((DevTextBox)frm.Controls[0]).IsSaved = true;
            }
            frm.Show();
            ToolStripButton btn = new ToolStripButton();
            btn.Name = "btn" + index;
            btn.Text = frm.Text;
            if (!string.IsNullOrEmpty(MyFile.Trim()))
            {
                btn.Text = frm.FileName;
            }
            btn.Click += delegate
            {
                this.ActivateMdiChild(frm);
                ActiveMdiChild.Focus();
                ActiveMdiChild.Controls["txt"].Focus();
            };
            ss.Items.Add(btn);
            frm.FormClosing += Frm_FormClosing;
        }

        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAsCancel = false;
            DevForm frm = ((DevForm)ActiveMdiChild);
            string fp = frm.FilePath + "";
            if (!string.IsNullOrEmpty(fp) && !File.Exists(fp))
            {
                File.WriteAllText(fp, frm.Controls["txt"].Text);
            }
            if (!string.IsNullOrEmpty(fp.Trim()) && !((DevTextBox)frm.Controls["txt"]).IsSaved)
            {
                DialogResult msg = MessageBox.Show("Do you want save changes?", "Save dialog", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                if (msg == DialogResult.Yes)
                {
                    saveToolStripMenuItem.PerformClick();
                }
                else if (msg == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (string.IsNullOrEmpty(fp.Trim()) && frm.Controls["txt"].Text.Trim() != "")
            {
                DialogResult msg = MessageBox.Show("Do you want save changes?", "Save dialog", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                if (msg == DialogResult.Yes)
                {
                    saveAsToolStripMenuItem.PerformClick();
                }
                else if (msg == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (SaveAsCancel)
                e.Cancel = true;
            else
                ss.Items.Remove(ss.Items["btn" + frm.Index]);
        }

        private void frmEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6)
            {
                if (MdiChildren.Count() == 0 || this.ActiveMdiChild.Controls[0].Focused)
                {
                    ts.Focus();
                    ts.Items[0].Select();
                }
                else if (ts.Focused)
                {
                    ss.Focus();
                    ss.Items[0].Select();
                }
                else
                    this.ActiveMdiChild.Controls[0].Focus();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select text File";
            ofd.Filter = "text files|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK && Path.GetExtension(ofd.FileName).ToLower() == ".txt")
            {
                if (!((DevForm)MdiChildren[0]).IsOpened && MdiChildren[0].Controls["txt"].Text == "")
                {
                    ActivateMdiChild(MdiChildren[0]);
                    ChangeProps(ofd.FileName);
                }
                else
                {
                    AddNewDocument(ofd.FileName);
                }
                this.ActiveMdiChild.Controls[0].Text = File.ReadAllText(ofd.FileName);
                ((DevTextBox)ActiveMdiChild.Controls["txt"]).IsSaved = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DevForm frm = ((DevForm)ActiveMdiChild);
            DevTextBox txt = ((DevTextBox)frm.Controls["txt"]);
            if (!string.IsNullOrEmpty(frm.FilePath) && !txt.IsSaved)
            {
                if (File.Exists(frm.FilePath))
                {
                    File.WriteAllText(frm.FilePath, txt.Text);
                    txt.IsSaved = true;
                    return;
                }
            }
            saveAsToolStripMenuItem.PerformClick();
        }

        bool SaveAsCancel = false;
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "saving file text";
            sfd.Filter = "text file only|*.txt";
            sfd.FileName = (((DevForm)ActiveMdiChild).IsOpened) ? ActiveMdiChild.Text + ".txt" : "*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fp = sfd.FileName;
                string fn = Path.GetFileNameWithoutExtension(fp);
                string dir = Path.GetDirectoryName(fp);
                fp = Path.Combine(dir, fn + ".txt");
                ChangeProps(fp);
                File.WriteAllText(fp, ActiveMdiChild.Controls["txt"].Text);
            }
            else
            {
                SaveAsCancel = true;
            }
        }

        public void ChangeProps(string fp)
        {
            DevForm frm = ((DevForm)ActiveMdiChild);
            frm.Focus();
            frm.IsOpened = true;
            frm.FileName = Path.GetFileNameWithoutExtension(fp);
            frm.Text = frm.FileName;
            frm.FilePath = fp;
            ((DevTextBox)frm.Controls["txt"]).IsSaved = true;
            ss.Items["btn" + frm.Index].Text = frm.Text;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = ((ToolStripItem)sender);
            DevTextBox txt = ((DevTextBox)ActiveMdiChild.Controls["txt"]);
            if (item.Name.ToLower().Contains("copy"))
                (txt as DevTextBox).Copy();
            else if (item.Name.ToLower().Contains("cut"))
                (txt as DevTextBox).Cut();
            else if (item.Name.ToLower().Contains("paste"))
                (txt as DevTextBox).Paste();
            else if (item.Name.ToLower().Contains("select"))
                (txt as DevTextBox).SelectAll();
            if (!txt.Focused)
                txt.Focus();
        }

        private void frmEditor_Shown(object sender, EventArgs e)
        {
            if (args.Count() > 1)
                AddNewDocument(args[1]);
            else
                AddNewDocument();
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            DevTextBox txt = ((DevTextBox)ActiveMdiChild.Controls[0]);
            if ((txt as DevTextBox).SelectedText == "")
            {
                copyToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
            }
            else
            {
                copyToolStripMenuItem.Enabled = true;
                cutToolStripMenuItem.Enabled = true;
            }

            if (Clipboard.GetText().Trim() == "")
                pasteToolStripMenuItem.Enabled = false;
            else
                pasteToolStripMenuItem.Enabled = true;

        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.ShowDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        int line = -1;
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            DevTextBox txt = ((DevTextBox)ActiveMdiChild.Controls["txt"]);
            Font f = txt.Font;
            e.HasMorePages = false;
            int currentHeight = 0;
            line++;
            for (; line < txt.Lines.Count(); line += 1)
            {
                e.Graphics.DrawString(txt.Lines[line], f, Brushes.Blue, 20, currentHeight + 50);
                currentHeight += f.Height;
                if (currentHeight >= e.PageBounds.Height - 150)
                {
                    e.HasMorePages = true;
                    break;
                }
            }
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {
            PrintPreviewDialog ppd = ((PrintPreviewDialog)sender);
            ToolStrip ts = ((ToolStrip)ppd.Controls[1]);
            ts.Items["printToolStripButton"].Visible = false;
        }
    }
}
