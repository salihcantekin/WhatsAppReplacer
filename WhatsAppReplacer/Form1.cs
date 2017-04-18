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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool pressed = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            HookManager.SubscribeToWindowEvents();
            HookManager.OnActiveAppChanged += HookManager_OnActiveAppChanged;

            globalKeyboardHook gkh = new globalKeyboardHook();
            
            gkh.HookedKeys.Add(Keys.D);
            gkh.HookedKeys.Add(Keys.D9);
            
            gkh.HookedKeys.Add(Keys.OemPeriod);
            gkh.HookedKeys.Add(Keys.LShiftKey);
            gkh.HookedKeys.Add(Keys.RShiftKey);
            //gkh.KeyDown += Gkh_KeyDown;
            gkh.KeyUp += Gkh_KeyUp;

            Task.Factory.StartNew(new Action(() => EventLoop.Run()));
        }

        private void HookManager_OnActiveAppChanged(object sender, string e)
        {
            this.Text = e;
        }

        private void Gkh_KeyUp(object sender, KeyEventArgs e)
        {
            //if (HookManager.activeAppName.Equals("whatsapp"))
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
                    pressed = e.KeyCode == Keys.OemPeriod | e.KeyCode == Keys.RShiftKey;

                Console.WriteLine("Pressed: " + pressed);
            }
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            Show();
            WindowState = FormWindowState.Normal;
        }
    }
}
