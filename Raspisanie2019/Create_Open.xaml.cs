using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Raspisanie2019
{
    /// <summary>
    /// Логика взаимодействия для Create_Open.xaml
    /// </summary>
    public partial class Create_Open : Window
    {
        SqlDataReader SDR;
        public int kod_ft;
        public int uch_god;
        public int semestr;
        public int vid_podgotovki;
        public int prog_spec;
        public int god_nabora;
        public int gruppa;
        public int id_rasp_year;
		List<string> uchYears;
		Dictionary<int, string> Faks = new Dictionary<int, string>(0);
        Dictionary<int, string> VidiPodg = new Dictionary<int, string>(0);
        Dictionary<int, string> Specials = new Dictionary<int, string>(0);
        Dictionary<int, string> Grupps = new Dictionary<int, string>(0);
        Dictionary<int, string> GodiNabora = new Dictionary<int, string>(0);
        /// <summary>
        /// true -- расписание существует, 
        /// false -- распасания нет
        /// </summary>
        public bool flag = false;
        public int prot_id_rasp_year;

        public Create_Open()
        {
            InitializeComponent();
			uchYears = new List<string>();
			for (int i = DateTime.Now.Year - 4; i < DateTime.Now.Year + 2; i++)
			{
				uchYears.Add(String.Format("{0} - {1}", i, i + 1));
			}
			cbYear.ItemsSource = uchYears;
			cbYear.SelectedIndex = 4;
			//int key = VidiPodg.FirstOrDefault(x => x.Value == "one").Key;
			//Выбор факультетов
			using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetFaks, Db.myConnection))
            {
                SDR = SC.ExecuteReader();
                //пропуск записи <<Все факультеты>>
                SDR.Read();
                while (SDR.Read())
                    Faks.Add(Convert.ToInt32(SDR["k_ft"]), SDR["fak_name"].ToString());
                SDR.Close();
            }
            if (Db.kod_ft == 0)
            {
                cbFak.IsEnabled = true;
                cbFak.ItemsSource = Faks.Values;
                kod_ft = 0;
            }
            else
            {
                cbFak.Items.Add(Faks[Db.kod_ft]);
                cbFak.SelectedIndex = 0;
            }
            //Выбор годов обучения

            //Выбор видов подготовки
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetVidPodgot, Db.myConnection))
            {
                SDR = SC.ExecuteReader();
                while (SDR.Read())
                    VidiPodg.Add(Convert.ToInt32(SDR["k_vid_podg"]), SDR["name"].ToString());
                cbVidPodg.ItemsSource = VidiPodg.Values;
                SDR.Close();
            }
            //Выбор специальностей
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetSpec, Db.myConnection))
            {
                //if (kod_ft != 0)
                //{
                //    SC.CommandText += Db.SQLCommands.Params.kod_ft;
                //    SC.Parameters.Add(new SqlParameter("@kod_ft", kod_ft));
                //}
                SDR = SC.ExecuteReader();
                while (SDR.Read())
                    Specials.Add(Convert.ToInt32(SDR["cod_sp"]), string.Format("{0} ({1})", SDR["name"].ToString(), SDR["snam"].ToString()));
                cbSpecial.ItemsSource = Specials.Values;
                SDR.Close();
            }
            //Выбор годов набора
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetYears, Db.myConnection))
            {
                //if (kod_ft != 0)
                //{
                //    SC.CommandText += Db.SQLCommands.Params.kod_ft;
                //    SC.Parameters.Add(new SqlParameter("@kod_ft", kod_ft));
                //}
                SDR = SC.ExecuteReader();
                while (SDR.Read())
                    cbGodNabora.Items.Add(SDR[0]);
                SDR.Close();
            }
            //Выбор групп
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetGrupps, Db.myConnection))
            {
                //if (kod_ft != 0)
                //{
                //    SC.CommandText += Db.SQLCommands.Params.kod_ft;
                //    SC.Parameters.Add(new SqlParameter("@kod_ft", kod_ft));
                //}
                SDR = SC.ExecuteReader();
                while (SDR.Read())
                    Grupps.Add(Convert.ToInt32(SDR["KOD_GRUP"]), SDR["NAME_GRUP"].ToString());
                cbGruppa.ItemsSource = Grupps.Values;
                SDR.Close();
            }
        }


        private void CbFak_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int key = VidiPodg.FirstOrDefault(x => x.Value == "one").Key;
            kod_ft = Faks.FirstOrDefault(x => x.Value == cbFak.SelectedItem as string).Key;
            cbGruppa.SelectedIndex = -1;
            cbSpecial.SelectedIndex = -1;
            cbVidPodg.SelectedIndex = -1;
            cbGodNabora.SelectedIndex = -1;

            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetVidPodgot, Db.myConnection))
            {
                SC.CommandText += Db.SQLCommands.Params.kod_ftGruppa;
                SC.Parameters.AddWithValue("@kod_ft", kod_ft);
                SDR = SC.ExecuteReader();
                VidiPodg.Clear();
                while (SDR.Read())
                    VidiPodg.Add(Convert.ToInt32(SDR["k_vid_podg"]), SDR["name"].ToString());
                //cbVidPorg.ItemsSource = VidiPodg.Values;
                cbVidPodg.Items.Refresh();
                SDR.Close();
            }

            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetSpec, Db.myConnection))
            {
                SC.CommandText += Db.SQLCommands.Params.kod_ftSpec;
                SC.Parameters.AddWithValue("@kod_ft", kod_ft);
                SDR = SC.ExecuteReader();
                Specials.Clear();
                while (SDR.Read())
                    Specials.Add(Convert.ToInt32(SDR["cod_sp"]), string.Format("{0} ({1})", SDR["name"].ToString(), SDR["snam"].ToString()));
                //cbSpecial.ItemsSource = Specials.Values;
                cbSpecial.Items.Refresh();
                SDR.Close();
            }

            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetGrupps, Db.myConnection))
            {

                SC.CommandText += Db.SQLCommands.Params.kod_ftGruppa;
                SC.Parameters.AddWithValue("@kod_ft", kod_ft);
                SDR = SC.ExecuteReader();
                Grupps.Clear();
                while (SDR.Read())
                    Grupps.Add(Convert.ToInt32(SDR["KOD_GRUP"]), SDR["NAME_GRUP"].ToString());
                //cbGruppa.ItemsSource = Grupps.Values;
                cbGruppa.Items.Refresh();
                SDR.Close();
            }
            
        }

        private void CbVidPodg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVidPodg.SelectedIndex == -1)
                return;
            vid_podgotovki = VidiPodg.FirstOrDefault(x => x.Value == cbVidPodg.SelectedItem as string).Key;
            cbGruppa.SelectedIndex = -1;
            cbSpecial.SelectedIndex = -1;
            cbGodNabora.SelectedIndex = -1;
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetSpec, Db.myConnection))
            {
                SC.CommandText += Db.SQLCommands.Params.kod_ftSpec;
                SC.CommandText += Db.SQLCommands.Params.vid_podg;
                SC.Parameters.AddWithValue("@kod_ft", kod_ft);
                SC.Parameters.AddWithValue("@vid_podg", vid_podgotovki);
                SDR = SC.ExecuteReader();
                Specials.Clear();
                while (SDR.Read())
                    Specials.Add(Convert.ToInt32(SDR["cod_sp"]), string.Format("{0} ({1})", SDR["name"].ToString(), SDR["snam"].ToString()));
                //cbSpecial.ItemsSource = Specials.Values;
                cbSpecial.Items.Refresh();
                SDR.Close();
            }

            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetGrupps, Db.myConnection))
            {
                SC.CommandText += Db.SQLCommands.Params.kod_ftGruppa;
                SC.CommandText += Db.SQLCommands.Params.vid_podg;
                SC.Parameters.AddWithValue("@kod_ft", kod_ft);
                SC.Parameters.AddWithValue("@vid_podg", vid_podgotovki);
                SDR = SC.ExecuteReader();
                Grupps.Clear();
                while (SDR.Read())
                    Grupps.Add(Convert.ToInt32(SDR["KOD_GRUP"]), SDR["NAME_GRUP"].ToString());
                //cbGruppa.ItemsSource = Grupps.Values;
                cbGruppa.Items.Refresh();
                SDR.Close();
            }
        }

        private void CbSpecial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //выбор групп по специальности, сбросить индексы, добавить проверки индексов на -1
            if (cbSpecial.SelectedIndex == -1)
                return;
            prog_spec = Specials.FirstOrDefault(x => x.Value == cbSpecial.SelectedItem as string).Key;
            cbGodNabora.SelectedIndex = -1;
            cbGruppa.SelectedIndex = -1;
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetGrupps, Db.myConnection))
            {
                SC.CommandText += Db.SQLCommands.Params.kod_ftGruppa;
                SC.CommandText += Db.SQLCommands.Params.vid_podg;
                SC.CommandText += Db.SQLCommands.Params.kod_spec;
                SC.Parameters.AddWithValue("@kod_ft", kod_ft);
                SC.Parameters.AddWithValue("@vid_podg", vid_podgotovki);
                SC.Parameters.AddWithValue("@kod_spec", prog_spec);
                SDR = SC.ExecuteReader();
                Grupps.Clear();
                while (SDR.Read())
                    Grupps.Add(Convert.ToInt32(SDR["KOD_GRUP"]), SDR["NAME_GRUP"].ToString());
                //cbGruppa.ItemsSource = Grupps.Values;
                cbGruppa.Items.Refresh();
                SDR.Close();
            }
        }

        private void CbGodNabora_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbGodNabora.SelectedIndex == -1)
                return;
            god_nabora = Convert.ToInt32(cbGodNabora.SelectedItem);
            cbGruppa.SelectedIndex = -1;
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetGrupps, Db.myConnection))
            {
                SC.CommandText += Db.SQLCommands.Params.kod_ftGruppa;
                SC.CommandText += Db.SQLCommands.Params.vid_podg;
                SC.CommandText += Db.SQLCommands.Params.kod_spec;
                SC.CommandText += Db.SQLCommands.Params.god_nabora;
                SC.Parameters.AddWithValue("@kod_ft", kod_ft);
                SC.Parameters.AddWithValue("@vid_podg", vid_podgotovki);
                SC.Parameters.AddWithValue("@kod_spec", prog_spec);
                SC.Parameters.AddWithValue("@god_nabora", god_nabora);
                SDR = SC.ExecuteReader();
                Grupps.Clear();
                while (SDR.Read())
                    Grupps.Add(Convert.ToInt32(SDR["KOD_GRUP"]), SDR["NAME_GRUP"].ToString());
                //cbGruppa.ItemsSource = Grupps.Values;
                cbGruppa.Items.Refresh();
                SDR.Close();
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            // если хотя бы один CB без данных, то выскочит ошибка 
            // (для красоты можно добавить подсвечивание незаполненного поля)
            if (cbFak.SelectedIndex == -1 ||
                cbVidPodg.SelectedIndex == -1 ||
                cbSpecial.SelectedIndex == -1 ||
                cbGodNabora.SelectedIndex == -1 ||
                cbGruppa.SelectedIndex == -1 ||
                (rbOsenniy.IsChecked == false && rbVesenniy.IsChecked == false))
            {
                MessageBox.Show("Введены не все данные");
                return;
            }
            // попытка поиска расписания
            // если расписание найдено, 
            // то форма вернет true, 
            // иначе останется null(DialogResult типа (bool?))
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetFromRaspYear, Db.myConnection))
            {
                SC.Parameters.AddWithValue("@kod_grup",gruppa);
				SC.Parameters.AddWithValue("@uch_god",cbYear.SelectedItem.ToString().Substring(0,4));
				SC.Parameters.AddWithValue("@vid_podg",vid_podgotovki);
                SC.Parameters.AddWithValue("@k_ft",kod_ft);
                SC.Parameters.AddWithValue("@n_sem", Convert.ToInt32(rbVesenniy.IsChecked));
                using (SqlDataReader SDR = SC.ExecuteReader())
                    if (SDR.Read())
                    {
                        id_rasp_year = Convert.ToInt32(SDR["id"]);
                        flag = true;
                    }
            }
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CbGruppa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbGruppa.SelectedIndex == -1)
                return;
            gruppa = Grupps.FirstOrDefault(x => x.Value == cbGruppa.SelectedItem as string).Key;
        }

        private void ChoosePrototype_Checked(object sender, RoutedEventArgs e)
        {
            Open_Edit OE = new Open_Edit();
            if (OE.ShowDialog() == false)
                return;
            prot_id_rasp_year = OE.id_rasp_year;
            flag = Convert.ToBoolean(1); // :)
        }
    }
}
