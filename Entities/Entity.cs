using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemeVisualisation
{
    public class Entity
    {
        private string _name;
        private List<string> _info;
        private List<string> _ports;

        public Entity() { }

        public Entity(string Name, List<string> Info, List<string> Ports)
        {
            this._name = Name;
            this._info = Info; // Точно так можно?
            this._ports = Ports;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<string> Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public List<string> Ports
        {
            get { return _ports; }
            set { _ports = value; }
        }



    }
}
