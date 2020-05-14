using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

public class LabeledTextBox : TextBox
{
    private string label;

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (this.Text == this.label)
        {
            this.Text = "";
            this.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (!Regex.IsMatch(this.Text, ".+"))
        {
            this.Text = label;
            this.ForeColor = Color.Gray;
        }
    }

    [Description("The label shown in the textbox"), Category("Appearance"), DefaultValue("...")]
    public string Label
    {
        get { return this.label; }
        set { this.label = value; this.Text = label; this.ForeColor = Color.Gray; }
    }
}
