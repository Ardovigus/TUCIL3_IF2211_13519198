using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace A_Star
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) // --> Event Button Browse diklik
        {
            // Browse Dialog, Inspirasi dari: https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-C-Sharp/
            OpenFileDialog openFileDialographContainer = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialographContainer.ShowDialog() == DialogResult.OK)    // --> Ketika Button OK pada Browse diklik
            {
                // ### Section Clear, untuk Membuka File Baru
                richTextBox1.Clear();

                comboBox1.Text = "";
                comboBox1.Items.Clear();
                comboBox2.Text = "";
                comboBox2.Items.Clear();

                textBox2.Text = "";

                pictureBox1.Image = null;
                // ###

                textBox1.Text = openFileDialographContainer.FileName;

                string[] lines = File.ReadAllLines(openFileDialographContainer.FileName);

                int counter = 0;

                foreach (string line in lines)
                {
                    string[] temp = line.Split('\t');

                    if (counter == 0)   // --> Inisialisasi Komponen Global Kontainer
                    {
                        Global.graph = new (double, double)[temp.Count() - 1, temp.Count() - 1];
                        Global.nodes = new Dictionary<int, string>();
                        Global.nodes_inv = new Dictionary<string, int>();
                        Global.graph_Dict = new graphDictionary();

                        for (int i = 1; i < temp.Count(); i++)
                        {
                            Global.nodes.Add(i - 1, temp[i]);
                            Global.nodes_inv.Add(temp[i], i - 1);
                            comboBox1.Items.Add(temp[i]);
                        }
                    }
                    
                    if (counter > 0)
                    {
                        for (int i = 1; i < temp.Count(); i++)
                        {
                            double dist_val = double.Parse(temp[i]);

                            Global.graph[counter - 1, i - 1] = (0.0, dist_val); // --> Value item1 = 0.0 sebagai default, item2 = Isi dari File Adj Matrix

                            if (dist_val != 0)
                            {
                                Global.graph_Dict.AddEdge(Global.nodes[counter - 1], Global.nodes[i - 1]);
                                Global.graph_Dict.AddEdge(Global.nodes[i - 1], Global.nodes[counter - 1]);
                            }
                        }
                    }
                    
                    counter++;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) // --> Event Node pada ComboBox1 dipilih
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";

            // Tambahkan Opsi Node ke ComboBox2 Selain Node yang Sudah Dipilih di ComboBox1
            foreach (string item in comboBox1.Items)
            {
                if (item != comboBox1.SelectedItem.ToString())
                {
                    comboBox2.Items.Add(item);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)  // --> Event Button Coords diklik
        {
            if (textBox1.Text == "")
            {
                richTextBox1.Text = "TIDAK ADA FILE YANG DIINPUTKAN, SILAKAN TEKAN BROWSE";
            }
            else
            {
                OpenFileDialog openFileDialographContainer = new OpenFileDialog
                {
                    InitialDirectory = @"C:\",
                    Title = "Browse Text Files",

                    CheckFileExists = true,
                    CheckPathExists = true,

                    DefaultExt = "txt",
                    Filter = "txt files (*.txt)|*.txt",
                    FilterIndex = 2,
                    RestoreDirectory = true,

                    ReadOnlyChecked = true,
                    ShowReadOnly = true
                };

                if (openFileDialographContainer.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = openFileDialographContainer.FileName;

                    string[] lines = File.ReadAllLines(openFileDialographContainer.FileName);

                    List<(double, double)> coord_list = new List<(double, double)>();

                    foreach (string line in lines)
                    {
                        string[] temp = line.Split(',');

                        double d1 = double.Parse(temp[1]);
                        double d2 = double.Parse(temp[2]);

                        coord_list.Add((d1, d2));
                    }

                    for (int i = 0; i < coord_list.Count; i++)
                    {
                        for (int j = i; j < coord_list.Count; j++)
                        {
                            // ### Section Algoritma Haversine, Inspirasi dari: https://gist.github.com/jammin77/033a332542aa24889452
                            double dLat = (coord_list[i].Item1 - coord_list[j].Item1) * (Math.PI / 180);
                            double dLon = (coord_list[i].Item2 - coord_list[j].Item2) * (Math.PI / 180);

                            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                                       Math.Cos((coord_list[j].Item1) * (Math.PI / 180)) * Math.Cos((coord_list[i].Item1) * (Math.PI / 180)) *
                                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

                            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

                            double d = 6371 * c;    // --> 6371 = default value untuk radius bumi dalam satuan km
                            // ###

                            Global.graph[i, j].Item1 = d;
                            Global.graph[j, i].Item1 = d;
                        }
                    }

                    // ### Section Print Graf Default
                    Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("");

                    for (int i = 0; i < Global.graph.GetLength(0); i++)
                    {
                        for (int j = i; j < Global.graph.GetLength(1); j++)
                        {
                            if (Global.graph[i, j].Item2 != 0.0)
                            {
                                graph.AddEdge(Global.nodes[i], Math.Round(Global.graph[i, j].Item1, 2).ToString(), Global.nodes[j]).Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                            }
                        }
                    }

                    foreach (string node in Global.nodes_inv.Keys)
                    {
                        graph.FindNode(node).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                        graph.FindNode(node).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Gray;
                    }

                    Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);

                    renderer.CalculateLayout();

                    int width = 267;

                    Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)));

                    renderer.Render(bitmap);

                    pictureBox1.Image = bitmap;
                    // ###
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)  // --> Event Button Submit diklik
        {
            richTextBox1.Clear();

            if (textBox1.Text == "")
            {
                richTextBox1.Text = "TIDAK ADA FILE YANG DIINPUTKAN, SILAKAN TEKAN BROWSE";
            }
            else if (textBox2.Text == "")
            {
                richTextBox1.Text = "TIDAK ADA FILE KOORDINAT YANG DIINPUTKAN, SILAKAN TEKAN COORDS";
            }
            else if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                richTextBox1.Text = "TIDAK ADA NODE YANG TERPILIH, SILAKAN PILIH NODE PADA KEDUA FIELD INTERSECTION";
            }
            else
            {
                string root = comboBox1.SelectedItem.ToString();
                string target = comboBox2.SelectedItem.ToString();

                List<(string, string)> searchOrder = new List<(string, string)>();  // --> Kontainer Tracking Edge Pencarian yang Terbentuk

                Dictionary<string, bool> visited = new Dictionary<string, bool>();  // --> Kontainer Tracking Node yang Sudah Dikunjungi

                foreach (string nodes in Global.nodes_inv.Keys)
                {
                    visited.Add(nodes, false);
                }
                
                List<(string, string)> edgeContainer = new List<(string, string)>();    // --> Kontainer Edge yang Sudah Dikunjungi / Terbentuk

                List<(string, string, double, double, double)> candidate = new List<(string, string, double, double, double)>();
                // --> Kontainer Himpunan Edge - Node yang Dapat Dikunjungi, Beserta Nilai Fungsi g(n) dan h(n)

                string cur_node = root;

                double gn = Global.graph[Global.nodes_inv[root], Global.nodes_inv[cur_node]].Item2;
                double hn = Global.graph[Global.nodes_inv[cur_node], Global.nodes_inv[target]].Item1;

                candidate.Add(("?", cur_node, gn, hn, gn + hn));    // --> Konsep Push / Enqueue Node Root

                bool target_found = false;  // --> Cek Route Terdefinisi atau Tidak

                if (checkBox1.Checked)
                {
                    richTextBox1.AppendText("Steps:\n");
                }

                while (!target_found && candidate.Count != 0)
                {
                    (string, string, double, double, double) node = candidate[0];
                    candidate.RemoveAt(0);  // --> Konsep Pop / Dequeue

                    cur_node = node.Item2;
                    visited[cur_node] = true;
                    gn = node.Item3;

                    edgeContainer.Add((node.Item1, node.Item2));

                    if (searchOrder.Count == 0) // --> Untuk Loop Pertama, Dapat Langsung Ditambahkan ke dalam List
                    {
                        searchOrder.Add((node.Item1, node.Item2));
                    }
                    else
                    {   // Untuk Loop Berikutnya Perlu Pengecekan
                        // Jika Kontinu (Edge Dapat Disambung dengan Elemen Terakhir Pencarian)
                        if (searchOrder[searchOrder.Count - 1].Item2 == node.Item1)
                        {
                            searchOrder.Add((node.Item1, node.Item2));
                        }
                        // Jika Tidak Kontinu
                        else
                        {   
                            // ### Section Buat Baru List Sequence Pencarian
                            List<(string, string)> sequence = new List<(string, string)>();
                            sequence.Add((node.Item1, node.Item2));

                            while (sequence[sequence.Count - 1].Item2 != root)
                            {
                                foreach ((string, string) edge in edgeContainer)
                                {
                                    if (edge.Item2 == sequence[sequence.Count - 1].Item1)
                                    {
                                        sequence.Add(edge);
                                    }
                                }
                            }

                            searchOrder.Clear();

                            sequence.Reverse();

                            foreach ((string, string) edge in sequence)
                            {
                                searchOrder.Add(edge);
                            }
                            // ###
                        }
                    }

                    if (cur_node.Equals(target))
                    {
                        target_found = true;    // --> Break
                    }

                    // --> Melakukan Pencarian Node Baru (yang Belum Dikunjungi) Sesuai Dictionary string -> List<string>
                    foreach (string node_adj in Global.graph_Dict.graph[cur_node])
                    {
                        if (!visited[node_adj])
                        {
                            double new_gn = Global.graph[Global.nodes_inv[cur_node], Global.nodes_inv[node_adj]].Item2 + gn;
                            hn = Global.graph[Global.nodes_inv[node_adj], Global.nodes_inv[target]].Item1;
                            candidate.Add((node.Item2, node_adj, new_gn, hn, new_gn + hn));
                        }
                    }

                    candidate.Sort((a, b) => a.Item5.CompareTo(b.Item5));
                    // --> Sort Berdasarkan gn + hn, Dengan Jumlah Terkecil Pertama
                    // --> Menyerupai Konsep Stack

                    if (checkBox1.Checked)
                    {
                        foreach ((string, string) edge in searchOrder)
                        {
                            if (edge == searchOrder[searchOrder.Count - 1])
                            {
                                richTextBox1.AppendText(edge.Item2 + "\n");
                            }
                            else
                            {
                                richTextBox1.AppendText(edge.Item2 + " → ");
                            }
                        }
                    }
                }

                if (checkBox1.Checked)
                {
                    richTextBox1.AppendText("\n");
                }

                if (!target_found)
                {
                    richTextBox1.AppendText("Route not available");
                }
                else
                {
                    richTextBox1.AppendText("Final:\n");

                    double distance = 0;

                    searchOrder.RemoveAt(0);

                    foreach ((string, string) el in searchOrder)
                    {
                        if (el == searchOrder[0])
                        {
                            richTextBox1.AppendText(el.Item1 + " → " + el.Item2);
                        }
                        else
                        {
                            richTextBox1.AppendText(" → " + el.Item2);
                        }

                        distance += Global.graph[Global.nodes_inv[el.Item1], Global.nodes_inv[el.Item2]].Item1;
                    }

                    richTextBox1.AppendText("\n\nDistance: " + Math.Round(distance, 2).ToString());

                    // ### Section Print Graf Baru
                    Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("");

                    for (int i = 0; i < Global.graph.GetLength(0); i++)
                    {
                        for (int j = i; j < Global.graph.GetLength(1); j++)
                        {
                            if (Global.graph[i, j].Item2 != 0)
                            {
                                if (searchOrder.Contains((Global.nodes[i], Global.nodes[j])))
                                {
                                    graph.AddEdge(Global.nodes[i], Math.Round(Global.graph[i, j].Item1, 2).ToString(), Global.nodes[j]).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                                }
                                else if (searchOrder.Contains((Global.nodes[j], Global.nodes[i])))
                                {
                                    graph.AddEdge(Global.nodes[j], Math.Round(Global.graph[i, j].Item1, 2).ToString(), Global.nodes[i]).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                                }
                                else
                                {
                                    graph.AddEdge(Global.nodes[i], Math.Round(Global.graph[i, j].Item1, 2).ToString(), Global.nodes[j]).Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;
                                }
                            }
                        }
                    }

                    foreach (string node in Global.nodes_inv.Keys)
                    {
                        graph.FindNode(node).Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                        graph.FindNode(node).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Gray;
                    }

                    foreach ((string, string) edge in searchOrder)
                    {
                        graph.FindNode(edge.Item1).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                        graph.FindNode(edge.Item2).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
                    }

                    Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);

                    renderer.CalculateLayout();

                    int width = 267;

                    Bitmap bitmap = new Bitmap(width, (int)(graph.Height * (width / graph.Width)));

                    renderer.Render(bitmap);

                    pictureBox1.Image = bitmap;
                    // ###
                }
            }
        }
    }

    static class Global
    {
        public static (double, double)[,] graph;    // --> Kontainer Representasi Adj Matrix dengan value (h(n), g(n))

        public static graphDictionary graph_Dict;   // --> Kontainer Representasi Graf Koneksi Antar Node

        public static Dictionary<int, string> nodes;    // --> Kontainer untuk Konversi Index -> Nama Node (digunakan pada graph)

        public static Dictionary<string, int> nodes_inv;    // --> Kontainer untuk Konversi Nama Node -> Index (digunakan pada graph)
    }

    class graphDictionary   // --> Kelas untuk menyimpan graf yang dalam bentuk dictionary (mapping string ke list of string)
    {
        public Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>();

        public void AddEdge(string v, string w)
        {
            if (graph.ContainsKey(v))
            {
                if (!graph[v].Contains(w))
                {
                    graph[v].Add(w);
                }
            }
            else
            {
                List<string> new_el = new List<string>();
                new_el.Add(w);

                graph.Add(v, new_el);
            }
        }
    }
}
