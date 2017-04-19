using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppReplacer
{
    public class MapList : List<Map>
    {
        private static MapList current;
        public static MapList Current => current ?? (current = new MapList());

        public void Add(char Prefix, char Value, String Symbol)
        {
            Add(new Map() { Prefix = Prefix, Value = Value, Symbol = Symbol });
        }

        public List<char> GetValues(char Prefix)
        {
            return this.Where(i => i.Prefix.Equals(Prefix)).
                Select(i => i.Value).
                ToList();
        }

        public bool Contains(char Prefix)
        {
            return this.Any(i => i.Prefix.Equals(Prefix));
        }

        public String GetSymbol(char Prefix, char Value)
        {
           return this.Where(i => i.Prefix.Equals(Prefix) && i.Value.Equals(Value)).
                Select(i => i.Symbol).
                First();
        }
    }
}
