using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchemeVisualisation
{
    public class Point
    {
        private float _x;
        private float _y;

        public Point(float X, float Y)
        {
            this._x = X;
            this._y = Y;
        }

        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

    }
}
