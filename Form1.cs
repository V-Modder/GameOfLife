using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Org.Vesic.WinForms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private SizeF s = new SizeF(13.0F, 15.0F);
        private FormState fs = new FormState();
        private Font ff = new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold);
        private GameOfLife gof;
        private Bitmap temp;
        private string sMenu = "PAUSE\n\nRandom\nLoad from file\nSave to file\nHand pick\nQuit\nRounds: ";
        private bool bHandPick;
        private char[,] HandPicked;
        private bool bMenu;
        private Point pMenu;
        private Size siMenu;
        private int iMenu;
        private int round;
        private Brush myBrush = Brushes.Green;

        public Form1()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(Screen.FromControl(this).Bounds.Width, Screen.FromControl(this).Bounds.Height);
            this.Location = new Point(0, 0);
            bMenu = false;
            bHandPick = false;
            Cursor.Hide();
            fs.Maximize(this);
            gof = new GameOfLife((int)(this.Size.Width / s.Width) - 1, (int)(this.Size.Height / s.Height) - 1);
            gof.PrintGrid += new GameOfLife.MyPrintHandler(gof_PrintGrid);
            gof.Start(ReadFile(null));
        }

        private char[,] ReadFile(string path)
        {
            char[,] startup = null;
            if (File.Exists(path))
            {
                StreamReader se = new StreamReader(path);
                int xcount = 0;
                int ycount = 0;

                char[] c = new char[1];
                while (c[0] != '\n')
                {
                    se.Read(c, 0, 1);
                    xcount++;
                }
                ycount++;
                while (!se.EndOfStream)
                {
                    se.ReadLine();
                    ycount++;
                }
                se.DiscardBufferedData();
                startup = new char[xcount - 1, ycount];
                xcount = 0;
                ycount = 0;
                se = new StreamReader(path);
                while (!se.EndOfStream)
                {
                    se.Read(c, 0, 1);
                    if (c[0] == '\n')
                    {
                        ycount++;
                        xcount = 0;
                    }
                    else if (c[0] == '0')
                        startup[xcount++, ycount] = ' ';
                    else if (c[0] == '1')
                        startup[xcount++, ycount] = 'O';
                }
            }
            return startup;
        }

        private void gof_PrintGrid(object sender, PrintEventArgs e)
        {
            if (!bMenu)
            {
                Bitmap b = new Bitmap(this.Size.Width, this.Size.Height);
                Graphics g = Graphics.FromImage(b);
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, b.Width, b.Height));
                for (int y = 0; y < e.Grid.GetLength(1); y++)
                    for (int x = 0; x < e.Grid.GetLength(0); x++)
                    {
                        g.DrawString(e.Grid[x, y].ToString(), ff, myBrush, new PointF(x * s.Width, y * s.Height));
                    }
                this.temp = b;
                this.round = e.RoundCount;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            if (temp != null)
            {
                g.DrawImage(temp, 0, 0, temp.Width, temp.Height);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (bHandPick)
                        return;
                    gof.Pause = !gof.Pause;
                    bMenu = gof.Pause;
                    if (bMenu)
                    {
                        Graphics g = Graphics.FromImage(temp);
                        SizeF d = g.MeasureString(this.sMenu + this.round.ToString(), ff);
                        siMenu = new Size((int)d.Width, (int)d.Height);
                        g.FillRectangle(Brushes.Blue, new Rectangle(((int)(this.Size.Width - (int)d.Width) / 2), (int)((this.Size.Height - (int)d.Height) / 2), (int)d.Width, (int)d.Height));
                        g.DrawString(this.sMenu + this.round.ToString(), ff, Brushes.White, (this.Size.Width - (int)d.Width) / 2, (this.Size.Height - (int)d.Height) / 2);
                        pMenu = new Point((int)((this.Size.Width - (int)d.Width) / 2), (int)((this.Size.Height - (int)d.Height) / 2));
                        iMenu = (int)g.MeasureString("X", ff).Height;
                        Cursor.Show();
                    }
                    else
                        Cursor.Hide();
                    this.Invalidate();
                    break;
                case Keys.Space:
                    if (bHandPick)
                    {
                        bHandPick = false;
                        gof = new GameOfLife((int)(this.Size.Width / s.Width) - 1, (int)(this.Size.Height / s.Height) - 1);
                        gof.PrintGrid += new GameOfLife.MyPrintHandler(gof_PrintGrid);
                        gof.Start(HandPicked);
                        HandPicked = null;
                        bMenu = gof.Pause;
                    }
                    break;
                case Keys.Up:
                    gof.Speed -= 20;
                    break;
                case Keys.Down:
                    gof.Speed += 20;
                    break;
                case Keys.Add:
                    gof.Speed -= 20;
                    break;
                case Keys.Subtract:
                    gof.Speed += 20;
                    break;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (bMenu && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.X > pMenu.X && e.X < pMenu.X + siMenu.Width)
                {
                    if (e.Y > pMenu.Y + iMenu * 2 && e.Y < pMenu.Y + iMenu * 3)
                    {//Random
                        gof = new GameOfLife((int)(this.Size.Width / s.Width) - 1, (int)(this.Size.Height / s.Height) - 1);
                        gof.PrintGrid += new GameOfLife.MyPrintHandler(gof_PrintGrid);
                        gof.Start(null);
                        bMenu = gof.Pause;
                    }
                    if (e.Y > pMenu.Y + iMenu * 3 && e.Y < pMenu.Y + iMenu * 4)
                    {//From file
                        if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            gof = new GameOfLife((int)(this.Size.Width / s.Width) - 1, (int)(this.Size.Height / s.Height) - 1);
                            gof.PrintGrid += new GameOfLife.MyPrintHandler(gof_PrintGrid);
                            gof.Start(ReadFile(openFileDialog1.FileName));
                            bMenu = gof.Pause;
                        }
                    }
                    if (e.Y > pMenu.Y + iMenu * 4 && e.Y < pMenu.Y + iMenu * 5)
                    {//To file
                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            char[,] c = gof.Grid;
                            for (int y = 0; y < c.GetLength(1); y++)
                            {
                                for (int x = 0; x < c.GetLength(0); x++)
                                {
                                    if (c[x, y] == 'O')
                                        sw.Write("1");
                                    else
                                        sw.Write("0");
                                }
                                if (y < c.GetLength(1) - 1)
                                    sw.Write("\n");
                            }
                            sw.Close();
                            bMenu = gof.Pause;
                        }
                    }
                    if (e.Y > pMenu.Y + iMenu * 5 && e.Y < pMenu.Y + iMenu * 6)
                    {//Hand pick
                        bMenu = false;
                        temp = new Bitmap(temp.Width, temp.Height);
                        Graphics g = Graphics.FromImage(temp);
                        g.FillRectangle(Brushes.Black, new Rectangle(0, 0, temp.Width, temp.Height));
                        gof = null;
                        HandPicked = new char[(int)(this.Size.Width / s.Width) - 1, (int)(this.Size.Height / s.Height) - 1];
                        for (int y = 0; y < HandPicked.GetLength(1); y++)
                            for (int x = 0; x < HandPicked.GetLength(0); x++)
                            {
                                HandPicked[x, y] = ' ';
                            }
                        bHandPick = true;
                    }
                    if (e.Y > pMenu.Y + iMenu * 6 && e.Y < pMenu.Y + iMenu * 7)
                    {//Quit
                        this.Close();
                    }
                    this.Invalidate();
                }
            }
            else if (bHandPick)
            {
                Point coord = new Point((int)(e.X / s.Width), (int)(e.Y / s.Height));
                if (HandPicked[coord.X, coord.Y] == 'O')
                {
                    HandPicked[coord.X, coord.Y] = ' ';
                }
                else
                {
                    HandPicked[coord.X, coord.Y] = 'O';
                }
                temp = new Bitmap(temp.Width, temp.Height);
                Graphics g = Graphics.FromImage(temp);
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, temp.Width, temp.Height));
                for (int y = 0; y < HandPicked.GetLength(1); y++)
                    for (int x = 0; x < HandPicked.GetLength(0); x++)
                    {
                        g.DrawString(HandPicked[x, y].ToString(), ff, myBrush, new PointF(x * s.Width, y * s.Height));

                    }
            }
            this.Invalidate();
        }
    }
}
