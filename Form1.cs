using Microsoft.Toolkit.Uwp.Notifications; // If we want to do windows toast notifs

using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace NueralMinesweeper
{
    public partial class Form1 : Form
    {
        private Minesweeper? myMinesweeper;
        readonly List<UIMine> uiMineList = new();

        public Form1()
        {
            InitializeComponent();
            //WinNotif("Test");
            StartNewGame(30, 30, 40);
        }

        // Function I made to create windows notifications, unused for now
        static void WinNotif(String notifText)
        {
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("AI Proj")
                .AddText(notifText)
                .Show();
        }

        private void StartNewGame(int width, int height, int mineCount)
        {
            myMinesweeper = new(width, height, mineCount);
            //progressBar1.Value = 0;
            //TaskbarProgress.SetValue(this.Handle, 0, 1);
            foreach (UIMine uimine in uiMineList)
            {
                uimine.Dispose();
            }
            uiMineList.Clear();
            int tempCount = myMinesweeper.Field.fieldSize;
            for (int i = 0; i < tempCount; i++)
            {
                UIMine button = new(i, myMinesweeper.Field.getRowCol(i));
                button.Click += (sender, EventArgs) => { OnMineClick(sender, EventArgs, 1); };
                pictureBox1.Controls.Add(button);
                uiMineList.Add(button);
            }
            chart1.Series["Uncovered"].Points.Clear();
            //progressBar1.Maximum = myMinesweeper.Field.count
            pictureBox1.Invalidate();
        }

        void OnMineClick(object? sender, EventArgs e, int nodeIndex)
        {
            UIMine btn = sender as UIMine ?? throw new ArgumentException();
            if(myMinesweeper != null)
            {
                myMinesweeper.Field.makeMove(btn.index);
                btn.setMineVal(myMinesweeper.Field[btn.index]);
            }
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
        public readonly int sideLen = 30; // Field is made of squares...
        public readonly int index = -1;
        public readonly int row = -1;
        public readonly int col = -1;
        public readonly int xOffset = 20;
        public readonly int yOffset = 20;

        public UIMine(int newIndex, (int, int)rowCol)
        {
            this.Font = new Font("Arial", 12, FontStyle.Regular);
            this.Height = sideLen;
            this.Width = sideLen;
            this.BackColor = Color.LightBlue;
            this.FlatAppearance.MouseOverBackColor = Color.Gold;
            this.FlatStyle = FlatStyle.Flat;

            this.index = newIndex;
            this.Text = (this.index + 1).ToString(); // Account for 0 index
            this.row = rowCol.Item1;
            this.col = rowCol.Item2;
            this.Location = new Point(row * sideLen + xOffset, col * sideLen + yOffset);
        }
        // public Mine Mine { set; get; }
        public void setMineVal(int     val) { this.Text = val.ToString(); }
        public void setMineVal(string text) { this.Text = text; } // For marking flags and bombs

        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new(0, 0, Width, Height));
            Region = new(path);
            base.OnPaint(e);
        }
    }
}
