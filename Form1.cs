using Microsoft.Toolkit.Uwp.Notifications; // If we want to do windows toast notifs

using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace NueralMinesweeper
{
    public partial class Form1 : Form
    {
        private List<Minefield> mineSweeperers;
        readonly List<UIMine> uiMineList = new();
        const int POP = 50;
        Stopwatch myAlgStopWatch = new();
        Task Gening;
        const int FIELDSIZE = 20;
        public Form1()
        {
            InitializeComponent();
            mineSweeperers = new();

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

        /*
        private void StartNewGame(int width, int height, int mineCount, bool multiLife = false)
        {
            myMinesweeper = new(width, height, mineCount, multiLife);

            //progressBar1.Value = 0;
            //TaskbarProgress.SetValue(this.Handle, 0, 1);

            foreach (UIMine uimine in uiMineList)
            {
                uimine.Dispose();
            }
            uiMineList.Clear();

            myAlgStopWatch.Start();

            for (int i = 0; i < myMinesweeper.fieldSize; i++)
            {
                UIMine button = new UIMine(i, myMinesweeper.getRowCol(i));
                button.MouseUp += (sender, EventArgs) => { OnMineClick(sender, EventArgs); };
                pictureBox1.Controls.Add(button);
                uiMineList.Add(button);
            }


            chart1.Series["Uncovered"].Points.Clear();
            //progressBar1.Maximum = myMinesweeper.Field.count
            pictureBox1.Invalidate();

            myAlgStopWatch.Stop();
            label1.Text = "Algorithm Completion \r\nTime (s): " + myAlgStopWatch.Elapsed.ToString("s'.'FFFFFFF");
        }
        */

        void OnMineClick(object? sender, EventArgs e)
        {
            MouseEventArgs myMouseEventArgs = (MouseEventArgs)e;
            if (sender != null)
            {
                UIMine btn = (UIMine)sender;
                if (myMouseEventArgs.Button == MouseButtons.Left)
                {
                    if (button1.ClientRectangle.Contains(myMouseEventArgs.Location))
                    {
                        //myMinesweeper.makeMove(btn.index);
                        UpdateUI();
                    }
                }
                else
                {
                    //btn.toggleFlag(myMinesweeper.toggleTileFlag(btn.index));
                }
            }
        }

        private void UpdateUI(double max = -1, double min = -1)
        {
            label1.Text = $"MAX: {max}, Min: {min}";

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics myGraphics = e.Graphics;
            myGraphics.Clear(Color.White);
            myGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            //myGraphics.DrawLine(new Pen(Brushes.Blue, 3), 0, 0, 0, 0);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Gen();
            mineSweeperers.Sort();
            this.Invoke(UpdateUI, mineSweeperers.Max(sweeper => sweeper.GetFitness()), mineSweeperers.Min(sweeper => sweeper.GetFitness()));


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //myMinesweeper.Reset();
            this.Invoke(UpdateUI, mineSweeperers.Max(sweeper => sweeper.GetFitness()), mineSweeperers.Min(sweeper => sweeper.GetFitness()));

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Task gen = Task.Run(() => { GenerateAI(); });

            await gen;
            Gening = Task.CompletedTask;
        }

        private async void GenerateAI()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < POP; i++)
            {
                tasks.Add(Task.Run(() => { mineSweeperers.Add(new(FIELDSIZE, FIELDSIZE, (int)(FIELDSIZE * 2.5), true)); }));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();
            for (int i = 0; i < POP; i++)
            {
                int index = i;
                tasks.Add(Task.Run(() => { mineSweeperers[index].CompleteGame(); }));
            }
            await Task.WhenAll(tasks);
            mineSweeperers.Sort();
            this.Invoke(UpdateUI, mineSweeperers.Max(sweeper => sweeper.GetFitness()), mineSweeperers.Min(sweeper => sweeper.GetFitness()));

        }

        private async Task<Task> Gen()
        {
            if (Gening != Task.CompletedTask)
                await Gening;
            Gening = Task.Delay(1000000000);
            mineSweeperers.Sort();
            this.Invoke(UpdateUI, mineSweeperers.Max(sweeper => sweeper.GetFitness()), mineSweeperers.Min(sweeper => sweeper.GetFitness()));
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < POP / 2; i++)
            {
                int index = i;

                tasks.Add(Task.Run(() =>
            {
                mineSweeperers[index + POP / 2] = new(FIELDSIZE, FIELDSIZE, (int)(2.5 * FIELDSIZE), mineSweeperers[i].GetNet(), true);
            }));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();
            for (int i = 0; i < POP / 2; i++)
            {
                int index = i;
                tasks.Add(Task.Run(() =>
                {
                    mineSweeperers[index].Reset();
                }));

                tasks.Add(Task.Run(() =>
                {
                    mineSweeperers[index].Mutate();
                }));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();

            for (int i = 0; i < POP; i++)
            {
                int index = i;
                tasks.Add(Task.Run(() => { mineSweeperers[index].CompleteGame(); }));
            }
            Gening = Task.WhenAll(tasks);
            return Task.WhenAll(tasks);
        }
    }
}
// Created class inheriting Button to customize its shape
class UIMine : Button
{
    public readonly int sideLen = 35; // Field is made of squares...
    public readonly int index = -1;
    public readonly int row = -1;
    public readonly int col = -1;
    public readonly int xOffset = 20;
    public readonly int yOffset = 20;

    public UIMine(int newIndex, (int, int) rowCol)
    {
        this.Font = new Font("Arial", 12, FontStyle.Regular);
        this.BackColor = Color.LightBlue;
        this.FlatAppearance.MouseOverBackColor = Color.Gold;
        this.FlatStyle = FlatStyle.Flat;
        this.BackgroundImageLayout = ImageLayout.Stretch;

        this.index = newIndex;
        this.Text = ""; // Account for 0 index
        this.BackgroundImage = Image.FromFile(@"..\..\..\MinesweeperCoveredTile.png");
        this.row = rowCol.Item1;
        this.col = rowCol.Item2;
        this.Height = sideLen;
        this.Width = sideLen;
        this.Location = new Point(row * sideLen + xOffset, col * sideLen + yOffset);
    }
    // public Mine Mine { set; get; }
    public void setTileVal(int val)
    {
        this.Text = val.ToString();
        if (val == -1)
        {
            this.BackColor = Color.Red;
            this.Text = "";
            this.BackgroundImage = Image.FromFile(@"..\..\..\MinesweeperMine.png");
        }
        else if (val == -2)
        {
            this.Text = ""; // Account for 0 index
            this.BackgroundImage = Image.FromFile(@"..\..\..\MinesweeperCoveredTile.png");
        }
        else
        {
            if (val == 0) { this.Text = ""; }
            this.BackgroundImage = Image.FromFile(@"..\..\..\MinesweeperUncoveredTile.png");
        }
    }
    public void setTileText(string text) { this.Text = text; } // For marking flags and bombs

    public void toggleFlag(bool flag)
    {
        if (flag) { this.BackgroundImage = Image.FromFile(@"..\..\..\MinesweeperFlag.png"); }
        else { this.BackgroundImage = Image.FromFile(@"..\..\..\MinesweeperCoveredTile.png"); }
    }

}

