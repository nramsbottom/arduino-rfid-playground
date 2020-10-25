using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AccessControl.WinApp
{

    public partial class MainForm : Form
    {
        private event EventHandler<string> MessageReceived;
        private delegate void SetTextBoxText(TextBox textBox, string text);

        private readonly SerialPortMessageReader _serialPortMessageReader;

        public MainForm()
        {
            InitializeComponent();
            _serialPortMessageReader = new SerialPortMessageReader("COM5", 9600);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Icon = trayIcon.Icon = Resource1.Tray;
            trayIcon.ContextMenuStrip = trayMenuStrip;

            _serialPortMessageReader.MessageReceived += SerialPortMessageReader_MessageReceived;
            _serialPortMessageReader.Start();

            MessageReceived += MainForm_MessageReceived;
        }

        private void SerialPortMessageReader_MessageReceived(object sender, string e)
        {
            MessageReceived?.Invoke(sender, e);
        }

        private void MainForm_MessageReceived(object sender, string e)
        {
            Debug.WriteLine($"Received message: {e}");

            var tokens = e.Split(new char[] { '|' });

            if (tokens[0] != "CARD")
            {
                return;
            }

            var cardId = tokens[1];

            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new SetTextBoxText(SetTextBoxTextImpl), textBox1, cardId);
            }
            else
            {
                SetTextBoxTextImpl(textBox1, cardId);
            }
        }

        private void SetTextBoxTextImpl(TextBox textBox, string text)
        {
            textBox.Text = text;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var settings = new SettingsForm())
            {
                if (settings.ShowDialog(this) == DialogResult.Cancel)
                {
                    return;
                }

                throw new NotImplementedException();
            }
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            settingsToolStripMenuItem_Click(sender, null);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
            }
        }

        private void showUIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _serialPortMessageReader.Stop();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _serialPortMessageReader.Stop();
            Application.Exit();
        }
    }

}
