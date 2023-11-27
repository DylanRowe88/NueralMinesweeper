using Microsoft.Toolkit.Uwp.Notifications; // If we want to do windows toast notifs

using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace NueralMinesweeper
{
    public partial class Form1 : Form
    {
        private readonly Minesweeper myMinesweeper;
        List<UIMine> uiMineList = new();
        // Mine Graphics
        readonly int sideLen = 50; // Field is made of squares...
        readonly double xscale = 5;
        readonly double yscale = 7;

        public Form1()
        {
            InitializeComponent();
            WinNotif("Test");
            myMinesweeper = new(10, 10, 20);
        }

        // Function I made to create windows notifications, unused for now
        static void WinNotif(String notifText)
        {
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("AI Proj")
                .AddText(notifText);
        }

        private void StartNewGame(int width, int height, int mineCount)
        {
            //progressBar1.Value = 0;
            //TaskbarProgress.SetValue(this.Handle, 0, 1);
            foreach (UIMine uimine in uiMineList)
            {
                uimine.Dispose();
            }
            uiMineList.Clear();
            for (int i = 0; i < myMinesweeper.Field.count; i++)
            {
                int x = (int)(node.X * xscale) - radius;
                int y = this.Height - yOffset - ((int)(node.Y * yscale)) - radius;
                UIMine button = new(sideLen);
                button.Location = new Point(x, y);
                button.BackColor = Color.LightBlue;
                button.FlatAppearance.MouseOverBackColor = Color.Gold;
                button.FlatStyle = FlatStyle.Flat;
                button.Click += (sender, EventArgs) => { OnNodeClick(sender, EventArgs, 1); };
                button.Text = (node.nodesIndex + 1).ToString(); // Account for 0 index
                pictureBox1.Controls.Add(button);
                uiMineList.Add(button);
            }
            chart1.Series["Uncovered"].Points.Clear();
            //progressBar1.Maximum = myMinesweeper.Field.count
            pictureBox1.Invalidate();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics myGraphics = e.Graphics;
            myGraphics.Clear(Color.White);
            myGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            //myGraphics.DrawLine(new Pen(Brushes.Blue, 3), 0, 0, 0, 0);
        }
    }
    // Created class inheriting Button to customize its shape
    class UIMine : Button
    {
        public UIMine(int SideLen)
        {
            this.Text = "";
            this.Font = new Font("Arial", 12, FontStyle.Regular);
            this.Height = SideLen;
            this.Width = SideLen;
        }

        // public Mine Mine { set; get; }
        public void setMineVal(int     val) { this.Text = val.ToString(); }
        public void setMineVal(string text) { this.Text = text; } // For marking flags and bombs

        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            // path.AddEllipse(
            //    ClientRectangle.X + ClientRectangle.Width / 4,
            //    ClientRectangle.Y + ClientRectangle.Height / 4,
            //    ClientRectangle.Width / 2,
            //    ClientRectangle.Height / 2);
            Region = new(path);
            base.OnPaint(e);
        }
    }
}
