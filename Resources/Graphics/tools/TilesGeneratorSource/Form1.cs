using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static int tileSize=16;

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerReportsProgress = true;
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Image = tilesBitmap;
            pictureBox2.Image = testbit;
            textBox1.Text = ausgabe;
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = (e.ProgressPercentage);
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateTitled(bit);
        }

        Bitmap bit, tilesBitmap, testbit;

        String ausgabe;

        public void CreateTitled(Bitmap bitmap2Perform)
        {
            //HashSet<Tiles> x = new HashSet<Tiles>();
            List<Tiles> list = new List<Tiles>();
            int[,] map = new int[bitmap2Perform.Width / tileSize, bitmap2Perform.Height / tileSize];
            int progMax = map.Length * 2;
            int prog = 0;
            int lastpercentage = 0;
            list.Add(new Tiles(new Bitmap(tileSize, tileSize)));
            for (int x = 0; x < bitmap2Perform.Width; x += tileSize)
            {
                for (int y = 0; y < bitmap2Perform.Height; y += tileSize)
                {
                    Tiles ti = new Tiles(bitmap2Perform, new Point(x, y));
                    int index = list.IndexOf(ti);
                    if (index == -1)
                    {
                        index = list.Count;
                        list.Add(ti);
                    }
                    map[x / tileSize, y / tileSize] = index;
                    prog++;
                }
                if (lastpercentage != 100 * prog / progMax)
                {
                    lastpercentage = 100 * prog / progMax;
                    backgroundWorker1.ReportProgress(lastpercentage);
                }
            }

            testbit = new Bitmap(map.GetLength(0) * tileSize, map.GetLength(1) * tileSize);
            Graphics g1 = Graphics.FromImage(testbit);
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    g1.DrawImage(list[map[x, y]], new Point(x * tileSize, y * tileSize));
                }

            }

            int collumCount = (int)Math.Ceiling(Math.Sqrt(list.Count - 1));
            int rowCount = (list.Count) / collumCount+1;

            tilesBitmap = new Bitmap(collumCount * tileSize, rowCount * tileSize);
            Graphics g = Graphics.FromImage(tilesBitmap);
            int row = 0;
            int collum = 0;
            for (int i = 1; i < list.Count; i++)
            {
                g.DrawImage(list[i], new Point(collum * tileSize, row * tileSize));
                collum++;
                if (collum >= collumCount)
                {
                    collum = 0;
                    row++;
                }
                prog++;
                if (lastpercentage != 100 * prog / progMax)
                {
                    lastpercentage = Math.Min(100, 100 * prog / progMax);
                    backgroundWorker1.ReportProgress(lastpercentage);
                }
            }



            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            sb2.AppendLine((list.Count - 1) + " Tiles");
            sb2.Append("value=\"V" + map.GetLength(1) + "," + map.GetLength(0));
            sb1.AppendLine("Widt = " + map.GetLength(0) + "Height = " + map.GetLength(1));
            sb1.Append("int[][] tiles = {");


            for (int y = 0; y < map.GetLength(1); y++)
            {
                sb1.Append((y == 0 ? "" : ",") + System.Environment.NewLine + "{");
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    sb1.Append((x == 0 ? "" : ", ") + map[x, y]);
                    sb2.Append("," + map[x, y]);
                }
                sb1.Append("}");


            }
            sb1.Append("};");
            sb2.Append("\"");

            sb1.Append(System.Environment.NewLine + System.Environment.NewLine);
            sb1.Append(sb2);

            ausgabe = sb2.ToString();

            backgroundWorker1.ReportProgress(100);

            //x.Add(new Tiles(new Bitmap(tileSize, tileSize)));
            //bool b = x.Contains(new Tiles(new Bitmap(tileSize, tileSize)));
            // Console.WriteLine(b);
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            Stream stream = openFileDialog1.OpenFile();
            bit = new Bitmap(stream);
            stream.Close();
            pictureBox1.Image = bit;
        }




        private class Tiles
        {
            Bitmap image;

            public Bitmap Image
            {
                get { return image; }
            }

            public Tiles(Image image)
            {
                if (image.Width != tileSize || image.Height != tileSize)
                    throw new ArgumentException("Tiles müssen 8x8 sein");
                this.image = new Bitmap(image);
            }

            public Tiles(Image image, Point source)
            {
                this.image = new Bitmap(tileSize, tileSize);
                Graphics g = Graphics.FromImage(this.image);
                g.DrawImage(image, new Rectangle(0, 0, tileSize, tileSize), new Rectangle(source.X, source.Y, tileSize, tileSize), GraphicsUnit.Pixel);
            }


            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Tiles))
                    return false;
                Tiles tile = (Tiles)obj;
                for (int x = 0; x < tileSize; x++)
                {
                    for (int y = 0; y < tileSize; y++)
                    {
                        if (image.GetPixel(x, y) != tile.image.GetPixel(x, y))
                            return false;
                    }
                }
                return true;
            }

            public override int GetHashCode()
            {
                int prime = 31;
                int result = 1;
                for (int x = 0; x < tileSize; x++)
                {
                    for (int y = 0; y < tileSize; y++)
                    {
                        result = prime * result + image.GetPixel(x, y).GetHashCode();
                    }
                }


                return result;
            }

            public static implicit operator Bitmap(Tiles t)
            {
                return t.Image;
            }


        }

        private void performButton_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            Stream stream = saveFileDialog1.OpenFile();
            pictureBox1.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Close();

            TextWriter streamWriter = new StreamWriter(saveFileDialog1.FileName + ".txt");

            streamWriter.Write(textBox1.Text);
            streamWriter.Flush();
            streamWriter.Close();
        }

    }
}
