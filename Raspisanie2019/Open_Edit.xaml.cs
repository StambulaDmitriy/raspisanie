using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для Open_Edit.xaml
    /// </summary>
    public partial class Open_Edit : Window
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
        Dictionary<int, string> Faks = new Dictionary<int, string>(0);
        Dictionary<int, string> VidiPodg = new Dictionary<int, string>(0);
        Dictionary<int, string> Specials = new Dictionary<int, string>(0);
        Dictionary<int, string> Grupps = new Dictionary<int, string>(0);
        Dictionary<int, string> GodiNabora = new Dictionary<int, string>(0);
		public ObservableCollection<Item> DGItems;
        List<int> IDs = new List<int>(0);
        List<int> Years = new List<int>(0);
        public Open_Edit()
        {
            InitializeComponent();
            // заполнение id
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetRasps, Db.myConnection))
            {
                SDR = SC.ExecuteReader();
                while (SDR.Read())
                    IDs.Add(Convert.ToInt32(SDR["id"]));
                cbIdRasp.ItemsSource = IDs;
            }
            // заполнение факультетов
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
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetYearsFromRasp, Db.myConnection))
            {
                SDR = SC.ExecuteReader();
                while (SDR.Read())
                    Years.Add(SDR.GetInt32(0));
                

            }
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

            // заливаем список расписаний
            Filt();

        }
        private void TbYears_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Back)
            {
                tbYears.Text.Remove(--tbYears.CaretIndex, 1);
                tbYears.Text.Insert(tbYears.CaretIndex, "*");
                e.Handled = true;
                return;
            }
        }

        private void TbYears_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }
            //if (tbYears.Text.Length == 3)
            //     tbYear.Text = " - " + (Convert.ToInt32(tbYears.Text + e.Text) - 1).ToString();
            try
            {
                int bufIndex = tbYears.CaretIndex;
                tbYears.Text = tbYears.Text.Remove(tbYears.CaretIndex, 1);
                tbYears.CaretIndex = bufIndex;
                if (tbYears.Text.IndexOf('*') < 0)
                    tbYear.Text = " - " + (Convert.ToInt32(tbYears.Text + e.Text) + 1).ToString();
            }
            catch (Exception exc)
            {
                e.Handled = true;
            }
        }

        private void TbYears_GotFocus(object sender, RoutedEventArgs e)
        {
            tbYears.CaretIndex = 0;
            e.Handled = true;
        }

        private void TbYears_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tbYears.IsEnabled = true;
            tbYears.Focus();
        }

        private void TbYears_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //cbYears.Focus();
            tbYears.CaretIndex = 0;
            tbYears.IsEnabled = true;
        }
        
        private void TbYears_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Back)
            {
                if (tbYears.CaretIndex == 0)
                {
                    e.Handled = true;
                    return;
                }
                int bufIndex = tbYears.CaretIndex;
                tbYears.Text = tbYears.Text.Remove(--tbYears.CaretIndex, 1);
                tbYears.CaretIndex = bufIndex;
                tbYears.Text = tbYears.Text.Insert(tbYears.CaretIndex, "*");
                tbYears.CaretIndex = bufIndex - 1;
                tbYear.Text = "";
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Delete)
            {
                e.Handled = true;
                return;
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
            Filt();
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
            Filt();
        }

        private void CbSpecial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //выбор групп по специальности, сбросить индексы, добавить проверки индексов на -1 +
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
                Filt();
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
                Filt();
            }
        }
        public class Item
        {
            public int Id { get; set; }
            public string gruppa { get; set; }
            public int Kod_grup { get; set; }
            public int Uch_god { get; set; }
            public string sem { get; set; }
            public int N_sem { get; set; }
            public string namefak { get; set; }
            public int k_ft { get; set; }
            public string namepodg { get; set; }
            public int Vid_podg { get; set; }
            public int Kurs { get; set; }
            public int Npp_g { get; set; }
            public int kod_fo { get; set; }
			public Item() {

			}
            public Item(int Id, string gruppa, int Kod_grup, int Uch_god, string sem, int N_sem, string namefak, int k_ft, string namepodg, int Vid_podg, int Kurs, int Npp_g, int kod_fo)
            {
                this.Id = Id;
                this.gruppa = gruppa;
                this.Kod_grup = Kod_grup;
                this.Uch_god = Uch_god;
                this.sem = sem;
                this.N_sem = N_sem;
                this.namefak = namefak;
                this.k_ft = k_ft;
                this.namepodg = namepodg;
                this.Vid_podg = Vid_podg;
                this.Kurs = Kurs;
                this.Npp_g = Npp_g;
                this.kod_fo = kod_fo;
            }
        }
        void Filt()
        {
			using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetRasps, Db.myConnection))
            {
                if(cbIdRasp.SelectedIndex != -1)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.id;
                    SC.Parameters.AddWithValue("@id", Convert.ToInt32(cbIdRasp.SelectedItem));
                }

                if (cbFak.SelectedIndex != -1)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.k_ft;
                    SC.Parameters.AddWithValue("@k_ft", Faks.First(x => x.Value == cbFak.SelectedItem.ToString()).Key);
                }

                if (tbYears.Text.Count(x => x == '*') == 0)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.uch_god;
                    SC.Parameters.AddWithValue("@uch_god", Convert.ToInt32(tbYears.Text));
                }

                if (rbOsenniy.IsChecked == true || rbVesenniy.IsChecked == true)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.n_sem;
                    SC.Parameters.AddWithValue("@n_sem", Convert.ToInt32(rbVesenniy.IsChecked));
                }

                if (cbVidPodg.SelectedIndex != -1)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.vid_podg;
                    SC.Parameters.AddWithValue("@vid_podg", VidiPodg.FirstOrDefault( x => x.Value == cbVidPodg.SelectedItem.ToString()).Key);
                }

                if (cbSpecial.SelectedIndex != -1)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.kod_spec;
                    SC.Parameters.AddWithValue("@kod_spec",Specials.FirstOrDefault(x => x.Value == cbSpecial.SelectedItem.ToString()).Key);
                }

                if (cbGodNabora.SelectedIndex != -1)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.god_nabora;
                    SC.Parameters.AddWithValue("@god_nabora", Convert.ToInt32(cbGodNabora.SelectedItem));
                }

                if (cbGruppa.SelectedIndex != -1)
                {
                    SC.CommandText += Db.SQLCommands.Params.ForGetRasps.kod_grup;
                    SC.Parameters.AddWithValue("@kod_grup", Grupps.FirstOrDefault(x => x.Value == cbGruppa.SelectedItem.ToString()).Key);
                }
                SDR = SC.ExecuteReader();
                DG.Items.Clear();
                while(SDR.Read())
                {
					Item t = new Item();
					t.Id = Convert.ToInt32(SDR["id"]);
					t.gruppa = SDR["name_grup"].ToString();
					t.Kod_grup = Convert.ToInt32(SDR["KOD_GRUP"]);
					t.Uch_god = Convert.ToInt32(SDR["UCH_GOD"]);
					t.sem = (Convert.ToInt32(SDR["N_SEM"]) == 0) ? "Осенний" : "Весенний";
					t.N_sem = Convert.ToInt32(SDR["N_SEM"]);
					t.namefak = SDR["fak_name"].ToString();
					t.k_ft = Convert.ToInt32(SDR["k_ft"]);
					t.namepodg = SDR["namepodg"].ToString();
					t.Vid_podg = Convert.ToInt32(SDR["vid_podg"]);
					t.Kurs = Convert.ToInt32(SDR["kurs"]);
					t.Npp_g = Convert.ToInt32(SDR["npp_g"]);
					t.kod_fo = Convert.ToInt32(SDR["kod_fo"]);

					DG.Items.Add(t); 
                }
				
                //DG.ItemsSource = DGItems;
                //DG.Items.Refresh();
            }
        }

        private void TbYears_KeyUp(object sender, KeyEventArgs e)
        {
           if(e.Key == Key.Enter)
            {
                Filt();
            }
        }

        private void CbIdRasp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filt();
        }

        private void CbGruppa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filt();
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //e.OriginalSource
            //e.Source
            // записать id
            // DG.SelectedItem
            id_rasp_year = (DG.SelectedItem as Item).Id;
            DialogResult = true;
    }

        private void RbOsenniy_Checked(object sender, RoutedEventArgs e)
        {
            Filt();
        }
    }
}
