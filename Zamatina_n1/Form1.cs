using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zamatina_n1
{
    public partial class Form1 : Form
    {
        public int a, b, c, z0, zn, i, n = 0;
        public double h;//Псевдо случайное число
        public const int n_max = 100;
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (i = 0; i < n_max; ++i)
                m[i] = 0;
            button1.Enabled = tabControl.SelectedIndex != 7;
        }

        public int nn_num = 10;
        int[] m = new int[n_max];
        public Form1()
        {
            InitializeComponent();
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
        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {

        }



  
        private void button1_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)//Равномерное
            {
                int mash = 100;
                chart1.Series[0].Points.Clear();
                a = 29;
                b = 31;
                c = 3991;
                lb_raspr.Items.Clear();
                z0 = (int)int.Parse(tb_raspr_first.Text);
                if (cb_raspr.Checked)
                {
                    z0 = DateTime.Now.Millisecond;
                    tb_raspr_first.Text = Convert.ToString(z0);
                    Refresh();
                }
                n = (int)int.Parse(tb_raspr_kol.Text);
                for (i = 0; i < n_max; i++)
                    m[i] = 0;
                for (i = 0; i < n; i++)
                {
                    if (cb_norm.Checked)
                        lb_raspr.Items.Add((object)((double)z0 / (double)c));
                    else
                        lb_raspr.Items.Add((object)z0);
                    m[(int)Math.Truncate((double)z0 / (double)c * mash)]++;
                    zn = (a * z0 + b) % c;
                    z0 = zn;
                }
                for (i = 0; i < mash; i++)
                    chart1.Series[0].Points.AddY(m[i]);
                //panel1.Refresh();
            }
            if (tabControl.SelectedIndex == 1)//равномерное на отрезке
            {
                int mash = 100;
                chart2.Series[0].Points.Clear();
                lb_ravnRaspr.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_ravnRaspr_kol.Text);
                a = (int)int.Parse(tb_ravnRaspr_from.Text);
                b = (int)int.Parse(tb_ravnRaspr_Do.Text);
                for (i = 0; i < n_max; i++)
                    m[i] = 0;
                for (i = 0; i < n; i++)
                {
                    h = nn(z0);
                    ++m[(int)Math.Truncate(h * (double)mash)];
                    h = (double)a + (double)(b - a) * h;
                    lb_ravnRaspr.Items.Add((object)h);
                }
                for (i = 0; i < mash; i++)
                    chart2.Series[0].Points.AddY(m[i]);
            }
            if (tabControl.SelectedIndex == 2)//треугольное
            {
                int mash = 100;
                chart3.Series[0].Points.Clear();
                lb_tre.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_tre_kol.Text);
                double min = double.Parse(tb_tre_max.Text);
                double moda = double.Parse(tb_tre_moda.Text);
                double max = double.Parse(tb_tre_min.Text);
                if (moda >= max || moda <= min)
                {
                    moda = Math.Round((min + max) / 2.0);
                    tb_tre_moda.Text = moda.ToString();
                    Refresh();
                }
                for (i = 0; i < mash; i++)
                    m[i] = 0;
                for (i = 0; i < n; i++)
                {
                    h = nn(z0);
                    if (h >= (moda - min) / (max - min))
                        h = max - Math.Sqrt((max - moda) * (max - min) * (1.0 - h));
                    else
                        h = min + Math.Sqrt((moda - min) * (max - min) * h);
                    ++m[(int)Math.Truncate((h - min) / (max - min) * mash)];
                    lb_tre.Items.Add((object)h);
                }
                for (i = 0; i < mash; i++)
                    chart3.Series[0].Points.AddY(m[i]);
               
            }
            if (tabControl.SelectedIndex == 3)//эрланд
            {
                int mash = 100;
                lb_erl.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_erl_kolEsp.Text);
                int kol_norm = (int)nUD_erl_kol_norm.Value;
                int moda = (int)int.Parse(tb_erl_moda.Text);
                for (i = 0; i < mash; i++)
                    m[i] = 0;
                for (i = 0; i < n; i++)
                {
                    h = 1.0;
                    for (int index = 1; index <= kol_norm; index++)
                        h = h * nn(z0);
                    h = (double)-moda * Math.Log(h);
                    if (h < 150.0)
                        ++m[(int)Math.Truncate(h * 2.0 / 3.0)];
                    lb_erl.Items.Add(h);
                }
                chart4.Series[0].Points.Clear();
                for (i = 0; i < mash; i++)
                    chart4.Series[0].Points.AddY(m[i]);
                //panel4.Refresh();
            }
            if (tabControl.SelectedIndex == 4)//пуасон
            {
                int mash = 100;
                lb_puas.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                int lambd = (int)int.Parse(tb_puas_lambd.Text);
                n = (int)int.Parse(tb_puas_kol.Text);
                for (i = 0; i < mash; ++i)
                    m[i] = 0;
                for (i = 0; i < n; ++i)
                {
                    int index = 0;
                    double num2 = Math.Exp((double)-lambd);
                    double num3 = nn(z0);
                    while (num3 > 0.0)
                    {
                        num2 = num2 * (double)lambd / (double)(index + 1);
                        ++index;
                        num3 -= num2;
                    }
                    if (index < 100)
                        ++m[index];
                    lb_puas.Items.Add((object)index);
                }
                chart5.Series[0].Points.Clear();
                for (i = 0; i < mash; i++)
                    chart5.Series[0].Points.AddY(m[i]);
            }
            if (tabControl.SelectedIndex == 5)
            {
                int mash = 100;
                lb_norm.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_norm_kol.Text);
                int m_ = (int)int.Parse(tb_norm_m.Text);
                int d = (int)int.Parse(tb_norm_d.Text);
                for (i = 0; i < mash; ++i)
                    m[i] = 0;
                for (i = 0; i < n; ++i)
                {
                    double num3 = Math.Sin(2.0 * Math.PI * nn(z0)) * Math.Sqrt(-2.0 * Math.Log(nn(z0)));
                    double num4 = (double)m_ + (double)d * num3;
                    if (num4 < 300.0 && num4 > 0.0)
                        ++m[(int)num4 / 3];
                    lb_norm.Items.Add((object)num4);
                }
                chart6.Series[0].Points.Clear();
                for (i = 0; i < mash; i++)
                    chart6.Series[0].Points.AddY(m[i]);
            }
            if (tabControl.SelectedIndex == 6)
            {
                int mash = 100;
                lb_exp.Items.Clear();
                nn_num = 10;
                z0 = DateTime.Now.Millisecond;
                n = (int)int.Parse(tb_exp_kol.Text);
                int M = (int)int.Parse(tb_exp_m.Text);
                for (i = 0; i < mash; ++i)
                    m[i] = 0;
                for (i = 0; i < n; ++i)
                {
                    double num1 = -Math.Log(nn(z0)) * (double)M;
                    if (num1 < 200.0 && num1 > 0.0)
                        ++m[(int)(num1 / 2)];
                    lb_exp.Items.Add(num1);
                }
                chart7.Series[0].Points.Clear();
                for (i = 0; i < mash; i++)
                    chart7.Series[0].Points.AddY(m[i]);
            }
        }
    }
}
