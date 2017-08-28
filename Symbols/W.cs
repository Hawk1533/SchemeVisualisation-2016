using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemeVisualisation
{
    public class W
    {
        private List<Symbol> _parts;

        public W() { this._parts = new List<Symbol>(); }

        public W(List<Symbol> Parts)
        {
            this._parts = Parts;
        }

        public List<Symbol> Parts
        {
            get { return this._parts; }
            set { this._parts = value; }
        }
    }
}
