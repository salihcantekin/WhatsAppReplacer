using System.Collections.Generic;

namespace WhatsAppReplacer
{
    public class KeyMap
    {

        private static KeyMap current;
        public static KeyMap Current => current ?? (current = new KeyMap());




        List<char> prefixList;
        List<char> valueList;
        List<string> symbolList;


        public KeyMap()
        {
            prefixList = new List<char>();
            valueList = new List<char>();
            symbolList = new List<string>();

            //Add(":D", "😂");
            //Add(":B", "😎");
        }

        public void Add(char Prefix, char Value, string Symbol)
        {
            prefixList.Add(Prefix);
            valueList.Add(Value);
            symbolList.Add(Symbol);
        }

        public string GetSymbol(char Prefix)
        {
            return symbolList[prefixList.IndexOf(Prefix)];
        }

        public bool ContainsKey(char Key)
        {
            return prefixList.IndexOf(Key) > -1;
        }

        public char GetValue(char Key)
        {
            return valueList[prefixList.IndexOf(Key)];
        }

        
    }
}
