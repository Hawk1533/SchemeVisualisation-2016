using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SchemeVisualisation
{
    public class PortS
    {
        private string _name;
        private List<Symbol> _sym;
        private List<Point> _points;

        public PortS() { }

        public PortS(string Name, List<Symbol> Sym)
        {
            this._name = Name;
            this._sym = Sym; //Точно можно?
            this._points = new List<Point>();
        }

        public PortS(string Name)
        {
            this._name = Name;
            this._sym = new List<Symbol>();
            this._points = new List<Point>();
        }

        public bool IsHunger()
        {
            if (this.Sym.Count == 1 || (this.Sym.Count == 2 && (this.Sym[0].Src.Name.ToLower().Contains("r_conv") || this.Sym[1].Src.Name.ToLower().Contains("r_conv")))) return true;
            else return false;
        }

        public bool InBox(int X, int Y, int kx, int ky, float xStart, float yStart)
        {
            foreach (Point P in this.Points)
            {
                float x1 = kx * (P.X - xStart);
                float y1 = ky * (P.Y - yStart);
                float t = 7; //Коэф перевода
                if (X < x1 + kx/t && X > x1-kx/t && Y < y1+ ky/t && Y > y1 - ky/t) return true;
                
            }
            return false;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void AddPoint(Point A)
        {
            bool HasGot = false;
            foreach (Point P in Points)
            {
                if (P.X == A.X && P.Y == A.Y)
                {
                    HasGot = true;
                    break;
                }
            }
            if (!HasGot) this.Points.Add(A);
        }

        public List<Symbol> Sym
        {
            get { return _sym; }
            set { _sym = value; }
        }

        public List<Point> Points
        {
            get { return _points; }
            set { _points = value; }
        }
    }
}
