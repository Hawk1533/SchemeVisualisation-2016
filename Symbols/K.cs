using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SchemeVisualisation
{
    public class K : Symbol
    {
        public K(Entity Src, List<PortS> P)
            : base(Src, P)
        {
            this.LPorts = new List<string>(1);
            this.RPorts = new List<string>(1);
            this.boxX = 2;
            this.boxY = 1;
            this.Zero = false;
        }

        public K(Entity Src)
            : base(Src)
        {
            this.LPorts = new List<string>(1);
            this.RPorts = new List<string>(1);
            this.boxX = 2;
            this.boxY = 1;
            this.Zero = false;
        }

        public override void DrawMe(Graphics e, int kx, int ky, float x0, float y0) //Масштаб + начальные коо
        {
            //Рисую в координатах это картинку с заданным размером и толщиной линии!!!
            int width = kx * this.boxX / 3; //Пытаюсь подобрать хороший
            int height = ky * this.boxY / 3;
            e.DrawRectangle(Setting.Global.Pen1, x0, y0, width, height);

            FillRec(x0, y0, width, height);
            e.DrawString("TL", Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
        }

        public override void DrawMe(Graphics e, int kx, int ky, float x0, float y0, Color col) //Масштаб + начальные коо
        {
            //Рисую в координатах это картинку с заданным размером и толщиной линии!!!
            Setting.Global.Pen4.Color = col;
            int width = kx * this.boxX / 3; //Пытаюсь подобрать хороший
            int height = ky * this.boxY / 3;

            e.DrawRectangle(Setting.Global.Pen4, x0, y0, width, height);
            FillRec(x0, y0, width, height);
            e.DrawString("TL", Setting.Global.Font, Setting.Global.Brush, Setting.Global.Rec, Setting.Global.Format); //Зашибись, сама центрует
        }

        public override void FillPortPoint(Symbol S, PortS P)
        {
            float width = (float)this.boxX / (float)3; //Пытаюсь подобрать хороший
            float height = (float)this.boxY / (float)3;

            if (!(LPorts.Count != 0 && LPorts[0] == P.Name) && !(RPorts.Count != 0 && RPorts[0] == P.Name)) //Нет такого порта - добавить
            {
                if ((this.X() - S.X() >= 0 && LPorts.Count == 0) || RPorts.Count == 1) //Ставим порт слева
                {
                    LPorts.Add(P.Name);
                    P.AddPoint(new Point(this.X(), this.Y() + height / 2));
                }
                else //Ставим порт справа
                {
                    RPorts.Add(P.Name);
                    P.AddPoint(new Point(this.X() + width, this.Y() + height / 2));
                }
            }
        }
    }
}
