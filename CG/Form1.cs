using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows;
using System.Diagnostics;

namespace CG
{
    public partial class Form1 : Form
    {
        private List<List<int>> p = new List<List<int>>();
        private List<Tuple<int, int>> a = new List<Tuple<int, int>>();

        struct Objeto
        {
            public List<List<int>> coord;
            public List<Tuple<int, int>> arestas;

            public Objeto(List<List<int>> coord, List<Tuple<int, int>> arestas)
            {
                this.coord = coord;
                this.arestas = arestas;
            }
        }

        Objeto cubo = new Objeto(
            new List<List<int>>() { new List<int>() { 1, 2, 1, 2, 1, 2, 1, 2 }, // x
                                    new List<int>() { 1, 1, 2, 2, 1, 1, 2, 2 }, // y
                                    new List<int>() { 1, 1, 1, 1, 2, 2, 2, 2 }, // z
                                    new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1 }}, // w
            new List<Tuple<int, int>>() {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(1, 3),
                new Tuple<int, int>(3, 2),
                new Tuple<int, int>(0, 2),
                new Tuple<int, int>(0, 4),
                new Tuple<int, int>(4, 5),
                new Tuple<int, int>(4, 6),
                new Tuple<int, int>(1, 5),
                new Tuple<int, int>(7, 5),
                new Tuple<int, int>(7, 3),
                new Tuple<int, int>(2, 6),
                new Tuple<int, int>(6, 7)});

        Objeto piramide = new Objeto(
            new List<List<int>>() { new List<int>() { 1, 2, 1, 2, 2 }, // x
                                    new List<int>() { 1, 1, 1, 1, 2 }, // y
                                    new List<int>() { 1, 1, 2, 2, 2 }, // z
                                    new List<int>() { 1, 1, 1, 1, 1 }}, // w
            new List<Tuple<int, int>>() {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(2, 3),
                new Tuple<int, int>(3, 1),
                new Tuple<int, int>(0, 4),
                new Tuple<int, int>(1, 4),
                new Tuple<int, int>(2, 4),
                new Tuple<int, int>(3, 4)});

        Objeto prisma = new Objeto(
            new List<List<int>>() { new List<int>() { 1, 2, 4, 5, 3, 1, 2, 4, 5, 3 }, // x
                                    new List<int>() { 1, 1, 1, 1, 1, 4, 4, 4, 4, 4 }, // y
                                    new List<int>() { 2, 1, 1, 2, 3, 2, 1, 1, 2, 3 }, // z
                                    new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }}, // w
            new List<Tuple<int, int>>() {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(2, 3),
                new Tuple<int, int>(3, 4),
                new Tuple<int, int>(4, 0),
                new Tuple<int, int>(5, 6),
                new Tuple<int, int>(6, 7),
                new Tuple<int, int>(7, 8),
                new Tuple<int, int>(8, 9),
                new Tuple<int, int>(9, 5),
                new Tuple<int, int>(0, 5),
                new Tuple<int, int>(1, 6),
                new Tuple<int, int>(2, 7),
                new Tuple<int, int>(3, 8),
                new Tuple<int, int>(4, 9)});

        int[] VetorNormal(int[] p1, int[] p2, int[] p3)
        {
            int X1X2 = p1[0] - p2[0];
            int Y1Y2 = p1[1] - p2[1];
            int Z1Z2 = p1[2] - p2[2];

            int X1X3 = p1[0] - p3[0];
            int Y1Y3 = p1[1] - p3[1];
            int Z1Z3 = p1[2] - p3[2];
            // a2b3 − a3b2, a3b1 − a1b3, a1b2 − a2b1
            int[] n = {
                (Y1Y2 * Z1Z3) - (Z1Z2 * Y1Y3),
                (Z1Z2 * X1X3) - (X1X2 * Z1Z3),
                (X1X2 * Y1Y3) - (Y1Y2 * X1X3)
            };

            return n;
        }

        (int, int, int) CalculoD(int[] n, int[] pontoPlano, int[] pontoPerspectiva)
        {

            int d0 = pontoPlano[0] * n[0] + pontoPlano[1] * n[1] + pontoPlano[2] * n[2];
            int d1 = pontoPerspectiva[0] * n[0] + pontoPerspectiva[1] * n[1] + pontoPerspectiva[2] * n[2];

            return (d0, d1, d0 - d1);
        }

        List<List<int>> MultiMatriz(List<List<int>> matA, List<List<int>> matB)
        {
            List<List<int>> matR = new List<List<int>>();
            for (int i = 0; i < matA.Count(); i++)
            {
                matR.Add(new List<int>());
                for (int j = 0; j < matB[0].Count(); j++)
                {
                    int r = 0;
                    for (int k = 0; k < matB.Count(); k++)
                    {
                        r += matA[i][k] * matB[k][j];
                    }
                    matR[i].Add(r);
                }
            }

            return matR;
        }

        List<List<int>> MatrizPerspectiva(Objeto objeto, int[] n, int[] pontoPlano, int[] pontoPerspectiva)
        {
            (int d0, int d1, int d) = CalculoD(n, pontoPlano, pontoPerspectiva);

            //d+anx ay      anz     -ad0
            //bnx   d+bny   bnz     -bd0
            //cnx   cny     d+cnz   -cd0
            //nx    ny      nz      -d1
            List<List<int>> per = new List<List<int>>
            {
                new List<int>() { d + pontoPerspectiva[0] * n[0], pontoPerspectiva[0] * n[1], pontoPerspectiva[0] * n[2], -(pontoPerspectiva[0] * d0) },
                new List<int>() { pontoPerspectiva[1] * n[0], d + pontoPerspectiva[1] * n[1], pontoPerspectiva[1] * n[2], -(pontoPerspectiva[1] * d0) },
                new List<int>() { pontoPerspectiva[2] * n[0], pontoPerspectiva[2] * n[1], d + pontoPerspectiva[2] * n[2], -(pontoPerspectiva[2] * d0) },
                new List<int>() { n[0], n[1],  n[2], -d1 }
            };

            return MultiMatriz(per, objeto.coord);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void ProjetarButton_Click(object sender, EventArgs e)
        {
            Objeto obj;
            if(comboBox.Text == "Cubo")
            {
                obj = cubo;
            } 
            else if (comboBox.Text == "Piramide")
            {
                obj = piramide;
            }
            else if (comboBox.Text == "Prisma Pentagonal")
            {
                obj = prisma;
            }
            else
            {
                MessageBox.Show("Selecione um objeto válido");
                return;
            }

            //Ponto de Vista
            int x = (int)PVXValueBox.Value;
            int y = (int)PVYValueBox.Value;
            int z = (int)PVZValueBox.Value;
            int[] pv = { x, y, z };
            //Pontos do Plano
            //P0
            int x0 = (int)PPP0XValueBox.Value;
            int y0 = (int)PPP0YValueBox.Value;
            int z0 = (int)PPP0ZValueBox.Value;
            int[] p0 = { x0, y0, z0 };
            //P1
            int x1 = (int)PPP1XValueBox.Value;
            int y1 = (int)PPP1YValueBox.Value;
            int z1 = (int)PPP1ZValueBox.Value;
            int[] p1 = { x1, y1, z1 };
            //P2
            int x2 = (int)PPP2XValueBox.Value;
            int y2 = (int)PPP2YValueBox.Value;
            int z2 = (int)PPP2ZValueBox.Value;
            int[] p2 = { x2, y2, z2 };

            if(( x0, y0, z0 ) == ( x1, y1, z1 ) || (x0, y0, z0) == ( x2, y2, z2 ) || (x1, y1, z1) == (x2, y2, z2))
            {
                MessageBox.Show("Pontos Iguais");
            } 
            else
            {
                int[] n = VetorNormal(p0, p1, p2);

                p = MatrizPerspectiva(obj, n, p0, pv);
                a = obj.arestas;

                panel1.Invalidate();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 3);
            List<PointF> points = new List<PointF>();

            if(p.Count > 0)
            {
                // Criar pontos do objetos
                for (int i = 0; i < p[0].Count; i++)
                {
                    if(p[3][i] != 0)
                    {
                        points.Add(new PointF(p[0][i] / p[3][i] * 50, p[1][i] / p[3][i] * 50));
                    } else
                    {
                        points.Add(new PointF());
                    }
                }

                // Desenhar linhas
                for (int i = 0; i < a.Count; i++)
                {
                    if(p[3][a[i].Item1] != 0 && p[3][a[i].Item2] != 0)
                    {
                        e.Graphics.DrawLine(blackPen, points[a[i].Item1], points[a[i].Item2]);
                    }
                }
            }
        }
    }
}
