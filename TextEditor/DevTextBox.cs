using System;
using System.Windows.Forms;

namespace TextEditor
{
    public class DevTextBox : TextBox
    {
        private bool _IsSaved;

        public bool IsSaved
        {
            get { return _IsSaved; }
            set { _IsSaved = value; }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            IsSaved = false;
        }
    }
}
