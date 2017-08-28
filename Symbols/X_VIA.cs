using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace SchemeVisualisation
{
    public class X_VIA : Symbol
    {
        private List<PortS> _pCopy;
        public X_VIA(Entity Src, List<PortS> P)
            : base(Src, P)
        {
            this.LPorts = new List<string>(); //Здесь не важна ориентация. Важно что на уровне по порту
            this.boxX = 1;
            CountBoxY(); 
            this.Zero = false;
            this._pCopy = new List<PortS>(P); //Создаем копию, что при сортировке не потерять последовательность
        }

        public X_VIA(Entity Src)
            : base(Src)
        {
            this.LPorts = new List<string>(); //Здесь не важна ориентация. Важно что на уровне по порту
            this.boxX = 1;
            //this.boxY = P.Count;
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
            e.DrawString("Via", Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
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
            e.DrawString("Via", Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
        }


        public override void FillPortPoint(Symbol S, PortS P)
        {
            if ((this.Src.Name == "X_VIA_2") && P.Name == "via_2_000000e_002_5_000000e_002_layer_3")
            {
                Console.Write('1');
            }

            CountBoxY(); 
            float width = (float)this.boxX / (float)3; //Пытаюсь подобрать хороший
            float height = (float)this.boxY / (float)3;

            int i, k = 0;
            for (i = 0; i < this.pCopy.Count; i++)
            {
                if (this.pCopy[i].Name == P.Name) break; //Находим номер этого порта
                if (this.pCopy[i].Sym.Count < 2) k++;
            }

            i -= k; //Вычитаем нулевые порты без связей
            double j = i;
            j += 0.5;

            if (this.X() - S.X() >= 0) //Здесь проблема! Нужно как-то сделать чтобы при десятикратной открисовке ядра  не менялся порт
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

        public List<PortS> pCopy
        {
            get { return this._pCopy; }
            set { this._pCopy = value; }
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
    }
}
