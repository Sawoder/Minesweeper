using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class MainForm : Form
    {
        public Button[,] Buttons { get; private set; }
        public StateButton[,] StateButtons { get; private set; }
        public StateLevel Level { get; private set; }
        public int N { get; private set; }
        public int Mines { get { return (int)(N * (int)Level * Math.Log10(N)) + 2; } }
        public Label LabelCount { get; private set; }
        public int FlagCount { get; private set; }
        public bool IsWithFlag { get; private set; }

        public MainForm()
        {
            InitializeComponent();
        }
        
        private void countTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ClearField();
                InitField();
                FlagCount = 0;
                LabelCount = new Label()
                {
                    Text = 0 + " / " + Mines.ToString(),
                    Location = Buttons[N - 2, N - 1].Location,
                    Size = new Size(45, 20)
                };
                Controls.Add(LabelCount);
                LabelCount.BringToFront();
                IsWithFlag = false;
            }
        }

        private void ClearField()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Controls.Remove(Buttons[i, j]);
                }
            }
            Controls.Remove(LabelCount);
        }

        private void GenerateMines()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < Mines; i++)
            {
                int randi = rand.Next(N);
                int randj = rand.Next(N);
                if (StateButtons[randi, randj] == StateButton.Mine)
                {
                    i--;
                    continue;
                }
                StateButtons[randi, randj] = StateButton.Mine;
            }
        }

        private void CheckState(object sender, MouseEventArgs e)
        {
            int i = int.Parse(((Button)sender).Name.Split(' ')[0]);
            int j = int.Parse(((Button)sender).Name.Split(' ')[1]);

            if (e.Button == MouseButtons.Right)
            {

                if (StateButtons[i, j] == StateButton.Closed || StateButtons[i, j] == StateButton.Mine)
                {
                    if (((Button)sender).Text.Equals("F"))
                    {
                        ((Button)sender).Text = "";
                        ((Button)sender).BackColor = SystemColors.ControlLight;
                        ((Button)sender).ForeColor = SystemColors.ControlText;
                        FlagCount--;
                    }
                    else
                    {
                        ((Button)sender).Text = "F";
                        ((Button)sender).BackColor = Color.Black;
                        ((Button)sender).ForeColor = Color.White;
                        IsWithFlag = true;
                        FlagCount++;
                    }
                }
                ChangeLabelCount();
                return;
            }
            if (((Button)sender).Text.Equals("F")) return;

            CheckLose(i, j);
            ZeroButtons(i, j);
            ChangeColor();
            CheckWin();
        }

        private void NumberButtons(int i, int j) // Не рабочий метод
        {
            if (StateButtons[i, j] > 0 && (int)StateButtons[i, j] < 9)
            {
                try
                {
                    if (i > 0) // Up
                        if (!Buttons[i - 1, j].Text.Equals("F"))
                            CheckState(Buttons[i - 1, j], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i - 1, j].Location.X, Buttons[i - 1, j].Location.Y, 0));
                    if (i < N - 1) // Down
                        if (!Buttons[i + 1, j].Text.Equals("F"))
                        {
                            CheckState(Buttons[i + 1, j], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i + 1, j].Location.X, Buttons[i + 1, j].Location.Y, 0));
                        }
                    if (j > 0) // Left
                        if (!Buttons[i, j - 1].Text.Equals("F"))
                        {
                            CheckState(Buttons[i, j - 1], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i, j - 1].Location.X, Buttons[i, j - 1].Location.Y, 0));
                        }
                    if (j < N - 1) // Right
                        if (!Buttons[i, j + 1].Text.Equals("F"))
                        {
                            CheckState(Buttons[i, j + 1], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i, j + 1].Location.X, Buttons[i, j + 1].Location.Y, 0));
                        }


                    if (i > 0 && j > 0) // Up left
                        if (!Buttons[i - 1, j - 1].Text.Equals("F"))
                        {
                            CheckState(Buttons[i - 1, j - 1], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i - 1, j - 1].Location.X, Buttons[i - 1, j - 1].Location.Y, 0));
                        }
                    if (i > 0 && j < N - 1) // Up right
                        if (!Buttons[i - 1, j + 1].Text.Equals("F"))
                        {
                            CheckState(Buttons[i - 1, j + 1], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i - 1, j + 1].Location.X, Buttons[i - 1, j + 1].Location.Y, 0));
                        }
                    if (i < N && j > 0) // Down left
                        if (!Buttons[i + 1, j - 1].Text.Equals("F"))
                        {
                            CheckState(Buttons[i + 1, j - 1], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i + 1, j - 1].Location.X, Buttons[i + 1, j - 1].Location.Y, 0));
                        }
                    if (i < N - 1 && j < N - 1) // Down right
                        if (!Buttons[i + 1, j + 1].Text.Equals("F"))
                        {
                            CheckState(Buttons[i + 1, j + 1], new MouseEventArgs(MouseButtons.Left, 1, Buttons[i + 1, j + 1].Location.X, Buttons[i + 1, j + 1].Location.Y, 0));
                        }
                }
                catch (Exception) { }
            }
        }
        
        private void ChangeLabelCount()
        {
            LabelCount.Text = FlagCount.ToString() + " / " + Mines.ToString();
        }

        private void ZeroButtons(int i, int j)
        {
            if (Logic(i, j) == 0)
            {
                StateButtons[i, j] = StateButton.Clear;
                HideZeroButtons(i, j);
                Buttons[i, j].Visible = false;
            }
            Buttons[i, j].Text = Logic(i, j).ToString();
            StateButtons[i, j] = (StateButton)Logic(i, j);
        }

        private void ChangeColor()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (StateButtons[i, j] > 0 && (int)StateButtons[i, j] < 9)
                        Buttons[i, j].FlatStyle = FlatStyle.Popup;
                    if (StateButtons[i, j] == StateButton.One)
                        Buttons[i, j].ForeColor = Color.Blue;
                    if (StateButtons[i, j] == StateButton.Two)
                        Buttons[i, j].ForeColor = Color.Green;
                    if (StateButtons[i, j] == StateButton.Three)
                        Buttons[i, j].ForeColor = Color.Red;
                    if (StateButtons[i, j] == StateButton.Four)
                        Buttons[i, j].ForeColor = Color.DarkBlue;
                    if (StateButtons[i, j] == StateButton.Five)
                        Buttons[i, j].ForeColor = Color.DarkGreen;
                    if (StateButtons[i, j] == StateButton.Six)
                        Buttons[i, j].ForeColor = Color.DarkRed;
                    if (StateButtons[i, j] == StateButton.Seven)
                        Buttons[i, j].ForeColor = Color.DarkOrange;
                    if (StateButtons[i, j] == StateButton.Eight)
                        Buttons[i, j].ForeColor = Color.DarkTurquoise;
                }
            }
        }

        private void CheckLose(int i, int j)
        {
            if (StateButtons[i, j] == StateButton.Mine)
            {
                for (int t = 0; t < N; t++)
                {
                    for (int g = 0; g < N; g++)
                    {
                        Buttons[t, g].Enabled = false;
                        if (StateButtons[t, g] == StateButton.Mine)
                        {
                            Buttons[t, g].Text = "M";
                        }
                    }
                }
                Buttons[i, j].BackColor = Color.Red;
                MessageBox.Show("Вы проиграли");
                menuStrip1.Visible = true;
            }
        }

        private void CheckWin()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (StateButtons[i, j] == StateButton.Closed)
                    {
                        return;
                    }
                }
            }
            MessageBox.Show("Вы победили " + (IsWithFlag ? "Bronze" : "Gold"));
            menuStrip1.Visible = true;
        }

        private void HideZeroButtons(int i, int j)
        {
            try
            {
                if (i > 0) // Up
                    if (StateButtons[i - 1, j] == StateButton.Closed)
                    {
                        ZeroButtons(i - 1, j);
                        Buttons[i - 1, j].Text = Logic(i - 1, j).ToString();
                        StateButtons[i - 1, j] = (StateButton)Logic(i - 1, j);
                    }
                if (i < N - 1) // Down
                    if (StateButtons[i + 1, j] == StateButton.Closed)
                    {
                        ZeroButtons(i + 1, j);
                    }
                if (j > 0) // Left
                    if (StateButtons[i, j - 1] == StateButton.Closed)
                    {
                        ZeroButtons(i, j - 1);
                    }
                if (j < N - 1) // Right
                    if (StateButtons[i, j + 1] == StateButton.Closed)
                    {
                        ZeroButtons(i, j + 1);
                    }


                if (i > 0 && j > 0) // Up left
                    if (StateButtons[i - 1, j - 1] == StateButton.Closed)
                    {
                        ZeroButtons(i - 1, j - 1);
                    }
                if (i > 0 && j < N - 1) // Up right
                    if (StateButtons[i - 1, j + 1] == StateButton.Closed)
                    {
                        ZeroButtons(i - 1, j + 1);
                    }
                if (i < N && j > 0) // Down left
                    if (StateButtons[i + 1, j - 1] == StateButton.Closed)
                    {
                        ZeroButtons(i + 1, j - 1);
                    }
                if (i < N - 1 && j < N - 1) // Down right
                    if (StateButtons[i + 1, j + 1] == StateButton.Closed)
                    {
                        ZeroButtons(i + 1, j + 1);
                    }
            }
            catch (Exception) { }
        }

        private int Logic(int i, int j)
        {
            int count = 0;
            try
            {
                if (i > 0) // Up
                    if (StateButtons[i - 1, j] == StateButton.Mine)
                    {
                        count++;
                    }
                if (i < N - 1) // Down
                    if (StateButtons[i + 1, j] == StateButton.Mine)
                    {
                        count++;
                    }
                if (j > 0) // Left
                    if (StateButtons[i, j - 1] == StateButton.Mine)
                    {
                        count++;
                    }
                if (j < N - 1) // Right
                    if (StateButtons[i, j + 1] == StateButton.Mine)
                    {
                        count++;
                    }


                if (i > 0 && j > 0) // Up left
                    if (StateButtons[i - 1, j - 1] == StateButton.Mine)
                    {
                        count++;
                    }
                if (i > 0 && j < N - 1) // Up right
                    if (StateButtons[i - 1, j + 1] == StateButton.Mine)
                    {
                        count++;
                    }
                if (i < N && j > 0) // Down left
                    if (StateButtons[i + 1, j - 1] == StateButton.Mine)
                    {
                        count++;
                    }
                if (i < N - 1 && j < N - 1) // Down right
                    if (StateButtons[i + 1, j + 1] == StateButton.Mine)
                    {
                        count++;
                    }
            }
            catch (Exception) { }
            return count;
        }

        private void InitField()
        {
            try
            {
                N = int.Parse(countTextBox.Text);
                if (N > 30 || N < 2) throw new FormatException("Поле должно быть от 2 до 20 клеток");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            Buttons = new Button[N, N];
            StateButtons = new StateButton[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    int size = 30;
                    Buttons[i, j] = new Button()
                    {
                        Location = new Point(i * size, j * size),
                        Name = i.ToString() + " " + j.ToString(),
                        Size = new Size(size, size),
                        Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 204)
                    };
                    Buttons[i, j].MouseDown += new MouseEventHandler(CheckState);
                    Controls.Add(Buttons[i, j]);
                    StateButtons[i, j] = StateButton.Closed;
                }
            }
            GenerateMines();
            countTextBox.Clear();
            countTextBox.Visible = false;
            menuStrip1.Visible = false;
        }

        private void levelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem send = (ToolStripMenuItem)sender;
            countTextBox.Visible = true;
            if (send.Text.Equals("Easy"))
            {
                Level = StateLevel.Easy;
                return;
            }
            if (send.Text.Equals("Medium"))
            {
                Level = StateLevel.Medium;
                return;
            }
            if (send.Text.Equals("Hard"))
            {
                Level = StateLevel.Hard;
            }
        }
    }
}
