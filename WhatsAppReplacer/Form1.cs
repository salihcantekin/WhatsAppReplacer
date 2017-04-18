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


            KeyMap.Current.Add(':', ')', "😂");
            KeyMap.Current.Add(':', 'D', "😂");
            KeyMap.Current.Add(':', 'd', "😂");
            KeyMap.Current.Add(';', ')', "😂");
        }

        private void Buffer_OnHandle(object sender, OnHandleValueEventArgs e)
        {
            for (int i = 0; i < e.Length; i++)
                SendKeys.SendWait("{BACKSPACE}");

            String willSendValue = KeyMap.Current.GetSymbol(e.Prefix);
            Console.WriteLine("willSendValue: " + willSendValue);
            SendKeys.SendWait("SALIH");
        }



        //globalKeyboardHook keyboardHook;
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
            if (e.ToLower().Equals("notepad"))
            {
                keyboardHook = new KeyboardHook(true);
                keyboardHook.KeyUp += KeyboardHook_KeyUp;

                lblStatus.Text = "Listening Started";

                //keyboardHook = new globalKeyboardHook();
                //keyboardHook.HookedKeys = ListeningKeys.Current;
                //keyboardHook.KeyUp += Gkh_KeyUp;
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

            buffer.Add(ConverToString(key));



            //if (pressed && (key == Keys.D || key == Keys.D9))
            //{
            //    SendKeys.SendWait("{BACKSPACE}");
            //    SendKeys.SendWait("{BACKSPACE}");
            //    //SendKeys.SendWait("😎");
            //    SendKeys.SendWait("😂");
            //}

            //if (pressed && key == Keys.RShiftKey) { }
            //else
            //    pressed = key == Keys.OemPeriod | (key == Keys.RShiftKey || key == Keys.LShiftKey);

            Console.WriteLine(ConverToString(key));
        }

        public string ConverToString(Keys Keys)
        {
            string name;
            switch (Keys)
            {
                case Keys.OemPeriod | Keys.LShiftKey:
                case Keys.OemPeriod | Keys.RShiftKey:
                    name = ":";
                    break;
                case Keys.Oemcomma:
                    name = "Comma";
                    break;
                default:
                    name = (new KeysConverter()).ConvertToString(Keys);
                    break;
            }

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
