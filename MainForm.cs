using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CybersecurityChatbot_GUI
{
    public partial class MainForm : Form
    {
        private ChatbotEngine? _chatbot;
        private TextBox? txtInput;
        private RichTextBox? rtxtOutput;
        private Button? btnSend;
        private Label? lblAsciiArt;
        private Label? lblStatus;

        public MainForm()
        {
            InitializeComponent();
            SetupCustomControls();
            _chatbot = new ChatbotEngine();

            // Play voice greeting
            string audioPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
            AudioPlayer.PlayGreeting(audioPath);

            // Display ASCII art
            if (lblAsciiArt != null)
            {
                lblAsciiArt.Text = AsciiArt.Logo;
                lblAsciiArt.Font = new Font("Consolas", 7);
                lblAsciiArt.ForeColor = Color.Cyan;
                lblAsciiArt.TextAlign = ContentAlignment.MiddleCenter;
                lblAsciiArt.BackColor = Color.Black;
            }

            // Welcome message
            AppendBotMessage("Hello! I'm your Cybersecurity Awareness Bot. What's your name?");
            AppendBotMessage("I have over 75 cybersecurity tips! Ask me about passwords, phishing, safe browsing, privacy, or scams.");
        }

        private void SetupCustomControls()
        {
            // Create controls
            rtxtOutput = new RichTextBox();
            txtInput = new TextBox();
            btnSend = new Button();
            lblAsciiArt = new Label();
            lblStatus = new Label();

            // RichTextBox (chat history)
            rtxtOutput!.Location = new Point(12, 220);
            rtxtOutput.Size = new Size(560, 300);
            rtxtOutput.ReadOnly = true;
            rtxtOutput.BackColor = Color.Black;
            rtxtOutput.ForeColor = Color.White;
            rtxtOutput.Font = new Font("Segoe UI", 10);
            rtxtOutput.BorderStyle = BorderStyle.FixedSingle;

            // TextBox (user input)
            txtInput!.Location = new Point(12, 530);
            txtInput.Size = new Size(450, 27);
            txtInput.Font = new Font("Segoe UI", 10);
            txtInput.BackColor = Color.White;
            txtInput.KeyDown += TxtInput_KeyDown!;

            // Send button
            btnSend!.Location = new Point(470, 528);
            btnSend.Size = new Size(100, 30);
            btnSend.Text = "Send";
            btnSend.BackColor = Color.FromArgb(0, 100, 0);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Click += BtnSend_Click!;

            // ASCII art label
            lblAsciiArt!.Location = new Point(12, 12);
            lblAsciiArt.Size = new Size(560, 190);
            lblAsciiArt.BackColor = Color.Black;
            lblAsciiArt.Font = new Font("Consolas", 7);
            lblAsciiArt.ForeColor = Color.Cyan;
            lblAsciiArt.TextAlign = ContentAlignment.MiddleCenter;

            // Status label
            lblStatus!.Location = new Point(12, 205);
            lblStatus.Size = new Size(560, 15);
            lblStatus.Text = "🎓 75+ cybersecurity tips available | Say 'tell me more' for another tip";
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Font = new Font("Segoe UI", 8);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to form
            Controls.Add(lblStatus);
            Controls.Add(lblAsciiArt);
            Controls.Add(btnSend);
            Controls.Add(txtInput);
            Controls.Add(rtxtOutput);
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            ProcessUserInput();
        }

        private void TxtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessUserInput();
                e.SuppressKeyPress = true;
            }
        }

        private void ProcessUserInput()
        {
            if (txtInput == null || rtxtOutput == null || _chatbot == null) return;

            string userMessage = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                AppendBotMessage("Please type something. I have 75+ tips to share!");
                return;
            }

            AppendUserMessage(userMessage);
            txtInput.Clear();

            string botResponse = _chatbot.GetResponse(userMessage);
            AppendBotMessage(botResponse);
        }

        private void AppendUserMessage(string msg)
        {
            if (rtxtOutput == null) return;
            rtxtOutput.SelectionColor = Color.LightGreen;
            rtxtOutput.AppendText($"👤 You: {msg}\n");
            rtxtOutput.ScrollToCaret();
        }

        private void AppendBotMessage(string msg)
        {
            if (rtxtOutput == null) return;
            rtxtOutput.SelectionColor = Color.Yellow;
            rtxtOutput.AppendText($"🤖 Bot: {msg}\n\n");
            rtxtOutput.ScrollToCaret();
            Application.DoEvents();
            Thread.Sleep(50);
        }
    }
}