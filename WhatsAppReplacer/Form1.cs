using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhatsAppReplacer
{
    public partial class frmMainReplacer : Form
    {
        public frmMainReplacer()
        {
            InitializeComponent();
        }

        globalKeyboardHook keyboardHook;

        private bool pressed = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            lblActive.Text = "";
            HookManager.SubscribeToWindowEvents();
            HookManager.OnActiveAppChanged += HookManager_OnActiveAppChanged;
        }

        private void HookManager_OnActiveAppChanged(object sender, string e)
        {
            if (e.ToLower().Equals("whatsapp"))
            {
                keyboardHook = new globalKeyboardHook();
                keyboardHook.HookedKeys = ListeningKeys.Current;
                keyboardHook.KeyUp += Gkh_KeyUp;
            }
            else
                keyboardHook = null;

            lblActive.Text = e;
        }

        private void Gkh_KeyUp(object sender, KeyEventArgs e)
        {
            if (pressed && (e.KeyCode == Keys.D || e.KeyCode == Keys.D9))
            {
                SendKeys.SendWait("{BACKSPACE}");
                SendKeys.SendWait("{BACKSPACE}");
                //SendKeys.SendWait("😎");
                SendKeys.SendWait("😂");
            }

            if (pressed && e.KeyCode == Keys.RShiftKey) { }
            else
                pressed = e.KeyCode == Keys.OemPeriod | (e.KeyCode == Keys.RShiftKey || e.KeyCode == Keys.LShiftKey);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipText = "Check Please !";
                notifyIcon1.BalloonTipTitle = "App Minimized";
                notifyIcon1.ShowBalloonTip(500);
                Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            Show();
            WindowState = FormWindowState.Normal;
        }
    }
}
