using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InputTester
{
    public class MainForm : Form
    {
        private Label focusLabel, lastKeyLabel, keyCountLabel, mousePositionLabel, mouseMoveLabel, wheelLabel;
        private Label[] mouseButtonLabels;
        private ListView keyList, eventList;
        private Panel mouseArea;
        private Dictionary<int, int> keyCounts = new Dictionary<int, int>();
        private int totalKeys, moveCount, wheelUp, wheelDown;
        private int leftCount, rightCount, middleCount, x1Count, x2Count;
        private Point lastMouse;
        private bool haveMouse;

        public MainForm()
        {
            Text = "//AAYDEV Input Tester";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(850, 610);
            MinimumSize = new Size(700, 500);
            KeyPreview = true;
            Font = new Font("Tahoma", 8.5f);
            BackColor = Color.FromArgb(240, 243, 247);

            Label title = new Label();
            title.Text = "//AAYDEV INPUT TESTER";
            title.Font = new Font("Tahoma", 17, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(35, 75, 115);
            title.SetBounds(16, 12, 700, 30);
            Controls.Add(title);

            // AAYDEV fox logo is embedded into the executable as a manifest resource.
            // No external PNG file is needed when distributing InputTester.exe.
            using (System.IO.Stream logoStream =
                typeof(MainForm).Assembly.GetManifestResourceStream("InputTester.AAYDEV.png"))
            {
                if (logoStream != null)
                {
                    using (Image embeddedLogo = Image.FromStream(logoStream))
                    {
                        PictureBox logo = new PictureBox();
                        logo.Name = "AAYDEVLogo";
                        logo.Size = new Size(64, 64);
                        logo.Location = new Point(770, 8);
                        logo.SizeMode = PictureBoxSizeMode.Zoom;
                        // Clone the image so it remains valid after the resource stream closes.
                        logo.Image = new Bitmap(embeddedLogo);
                        logo.BackColor = Color.Transparent;
                        Controls.Add(logo);
                    }
                }
            }

            focusLabel = new Label();
            focusLabel.Text = "Click this window, then press keys to test the keyboard.";
            focusLabel.ForeColor = Color.DarkGreen;
            focusLabel.SetBounds(18, 48, 800, 22);
            Controls.Add(focusLabel);

            GroupBox keyboardGroup = new GroupBox();
            keyboardGroup.Text = "KEYBOARD";
            keyboardGroup.SetBounds(15, 78, 405, 475);
            Controls.Add(keyboardGroup);

            lastKeyLabel = new Label();
            lastKeyLabel.Text = "Last key: -";
            lastKeyLabel.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lastKeyLabel.SetBounds(12, 22, 370, 24);
            keyboardGroup.Controls.Add(lastKeyLabel);

            keyCountLabel = new Label();
            keyCountLabel.Text = "Key presses: 0";
            keyCountLabel.SetBounds(12, 48, 370, 20);
            keyboardGroup.Controls.Add(keyCountLabel);

            keyList = new ListView();
            keyList.View = View.Details;
            keyList.FullRowSelect = true;
            keyList.GridLines = true;
            keyList.HideSelection = false;
            keyList.Columns.Add("Key", 150);
            keyList.Columns.Add("Virtual key", 85);
            keyList.Columns.Add("Presses", 75);
            keyList.Columns.Add("State", 65);
            keyList.SetBounds(12, 75, 378, 385);
            keyboardGroup.Controls.Add(keyList);

            GroupBox mouseGroup = new GroupBox();
            mouseGroup.Text = "MOUSE";
            mouseGroup.SetBounds(435, 78, 400, 475);
            Controls.Add(mouseGroup);

            mousePositionLabel = new Label();
            mousePositionLabel.Text = "Position: X=0  Y=0";
            mousePositionLabel.SetBounds(12, 23, 370, 20);
            mouseGroup.Controls.Add(mousePositionLabel);

            mouseMoveLabel = new Label();
            mouseMoveLabel.Text = "Movement events: 0";
            mouseMoveLabel.SetBounds(12, 45, 370, 20);
            mouseGroup.Controls.Add(mouseMoveLabel);

            mouseArea = new Panel();
            mouseArea.BackColor = Color.White;
            mouseArea.BorderStyle = BorderStyle.FixedSingle;
            mouseArea.SetBounds(12, 72, 376, 150);
            mouseGroup.Controls.Add(mouseArea);
            Label areaText = new Label();
            areaText.Text = "MOVE THE MOUSE HERE\r\nClick buttons and use the wheel";
            areaText.TextAlign = ContentAlignment.MiddleCenter;
            areaText.ForeColor = Color.Gray;
            areaText.Dock = DockStyle.Fill;
            areaText.Enabled = false;
            mouseArea.Controls.Add(areaText);

            mouseButtonLabels = new Label[5];
            string[] names = new string[] { "Left button: 0", "Right button: 0", "Middle button: 0", "Mouse 4: 0", "Mouse 5: 0" };
            for (int i = 0; i < 5; i++)
            {
                mouseButtonLabels[i] = new Label();
                mouseButtonLabels[i].Text = names[i];
                mouseButtonLabels[i].BorderStyle = BorderStyle.FixedSingle;
                mouseButtonLabels[i].BackColor = Color.White;
                mouseButtonLabels[i].TextAlign = ContentAlignment.MiddleLeft;
                mouseButtonLabels[i].Padding = new Padding(6, 0, 0, 0);
                mouseButtonLabels[i].SetBounds(12 + (i % 2) * 190, 235 + (i / 2) * 31, 180, 25);
                mouseGroup.Controls.Add(mouseButtonLabels[i]);
            }

            wheelLabel = new Label();
            wheelLabel.Text = "Wheel: Up 0    Down 0";
            wheelLabel.SetBounds(12, 333, 370, 22);
            mouseGroup.Controls.Add(wheelLabel);

            Label hint = new Label();
            hint.Text = "Mouse tests are captured while the pointer is over this window.\r\nKeyboard tests are captured while this window has focus.";
            hint.ForeColor = Color.DimGray;
            hint.SetBounds(12, 365, 370, 42);
            mouseGroup.Controls.Add(hint);

            eventList = new ListView();
            eventList.View = View.Details;
            eventList.FullRowSelect = true;
            eventList.Columns.Add("Recent input events", 365);
            eventList.SetBounds(12, 410, 376, 64);
            mouseGroup.Controls.Add(eventList);

            Button clearButton = new Button();
            clearButton.Text = "Reset counters";
            clearButton.SetBounds(15, 565, 115, 28);
            clearButton.Click += new EventHandler(ClearButton_Click);
            Controls.Add(clearButton);

            Label footer = new Label();
            footer.Text = "Windows XP compatible  |  .NET Framework 2.0";
            footer.ForeColor = Color.Gray;
            footer.TextAlign = ContentAlignment.MiddleRight;
            footer.SetBounds(450, 568, 385, 22);
            Controls.Add(footer);

            KeyDown += new KeyEventHandler(MainForm_KeyDown);
            KeyUp += new KeyEventHandler(MainForm_KeyUp);
            MouseMove += new MouseEventHandler(Mouse_Move);
            MouseDown += new MouseEventHandler(Mouse_Down);
            MouseUp += new MouseEventHandler(Mouse_Up);
            MouseWheel += new MouseEventHandler(Mouse_Wheel);
            mouseArea.MouseMove += new MouseEventHandler(Mouse_Move);
            mouseArea.MouseDown += new MouseEventHandler(Mouse_Down);
            mouseArea.MouseUp += new MouseEventHandler(Mouse_Up);
            mouseArea.MouseWheel += new MouseEventHandler(Mouse_Wheel);
            mouseArea.MouseEnter += new EventHandler(Mouse_Enter);
            Deactivate += new EventHandler(MainForm_Deactivate);
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            focusLabel.Text = "Keyboard capture paused - click this window to continue.";
            focusLabel.ForeColor = Color.DarkRed;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            focusLabel.Text = "Keyboard active - press any key. Mouse tests work over this window.";
            focusLabel.ForeColor = Color.DarkGreen;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(311, 266);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }

        private string KeyName(Keys key)
        {
            Keys k = key & Keys.KeyCode;
            string s = k.ToString();
            if (k >= Keys.A && k <= Keys.Z) return s;
            if (k >= Keys.D0 && k <= Keys.D9) return s.Substring(1);
            if (k >= Keys.NumPad0 && k <= Keys.NumPad9) return "NumPad " + s.Substring(6);
            if (k == Keys.Oemcomma) return ",";
            if (k == Keys.OemPeriod) return ".";
            if (k == Keys.OemMinus) return "-";
            if (k == Keys.Oemplus) return "+";
            if (k == Keys.Oem1) return "; / :";
            if (k == Keys.Oem2) return "/ / ?";
            if (k == Keys.Oem3) return "` / ~";
            if (k == Keys.Oem4) return "[ / {";
            if (k == Keys.Oem5) return "\\ / |";
            if (k == Keys.Oem6) return "] / }";
            if (k == Keys.Oem7) return "' / \"";
            return s;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            int code = (int)(e.KeyCode & Keys.KeyCode);
            int count;
            keyCounts.TryGetValue(code, out count);
            // Count physical key-down messages, including auto-repeat.
            keyCounts[code] = count + 1;
            totalKeys++;
            lastKeyLabel.Text = "Last key: " + KeyName(e.KeyCode) + "  (" + e.KeyCode.ToString() + ")";
            keyCountLabel.Text = "Key presses: " + totalKeys.ToString();
            UpdateKeyRow(code, e.KeyCode, count + 1, "DOWN");
            AddEvent("Key down: " + KeyName(e.KeyCode));
            e.Handled = false;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            int code = (int)(e.KeyCode & Keys.KeyCode);
            int count;
            keyCounts.TryGetValue(code, out count);
            UpdateKeyRow(code, e.KeyCode, count, "UP");
            AddEvent("Key up: " + KeyName(e.KeyCode));
        }

        private void UpdateKeyRow(int code, Keys key, int count, string state)
        {
            string vk = "0x" + code.ToString("X2");
            ListViewItem item = null;
            foreach (ListViewItem row in keyList.Items)
                if (row.SubItems[1].Text == vk) { item = row; break; }
            if (item == null)
            {
                item = new ListViewItem(KeyName(key));
                item.SubItems.Add(vk);
                item.SubItems.Add(count.ToString());
                item.SubItems.Add(state);
                keyList.Items.Add(item);
            }
            else
            {
                item.Text = KeyName(key);
                item.SubItems[2].Text = count.ToString();
                item.SubItems[3].Text = state;
            }
            item.BackColor = state == "DOWN" ? Color.LightGreen : Color.White;
            item.EnsureVisible();
        }

        private void Mouse_Enter(object sender, EventArgs e)
        {
            mouseArea.Focus();
        }

        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            Point p = Control.MousePosition;
            mousePositionLabel.Text = "Position: X=" + p.X.ToString() + "  Y=" + p.Y.ToString();
            if (!haveMouse || p != lastMouse)
            {
                moveCount++;
                mouseMoveLabel.Text = "Movement events: " + moveCount.ToString();
                lastMouse = p;
                haveMouse = true;
            }
        }

        private void Mouse_Down(object sender, MouseEventArgs e)
        {
            SetMouseButton(e.Button, true);
        }

        private void Mouse_Up(object sender, MouseEventArgs e)
        {
            SetMouseButton(e.Button, false);
        }

        private void SetMouseButton(MouseButtons button, bool down)
        {
            if (down)
            {
                if (button == MouseButtons.Left) leftCount++;
                else if (button == MouseButtons.Right) rightCount++;
                else if (button == MouseButtons.Middle) middleCount++;
                else if (button == MouseButtons.XButton1) x1Count++;
                else if (button == MouseButtons.XButton2) x2Count++;
                RefreshMouseCounts();
                AddEvent("Mouse " + button.ToString() + " down");
            }
            else AddEvent("Mouse " + button.ToString() + " up");
            int index = button == MouseButtons.Left ? 0 : button == MouseButtons.Right ? 1 : button == MouseButtons.Middle ? 2 : button == MouseButtons.XButton1 ? 3 : button == MouseButtons.XButton2 ? 4 : -1;
            if (index >= 0) mouseButtonLabels[index].BackColor = down ? Color.LightGreen : Color.White;
        }

        private void Mouse_Wheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) wheelUp += Math.Max(1, e.Delta / 120);
            else if (e.Delta < 0) wheelDown += Math.Max(1, -e.Delta / 120);
            wheelLabel.Text = "Wheel: Up " + wheelUp.ToString() + "    Down " + wheelDown.ToString();
            AddEvent(e.Delta > 0 ? "Wheel up" : "Wheel down");
        }

        private void RefreshMouseCounts()
        {
            mouseButtonLabels[0].Text = "Left button: " + leftCount.ToString();
            mouseButtonLabels[1].Text = "Right button: " + rightCount.ToString();
            mouseButtonLabels[2].Text = "Middle button: " + middleCount.ToString();
            mouseButtonLabels[3].Text = "Mouse 4: " + x1Count.ToString();
            mouseButtonLabels[4].Text = "Mouse 5: " + x2Count.ToString();
        }

        private void AddEvent(string message)
        {
            ListViewItem item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss") + "  " + message);
            eventList.Items.Insert(0, item);
            while (eventList.Items.Count > 3) eventList.Items.RemoveAt(eventList.Items.Count - 1);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            keyCounts.Clear();
            totalKeys = moveCount = wheelUp = wheelDown = 0;
            leftCount = rightCount = middleCount = x1Count = x2Count = 0;
            keyList.Items.Clear();
            eventList.Items.Clear();
            lastKeyLabel.Text = "Last key: -";
            keyCountLabel.Text = "Key presses: 0";
            mouseMoveLabel.Text = "Movement events: 0";
            wheelLabel.Text = "Wheel: Up 0    Down 0";
            RefreshMouseCounts();
            for (int i = 0; i < mouseButtonLabels.Length; i++) mouseButtonLabels[i].BackColor = Color.White;
            haveMouse = false;
        }
    }

}
