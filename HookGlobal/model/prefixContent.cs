using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HookGlobal.model
{
    public class prefixContent
    {
        public string _prefix { get; set; }
        public Control _cl { get; set; }
        public prefixContent() { }
        public prefixContent(string p,Control c)
        {
            _prefix = p;
            _cl = c;
        }

    }
}
