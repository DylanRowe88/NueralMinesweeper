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
        int sideLen = 50; // Field is made of squares...
        double xscale = 5;
        double yscale = 7;

        public Form1()
        {
            InitializeComponent();
            WinNotif("Test");
            myMinesweeper = new(10,10,20);
        }
        // Function I made to create windows notifications, unused for now
        void WinNotif(String notifText)
        {
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("AI Proj")
                .AddText(notifText);
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
