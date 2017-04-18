using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppReplacer
{
    public class Buffer
    {
        StringBuilder buffer;

        public bool IsWatching { get; set; }
        public event EventHandler<OnHandleValueEventArgs> OnHandle;
        private List<char> handleValueList;
        private char lastPrefix;

        public Buffer()
        {
            buffer = new StringBuilder();
        }

        public void Add(String Value)
        {
            for (int i = 0; i < Value.Length; i++)
                Add(Value[i]);
        }

        public void AddLine(String Value)
        {
            for (int i = 0; i < Value.Length; i++)
                AddLine(Value[i]);
        }

        public void Add(char Value)
        {
            if (!IsWatching)
            {
                if (KeyMap.Current.ContainsKey(Value))
                {
                    lastPrefix = Value;
                    handleValueList = KeyMap.Current.GetValue(Value);
                    StartWatching();
                }
            }
            else
            {
                if (handleValueList?.IndexOf(Value) > -1)
                {
                    Console.WriteLine("OnHandle?.Invoke");
                    OnHandle?.Invoke(this, new OnHandleValueEventArgs(lastPrefix, Value));
                    lastPrefix = '\0';
                }
                StopWatching();
            }




            buffer.Append(Value);
        }

        public void AddLine(char Value)
        {
            Add(Value);
            Add("");
        }

        private void StartWatching()
        {
            IsWatching = true;
            Console.WriteLine("Start Watching");
        }

        private void StopWatching()
        {
            IsWatching = false;
            Console.WriteLine("Stop Watching");
        }

    }
}
