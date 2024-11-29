===========================================================================
# display a Windows Form in full screen?

```cs
this.WindowState = FormWindowState.Maximized;
this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
this.TopMost = true; // nếu muốn cửa sổ của "Form" hiện trên tất cả các cửa sửa khác
```

===========================================================================
# Open another form from the main form
```cs
// program.cs
[STAThread]
static void Main()
{
    ApplicationConfiguration.Initialize();
    Application.Run(new Form1()); // register 'Form1' as main form
}

// Form1.cs
this.hide();  
Form2 f2 = new form2();  
f2.Show();  
```

===========================================================================
# Modify main form from the child form
```cs
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        Form2 frm = new Form2(this); // pass main form to child form 
        frm.Show();
    }

    public string LabelText
    {
        get { return Lbl.Text; }
        set { Lbl.Text = value; }
    }
}

public partial class Form2 : Form
{
    public Form2()
    {
        InitializeComponent();
    }

    private Form1 mainForm = null;
    public Form2(Form callingForm) // create an additional constructor
    {
        mainForm = callingForm as Form1; 
        InitializeComponent();
    }

    private void Form2_Load(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {
        // modify 'Label' control of Form1
        this.mainForm.LabelText = txtMessage.Text; 
    }
}
```

===========================================================================
# Dynamically Resizing and Positioning Controls on Form - 'Anchor' property
* -> the **Anchor Property** determines how **`a control is automatically resized when its parent control is resized`** and **`defines the position for the control`** on the form
* => to design **a form** that can be **`resized at run-time`** with the **controls** on the form should **`resize and reposition accordingly`**

## programmatically setting 'Anchor' property
* -> the **Anchor** property is set programmatically with **`a bitwise combination`** of **`Anchor Style values`**
* -> the default is Top and Left.

* Example 1:
```cs 
// Toolbox -> Button -> properties -> Anchor -> set value to "Bottom, Right"
button1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);

// => The combination of AnchorStyles.Bottom | AnchorStyles.Right anchors the button to the "bottom right corner" of the form
// => now when you resize your form by dragging the right bottom of the corner you can view a change indicating that the button maintains the same distance from the bottom of the form as well as to its right.
```

* Example2:
<img title="a title" alt="Alt text" src="/nonrelated/anchorproperty1.webp">
<img title="a title" alt="Alt text" src="/nonrelated/anchorpropery2.gif">

```cs
// control "cmb_Search" Anchor property (Top, Left, and Right) - means that the ComboBox is anchored to Top Left corner of the form and AnchorStyle Right indicates that the ComboBox is stretched to maintain the anchored distance to the top edges of the Form as the height of the Form is resized

// control "dg_UserInfo" Anchor property (DockProperty - Fill) -  The Anchor and Dock properties are contradictory. Only one can be set at a time, and the last one set takes priority
```