using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemeVisualisation
{
    public class Port
    {
        private string _name;
        private List<string> _ent;  //Sorting for W

        public Port() { }

        public Port(string Name, List<string> Ent)
        {
            this._name = Name;
            this._ent = Ent; //Точно можно?
        }

        public Port(string Name, string Ent)
        {
            this._name = Name;
            List<string> Li = new List<string>();
            Li.Add(Ent);
            this._ent = Li;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<string> Ent
        {
            get { return _ent; }
            set { _ent = value; }
        }
    }
}
