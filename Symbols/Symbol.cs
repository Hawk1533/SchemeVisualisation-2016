using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SchemeVisualisation
{
    public abstract class Symbol
    {
        private List<Point> _points; //Draw coordinates
        private Entity _src; 
        private List<PortS> _ports;
        private bool _ready;
        private Symbol _parent;
        private List<Symbol> _children;
        private int _boxX, _boxY; //Относительные отрисовочные размеры
        private List<string> _lPorts, _rPorts;
        private bool _zero;

        public Symbol(Entity Src, List<PortS> P)
        {
            this._src = Src;
            this._ports = P;
            this._points = new List<Point>();
            this._children = new List<Symbol>();
            this._ready = false;     
        }

        public Symbol(Entity Src)
        {
            this._src = Src;
            this._ports = new List<PortS>();
            this._points = new List<Point>();
            this._ready = false;             //!!Везде ли
            this._children = new List<Symbol>();
        }

        public virtual void DrawMe(Graphics e, int kx, int ky, float x0, float y0) { }

        public virtual void DrawMe(Graphics e, int kx, int ky, float x0, float y0, Color col) { }

        public virtual void FillPortPoint(Symbol S, PortS P) { }

        public bool InBox(int x, int y, int kx, int ky, float xStart, float yStart) //x -точка, kx - масштаб, xStart - начало экрана в сетке
        {
            float x1 = kx * (this.X() - xStart);
            float y1 = ky * (this.Y() - yStart);
            float x2 = x1 + kx * this.boxX / 3;
            float y2 = y1 + ky * this.boxY / 3;

            if (x < x2 && x > x1 && y < y2 && y > y1) return true;
            else return false;
        }

        public void MoveY(float n) //Сдвигаем в сетке вниз + вверх -
        {
            for (int i = 0; i < this.Points.Count; i++)
            {
                this._points[i].Y += n;
            }
        }

        public void FillRec(float x0, float y0, float width, float height)
        {
            Setting.Global.Rec.X = x0;
            Setting.Global.Rec.Y = y0;
            Setting.Global.Rec.Width = width;
            Setting.Global.Rec.Height = height;
        }

        public virtual int CountBoxY() { return 1; }

        public virtual void CreatePortCopy() { }


        public virtual char GetType()
        {
            return this.Src.Name[0];
        }

        public float X()
        {
            return this.Points[0].X;
        }

        public float Y()
        {
            return this.Points[0].Y;
        }

        public bool Zero
        {
            get { return _zero; }
            set { _zero = value; }
        }

        public Entity Src
        {
            get { return _src; }
            set { _src = value; }
        }

        public List<PortS> Ports
        {
            get { return _ports; }
            set { _ports = value; }
        }

        public List<Point> Points
        {
            get { return _points; }
            set { _points = value; }
        }
        
        public List<string> LPorts
        {
            get { return _lPorts; }
            set { _lPorts = value; }
        }

        public List<string> RPorts
        {
            get { return _rPorts; }
            set { _rPorts = value; }
        }

        public bool Ready
        {
            get { return _ready; }
            set { _ready = value; }
        }


        public int boxX
        {
            get { return _boxX; }
            set { _boxX = value; }
        }

        public int boxY
        {
            get { return _boxY; }
            set { _boxY = value; }
        }

        public Symbol Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<Symbol> Children
        {
            get { return _children; }
            set { _children = value; }
        }
    }
}
