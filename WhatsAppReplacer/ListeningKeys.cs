using System.Collections.Generic;
using System.Windows.Forms;

namespace WhatsAppReplacer
{
    public class ListeningKeys: List<Keys>
    {
        public ListeningKeys()
        {
            Add(Keys.D);
            Add(Keys.D9);
            Add(Keys.OemPeriod);
            Add(Keys.LShiftKey);
            Add(Keys.RShiftKey);
        }

        private static ListeningKeys current;
        public static ListeningKeys Current => current ?? (current = new ListeningKeys());
    }
}
