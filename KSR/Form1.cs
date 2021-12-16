using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace KSR
{
    public partial class Form1 : Form
    {
        List<Us> groups = new List<Us>();
        List<Tram> trams = new List<Tram>();
        Bitmap newImage = new Bitmap("tr1.png", true);
        Bitmap trImage = new Bitmap("tr2.png", true);
        String[] numbers = {"1", "5", "15", "20", "22", "1", "5", "15", "20", "22" };
        List<int> rndMyAss = new List<int>();
       
        
        //GMarkerGoogle mapM;
        bool ac = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
           
            AddInTram();
            mapInitialize();


            Timer timer = new Timer();
            timer.Interval = (10); // 10 millsecs
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            foreach (var t in trams)
            {
                t.OldPosition = t.Position;
                t.setPosition(AlongLine(t));
                //Console.WriteLine(t.Position);
            }
            gMapControl1.Update();
        }
        public GMap.NET.PointLatLng AlongLine(Tram t)
        {
            GMap.NET.PointLatLng V2 = Lerp(new GMap.NET.PointLatLng(t.Position.Lat, t.Position.Lng), new GMap.NET.PointLatLng(t.Cords[t.Ost].Lat, t.Cords[t.Ost].Lng), t.h);
            if(t.h > 1)
                t.h--;
            //Vector2 V2 = Vector2.Lerp(new Vector2((float)t.Position.Lat, (float)t.Position.Lng), new Vector2((float)t.Cords[t.Ost].Lat, (float)t.Cords[t.Ost].Lng), 0.001f);
            GMap.NET.PointLatLng pos = new GMap.NET.PointLatLng(V2.Lat, V2.Lng);
            if (pos == t.OldPosition && t.Ost < t.Cords.Count - 1)
            {
                t.h = 1000;
                t.Ost++;
            }
            //Console.WriteLine(t.Ost + " - " + t.Cords.Count);
            if (t.Ost >= t.Cords.Count - 1)
            {
                //Console.WriteLine("андерсенд мистер андлерсон");
                t.Cords.Reverse();
                t.Ost = 0;
            }
            //Console.WriteLine(t.Ost);
            return pos;
        }
        private GMarkerGoogle GetMarker(Us us, Bitmap bitmap)
        {
            GMarkerGoogle mapMarker = new GMarkerGoogle(new GMap.NET.PointLatLng(us.X, us.Y), bitmap);//широта, долгота, тип маркера
            mapMarker.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(mapMarker);//всплывающее окно с инфой к маркеру
            mapMarker.ToolTipText = us.Id; // текст внутри всплывающего окна
            mapMarker.Tag = us.Id;
            mapMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver; //отображение всплывающего окна (при наведении)
            return mapMarker;
        }
        private GMarkerGoogle GetMarker(Tram tram, Bitmap bitmap)
        {
            GMarkerGoogle mapMarker = new GMarkerGoogle(new GMap.NET.PointLatLng(tram.Position.Lat, tram.Position.Lng), bitmap);
            mapMarker.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(mapMarker);//всплывающее окно с инфой к маркеру
            mapMarker.ToolTipText = tram.Number; // текст внутри всплывающего окна
            mapMarker.Tag = tram.Number;
            mapMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver; //отображение всплывающего окна (при наведении)
            return mapMarker;
        }
        private GMapOverlay GetOverlayMarkers(List<Us> uss, string name, Bitmap img)
        {
            GMapOverlay gMapMarkers = new GMapOverlay(name);// создание именованного слоя 
            foreach (Us us in uss)
            {
                gMapMarkers.Markers.Add(GetMarker(us, img));// добавление маркеров на слой
            }
            return gMapMarkers;
        }
        private GMapOverlay GetOverlayMarkers(List<Tram> tram, string name, Bitmap img)
        {
            GMapOverlay gMapMarkers = new GMapOverlay(name);// создание именованного слоя 
            foreach (Tram tr in tram)
            {
                tr.MapM = GetMarker(tr, img);
                gMapMarkers.Markers.Add(tr.MapM);// добавление маркеров на слой
                
            }
            return gMapMarkers;
        }

        private void AddInGroupe()
        {
            groups.Add(new Us("Ул. Полевая", 53.205702598719064, 50.121883307663246, new int[] {88, 82, 88, 88, 87, 88, 88, 86, 86, 87, 88, 86, 86, 87, 87, 86, 85, 86, 87, 85 }, "Средний уровень звукового воздействия 86,6 дБА \nЭквивалентный уровень звука потока трамваев 96,6 дБА"));
            groups.Add(new Us("Дворец спорта и Цирк", 53.20337309312067, 50.11611238415037, new int[] {89, 81, 82, 83, 86, 86, 87, 88, 88, 89, 90, 89, 91, 87, 88}, "Средний уровень звукового воздействия 86,9 дБА \nЭквивалентный уровень звука потока трамваев 95,7 дБА"));
            groups.Add(new Us("Дворец спорта и Цирк", 53.20275645015193, 50.11531917203825, new int[] { 85, 86, 87, 87, 85, 86, 87, 88, 89, 89, 90, 90, 89, 89, 88 }, "Средний уровень звукового воздействия 87,5 дБА \nЭквивалентный уровень звука потока трамваев 96,2 дБА"));
            groups.Add(new Us("пл. Самарская", 53.201213279711354, 50.111784874977936, new int[] {82,83,83,85,79,77,86, 86,89, 85,87,86,79,80,84,86,17, 82, 18, 86 }, "Средний уровень звукового воздействия 83,3 дБА \nЭквивалентный уровень звука потока трамваев 92,9 дБА"));
            groups.Add(new Us("пл. Самарская", 53.201306901594606, 50.11133193726394, new int[] { 91, 90, 89, 88, 90, 85, 87, 86, 88, 84, 87, 90, 89, 89, 88, 86, 85, 86 }, "Средний уровень звукового воздействия 87,7 дБА \nЭквивалентный уровень звука потока трамваев 94,2 дБА"));
            groups.Add(new Us("ул. Вилоновская", 53.19752869187051, 50.10581930609551, new int[] { 91, 94, 89, 89, 86, 91, 90, 88, 88, 89, 90, 87, 88, 89, 89, 93, 90, 91, 92, 89 }, "Средний уровень звукового воздействия 89,7 дБА \nЭквивалентный уровень звука потока трамваев 99,7 дБА"));
            groups.Add(new Us("ул. Вилоновская", 53.19671778898337, 50.10555271505243, new int[] { 89,89,88,90,91,90,87,91,90,89,88,87,88,90,91,90,92,89,89,86}, "Средний уровень звукового воздействия 83,2 дБА \nЭквивалентный уровень звука потока трамваев 99,2 дБА"));
            groups.Add(new Us("ул. Красноармейская", 53.19381248257152, 50.10363665184697, new int[] { 87, 86, 85, 84, 87, 89, 86, 87, 90, 83, 84, 87 }, "Средний уровень звукового воздействия 86,3 дБА \nЭквивалентный уровень звука потока трамваев 96,6 дБА"));
            groups.Add(new Us("ул. Красноармейская", 53.1933602419262, 50.10299180510309, new int[] { 90, 91, 89, 90, 86, 87, 89, 90, 91, 89, 89, 86 }, "Средний уровень звукового воздействия 86,6 дБА \nЭквивалентный уровень звука потока трамваев 94 дБА"));
            groups.Add(new Us("ул. Льва Толстого", 53.189967915362715, 50.101023929301334, new int[] {92,90,89,90,87,89,86,86,92,90 }, "Средний уровень звукового воздействия 88,9 дБА \nЭквивалентный уровень звука потока трамваев 96,7 дБА"));
            groups.Add(new Us("ул. Льва Толстого", 53.19152198559205, 50.10178627471761, new int[] { 90, 92, 90, 89, 90, 87, 89, 86, 86, 92, }, "Средний уровень звукового воздействия 89,1 дБА \nЭквивалентный уровень звука потока трамваев 96,1 дБА"));
            groups.Add(new Us("ул. Некрасовская", 53.18889612206298, 50.09995824390895, new int[] { 92, 90, 91, 91, 87, 90, 93, 89, 91, 91 }, "Средний уровень звукового воздействия 90,5 дБА \nЭквивалентный уровень звука потока трамваев 97,5 дБА"));
            groups.Add(new Us("ул. Высоцкого", 53.18651755039875, 50.09867714272633, new int[] { 87, 86, 85, 79, 87, 86, 80, 81, 84, 83, 87 }, "Средний уровень звукового воздействия 84,1 дБА \nЭквивалентный уровень звука потока трамваев 91,5 дБА"));
            groups.Add(new Us("ул. Высоцкого", 53.18623381136338, 50.098145889866984, new int[] { 84, 85, 86, 89, 87, 87, 83, 79, 80, 85, 84 }, "Средний уровень звукового воздействия 84,5 дБА \nЭквивалентный уровень звука потока трамваев 91,9 дБА"));
            groups.Add(new Us("Троицкий рынок", 53.184167868112034, 50.09669913036698, new int[] { 91, 90, 89, 88, 89, 90, 91, 91, 92 }, "Средний уровень звукового воздействия 90,1 дБА \nЭквивалентный уровень звука потока трамваев 96,6 дБА"));
            groups.Add(new Us("Троицкий рынок", 53.1843922285932, 50.09725848440023, new int[] { 90, 89, 89, 86, 91, 93, 90, 89, 91 }, "Средний уровень звукового воздействия 89,8 дБА \nЭквивалентный уровень звука потока трамваев 96,3 дБА"));

        }
        private void AddInTram()
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                using (FileStream fstream = File.OpenRead($"name{i}.txt"))
                {
                    List<GMap.NET.PointLatLng> cords = new List<GMap.NET.PointLatLng>();
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array, 0, array.Length);
                    string textFromFile = Encoding.Default.GetString(array);
                    textFromFile = textFromFile.Replace('.', ',');
                    textFromFile = Regex.Replace(textFromFile, @"[ \r\n\t]", "");
                    string[] st = textFromFile.Split('/');
                    
                    for(int j = 0; j < st.Length-1; j=j+2)
                    {
                        double d1 = Convert.ToDouble(st[j]);
                        double d2 = Convert.ToDouble(st[j+1]);
                        cords.Add(new GMap.NET.PointLatLng(d1, d2));
                        Console.WriteLine($"Текст из файла {i}: {d1} and {d2}");
                    }
                    Random r = new Random();
                    int rnd;
                    do {
                        rnd = r.Next(cords.Count);
                    } while (rndMyAss.Contains(rnd));
                    rndMyAss.Add(rnd);
                    Console.WriteLine(rnd);
                    if (r.Next(2) == 0)
                        cords.Reverse();
                    trams.Add(new Tram($"{i}", numbers[i], new GMap.NET.PointLatLng(cords[rnd].Lat, cords[rnd].Lng), rnd, cords));
                }
            }


        }
        public GMap.NET.PointLatLng Lerp(GMap.NET.PointLatLng a, GMap.NET.PointLatLng b, double h)
        {
            double xI = a.Lat - b.Lat;
            double yI = a.Lng - b.Lng;
            GMap.NET.PointLatLng i = new GMap.NET.PointLatLng(xI, yI);
            double rast = Math.Sqrt(Math.Pow(b.Lat - a.Lat, 2) + (Math.Pow(b.Lng - a.Lng, 2)));
            double x = i.Lat / h;
            double y = i.Lng / h;
            GMap.NET.PointLatLng newPoint = new GMap.NET.PointLatLng(x, y);
            //double rast = Math.Sqrt(Math.Pow(b.Lat-a.Lat,2)+ (Math.Pow(b.Lng - a.Lng, 2)));
            //int g = (int)(rast / h);
            double newX = a.Lat - newPoint.Lat;
            double newY = a.Lng - newPoint.Lng;
            return new GMap.NET.PointLatLng(newX, newY);
        }
        /*public GMap.NET.PointLatLng Lerp(GMap.NET.PointLatLng a, GMap.NET.PointLatLng b, double h)
        {
            double xI = a.Lat - b.Lat;
            double yI = a.Lng - b.Lng;
            GMap.NET.PointLatLng i = new GMap.NET.PointLatLng(xI, yI);
            double x = i.Lat / h;
            double y = i.Lng / h;
            GMap.NET.PointLatLng newPoint = new GMap.NET.PointLatLng(x, y);
            //double rast = Math.Sqrt(Math.Pow(b.Lat-a.Lat,2)+ (Math.Pow(b.Lng - a.Lng, 2)));
            //int g = (int)(rast / h);
            double newX = a.Lat - newPoint.Lat;
            double newY = a.Lng - newPoint.Lng;


            return new GMap.NET.PointLatLng(newX, newY);
        }*/
        private void mapInitialize()
        {
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //выбор подгрузки карты – онлайн или из ресурсов
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance; //какой провайдер карт используется (в нашем случае гугл) 
            gMapControl1.Position = new GMap.NET.PointLatLng(53.204538062023744, 50.12463519876257);// точка в центре карты при открытии (центр России)
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает (просто в центр карты или по положению мыши)
            gMapControl1.DragButton = MouseButtons.Left; // какой кнопкой осуществляется перетаскивание
            gMapControl1.ShowCenter = false; //показывать или скрывать красный крестик в центре
            gMapControl1.ShowTileGridLines = false; //показывать или скрывать тайлы
            gMapControl1.OnMarkerClick += new MarkerClick(this.gmap_OnMarkerClick);
            AddInGroupe();
            gMapControl1.Overlays.Add(GetOverlayMarkers(groups, "GroupsMarkers", newImage));
            gMapControl1.Overlays.Add(GetOverlayMarkers(trams, "GroupsTram", trImage));
            gMapControl1.Update();
        }
        private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
            
            if (Char.IsDigit(item.Tag.ToString(),0))
            {
                label1.Text = "Трамвай "+item.Tag;
                chart1.Visible = false;
                label2.Visible = false;
                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
            }
            else
            {
                chart1.Visible = true;
                label2.Visible = true;
                pictureBox1.Visible = true;
                pictureBox2.Visible = false;

                for (int i = 0; i < groups.Count; i++)
                {
                    if(item.Tag.ToString().Equals(groups[i].Id))
                    {
                        int[] noise = groups[i].Noise;
                        label1.Text = groups[i].Id;
                        label2.Text = groups[i].Info;
                        pictureBox1.Image = new Bitmap(groups[i].Id+".jpg");
                        if (noise != null)
                        {
                            chart1.ChartAreas[0].AxisX.Title = "Порядковый номер трамвая";
                            chart1.ChartAreas[0].AxisY.Title = "Средний уровень звукового воздействия, дБА";
                            chart1.Series[0].Points.DataBindY(noise);
                        }
                    }
                }
            }
            
            Console.WriteLine(String.Format("Marker {0} was clicked.", item.Tag));

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            chart1.Visible = false;
            label2.Visible = false;
            pictureBox1.Visible = false;

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            label1.Text = "Информация";
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox1.Image = new Bitmap("табл.jpg");
            pictureBox2.Image = new Bitmap("граф.jpg");
            panel2.Visible = false;
            panel1.Visible = true;
        }
    }
}
