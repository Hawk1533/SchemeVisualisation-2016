using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SchemeVisualisation
{
    public class Setting //Класс для общих объектов
    {
        public StringFormat Format;
        public Font Font;
        public Brush Brush;
        public PointF ans;
        public RectangleF Rec;
        public Pen Pen1;
        public Pen Pen2;
        public Pen Pen3;
        public Pen Pen4;

        //To make object of this class global (Singleton)
        private static Setting _global; 

        private Setting() { } //no one can create another instance

        public static Setting Global
           {
              get 
              {
                  if (_global == null)
                 {
                     _global = new Setting();
                 }
                  return _global;
              }
           }

        public void Set()
        {
            this.Format = new StringFormat();
            this.Format.Alignment = StringAlignment.Center;
            this.Format.LineAlignment = StringAlignment.Center;

            this.Rec = new RectangleF();
            this.Font = new Font(FontFamily.GenericSansSerif, 10);
            this.Brush = new SolidBrush(Color.Black);
            this.Pen1 = new Pen(new SolidBrush(Color.Black));
            this.Pen2 = new Pen(new SolidBrush(Color.Red),2);
            this.Pen3 = new Pen(new SolidBrush(Color.Black), 3);
            this.Pen4 = new Pen(new SolidBrush(Color.Black), 5);
        }

        
    }
}
