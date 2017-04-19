using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WhatsAppReplacer
{
    public partial class frmMainReplacer : Form
    {
        Buffer buffer;

        public frmMainReplacer()
        {
            InitializeComponent();

            buffer = new Buffer();
            buffer.OnHandle += Buffer_OnHandle;

            //MapList.Current.Add(':', ')', "😊");
            MapList.Current.Add(':', ')', "gulucuk");
            //MapList.Current.Add(':', 'D', "😂");
            //MapList.Current.Add(':', 'd', "😂");
            //MapList.Current.Add(';', ')', "😉");
        }

        private void Buffer_OnHandle(object sender, OnHandleValueEventArgs e)
        {
            Console.WriteLine("Buffer_OnHandle." + e.ToString());
            for (int i = 0; i < e.Length; i++)
                SendKeys.SendWait("{BACKSPACE}");

            String willSendValue = MapList.Current.GetSymbol(e.Prefix, e.Value);
            Console.WriteLine("willSendValue: " + willSendValue);
            SendKeys.SendWait(willSendValue);
        }

        KeyboardHook keyboardHook;

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
                keyboardHook = new KeyboardHook(true);
                keyboardHook.KeyUp += KeyboardHook_KeyUp;

                lblStatus.Text = "Listening Started";
            }
            else
            {
                keyboardHook?.Dispose();
                lblStatus.Text = "Listening Stopped";
            }

            lblActive.Text = e;
        }

        private void KeyboardHook_KeyUp(Keys key, bool Shift, bool Ctrl, bool Alt)
        {
            if (key == Keys.RShiftKey || key == Keys.LShiftKey)
                return;

            buffer.Add(ConverToString(key, Shift));

            Console.WriteLine(ConverToString(key, Shift));
        }

        public string ConverToString(Keys Keys, bool Shift = false)
        {
            Console.WriteLine(Keys.ToString());
            string name = String.Empty;

            if (Shift)
            {
                if (Keys == (Keys.D9))
                    name = ")";
                else
                if (Keys == (Keys.D8))
                    name = "(";
                else if (Keys == (Keys.OemPeriod))
                    name = ":";
                else if (Keys == (Keys.Oemcomma))
                    name = ";";
                else if (Keys == (Keys.OemQuestion))
                    name = "*";
            }
            else
                name = (new KeysConverter()).ConvertToString(Keys);

            return name;
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
