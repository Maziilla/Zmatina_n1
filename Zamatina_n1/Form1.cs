using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics.Distributions;
using MathNet.Numerics;

namespace Zamatina_n1
{
    public partial class Form1 : Form
    {
        Series barSeries = new Series();
        public int a, b, c, z0, zn, i, n = 0;
        public double h;//Псевдо случайное число
        public const int n_max = 100;
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //barSeries.Points.Clear();
        }

        public int nn_num = 10;



        int[] m = new int[n_max];
        public Form1()
        {
            InitializeComponent();


            chart1.Series.Add(barSeries);
            chart2.Series.Add(barSeries);
            chart3.Series.Add(barSeries);
            chart4.Series.Add(barSeries);
            chart5.Series.Add(barSeries);
            chart6.Series.Add(barSeries);
            chart7.Series.Add(barSeries);
        }
        private double nn(int z0)
        {
            int num = 1;
            for (int index = 0; index < nn_num; ++index)
            {
                num = (29 * z0 + 31) % 3991;
                z0 = num;
            }
            ++nn_num;
            return (double)num / 3991.0;
        }
        private Dictionary<double, double> GetOverlayData(double step, Chart cherte)
        {
            var min = cherte.Series[0].Points.Min(x => x.XValue);
            var max = cherte.Series[0].Points.Max(x => x.XValue);
            var overlayData = new Dictionary<double, double>();
            Func<int, double, double> g = (tb, x) =>
            {
                switch (tabControl.SelectedIndex)
                {
                    case 0:
                        return 1;
                    case 1:
                        return 1 / (max - min);
                    case 2:
                        return Triangular.PDF(Convert.ToDouble(tb_tre_max.Text), Convert.ToDouble(tb_tre_min.Text), Convert.ToDouble(tb_tre_moda.Text), x);
                    case 3:
                        return Erlang.PDF(Convert.ToInt32(nUD_erl_kol_norm.Value), 1.0 / Convert.ToDouble(tb_erl_moda.Text), x);
                    case 4:
                        return Poisson.PMF(Convert.ToDouble(tb_puas_lambd.Text), (int)x);
                    case 5:
                        return Normal.PDF(Convert.ToDouble(tb_norm_m.Text), Convert.ToDouble(tb_norm_d.Text), x);
                    case 6:
                        return Exponential.PDF(1.0 / Convert.ToDouble(tb_exp_m.Text), x);
                    default:
                        return double.NaN;
                }
            };
            for (var x = min; x <= max; x += step)
            {
                overlayData[x] = g(tabControl.SelectedIndex, x);
            }
            return overlayData;
        }
        private void RebuildChart(double[] values, int stepsNumber, ListBox list,Chart cherte)
        {
            cherte.Series[0].Points.Clear();
            list.DataSource = values
                .Take(100)
                .ToArray();
            barSeries.Points.Clear();
            var stepWidth = (values.Max() - values.Min()) / stepsNumber;
            var bucketeer = new Dictionary<double, double>();
            for (var current = values.Min(); current <= values.Max(); current += stepWidth)
            {
                var mid = current + stepWidth / 2;
                var count = values.Where(x => x >= current && x < current + stepWidth).Count();
                bucketeer.Add(mid, count / (values.Length * stepWidth));
            }
            foreach (var pair in bucketeer.OrderBy(x => x.Key))
                cherte.Series[0].Points.AddXY(pair.Key, pair.Value);
            //barSeries.Points.AddXY(pair.Key, pair.Value);
        }
        private void PlotOverlayGraph(double step, Chart charte)
        {
            charte.Series[1].Points.Clear();
            var overlayData = GetOverlayData(tabControl.SelectedIndex == 4 ? 1 : step, charte);
            foreach (var e in overlayData.OrderBy(x => x.Key))
            {
                charte.Series[1].Points.AddXY(e.Key, e.Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double[] values;
            int stepsNumber;
            if (tabControl.SelectedIndex == 0)//Равномерное
            {
                int mash = 100;
                chart1.Series[0].Points.Clear();
                a = 29;
                b = 31;
                c = 3991;
                //lb_raspr.Items.Clear();
                z0 = (int)int.Parse(tb_raspr_first.Text);
                if (cb_raspr.Checked)
                {
                    z0 = DateTime.Now.Millisecond;
                    tb_raspr_first.Text = Convert.ToString(z0);
                    Refresh();
                }
                n = (int)int.Parse(tb_kol.Text);
                values = new double[n];
                //for (i = 0; i < n_max; i++)
                //    m[i] = 0;
                for (i = 0; i < n; i++)
                {

                    
                    values[i] = ((double)z0 / (double)c);                   
                    //lb_raspr.Items.Add(values[i]);
                    m[(int)Math.Truncate((double)z0 / (double)c * mash)]++;
                    zn = (a * z0 + b) % c;
                    z0 = zn;
                }
                stepsNumber = (int)(Math.Log(n, 2.0));
                RebuildChart(values, stepsNumber, lb_raspr,chart1);
                PlotOverlayGraph(0.05, chart1);
                //for (i = 0; i < mash; i++)
                //    chart1.Series[0].Points.AddY(m[i]);
                //panel1.Refresh();
            }
            if (tabControl.SelectedIndex == 1)//равномерное на отрезке
            {
                int mash = 100;
                chart2.Series[0].Points.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_kol.Text);
                values = new double[n];
                a = (int)int.Parse(tb_ravnRaspr_from.Text);
                b = (int)int.Parse(tb_ravnRaspr_Do.Text);
                //for (i = 0; i < n_max; i++)
                //    m[i] = 0;
                for (i = 0; i < n; i++)
                {
                    values[i] = nn(z0);
                    //++m[(int)Math.Truncate(h * (double)mash)];
                    values[i] = (double)a + (double)(b - a) * values[i];
                    //lb_ravnRaspr.Items.Add((object)h);
                }
                stepsNumber = (int)(Math.Log(n, 2.0));
                RebuildChart(values, stepsNumber, lb_ravnRaspr, chart2);
                PlotOverlayGraph(0.01, chart2);
                //for (i = 0; i < mash; i++)
                //    chart2.Series[0].Points.AddY(m[i]);
            }
            if (tabControl.SelectedIndex == 2)//треугольное
            {
                int mash = 100;
                chart3.Series[0].Points.Clear();
                //lb_tre.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_kol.Text);
                values = new double[n];
                double min = double.Parse(tb_tre_max.Text);
                double moda = double.Parse(tb_tre_moda.Text);
                double max = double.Parse(tb_tre_min.Text);
                if (moda >= max || moda <= min)
                {
                    moda = Math.Round((min + max) / 2.0);
                    tb_tre_moda.Text = moda.ToString();
                    Refresh();
                }
                //for (i = 0; i < mash; i++)
                //    m[i] = 0;
                for (i = 0; i < n; i++)
                {
                    values[i] = nn(z0);
                    if (values[i] >= (moda - min) / (max - min))
                        values[i] = max - Math.Sqrt((max - moda) * (max - min) * (1.0 - values[i]));
                    else
                        values[i] = min + Math.Sqrt((moda - min) * (max - min) * values[i]);
                    //++m[(int)Math.Truncate((h - min) / (max - min) * mash)];
                    //lb_tre.Items.Add((object)h);
                }
                stepsNumber = (int)(Math.Log(n, 2.0));
                RebuildChart(values, stepsNumber, lb_tre, chart3);
                PlotOverlayGraph(0.01, chart3);
                //for (i = 0; i < mash; i++)
                //    chart3.Series[0].Points.AddY(m[i]);

            }
            if (tabControl.SelectedIndex == 3)//эрланд
            {
                int mash = 100;
                //lb_erl.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_kol.Text);
                values = new double[n];
                int kol_norm = (int)nUD_erl_kol_norm.Value;
                int moda = (int)int.Parse(tb_erl_moda.Text);
                //for (i = 0; i < mash; i++)
                //    m[i] = 0;
                for (i = 0; i < n; i++)
                {
                    values[i] = 1.0;
                    for (int index = 1; index <= kol_norm; index++)
                        values[i] = values[i] * nn(z0);
                    values[i] = (double)-moda * Math.Log(values[i]);
                    //if (h < 150.0)
                    //    ++m[(int)Math.Truncate(h * 2.0 / 3.0)];
                    //lb_erl.Items.Add(h);
                }
                stepsNumber = (int)(Math.Log(n, 2.0));
                RebuildChart(values, stepsNumber, lb_erl, chart4);
                PlotOverlayGraph(0.01, chart4);
                //chart4.Series[0].Points.Clear();
                //for (i = 0; i < mash; i++)
                //    chart4.Series[0].Points.AddY(m[i]);
                //panel4.Refresh();
            }
            if (tabControl.SelectedIndex == 4)//пуасон
            {
                int mash = 100;
                //lb_puas.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                int lambd = (int)int.Parse(tb_puas_lambd.Text);
                n = (int)int.Parse(tb_kol.Text);
                values = new double[n];
                //for (i = 0; i < mash; ++i)
                //    m[i] = 0;
                for (i = 0; i < n; ++i)
                {
                    values[i] = 0;
                    double num2 = Math.Exp((double)-lambd);
                    double num3 = nn(z0);
                    while (num3 > 0.0)
                    {
                        num2 = num2 * (double)lambd / (double)(values[i] + 1);
                        ++values[i];
                        num3 -= num2;
                    }
                    //if (index < 100)
                    //    ++m[index];
                    //lb_puas.Items.Add((object)index);
                }
                stepsNumber = (int)(Math.Log(n, 2.0));
                RebuildChart(values, stepsNumber, lb_puas, chart5);
                PlotOverlayGraph(0.01, chart5);
                //chart5.Series[0].Points.Clear();
                //for (i = 0; i < mash; i++)
                //    chart5.Series[0].Points.AddY(m[i]);
            }
            if (tabControl.SelectedIndex == 5)//нормальное
            {
                int mash = 100;
                //lb_norm.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_kol.Text);
                values = new double[n];
                int m_ = (int)int.Parse(tb_norm_m.Text);
                int d = (int)int.Parse(tb_norm_d.Text);
                //for (i = 0; i < mash; ++i)
                //    m[i] = 0;
                for (i = 0; i < n; ++i)
                {
                    double num3 = Math.Sin(2.0 * Math.PI * nn(z0)) * Math.Sqrt(-2.0 * Math.Log(nn(z0)));
                     values[i] = (double)m_ + (double)d * num3;
                    //if (num4 < 300.0 && num4 > 0.0)
                    //    ++m[(int)num4 / 3];
                    //lb_norm.Items.Add((object)num4);
                }
                stepsNumber = (int)(Math.Log(n, 2.0));
                RebuildChart(values, stepsNumber, lb_norm, chart6);
                PlotOverlayGraph(0.01, chart6);
                //chart6.Series[0].Points.Clear();
                //for (i = 0; i < mash; i++)
                //    chart6.Series[0].Points.AddY(m[i]);
            }
            if (tabControl.SelectedIndex == 6)//exp
            {
                int mash = 100;
                //lb_exp.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_kol.Text);
                values = new double[n];
                int M = (int)int.Parse(tb_exp_m.Text);
                //for (i = 0; i < mash; ++i)
                //    m[i] = 0;
                for (i = 0; i < n; ++i)
                {
                    values[i] = -Math.Log(nn(z0)) * (double)M;
                    //if (num1 < 200.0 && num1 > 0.0)
                    //    ++m[(int)(num1 / 2)];
                    //lb_exp.Items.Add(num1);
                }
                stepsNumber = (int)(Math.Log(n, 2.0));
                RebuildChart(values, stepsNumber, lb_exp, chart7);
                PlotOverlayGraph(0.01, chart7);
                //chart7.Series[0].Points.Clear();
                //for (i = 0; i < mash; i++)
                //    chart7.Series[0].Points.AddY(m[i]);
            }
        }
    }
}
