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
    //To do:
    //X - запоминать таки с какой стороны порт
    ////Здесь проблема! Нужно как-то сделать чтобы при десятикратной открисовке ядра  не менялся порт

    public partial class Form1 : Form
    {
        public Dictionary<string, Entity> Elements;
        public Dictionary<string, Port> Ports;
        public List<Symbol> Symbols, UpperSymbols;
        public Dictionary<string, PortS> PortsS, PortsScopy;
        string currentEnt;
        List<string> PortNames;
        string entName, portName;
        Entity E;
        int i = 0, portsRead = 0, n = 1, upperK = 1, lowerK = 14, kx = 65, ky = 55;
        char first, currentElem = ' ', nextElem = ' ';
        string t;
        PortS P;
        Symbol S, Clicked;
        bool draw = false, move = false, wDraw = false, haveMoved = false;
        int mouseX = -1, mouseY = -1, wNum = 0;
        float xStart = 0, yStart = 0, d;
        List<W> ListW;
        List<Symbol> Found = new List<Symbol>();
        public Dictionary<string, bool> Y;
        public int[] Placed;
        PointF A, B;



        public Form1()
        {
            InitializeComponent();
            Elements = new Dictionary<string, Entity>();
            Ports = new Dictionary<string, Port>();
            pb.Minimum = 1;
            pb.Value = 1;
            DoubleBuffered = true; //Чтобы не моргало
            this.MouseWheel += new MouseEventHandler(this_MouseWheel);
            Setting.Global.Set();
        }

        //******************************************************************Base Reading*****************************************************
        public void CreateBase(string filename)
        {
            Elements = new Dictionary<string, Entity>();
            Ports = new Dictionary<string, Port>();
            Y = new Dictionary<string, bool>();

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            StreamReader sr = new StreamReader(fs);
            string line = null, nextLine = null;
            first = '*';
            int i;

            pb.Value = 1;
            if (fs != null) pb.Maximum = (int)fs.Length;
            pb.Step = 165;

            line = sr.ReadLine();
            while ((nextLine = sr.ReadLine()) != null)
            {
                n++;
                if (line != "") first = line[0];
                if (nextLine != "") nextElem = nextLine[0];
                else nextElem = ' ';


                if (line != "" && first != '*') //then just skip  
                {
                    switch (first)
                    {
                        case 'C':
                        case 'R':
                        case 'L':
                        case 'V':
                            if (currentElem != 'S') currentElem = first;
                            ReadPorts(2, line);
                            break;
                        case 'T':
                            if (currentElem != 'S') currentElem = first;
                            ReadPorts(4, line);
                            break;
                        case 'W':
                        case 'X':

                            if (currentElem != 'S') currentElem = first;
                            ReadPorts(0, line); //Считываем порты 
                            break;
                        case 'Y':
                            currentElem = first;
                            i = 1;
                            currentEnt = ReadPart(line, ref i);
                            try { Y.Add(currentEnt, false); }//Добавили только имя в список
                            catch { };
                            break;
                        case '+': //Точно только здесь +!!!
                            if (currentElem == 'W') Elements[currentEnt].Info.Add(line);
                            else if (currentElem == 'X')
                            {
                                ReadPlus(line); ///!!!2
                                Elements[currentEnt].Info.Add(line);
                            }
                            else if (currentElem == 'Y')
                            {
                                if (line.Contains("sdata=")) Y[currentEnt] = true; //Типо ресивер значит
                            }
                            break;
                        case '.':
                            if (line[1] == 's')
                            {
                                currentElem = 'S';
                            }
                            else if (line[1] == 'e') currentElem = first;
                            break;
                        default:
                            break;
                    }
                }

                line = nextLine;
                pb.PerformStep();
            }
            fs.Close();
        }

        public void ReadPorts(int n, string file)
        {
            if (currentElem == 'S' || currentElem == '.') return; //Ничего не считвыаем если это subckt или параметры модели

            i = 0;
            portsRead = 0;
            PortNames = new List<string>();

            entName = ReadPart(file, ref i);
            currentEnt = entName;
            if (currentElem == 'W') portName = ReadPart(file, ref i); //Считаем лишний кусок где N=..!!!

            while (i < file.Length - 1 && ((portsRead < n) || (n == 0))) //-1?  //Getting all ports from spice element
            {
                portName = ReadPart(file, ref i);   //ToDo: Zero Port!!!
                PortNames.Add(portName);
                portsRead++;
            }

            if (currentElem == 'X' && nextElem != '+') PortNames.RemoveAt(PortNames.Count - 1); //Удалить последний если это конец элемента X

            foreach (string port in PortNames)
            {
                if (!Ports.ContainsKey(port)) Ports.Add(port, new Port(port, entName)); //Новый порт
                else Ports[port].Ent.Add(entName); //Существующий порт
            }

            if (!Elements.ContainsKey(entName)) //Создаем новую сущность
            {
                E = new Entity();
                E.Name = entName;
                E.Ports = new List<string>();
                foreach (string port in PortNames) E.Ports.Add(port);
                E.Info = new List<string>();
                E.Info.Add(file); //первая строка сущности
                Elements.Add(E.Name, E); //Следующие строки
            }
            else
            {
                foreach (string port in PortNames) Elements[entName].Ports.Add(port); //Добавляем порты к существующей сущности     
            }
        }

        public void ReadPlus(string file)
        {
            i = 1; //+ не считываем
            PortNames = new List<string>();

            while (i < file.Length - 2) //-1?  !!!//Getting all ports from spice element
            {
                portName = ReadPart(file, ref i);   //ToDo: Zero Port!!!
                PortNames.Add(portName);
            }

            if (nextElem != '+') PortNames.RemoveAt(PortNames.Count - 1); //Удалить последний если это конец элемента X

            foreach (string port in PortNames)
            {
                if (!Ports.ContainsKey(port)) Ports.Add(port, new Port(port, entName));
                else Ports[port].Ent.Add(entName); //add Entity to Port!!!
            }

            foreach (string port in PortNames) Elements[entName].Ports.Add(port); //Add some ports
        }

        public static string ReadPart(string file, ref int i) //Чтение одного поля до " "
        {
            if (i != 0) i++;

            string s = "";

            for (; (i < file.Length) && (file[i] != ' '); i++) //<=!!!
            {
                s += file[i];
            }

            return s;
        }


        //******************************************************************Symbol Creating*****************************************************
        public void FillSymbol(Symbol Rt)
        {
            for (int i = 0; i < E.Ports.Count; i++)
            {
                if (!PortsS.ContainsKey(E.Ports[i])) //Такого порта еще нет - создаем
                {
                    //Создаем объекты
                    P = new PortS(E.Ports[i]);
                    P.Sym.Add(Rt);
                    Rt.Ports.Add(P);

                    //Добавляем в структуры
                    PortsS.Add(E.Ports[i], P);
                }

                else //Такой порт уже есть - добавляем
                {
                    Rt.Ports.Add(PortsS[E.Ports[i]]);
                    PortsS[E.Ports[i]].Sym.Add(Rt);
                }
            }
            Symbols.Add(Rt);
        }

        public int CountAnalog(Entity E)
        {
            int ans = 0;
            for (int i = 0; i < E.Ports.Count; i++)
            {
                if (!E.Ports[i].Contains("d_control") && !E.Ports[i].Contains("d_receive"))
                {
                    ans++;
                }
            }
            return ans;
        }

        public void CreateSymbols()
        {
            Symbols = new List<Symbol>();
            PortsS = new Dictionary<string, PortS>();
            PortNames = new List<string>();
            ListW = new List<W>();

            foreach (string s in Elements.Keys)
            {
                char f = s[0];
                E = Elements[s]; //С памятью ничего не будет?
                switch (f)
                {
                    case 'R':
                        S = new R(E);
                        FillSymbol(S);
                        break;
                    case 'C':
                        S = new C(E);
                        FillSymbol(S);
                        break;
                    case 'L':
                        S = new L(E);
                        FillSymbol(S);
                        break;
                    case 'T':
                        S = new T(E);
                        FillSymbol(S);
                        break;
                    case 'X':
                        bool haveY = false;
                        if (s.Contains("X_U") && CountAnalog(E)==1)
                        {
                            foreach (string key in Y.Keys) //Неэффективно, просто перебор. Но там косяк с названиями у Y. Добавляют постфикс в конце типа _1
                            {
                                if (key.Contains(E.Ports[1]) && E.Ports.Count == 3)
                                {
                                    S = new X_U(E, Y[key]); //Выделили новый подкласс иксов.
                                    haveY = true;
                                }
                            }

                            if (!haveY) S = new X_Box(E);
                        }
                        else if (s.Contains("X_VIA")) S = new X_VIA(E);
                        else
                        {
                            S = new X_Box(E); //Типо на крайний случай. Здесь почти та же вия сейчас. Только пишет BB
                        }

                        FillSymbol(S);
                        break;
                    case 'V':
                        S = new V(E);
                        FillSymbol(S);
                        break;
                    case 'W':////////////////////////////////////////////!!!Структура W-элементов - сделать!
                        //Разделение на сущности + структура хранения!
                        W elemW = new W();
                        for (int i = 0; i < (E.Ports.Count - 2) / 2; i++)
                        {
                            S = new K(E);
                            for (int j = 0; j < E.Ports.Count; j += ((E.Ports.Count) / 2))
                            {
                                if (!PortsS.ContainsKey(E.Ports[i + j])) //Такого порта еще нет - создаем
                                {
                                    //Создаем объекты
                                    P = new PortS(E.Ports[i + j]);
                                    P.Sym.Add(S);
                                    S.Ports.Add(P);

                                    //Добавляем в структуры
                                    PortsS.Add(E.Ports[i + j], P);
                                    //Symbols.Add(Rt);
                                }

                                else //Такой порт уже есть - добавляем
                                {
                                    S.Ports.Add(PortsS[E.Ports[i + j]]);

                                    //Symbols.Add(Rt);
                                    PortsS[E.Ports[i + j]].Sym.Add(S);
                                }
                            }
                            Symbols.Add(S);
                            elemW.Parts.Add(S); //Заполняем W элемент
                        }
                        if (elemW.Parts.Count > 1) ListW.Add(elemW);
                        break;
                }
            }
        }

        public void SortPorts() //Чтобы в строчку вытянуть - сортирую
        {
            foreach (string P in PortsS.Keys) if (P != "0") PortsS[P].Sym.Sort(CompareSym);

            foreach (Symbol S in Symbols)
            {
                if (S.Src.Name[0] == 'X') S.CreatePortCopy(); //Для X создаем копию портов- баг с последовательностью
                else S.Ports.Sort(ComparePorts);
            }
        }

        private static int CompareSym(Symbol x, Symbol y) //Пользуюсь системной процедурой сортировки
        {
            int xCount = x.Ports.Count;
            foreach (PortS P in x.Ports) if (P.Name == "0") xCount--;
            int yCount = y.Ports.Count;
            foreach (PortS P in y.Ports) if (P.Name == "0") yCount--;

            int retval = xCount.CompareTo(yCount);

            if (retval != 0) return -retval;
            else return 0; //Типо любой
        }

        private static int ComparePorts(PortS x, PortS y) //Пользуюсь системной процедурой сортировки
        {
            int xCount = 0, yCount = 0;
            foreach (Symbol S in x.Sym)
            {
                xCount += S.Ports.Count;
                foreach (PortS P in S.Ports) if (P.Name == "0") xCount--;
            }

            foreach (Symbol S in y.Sym)
            {
                yCount += S.Ports.Count;
                foreach (PortS P in S.Ports) if (P.Name == "0") yCount--;
            }

            int retval = xCount.CompareTo(yCount);

            if (retval != 0) return -retval;
            else return 0; //Типо любой
        }

        public Symbol GetRoot() //Правильная послед приоритетов: драйвер-ресивер-иксбокс-хангер-любой
        {
            Symbol Dr = null, Re = null, Xb = null, Hu = null, Any = null;
            foreach (PortS P in PortsScopy.Values)
            {
                foreach (Symbol S in P.Sym)
                {
                    if (Dr == null && S.GetType() == 'D') Dr = S;
                    else if (Re == null && S.GetType() == 'E') Re = S;
                    else if (Xb == null && S.GetType() == 'B') Xb = S;
                    else if (Hu == null && P.IsHunger())// && S.Src.Name.ToLower().Contains("r_conv"))
                    {
                        Hu = S;
                    }
                    else if (Any == null && S.GetType() != 'X') Any = S;
                }
            }

            if (Dr != null) return Dr;
            if (Re != null) return Re;
            if (Xb != null) return Xb;
            if (Hu != null) return Hu;
            if (Any != null) return Any;
            return PortsScopy.Values.ElementAt(0).Sym[0];
        }

        public void PlaceSymbols()
        {
            PortsScopy = new Dictionary<string, PortS>(PortsS);
            Placed = new int[1000];
            for (int i = 0; i < 1000; i++) Placed[i] = 0;
            int currentX;
            List<Symbol> CurrentNet;
            int[] CurrentPlaced;
            while (PortsScopy.Count != 0)
            {
                CurrentNet = new List<Symbol>();
                CurrentPlaced = new int[1000];
                for (int i = 0; i < 1000; i++) CurrentPlaced[i] = 0;

                Symbol root = GetRoot();
                CurrentNet.Add(root);
                Queue Line = new Queue();
                Line.Add(root);
                root.Points.Add(new Point(0, 1));
                CurrentPlaced[0] = 1;
                root.Ready = true;

                while (Line.NotEmpty())
                {
                    S = Line.Get();
                    currentX = Convert.ToInt32(S.X()) + 1; //В каком мы столбце    

                    while (S.Y() - CurrentPlaced[currentX] > upperK && S.Y() > CurrentPlaced[currentX])
                    {
                        CurrentPlaced[currentX]++;  //Чтобы сильно не уходило вверх
                    }


                    foreach (PortS P in S.Ports)
                    {
                        try { PortsScopy.Remove(P.Name); }
                        catch { }
                        foreach (Symbol K in P.Sym)
                        {
                            if (K != S && !K.Ready && P.Name != "0") //Можно улучшить!!!
                            {
                                if (K.GetType() == 'X' || K.GetType() == 'B')
                                {
                                    K.CountBoxY();
                                    n = (K.boxY / 3) + 1;
                                }
                                else n = 1; // --Это размер сущности по вертикали в точках!!!

                                for (int i = 0; i < n; i++)
                                {
                                    K.Points.Add(new Point(currentX, CurrentPlaced[currentX] + 1));
                                    CurrentPlaced[currentX] += 1;
                                }
                                K.Ready = true; //Типо отрисовал уже.
                                K.Parent = S;
                                S.Children.Add(K);
                                Line.Add(K); //Добавить в очередь.
                                CurrentNet.Add(K);
                            }
                        }
                    }

                    while (CurrentPlaced[currentX] + S.Children.Count - S.Y() > lowerK && CurrentPlaced[currentX] > S.Y() && S.Children.Count != 0)
                    {
                        foreach (Symbol Sy in Symbols) //То, что снизу тоже сдвинем
                        {
                            if (Sy.Ready && Sy.X() == S.X() && Sy.Y() > S.Y())
                            {
                                Sy.MoveY(1);  //Чтобы сильно не уходило вниз
                            }
                        }

                        CurrentPlaced[currentX - 1]++;
                        S.MoveY(1);  //Чтобы сильно не уходило вниз
                    }
                }
                //Располагаем нет относительно прошлых
                int j, max = -1;
                for (i = 0; i < 1000; i++) if (CurrentPlaced[i] == 0) break; //Нашли длину нета
                for (j = 0; j <= i; j++) if (Placed[j] > max) max = Placed[j]; //Нашли макс высоту до этой длины
                foreach (Symbol Sy in CurrentNet) Sy.MoveY(max + 2); //Сдвинули нет
                for (j = 0; j < i; j++) Placed[j] = CurrentPlaced[j] + max + 2; //Отредактировали полную заполненность
                root.Ready = true;
            }
        }

        public void FillPortPoints()
        {
            UpperSymbols = new List<Symbol>();
            Cursor.Current = Cursors.WaitCursor;


            foreach (Symbol S in Symbols) //Здесь рисуем все кроме символов со свзяью на своем уровне
            {
                foreach (PortS P in S.Ports)
                {
                    foreach (Symbol K in P.Sym)
                    {
                        if (K != S && P.Name != "0")
                        {
                            if (K.X() == S.X() && K.Parent.Src.Name != S.Parent.Src.Name) //Не рисуем пока верхние связи
                            {
                                UpperSymbols.Add(S);
                                UpperSymbols.Add(K);
                            }
                            else
                            {
                                S.FillPortPoint(K, P);
                                K.FillPortPoint(S, P);
                            }
                        }
                    }
                }
            }

            foreach (Symbol S in UpperSymbols)
            {
                foreach (PortS P in S.Ports)
                {
                    foreach (Symbol K in P.Sym)
                    {
                        if (K != S && P.Name != "0")
                        {
                            S.FillPortPoint(K, P);
                            K.FillPortPoint(S, P);
                        }
                    }
                }
            }
        }

        //******************************************************************Form Activity*****************************************************
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "openFileDialog1")
            {
                textBox1.Clear();
                textBox4.Clear();
                textBox3.Clear();
                CreateBase(openFileDialog1.FileName);

                wNum = 0;
                numericUpDown5.Value = 0;
                Form1.ActiveForm.Text = "Spice Viewer  " + openFileDialog1.FileName;
                CreateSymbols();
                SortPorts();
                PlaceSymbols();
                FillPortPoints();
                draw = true;
                Clicked = null;
                ZoomAll();
                button1.Focus();


                Invalidate();
            }
        }

        public void DrawPort(PortS P, PaintEventArgs e, Pen Pen)
        {
            if (P.Points.Count > 1)
            {
                PointF[] A = new PointF[10];
                for (int i = 0; i < A.Length; i++) A[i] = new PointF();
                for (int i = 0; i < A.Length; i++) { }
                A = new PointF[P.Points.Count];
                for (int i = 0; i < P.Points.Count; i++)
                {
                    d = (float)kx / 10; //для палочки
                    A[i] = new PointF();

                    A[i].X = kx * (P.Points[i].X - xStart);
                    A[i].Y = ky * (P.Points[i].Y - yStart);

                    if (Math.Abs((int)(P.Points[i].Y) - P.Points[i].Y) <= 1.0 / 100.0)
                    {
                        e.Graphics.DrawLine(Pen, A[i].X, A[i].Y, A[i].X, A[i].Y - d);
                        e.Graphics.FillEllipse(Setting.Global.Brush, new RectangleF(A[i].X - Math.Abs(d / 4), A[i].Y - Math.Abs(5 * d / 4), Math.Abs(d / 2), Math.Abs(d / 2))); //Рисуем кружок и линию до элемента
                        A[i].Y -= d;
                    }
                    else
                    {
                        float a = (int)(P.Points[i].X) - P.Points[i].X;
                        if (a < 1.0 / 100.0 && a >= 0.0)
                        {
                            d = -d;
                        }

                        e.Graphics.DrawLine(Pen, A[i].X, A[i].Y, A[i].X + d, A[i].Y);
                        float dx = 0;
                        if (d < 0) dx = d / 2; //Доп сдвиг налево! Из-за свойств отрисовки эллипса
                        e.Graphics.FillEllipse(Setting.Global.Brush, new RectangleF(A[i].X + 3 * d / 4 + dx, A[i].Y - Math.Abs(d / 4), Math.Abs(d / 2), Math.Abs(d / 2))); //Рисуем кружок и линию до элемента
                        A[i].X += d;
                    }

                }

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A.Length; j++)
                    {
                        if (Math.Abs(P.Points[i].X - P.Points[j].X) > 3.0 / 10.0 || Math.Abs(P.Points[i].Y - P.Points[j].Y) < 4.0 / 10.0) e.Graphics.DrawLine(Pen, A[i], A[j]);
                    }
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            if (draw)
            {
                foreach (Symbol S in Symbols) //Здесь рисуем все кроме символов со свзяью на своем уровне
                {
                    S.DrawMe(e.Graphics, kx, ky, kx * (S.X() - xStart), ky * (S.Y() - yStart));
                }

                foreach (string P in PortsS.Keys)
                {
                    DrawPort(PortsS[P], e, Setting.Global.Pen1);
                }


                if (wDraw) //W - элементы стрелками
                {
                    foreach (W elem in ListW)
                    {
                        for (int i = 0; i < elem.Parts.Count - 1; i++)
                        {
                            e.Graphics.DrawLine(Setting.Global.Pen2, kx * (elem.Parts[i].X() - xStart), ky * (elem.Parts[i].Y() - yStart), kx * (elem.Parts[i + 1].X() - xStart), ky * (elem.Parts[i + 1].Y() - yStart));
                        }
                    }
                }

                if (ListW.Count != 0) //W - элементы по номеру
                {
                    foreach (Symbol S in ListW[wNum].Parts)
                    {
                        S.DrawMe(e.Graphics, kx, ky, kx * (S.X() - xStart), ky * (S.Y() - yStart), Color.Red);
                    }
                }

                foreach (Symbol S in Found) //поиск
                {
                    S.DrawMe(e.Graphics, kx, ky, kx * (S.X() - xStart), ky * (S.Y() - yStart), Color.Green);
                }


                if (Clicked != null) //Подсветка
                {
                    S = Clicked;
                    S.DrawMe(e.Graphics, kx, ky, kx * (S.X() - xStart), ky * (S.Y() - yStart), Color.Black);
                    foreach (PortS P in S.Ports)
                    {
                        DrawPort(P, e, Setting.Global.Pen3);
                    }
                }

                draw = true;
                Cursor.Current = Cursors.Default;
            }


        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (move && draw)
            {
                haveMoved = true;

                if (mouseX > 0)
                {
                    xStart += ((float)(mouseX - e.X) / (float)kx);
                    yStart += ((float)(mouseY - e.Y) / (float)ky);
                }
                mouseX = e.X;
                mouseY = e.Y;
                Invalidate();
            }
        }

        public void this_MouseWheel(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                button1.Focus();
                //В старом масштабе координаты в сетке
                float netX = (float)e.X / (float)kx + xStart;
                float netY = (float)e.Y / (float)ky + yStart;

                if (ky + Convert.ToInt32(e.Delta / 20) > 2)
                {
                    kx += Convert.ToInt32(e.Delta / 20);
                    ky = Convert.ToInt32(kx * 55 / 65);
                }

                //Пересчитываю в новом масштабе
                xStart = -(float)e.X / (float)kx + netX;
                yStart = -(float)e.Y / (float)ky + netY;
                Invalidate();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
            mouseX = -1;
            mouseY = -1;

            button1.Focus();
            if (draw && !haveMoved)
            {
                foreach (string P in PortsS.Keys)
                {
                    if (PortsS[P].InBox(e.X, e.Y, kx, ky, xStart, yStart))
                    {
                        Clicked = null;
                        textBox4.Text = "";
                        textBox1.Text = "Port names: " + P + Environment.NewLine;

                        for (int i = 0; i < PortsS[P].Sym.Count; i++)
                        {
                            textBox1.Text += PortsS[P].Sym[i].Src.Name + Environment.NewLine;
                        }
                    }
                }

                foreach (Symbol Sy in Symbols)
                {
                    if (Sy.Ready && Sy.InBox(e.X, e.Y, kx, ky, xStart, yStart))
                    {
                        textBox1.Text = "";
                        for (int i = 0; i < Sy.Src.Info.Count; i++)
                        {
                            textBox1.Text += Sy.Src.Info[i] + Environment.NewLine;
                        }
                        textBox1.Text += "!Ports = " + Sy.Ports.Count + Environment.NewLine;
                        //textBox1.Text += "!Points = " + Sy.Points.Count + Environment.NewLine;
                        //textBox1.Text += "!Type = " + Sy.GetType()+ Environment.NewLine;

                        textBox4.Text = "Port Name:" + Environment.NewLine;

                        if (Sy.GetType() != 'D' && Sy.GetType() != 'E') //Для них только 1 порт надо подписать
                        {
                            bool hadZero = false;
                            foreach (PortS P in Sy.Ports)
                            {
                                if (!(hadZero && P.Name == "0")) textBox4.Text += P.Name + Environment.NewLine;
                                if (P.Name == "0") hadZero = true;
                            }
                        }
                        else
                        {
                            textBox4.Text += Sy.Ports[0].Name + Environment.NewLine;
                        }

                        Clicked = Sy;
                        break;
                    }
                }


                Invalidate();
            }
            haveMoved = false;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (draw)
            {
                wNum = Convert.ToInt32(numericUpDown5.Value);
                if (wNum >= ListW.Count && numericUpDown5.Value > 0)
                {
                    numericUpDown5.Value--;
                    wNum--;
                }
                Invalidate();
            }
        }

        public void ZoomAll()
        {
            if (draw)
            {
                int maxPlaced = 0, i;
                for (i = 0; i < 1000; i++)
                {
                    if (Placed[i] > maxPlaced) maxPlaced = Placed[i];
                    if (Placed[i] == 0) break;
                }
                ky = (this.Height - panel1.Size.Height) / maxPlaced;
                kx = Convert.ToInt32(ky * 65 / 55);

                if (kx > this.Width / i)
                {
                    kx = this.Width / i;
                    ky = Convert.ToInt32(kx * 55 / 65);
                }

                float wid = (float)this.Width / (float)kx;
                float hei = (float)(this.Height - panel1.Size.Height) / (float)ky;

                xStart = -(wid - (float)i) / 2;
                yStart = -(hei - (float)maxPlaced) * 1 / 2 - (float)panel1.Size.Height / (float)ky / 2;
                Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ZoomAll();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (draw)
            {
                Found = new List<Symbol>();
                if (textBox3.Text.Length > 2) //Имеет смысл искать
                {
                    //А можно ещё по именам портов искать же!
                    Found = new List<Symbol>();
                    foreach (Symbol S in Symbols)
                    {
                        if (S.Src.Name.ToLower().Contains(textBox3.Text.ToLower())) Found.Add(S);
                    }
                }
                textBox2.Text = Found.Count.ToString();
                Invalidate();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            wDraw = checkBox1.Checked;
            Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (draw) Process.Start(openFileDialog1.FileName);
        }

    }
}
                
