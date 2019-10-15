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
    /// Логика взаимодействия для AddDataToCell.xaml
    /// </summary>
    public partial class AddDataToCell : Window
    {
        myCell NewCell;
        TextBox TB;
        Dictionary<int, string> AllDiscips = new Dictionary<int, string>(0);
        Dictionary<int, string> Discips = new Dictionary<int, string>(0);
        Dictionary<int, string> VidiZanyatiy = new Dictionary<int, string>(0);
        ObservableCollection<int> Korpusa = new ObservableCollection<int>(new List<int>(0));
        Dictionary<int, string> Auditorii = new Dictionary<int, string>(0);
        Dictionary<int, string> Preps = new Dictionary<int, string>(0);
        public AddDataToCell(myCell NewCell, Dictionary<int, string> Discips)
        {
            InitializeComponent();
            //Выбор данных, если они уже заполнены
            //cbPodgruppa.SelectionChanged += new SelectionChangedEventHandler(CbPodgruppa_SelectionChanged(cbPodgruppa,new SelectionChangedEventArgs());
            this.NewCell = NewCell;
            cbPodgruppa.ItemsSource = new ObservableCollection<string> { "Все", "1", "2" };
            #region Старое
            //cbPodgruppa.SelectedIndex = 0;
            //switch (NewCell.CollectionOfItems.Count)
            //{
            //    case 1:
            //        //ЗАПОЛНИТЬ КОМБОБОКСЫ
            //        Single.IsChecked = true;
            //        break;
            //    case 2:
            //        //ЗАПОЛНИТЬ КОМБОБОКСЫ
            //        Splited.IsChecked = true;
            //        break;
            //}
            #endregion
            this.Discips = Discips;
            // добавляем список дисциплин
            cbDiscips.ItemsSource = Discips.Values;
            //добавляем список видов подготовки
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetVidZanyatiy,Db.myConnection))
            {
                using (SqlDataReader SDR = SC.ExecuteReader())
                    while (SDR.Read())
                        VidiZanyatiy.Add(Convert.ToInt32(SDR["id"]), SDR["name_nagruzka"].ToString());
                cbVidZanyatiy.ItemsSource = VidiZanyatiy.Values;
            }
            // добавляем список корпусов
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetKorpusa, Db.myConnection))
            {
                using (SqlDataReader SDR = SC.ExecuteReader())
                    while (SDR.Read())
                        Korpusa.Add(Convert.ToInt32(SDR["AUD_KORPUS"]));
                cbKorpusa.ItemsSource = Korpusa;
            }
            
            //добавляем список преподавателей в cbPreps
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetPreps, Db.myConnection))
            {
                using (SqlDataReader SDR = SC.ExecuteReader())
                    while (SDR.Read())
                        Preps.Add(Convert.ToInt32(SDR["tabn_s"]),SDR["fio"].ToString());
                cbPreps.ItemsSource = Preps.Values;
            }
            this.NewCell.InnerDataGrid.Items.Refresh();

            // проверяем была ли ячейка заполнена
            
            switch (NewCell.InnerDataGrid.Items.Count)
            {
                case 0: Single.IsChecked = true; return;
                case 1:
                    Single.IsChecked = true;
                    #region Старое
                    //cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[NewCell.CollectionOfItems[0].CodeDisc]);
                    //cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[0].CodeVid]);
                    //cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[0].CodeGr;
                    //cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[0].NomerKorpusa]) - 1;
                    //cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[0].CodeAudit]);
                    //cbPreps.SelectedIndex = cbPreps.Items.IndexOf(Preps[NewCell.CollectionOfItems[0].CodePrep]);
                    //NewCell.CollectionOfItems[0]
                    #endregion
                    break;
                case 2:
                    Splited.IsChecked = true;

                    break;
            }
        }

        #region Старое событие для RbGr1
        private void Rb_Checked(object sender, RoutedEventArgs e)
        {
            NewCell.CollectionOfItems.Clear();
            switch (Single.IsChecked)
            {
                case true:
                    NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                    NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight;
                    break;
                case false:
                    NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                    NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                    NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight / 2 - 3;
                    break;
            }
        }
        #endregion

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if ((sender as Button).Content.ToString() == "OK")
        //    {
        //        #region Старое
        //        //DialogResult = true;
        //        //NewCell.InnerDataGrid.RowHeight = OldCell.InnerDataGrid.RowHeight - 2;
        //        //NewCell.CollectionOfItems = OldCell.CollectionOfItems;
        //        //NewCell.InnerDataGrid.Items.Refresh();
        //        #endregion
        //        Close();
        //        return;
        //    }
        //    try
        //    {
        //        NewCell.CollectionOfItems[NewCell.InnerDataGrid.Items.IndexOf(NewCell.InnerDataGrid.CurrentItem)] = new OneItem();
        //        cbDiscips.SelectedIndex =
        //            cbVidZanyatiy.SelectedIndex =
        //            cbKorpusa.SelectedIndex =
        //            cbAuditorii.SelectedIndex =
        //            cbPreps.SelectedIndex = -1;
        //            cbPodgruppa.SelectedIndex = -1;
        //        cbAuditorii.IsEnabled = false;
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Не выбрана ячейка для очистки", "Ошибка", MessageBoxButton.OK);
        //    }
        //    //NewCell.InnerDataGrid.Items.Refresh();
        //}
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            NewCell.CollectionOfItems[NewCell.InnerDataGrid.Items.IndexOf(NewCell.InnerDataGrid.CurrentItem)] = new OneItem();
        }


        #region
        private void RbGr1_Checked(object sender, RoutedEventArgs e)
        {
            NewCell.CollectionOfItems.Clear();
            Up.IsEnabled = Down.IsEnabled = (bool)!Single.IsChecked;
            Up.IsChecked = Down.IsChecked = false;
            switch (Single.IsChecked)
            {
                case true:
                    //NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                    NewCell.CollectionOfItems.Add(new OneItem ());
                    NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight;
                    NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[0];
                    break;
                case false:
                    //NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                    //NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                    NewCell.CollectionOfItems.Add(new OneItem ());
                    NewCell.CollectionOfItems.Add(new OneItem ());
                    NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight / 2 - 3;
                    break;
            }
        }
        #endregion

        private void Single_Checked(object sender, RoutedEventArgs e)
        {
            //NewCell.CollectionOfItems.Clear();
            cbDiscips.IsEnabled = 
                cbVidZanyatiy.IsEnabled = 
                cbKorpusa.IsEnabled = 
                cbPreps.IsEnabled = 
                cbPodgruppa.IsEnabled = true;
            Up.IsEnabled = Down.IsEnabled = false;
            Up.IsChecked = Down.IsChecked = false;
            cbAuditorii.IsEnabled = false;
            NewCell.InnerDataGrid.UnselectAll();
            #region
            //if (NewCell.CollectionOfItems.Count == 2)
            //{
            //    try
            //    {
            //        NewCell.CollectionOfItems.Remove(NewCell.CollectionOfItems.First((x) => x.AllAttr == ""));
            //    }
            //    catch
            //    {
            //        NewCell.CollectionOfItems.Remove(NewCell.CollectionOfItems[1]);
            //    }
            //    #region
            //    //NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight;
            //    //NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[0];
            //    //NewCell.InnerDataGrid.SelectedIndex = 0;
            //    #endregion
            //    return;
            //}
            //else 
            //    if(NewCell.CollectionOfItems.Count == 0)
            ////NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
            //NewCell.CollectionOfItems.Add(new OneItem ());
            //NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[0];
            #endregion
            switch (NewCell.CollectionOfItems.Count)
            {
                case 0:
                    NewCell.CollectionOfItems.Add(new OneItem());
                    
                    break;
                case 1:
                    try
                    {
                        cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[NewCell.CollectionOfItems[0].CodeDisc]);
                        #region Старое
                        //cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[0].CodeVid]);
                        //cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[0].CodePodgr;
                        //cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[0].NomerKorpusa]) - 1;
                        //cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[0].CodeAudit]);
                        //cbPreps.SelectedIndex = cbPreps.Items.IndexOf(Preps[NewCell.CollectionOfItems[0].CodePrep]);
                        #endregion
                    }
                    catch { }
                    try { cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[0].CodeVid]); }
                    catch { }
                    try { cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[0].CodePodgr; }
                    catch { }
                    try { cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[0].NomerKorpusa]) - 1; }
                    catch { }
                    try { cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[0].CodeAudit]); }
                    catch { }
                    try { cbPreps.SelectedIndex = cbPreps.Items.IndexOf(Preps[NewCell.CollectionOfItems[0].CodePrep]); }
                    catch { }
                    break;
                case 2:
                    try
                    {
                        NewCell.CollectionOfItems.Remove(NewCell.CollectionOfItems.First((x) => x.AllAttr == ""));
                    }
                    catch
                    {
                        NewCell.CollectionOfItems.Remove(NewCell.CollectionOfItems[1]);
                    }
                    #region
                    //NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight;
                    //NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[0];
                    //NewCell.InnerDataGrid.SelectedIndex = 0;
                    #endregion
                    break;
            }
            NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight;
            NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[0];
            NewCell.InnerDataGrid.SelectedIndex = 0;
            EditCell();
            #region
            //NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight;
            //NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[0];
            //NewCell.InnerDataGrid.SelectedIndex = 0;
            #endregion
        }

        private void Splited_Checked(object sender, RoutedEventArgs e)
        {
            //NewCell.CollectionOfItems.Clear();

            // еще посмотреть с дизейблом корпусов


            cbDiscips.IsEnabled = 
                cbVidZanyatiy.IsEnabled = 
                cbKorpusa.IsEnabled = 
                cbPreps.IsEnabled = 
                cbPodgruppa.IsEnabled =
                Up.IsEnabled = 
                Down.IsEnabled = true;
            cbAuditorii.IsEnabled = false;
            NewCell.InnerDataGrid.UnselectAll();
            //NewCell.InnerDataGrid.CurrentItem = null;
            NewCell.InnerDataGrid.RowHeight = NewCell.ActualHeight / 2 - 3;
            if ( NewCell.CollectionOfItems.Count == 0 )
            {
                //NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                //NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                NewCell.CollectionOfItems.Add(new OneItem ());
                NewCell.CollectionOfItems.Add(new OneItem ());
                Up.IsChecked = true;
                return;
            }
            if ( NewCell.CollectionOfItems.Count == 1 )
            {
                //NewCell.CollectionOfItems.Add(new OneItem { AllAttr = "ПО-15 / Высшая математика / 4.26" });
                NewCell.CollectionOfItems.Add(new OneItem ());
                Up.IsChecked = true;
                return;
            }
            Down.IsChecked = true;
            Up.IsChecked = true;

            EditCell();
            #region Старое
            //cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[NewCell.CollectionOfItems[0].CodeDisc]);
            //cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[0].CodeVid]);
            //cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[0].CodeGr;
            //cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[0].NomerKorpusa]) - 1;
            //cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[0].CodeAudit]);
            //cbPreps.SelectedIndex = cbPreps.Items.IndexOf(Preps[NewCell.CollectionOfItems[0].CodePrep]);
            #endregion
        }

        private void Up_Checked(object sender, RoutedEventArgs e)
        {
            NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[0];
            NewCell.InnerDataGrid.SelectedIndex = 0;

            try { cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[NewCell.CollectionOfItems[0].CodeDisc]); }
            catch { cbDiscips.SelectedIndex = -1; }

            try { cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[0].CodeVid]); }
            catch { cbVidZanyatiy.SelectedIndex = -1; }

            try { cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[0].CodePodgr; }
            catch { cbPodgruppa.SelectedIndex = -1; }

            try { cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[0].NomerKorpusa]) - 1; }
            catch { cbKorpusa.SelectedIndex = -1; }

            try { cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[0].CodeAudit]); }
            catch { cbAuditorii.SelectedIndex = -1; }

            try
            {
                //изменять выбранный элемент на тот, что в ячейке +
                // придумать, как получать не каррент, а тот, который выбрался +
                #region Старое
                //cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[(NewCell.InnerDataGrid.CurrentItem as OneItem).CodeDisc]);
                //cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[NewCell.CollectionOfItems[0].CodeDisc]);
                //cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[0].CodeVid]);
                //cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[0].CodePodgr;
                //cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[0].NomerKorpusa]) - 1;
                //cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[0].CodeAudit]);
                #endregion
                cbPreps.SelectedIndex = cbPreps.Items.IndexOf(Preps[NewCell.CollectionOfItems[0].CodePrep]);
            }
            catch
            {
                #region Старое
                //cbDiscips.SelectedIndex =
                //cbVidZanyatiy.SelectedIndex =
                //cbKorpusa.SelectedIndex =
                //cbAuditorii.SelectedIndex =
                #endregion
                cbPreps.SelectedIndex = -1;
                //cbPodgruppa.SelectedIndex = 0;
            }
            EditCell();
        }

        private void Down_Checked(object sender, RoutedEventArgs e)
        {
            //изменять выбранный элемент на тот, что в ячейке

            NewCell.InnerDataGrid.CurrentItem = NewCell.InnerDataGrid.Items[1];
            NewCell.InnerDataGrid.SelectedIndex = 1;
            try { cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[NewCell.CollectionOfItems[1].CodeDisc]); }
            catch { cbDiscips.SelectedIndex = -1; }

            try { cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[1].CodeVid]); }
            catch { cbVidZanyatiy.SelectedIndex = -1; }

            try { cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[1].CodePodgr; }
            catch { cbPodgruppa.SelectedIndex = -1; }

            try { cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[1].NomerKorpusa]) - 1; }
            catch { cbKorpusa.SelectedIndex = -1; }

            try { cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[1].CodeAudit]); }
            catch { cbAuditorii.SelectedIndex = -1; }

            try
            {
                #region Старое
                //cbDiscips.SelectedIndex = cbDiscips.Items.IndexOf(Discips[NewCell.CollectionOfItems[1].CodeDisc]);
                //cbVidZanyatiy.SelectedIndex = cbVidZanyatiy.Items.IndexOf(VidiZanyatiy[NewCell.CollectionOfItems[1].CodeVid]);
                //cbPodgruppa.SelectedIndex = NewCell.CollectionOfItems[1].CodePodgr;
                //cbKorpusa.SelectedIndex = cbKorpusa.Items.IndexOf(Korpusa[NewCell.CollectionOfItems[1].NomerKorpusa]) - 1;
                //cbAuditorii.SelectedIndex = cbAuditorii.Items.IndexOf(Auditorii[NewCell.CollectionOfItems[1].CodeAudit]);
                #endregion 
                cbPreps.SelectedIndex = cbPreps.Items.IndexOf(Preps[NewCell.CollectionOfItems[1].CodePrep]);
            }
            catch
            {
                #region старое
                //cbDiscips.SelectedIndex =
                //cbVidZanyatiy.SelectedIndex =
                //cbKorpusa.SelectedIndex =
                //cbAuditorii.SelectedIndex =
                #endregion
                cbPreps.SelectedIndex = -1;
                //cbPodgruppa.SelectedIndex = 0;
            }
            EditCell();
        }

        private void CbDiscips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // вот это со всех хреней вынести в отдельный метод для записи данных +
                //(NewCell.InnerDataGrid.CurrentItem as OneItem).CodeDisc = Discips.FirstOrDefault(x => x.Value == e.AddedItems[0] as string).Key;

                // список преподавателей по дисциплине -- пока реализовать невозможно
                EditCell();
            }
            catch(Exception exc)
            {
                //MessageBox.Show("Не выбран вид недели", "Ошибка", MessageBoxButton.OK);
            }
        }

        private void CbVidZanyatiy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //(NewCell.InnerDataGrid.CurrentItem as OneItem).CodeVid = VidiZanyatiy.FirstOrDefault(x => x.Value == e.AddedItems[0] as string).Key;
                EditCell();
            }
            catch { }
        }

        private void CbPodgruppa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // обработка индекса -1
            try
            {
                //(NewCell.InnerDataGrid.CurrentItem as OneItem).CodePodgr = cbPodgruppa.SelectedIndex;
                
                EditCell();
            }
            catch { }
        }
        
        private void CbKorpusa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //NewCell.id_aud = .FirstOrDefault(x => x.Value == e.AddedItems[0] as string).Key;
            //добавляем список аудиторий
            try
            {
                // (NewCell.InnerDataGrid.CurrentItem as OneItem).NomerKorpusa = Convert.ToInt32(cbKorpusa.SelectedItem);
                using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetAuditorii, Db.myConnection))
                {
                    SC.Parameters.AddWithValue("@aud_korpus", Convert.ToInt32(e.AddedItems[0]));
                    Auditorii.Clear();
                    using (SqlDataReader SDR = SC.ExecuteReader())
                        while (SDR.Read())
                            Auditorii.Add(Convert.ToInt32(SDR["id_aud"]), SDR["AUD_NOMER"].ToString());
                    cbAuditorii.ItemsSource = Auditorii.Values;
                    cbAuditorii.Items.Refresh();
                }
                cbAuditorii.IsEnabled = true;
            }
            catch { }
        }

        private void CbAuditorii_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // обработка индекса -1
            try
            {
                //(NewCell.InnerDataGrid.CurrentItem as OneItem).CodeAudit = Auditorii.FirstOrDefault(x => x.Value == e.AddedItems[0] as string).Key;
                EditCell();
            }
            catch { }
        }

        private void CbPreps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //(NewCell.InnerDataGrid.CurrentItem as OneItem).CodePrep = Preps.FirstOrDefault(x => x.Value == e.AddedItems[0] as string).Key;
                EditCell();
            }
            catch { }
        }

        /// <summary>
        /// Метод, который переписывает содержимое выбранной ячейки
        /// </summary>
        public void EditCell()
        {
            (NewCell.InnerDataGrid.CurrentItem as OneItem).AllAttr = "";
            if (cbPodgruppa.SelectedIndex > 0)
                (NewCell.InnerDataGrid.CurrentItem as OneItem).AllAttr += string.Format("{0} подгруппа: ", cbPodgruppa.SelectedItem);
            if (cbDiscips.SelectedIndex != -1)
            {// выбор короткого имени дисциплины
                //(NewCell.InnerDataGrid.CurrentItem as OneItem).CodeDisc
                using (SqlCommand SC = new SqlCommand())
                {
                    SC.Connection = Db.myConnection;
                    SC.CommandText = "select snam_rus from DONNTU.dbo.DISCIP " +
                        "where cod_disc = @arg";
                    SC.Parameters.AddWithValue("@arg", Discips.FirstOrDefault( x => x.Value == cbDiscips.SelectedItem as string).Key);
                    using (SqlDataReader SDR = SC.ExecuteReader())
                    {
                        SDR.Read();
                        (NewCell.InnerDataGrid.CurrentItem as OneItem).AllAttr += string.Format("{0} ", SDR.GetString(0));
                    }
                }
            }
            if (cbVidZanyatiy.SelectedIndex != -1)
                (NewCell.InnerDataGrid.CurrentItem as OneItem).AllAttr += string.Format("- {0} ",cbVidZanyatiy.SelectedItem.ToString());
            if (cbAuditorii.SelectedIndex != -1)
                (NewCell.InnerDataGrid.CurrentItem as OneItem).AllAttr += string.Format("({0}.{1}) ",cbKorpusa.SelectedItem, cbAuditorii.SelectedItem);
            if (cbPreps.SelectedIndex != -1)
                (NewCell.InnerDataGrid.CurrentItem as OneItem).AllAttr += cbPreps.SelectedItem.ToString();
            NewCell.InnerDataGrid.CommitEdit();
            NewCell.InnerDataGrid.Items.Refresh();

        }
        public void EditCell(OneItem item)
        {

        }

        // событие для живого поиска в комбобоксах c использованием фильтра
        // не используется, т.к. WPF -- дерьмо 
        private void Cb_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ComboBox bufcb = (sender as TextBox).TemplatedParent as ComboBox;
                //bufcb.IsDropDownOpen = true;
                TextBox buftb = e.OriginalSource as TextBox;
                buftb.Select(buftb.SelectionStart + buftb.SelectionLength, 0);
                CollectionView CV = (CollectionView)CollectionViewSource.GetDefaultView(bufcb.ItemsSource);
                CV.Filter = s =>
                    ((string)s).IndexOf(bufcb.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
            catch { }
        }

        private void CbDiscips_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox buf = sender as ComboBox;
            #region
            //try
            //{
            //    TextBox TB = buf.Template.FindName("PART_EditableTextBox", buf) as TextBox;
            //    TB.TextChanged += Cb_TextChanged;
            //    TB.Focus();
            //    TB.SelectionStart = 0;
            //}
            //catch
            //{

            //}
            #endregion
        }

        private void CbDropDownClosed(object sender, EventArgs e)
        {
            ComboBox buf = sender as ComboBox;
            buf.IsEditable = false;
            #region
            //try
            //{
            //    TextBox TB = buf.Template.FindName("PART_EditableTextBox", buf) as TextBox;
            //    TB.TextChanged -= Cb_TextChanged;
            //    CollectionView CV = (CollectionView)CollectionViewSource.GetDefaultView(buf.ItemsSource);
            //    CV.Filter = null;

            //    buf.Focus();
            //}
            //catch(Exception exc)
            //{

            //}
            #endregion
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OneItem buf = NewCell.InnerDataGrid.CurrentItem as OneItem;
            try { cbDiscips.SelectedItem = Discips[buf.CodeDisc]; }
            catch { cbDiscips.SelectedIndex = -1; }
            try
            { cbVidZanyatiy.SelectedItem = VidiZanyatiy[buf.CodeVid]; }
            catch { cbVidZanyatiy.SelectedIndex = -1; }
            try
            { cbPodgruppa.SelectedIndex = buf.CodePodgr; }
            catch { cbPodgruppa.SelectedIndex = -1; }
            try
            {
                cbKorpusa.SelectedIndex = Korpusa.IndexOf(Korpusa.FirstOrDefault(x => x == buf.NomerKorpusa));
            }
            catch { cbKorpusa.SelectedIndex = -1; }
            try
            { cbAuditorii.SelectedItem = Auditorii[buf.CodeAudit]; }
            catch { cbAuditorii.SelectedIndex = -1; }
            try
            { cbPreps.SelectedItem = Preps[buf.CodePrep]; }
            catch { cbPreps.SelectedIndex = -1; }
        }

        private void BtnSaveExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Сохранить изменения?", "Добавление дисциплины", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                Close();
                return;
            }
            OneItem buf = NewCell.InnerDataGrid.CurrentItem as OneItem;
            try
            {
                buf.CodeDisc = Discips.FirstOrDefault(x => x.Value == cbDiscips.SelectedItem as string).Key;
                buf.CodeVid = VidiZanyatiy.FirstOrDefault(x => x.Value == cbVidZanyatiy.SelectedItem as string).Key;
                buf.CodePodgr = cbPodgruppa.SelectedIndex;
                buf.NomerKorpusa = Convert.ToInt32(cbKorpusa.SelectedItem);
                buf.CodeAudit = Auditorii.FirstOrDefault(x => x.Value == cbAuditorii.SelectedItem as string).Key;
                buf.CodePrep = Preps.FirstOrDefault(x => x.Value == cbPreps.SelectedItem as string).Key;
                EditCell();
            }
            catch(Exception exc)
            {
                MessageBox.Show("Введены не все данные", "Ошибка", MessageBoxButton.OK);
            }
            Close();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Очистить выбранную ячейку?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            try
            {
                NewCell.CollectionOfItems[NewCell.InnerDataGrid.Items.IndexOf(NewCell.InnerDataGrid.CurrentItem)] = new OneItem();
                cbDiscips.SelectedIndex =
                    cbVidZanyatiy.SelectedIndex =
                    cbKorpusa.SelectedIndex =
                    cbAuditorii.SelectedIndex =
                    cbPreps.SelectedIndex = -1;
                cbPodgruppa.SelectedIndex = -1;
                cbAuditorii.IsEnabled = false;
            }
            catch
            {
                MessageBox.Show("Не выбрана ячейка для очистки", "Ошибка", MessageBoxButton.OK);
            }
        }
    }
}
