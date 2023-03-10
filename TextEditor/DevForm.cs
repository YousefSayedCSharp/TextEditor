using System.ComponentModel;
using System.Windows.Forms;

namespace TextEditor
{
    public class DevForm:Form
    {
        private int _Index;
        [Category("File"),Description("form indexin Mdi")]
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                _Index = value;
            }
        }

        private string _FilePath;
        [Category("File"), Description("test")]
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }

        private bool _IsOpened;
        [Category("File"), Description("status file is opened or not")]
        public bool IsOpened
        {
            get { return _IsOpened; }
            set { _IsOpened = value; }
        }

        private string _FileName;
        [Category("File"), Description("File name Only")]
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }



    }
}
