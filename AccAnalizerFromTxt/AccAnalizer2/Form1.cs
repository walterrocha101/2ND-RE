using Equin.ApplicationFramework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccAnalizer2
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
            Text = Application.StartupPath + " - AccAnalizerFromFile";
        }
        public List<string> ip_listFtp;// = new List<string>();
        public List<DataForAnalize> dataForAnalizes = new List<DataForAnalize>();
        List<StrIP> strIPs = new List<StrIP>();
        private void BtGetDataFromCloud_Click(object sender, EventArgs e)
        {
            List<string> counrtysCodes = new List<string>();
            List<string> counrtysNames = new List<string>();
            foreach (var item in File.ReadAllLines("countrys.txt"))
            {
                counrtysNames.Add(item.Split(';')[0]);
                counrtysCodes.Add(item.Split(';')[1]);
            }
            dataForAnalizes.Clear();
            strIPs.Clear();
            this.dataForAnalizeBindingSource.DataSource = dataForAnalizes;
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            ip_listFtp = new List<string>(File.ReadAllLines(openFileDialog1.FileName));
            List<string> countrys = new List<string>();
            foreach (string item in ip_listFtp)
            {
                try
                {
                    StrIP strIP = new StrIP();
                    strIP.logpass = item.Split('|')[0];
                    strIP.dr = item.Split('|')[1];
                    strIP.id = item.Split('|')[2];
                    try
                    {
                        strIP.dtreg = item.Split('|')[3];
                    }
                    catch
                    {
                        strIP.dtreg = "null";
                    }
                    try
                    {
                        strIP.mpass = item.Split('|')[4];
                    }
                    catch
                    {
                        strIP.mpass = "null";
                    }
                    try
                    {
                        strIP.country = item.Split('|')[5];
                        if (string.IsNullOrEmpty(strIP.country))
                        {
                            strIP.country = "null";
                        }
                        if (strIP.country.Contains('{'))
                        {
                            strIP.country = strIP.country.Split('"')[7];
                        }
                    }
                    catch
                    {
                        strIP.country = "null";
                    }
                    try
                    {
                        strIP.ip = item.Split('|')[6];
                    }
                    catch
                    {
                        strIP.ip = "null";
                    }
                    try
                    {
                        strIP.fln = item.Split('|')[7];
                    }
                    catch
                    {
                        strIP.fln = "null";
                    }

                    strIPs.Add(strIP);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message +" - "+ item);
                }
            }
            foreach (StrIP item in strIPs)
            {
                countrys.Add(item.country);
            }
            countrys = countrys.Distinct().ToList();
            int all_count_reg = 0;
            int all_count_reg_good = 0;
            int all_count_reg_fail = 0;
            int count_ID = 0;
            foreach (string item in countrys)
            {
                count_ID++;
                DataForAnalize dataForAnalize = new DataForAnalize();
                dataForAnalize.ID = count_ID;
                dataForAnalize.country = item;
                int count_reg = 0;
                int count_reg_good = 0;
                int count_reg_fail = 0;
                foreach (StrIP strIP in strIPs)
                {
                    if (strIP.country == item)
                    {
                        count_reg++;
                        if (strIP.id != "null")
                        {
                            count_reg_good++;
                        }
                        else
                        {
                            count_reg_fail++;
                        }
                    }
                }
                dataForAnalize.countReg = count_reg;
                dataForAnalize.countGoodReg = count_reg_good;
                dataForAnalize.countFailReg = count_reg_fail;
                all_count_reg += count_reg;
                all_count_reg_good += count_reg_good;
                all_count_reg_fail += count_reg_fail;
                dataForAnalize.percent = ((int)(count_reg_good * 100 / count_reg));
                dataForAnalize.ratio = ((int)(count_reg_good * count_reg));
                int poz = counrtysCodes.IndexOf(dataForAnalize.country); //(ConvertStringToPredicate(f));
                if (poz != -1)
                {
                    dataForAnalize.countryName = counrtysNames[poz];
                }
                dataForAnalizes.Add(dataForAnalize);
            }
            BindingListView<DataForAnalize> dataForAnalizesBLV = new BindingListView<DataForAnalize>(dataForAnalizes);
            this.dataForAnalizeBindingSource.DataSource = dataForAnalizesBLV;
            //            string[] row = new string[] { count_ID.ToString(), all_count_reg.ToString(), all_count_reg_good.ToString(), all_count_reg_fail.ToString(), ((int)(all_count_reg_good * 100 / all_count_reg)).ToString(), "" };
            //dataGridView2.Rows.Add(row);
            //dataGridView2.Rows[0].HeaderCell.Value = "Итог";
        }
        private void FillRecordNo()
        {
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                this.dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }
        private void BtGetAccIDFromCountry_Click(object sender, EventArgs e)
        {
            int x = dataGridView1.CurrentRow.Index;
            //nujno poluchit zyachenie 1.x
            string selectedCountry = (string)dataGridView1[0, x].Value;
            // if exist delete
            File.WriteAllText(selectedCountry + "_IDs.txt", "");
            //
            foreach (StrIP strIP in strIPs)
            {
                if (strIP.country == selectedCountry)
                {
                    if (strIP.id != "null")
                    {
                        File.AppendAllText(selectedCountry + "_IDs.txt", strIP.id + Environment.NewLine);
                    }
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.FillRecordNo();
        }
    }
}
