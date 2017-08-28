using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace SchemeVisualisation
{
    public class X_Box : Symbol //
    {
        private List<PortS> _pCopy;
        public X_Box(Entity Src, List<PortS> P)
            : base(Src, P)
        {
            this.LPorts = new List<string>(); //Здесь не важна ориентация. Важно что на уровне по порту
            this.boxX = 1;
            this.boxY = P.Count;
            this.Zero = false;
            this._pCopy = new List<PortS>(P); //Создаем копию, что при сортировке не потерять последовательность
        }

        public X_Box(Entity Src)
            : base(Src)
        {
            this.LPorts = new List<string>(); //Здесь не важна ориентация. Важно что на уровне по порту
            this.boxX = 1;
            this.boxY = CountBoxY();
            this.Zero = false;
            this._pCopy = new List<PortS>();
        }

        public override void DrawMe(Graphics e, int kx, int ky, float x0, float y0) //Масштаб + начальные коо
        {
            CountBoxY(); //Не вышло в конструкторе
            int width = kx * this.boxX / 3; //Пытаюсь подобрать хороший
            int height = ky * this.boxY / 3;
            e.DrawRectangle(Setting.Global.Pen1, x0, y0, width, height);


            FillRec(x0, y0, width, height);
            e.DrawString("X", Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
            FillRec(x0 - width , y0 - height * 2 / 8, width * 3, height * 3 / 8);
            string name = this.Src.Name.Substring(2);
            e.DrawString(name, Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует; 
        }

        public override void DrawMe(Graphics e, int kx, int ky, float x0, float y0, Color col) //Масштаб + начальные коо
        {
            CountBoxY(); 
            //Рисую в координатах это картинку с заданным размером и толщиной линии!!!
            Setting.Global.Pen4.Color = col;
            int width = kx * this.boxX / 3; //Пытаюсь подобрать хороший
            int height = ky * this.boxY / 3;
            e.DrawRectangle(Setting.Global.Pen4, x0, y0, width, height);

            FillRec(x0, y0, width, height);
            e.DrawString("X", Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
            FillRec(x0 - width , y0 - height * 2 / 8, width * 3, height * 3 / 8);
            string name = this.Src.Name.Substring(2);
            e.DrawString(name, Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует; 
        }

        public override void FillPortPoint(Symbol S, PortS P)
        {
            CountBoxY(); 
            float width = (float)this.boxX / (float)3; //Пытаюсь подобрать хороший
            float height = (float)this.boxY / (float)3;

            int i, k = 0;
            for (i = 0; i < this.pCopy.Count; i++)
            {
                if (this.pCopy[i].Name == P.Name) break; //Находим номер этого порта
                if (this.pCopy[i].Sym.Count < 2) k++;
            }

            CountBoxY();
            i -= k; //Вычитаем нулевые порты без связей
            double j = i;
            j += 0.5;

            if (this.X() - S.X() >= 0)
            {
                P.AddPoint(new Point(this.X(), this.Y() + height * (float)(j) / (this.boxY)));
            }
            else
            {
                P.AddPoint(new Point(this.X() + width, this.Y() + height * (float)(j) / (this.boxY)));
            }
        }

        public override void CreatePortCopy() 
        {
            this._pCopy = new List<PortS>(this.Ports); //Создаем копию, что при сортировке не потерять последовательность
        }

        public override int CountBoxY()
        {
            int ans = 0;
            foreach (PortS P in this.Ports)
            {
                if (P.Sym.Count > 1) ans++;
            }

            this.boxY = ans;
            return ans;
        }

        public List<PortS> pCopy
        {
            get { return this._pCopy; }
            set { this._pCopy = value; }
        }

        public override char GetType()
        {
            return 'B';
        }
    }
}