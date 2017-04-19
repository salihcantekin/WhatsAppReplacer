using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppReplacer
{
    public class OnHandleValueEventArgs : EventArgs
    {
        public char Prefix { get; set; }

        public char Value { get; set; }

        public int Length { get { return 2; } }

        public OnHandleValueEventArgs(char Prefix, char Value)
        {
            this.Prefix = Prefix;
            this.Value = Value;
        }

        public override string ToString()
        {
            return $"Prefix: {Prefix}, Value: {Value}";
        }
    }
}
