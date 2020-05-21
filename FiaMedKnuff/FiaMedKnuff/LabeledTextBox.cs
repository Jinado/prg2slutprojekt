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

    // As soon as someone starts writing something, remove the label from the textbox
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (this.Text == this.label)
        {
            this.Text = "";
            this.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
        }
    }

    // As soon as someone stops writing, write the label to the textbox IF the textbox is empty
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
