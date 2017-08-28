using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemeVisualisation
{
    class Queue
    {
        private List<Symbol> _line;

        public Queue()
        {
            this._line = new List<Symbol>();
        }


        public void Add(Symbol S)
        {
            this._line.Add(S);
        }

        public Symbol Get()
        {
            Symbol S = this._line[0];
            this._line.RemoveAt(0);
            return S;
        }

        public bool NotEmpty()
        {
            return this._line.Count != 0;
        }
    }
}
