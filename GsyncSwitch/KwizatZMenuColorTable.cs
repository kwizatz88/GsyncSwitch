using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GsyncSwitch
{
    public class KwizatZMenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemBorder
        {
            get { return Color.WhiteSmoke; }
        }
        public override Color MenuItemSelected
        {
            get { return Color.WhiteSmoke; }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return Color.White; }
        }
        public override Color ImageMarginGradientBegin
        {
            get { return Color.White; }
        }
        public override Color ImageMarginGradientMiddle
        {
            get { return Color.White; }
        }
        public override Color ImageMarginGradientEnd
        {
            get { return Color.White; }
        }
    }
}
