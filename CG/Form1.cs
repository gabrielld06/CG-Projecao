using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace CG
{
    public partial class Form1 : Form
    {
        struct Objeto
        {
            public int nv, ns;
            public List<int> nvps;
            public List<List<int>> coord, vs;

            //NV Número de Vértices
            //X[I], Y[I], Z[I] Coordenadas dos Vértices
            //NS Número de Superfícies
            //NVPS[I] Número de Vértices por Superfície
            //VS[I] Vértices de uma determinada superfície – regra da mão direita

            public Objeto(int nv, int ns, List<int> nvps, List<List<int>> coord, List<List<int>> vs)
            {
                this.nv = nv;
                this.ns = ns;
                this.nvps = nvps;
                this.coord = coord;
                this.vs = vs;
            }
        }

        Objeto cubo = new Objeto(8, 6,
            new List<int>() { 4, 4, 4, 4, 4, 4 },
            new List<List<int>>() { new List<int>() { 1, 2, 1, 2, 1, 2, 1, 2 }, // x
                                    new List<int>() { 1, 1, 2, 2, 1, 1, 2, 2 }, // y
                                    new List<int>() { 1, 1, 1, 1, 2, 2, 2, 2 }, // z
                                    new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1 }}, // w
            new List<List<int>>() { new List<int>() { 0, 1, 2, 3 },
                                    new List<int>() { 4, 5, 6, 7 },
                                    new List<int>() { 0, 2, 4, 6 },
                                    new List<int>() { 1, 3, 5, 7 },
                                    new List<int>() { 2, 3, 6, 7 },
                                    new List<int>() { 0, 1, 4, 5 }});

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

            int[] n = VetorNormal(p0, p1, p2);

            List <List<int>> p = MatrizPerspectiva(cubo, n, p0, pv);

            String output = "Vetor Normal:\n";
            for (int i = 0; i < n.Length; i++)
            {
                output += i + ": " + n[i] + "\n";
            }
            MessageBox.Show(output);

            output = "per:\n";
            for (int i = 0; i < p.Count; i++)
            {
                output += i + ": " + p[i][0] + "\n";
            }
            MessageBox.Show(output);

            //parece certo seila
        }

    }
}
