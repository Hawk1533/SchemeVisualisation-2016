using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace SchemeVisualisation
{
    public class X_U : Symbol
    {
        private bool _isDriver;
        public X_U(Entity Src, List<PortS> P)
            : base(Src, P)
        {
            this.LPorts = new List<string>();
            this.boxX = 2;
            this.boxY = 2;
            this.Zero = false;
        }

        public X_U(Entity Src, bool driver)
            : base(Src)
        {
            this.LPorts = new List<string>();
            this.boxX = 2;
            this.boxY = 2;
            this.Zero = false;
            this._isDriver = driver;
        }

        public override void DrawMe(Graphics e, int kx, int ky, float x0, float y0) //Масштаб + начальные коо
        {
            //Рисую в координатах это картинку с заданным размером и толщиной линии!!!
            int width = kx * this.boxX / 3; //Пытаюсь подобрать хороший
            int height = ky * this.boxY / 3;
            e.DrawPolygon(Setting.Global.Pen1, new PointF[] { new PointF(x0, y0), new PointF(x0, y0 + height), new PointF(x0 + width, y0 + height / 2) });

            string line = ""; //Определяем что писать!
            if (IsDriver) line = "D";
            else line = "Re";
            FillRec(x0, y0 - height * 1 / 2, width * 20 / 10, height * 3 / 4);
            string name = this.Src.Name.Substring(2);
            e.DrawString(name, Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
            FillRec(x0, y0, width * 8 / 10, height);
            e.DrawString(line, Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
        }

        public override void DrawMe(Graphics e, int kx, int ky, float x0, float y0, Color col) //Масштаб + начальные коо
        {
            //Рисую в координатах это картинку с заданным размером и толщиной линии!!!
            int width = kx * this.boxX / 3; //Пытаюсь подобрать хороший
            int height = ky * this.boxY / 3;
            e.DrawPolygon(Setting.Global.Pen4, new PointF[] { new PointF(x0, y0), new PointF(x0, y0 + height), new PointF(x0 + width, y0 + height / 2) });

            string line = "";
            if (IsDriver) line = "D";
            else line = "Re";
            FillRec(x0, y0 - height * 1 / 2, width * 20 / 10, height * 3 / 4);
            string name = this.Src.Name.Substring(2);
            e.DrawString(name, Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
            FillRec(x0, y0, width * 8 / 10, height);
            e.DrawString(line, Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
        }


        public override void FillPortPoint(Symbol S, PortS P)
        {
            float width = (float)this.boxX / (float)3; //Пытаюсь подобрать хороший
            float height = (float)this.boxY / (float)3;
            
            if (IsDriver)
            {
                P.AddPoint(new Point(this.X() + width, this.Y() + height / 2));
            }

            else
            {
                P.AddPoint(new Point(this.X(), this.Y() + height / 2));
            }
        }

        public bool IsDriver
        {
            get { return this._isDriver; }
            set { this._isDriver = value; }
        }

        public override char GetType()
        {
            if (IsDriver) return 'D';
            else return 'E'; //Типо ресивер
        }
    }
}