using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace Raspisanie2019
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<myTable> ListOfTables = new List<myTable>(0);
        int id_rasp_year;
        myCell bufCell;
        public MainWindow()
        {
            InitializeComponent();
            LoginWindow lw = new LoginWindow();
            lw.ShowDialog();
            //GC.Collect();
        }

        
        private void NewRasp_Click(object sender, RoutedEventArgs e)
        {
            Create_Open CO = new Create_Open();
            if (CO.ShowDialog() == false)
                return;
            //Считывание данных с формы и занесение в новую форму таблицы

            myTable NewTab = new myTable(
                CO.cbGruppa.SelectedItem as string,
                CO.cbFak.SelectedItem as string,
                CO.kod_ft,
                CO.god_nabora,
                CO.vid_podgotovki,
                CO.prog_spec,
                Convert.ToInt32(CO.tbYears.Text),
                Convert.ToBoolean(CO.rbOsenniy.IsChecked),
                CO.gruppa,
                CO.id_rasp_year);

            // если расписание для выбранной группы уже существует, то добавляем его в таблицу
            if (CO.DialogResult == true && CO.flag)
            {
                using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetFromRaspDiscipSpDayVWRaspTime, Db.myConnection))
                {
                    myCell buf;
                    // считывать по прототипу или нет
                    if (CO.prot_id_rasp_year != 0)
                        SC.Parameters.AddWithValue("@id_rasp_year", CO.prot_id_rasp_year);
                    else
                        SC.Parameters.AddWithValue("@id_rasp_year", CO.id_rasp_year);
                    using (SqlDataReader SDR = SC.ExecuteReader())
                    {
                        while (SDR.Read())
                        {
                            #region 
                            //using(SqlCommand innerSC = new SqlCommand(Db.SQLCommands.GetDay,Db.myConnection))
                            //{
                            //    innerSC.Parameters.AddWithValue("@id_ned",SDR["d_ned"]);
                            //    SqlDataReader innerSDR = SC.ExecuteReader();
                            //    innerSDR.Read();
                            //    buf = NewTab.myLessons.First((x) => x.Day == innerSDR.GetString(0));
                            //}
                            #endregion
                            buf = NewTab.myLessons.First((x) => x.Day == SDR["name"].ToString())[Convert.ToInt32(SDR["Nom"]) - 1];
                            if (Convert.ToInt32(SDR["pr_ned"]) == 0)
                            {
                                buf.CollectionOfItems.Add(
                                    new OneItem(Convert.ToInt32(SDR["k_discip"]),
                                    Convert.ToInt32(SDR["id_nagruzka_vid"]),
                                    Convert.ToInt32(SDR["id_aud"]),
                                    Convert.ToInt32(SDR["TABN_S"]),
                                    Convert.ToInt32(SDR["podgruppa"])));
                                continue;
                            }
                            if ((Convert.ToInt32(SDR["pr_ned"]) == 1 ||
                                Convert.ToInt32(SDR["pr_ned"]) == 2) && buf.CollectionOfItems.Count == 0)
                            {
                                buf.InnerDataGrid.RowHeight = 45.5;
                                buf.CollectionOfItems.Add(new OneItem());
                                buf.CollectionOfItems.Add(new OneItem());
                            }
                            buf.CollectionOfItems[Convert.ToInt32(SDR["pr_ned"]) - 1] =
                                new OneItem(Convert.ToInt32(SDR["k_discip"]),
                                    Convert.ToInt32(SDR["id_nagruzka_vid"]),
                                    Convert.ToInt32(SDR["id_aud"]),
                                    Convert.ToInt32(SDR["TABN_S"]),
                                    Convert.ToInt32(SDR["podgruppa"]));
                        }
                        foreach (Cells temp in NewTab.myDataGrid.Items)
                            for (int i = 0; i < 5; i++)
                            {
                                foreach (OneItem innertemp in (temp[i] as myCell).InnerDataGrid.Items)
                                    innertemp.DrawItem();
                                (temp[i] as myCell).InnerDataGrid.Items.Refresh();
                            }
                    }
                    //NewTab.myLessons
                }

            }

            ListOfTables.Add(NewTab);
            MenuItem menuItem = new MenuItem() { Header = CO.cbGruppa.SelectedItem };
            menuItem.Click += new RoutedEventHandler(OpenWin);
            OpenedWins.Items.Add(menuItem);

            Button NewTabButton = new Button
            {
                Template = (ControlTemplate)Resources["IconForButtonTab"],
                Margin = new Thickness(0, 0, -13, 0),
                Content = CO.cbGruppa.SelectedItem,
            };
            NewTabButton.Click += new RoutedEventHandler(OpenWin);
            MenuOfTabs.Children.Insert(MenuOfTabs.Children.Count, NewTabButton);
            OpenWin(menuItem);
            CO = null;
        }

        void OpenWin(object sender, RoutedEventArgs e)
        {
            #region
            //OwnedWindows[OpenedWins.Items.IndexOf(sender as MenuItem)].WindowState = WindowState.Normal;
            // WindowsContent.Content = ListOfTables[OpenedWins.Items.IndexOf(sender as MenuItem)];
            // foreach (MenuItem temp in OpenedWins.Items)
            //     temp.Icon = null;
            //(sender as MenuItem).Icon = "+";
            #endregion
            OpenWin(sender);
        }

        void OpenWin(object item)
        {
            int num = -1;
            foreach (MenuItem temp in OpenedWins.Items)
                temp.Icon = null;
            for (int i = 1; i < MenuOfTabs.Children.Count; i++)
                MenuOfTabs.Children[i].Opacity = 0.8;
            switch (item.GetType().Name)
            {
                case "MenuItem":
                    num = OpenedWins.Items.IndexOf((MenuItem)item);
                    WindowsContent.Content = ListOfTables[num];
                    break;
                case "Button":
                    num = MenuOfTabs.Children.IndexOf((Button)item) - 1;
                    WindowsContent.Content = ListOfTables[num];
                    break;
            }
            Title = string.Format("Расписание занятий {0}, гр.{1}, уч.год. - {2}, {3} семестр",
                (WindowsContent.Content as myTable).FAK,
                (WindowsContent.Content as myTable).gruppa,
                (WindowsContent.Content as myTable).uch_god,
                ((WindowsContent.Content as myTable).nomer_semestra % 2 == 0) ? "Весенний" : "Осенний");
            (OpenedWins.Items[num] as MenuItem).Icon = "+";
            MenuOfTabs.Children[num + 1].Opacity = 1;
        }


        private void MyBtnAddTable_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Button).Opacity = 0.8;
        }

        private void MyBtnAddTable_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Button).Opacity = 1;
        }

        private void MenuItem_Click_Clear(object sender, RoutedEventArgs e)
        {
            try
            {
                myTable buf = WindowsContent.Content as myTable;
                if (MessageBox.Show("Очистить ячейку?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
                foreach (var temp in buf.myDataGrid.SelectedCells)
                    buf.myLessons[buf.myDataGrid.Items.IndexOf(temp.Item)][temp.Column.DisplayIndex] = new myCell();
                buf.myDataGrid.CommitEdit();
                buf.myDataGrid.Items.Refresh();
            }
            catch (Exception exc)
            {
                if (exc.HResult == -2147467261)
                    MessageBox.Show("Нет расписания", "Ошибка", MessageBoxButton.OK);
            }
        }

        private void MenuItem_MouseLeftButtonDown_Add_Dis(object sender, MouseButtonEventArgs e)
        {
            //(WindowsContent as myTable).ContentControl_MouseDoubleClick(sender, e);
        }

        private void MenuItem_Click_Add_Dis(object sender, RoutedEventArgs e)
        {
            try
            {
                (WindowsContent.Content as myTable).ContentControl_MouseDoubleClick(sender, null);
            }
            catch(Exception exc)
            {
                if (exc.HResult == -2147467261)
                {
                    MessageBox.Show("Нет расписания", "Ошибка", MessageBoxButton.OK);
                }
                //
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (WindowsContent.Content as myTable).Save();
            }
            catch (Exception exc)
            {
                if (exc.HResult == -2147467261)
                {
                    MessageBox.Show("Нет расписания", "Ошибка", MessageBoxButton.OK);
                }
                //
            }
        }
        

        private void CommandBinding_Executed_Save(object sender, ExecutedRoutedEventArgs e)
        {
            (WindowsContent.Content as myTable).Save();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            // посмотреть что в нем будет
            //e.OriginalSource
            // если не будет ячейки, то найти способ достать данные из выбранной ячейки 
            // DataGridCell
            // DataGridCellInfo
            try
            {
                bufCell = (WindowsContent.Content as myTable).myDataGrid.CurrentCell.Item as myCell;
            }
            catch (Exception exc)
            {
                if (exc.HResult == -2147467261)
                {
                    MessageBox.Show("Нет расписания", "Ошибка", MessageBoxButton.OK);
                }
                //
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            //exlapp = new Microsoft.Office.Interop.Excel.Application();
            {
                int RowOffset = 6;
                int ColOffset = 2;
                dynamic ExcelApp = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application"));
                // сделать шаблон в папке Shabl
                dynamic Book = ExcelApp.Workbooks.Open(System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"ShablSprav\raspm.xlt"));
                dynamic Sheet = ExcelApp.Worksheets[1];
                // метод работает по принципу "Найти и заменить" в Excel
                // первый аргумент -- что надо заменить
                // второй аргумент -- чем заменить
                // третий аргумент -- аналог галочки "ячейка целиком"
                // четвертый аргумент -- аналог списка "Просматривать:"
                // пятый агрумент -- как я понял, это "учитывать регистр" 
                // 6,7 -- форматы
                // изменяем группу
                ExcelApp.Cells.Replace(
                    "$group$",
                    (WindowsContent.Content as myTable).gruppa,
                    Excel.XlLookAt.xlPart,
                    Excel.XlSearchOrder.xlByRows,
                    false,
                    true,
                    false);
                // изменяем учебный год
                ExcelApp.Cells.Replace(
                    "$year$",
                    (WindowsContent.Content as myTable).uch_god,
                    Excel.XlLookAt.xlPart,
                    Excel.XlSearchOrder.xlByRows,
                    false,
                    true,
                    false);
                // изменяем семестр
                ExcelApp.Cells.Replace(
                    "$sem$",
                    ((WindowsContent.Content as myTable).nomer_semestra % 2 == 0) ? "Весенний" : "Осенний",
                    Excel.XlLookAt.xlPart,
                    Excel.XlSearchOrder.xlByRows,
                    false,
                    true,
                    false);
                // изменяем тип недели. Пока только любая
                ExcelApp.Cells.Replace(
                    "$week$",
                    "Любая",
                    Excel.XlLookAt.xlPart,
                    Excel.XlSearchOrder.xlByRows,
                    false,
                    true,
                    false);

                //rangeB1[]
                foreach (var outtemp in (WindowsContent.Content as myTable).myLessons)
                    for (int i = 0; i < 5; i++)
                    {
                        if (outtemp[i].CollectionOfItems.Count == 0)
                            continue;
                        if (outtemp[i].CollectionOfItems.Count == 1)
                        {
                            // очищаем, чтоб не предупреждало при объединении
                            Sheet.Range[
                                  Sheet.Cells[RowOffset + (WindowsContent.Content as myTable).myLessons.IndexOf(outtemp) * 2,
                                  ColOffset + i],
                                  Sheet.Cells[RowOffset + (WindowsContent.Content as myTable).myLessons.IndexOf(outtemp) * 2 + 1,
                                  ColOffset + i]].ClearContents();
                            // объединяем
                            Sheet.Range[
                                  Sheet.Cells[RowOffset + (WindowsContent.Content as myTable).myLessons.IndexOf(outtemp) * 2,
                                  ColOffset + i],
                                  Sheet.Cells[RowOffset + (WindowsContent.Content as myTable).myLessons.IndexOf(outtemp) * 2 + 1,
                                  ColOffset + i]].Merge();
                            // заносим занчение
                            Sheet.Cells[RowOffset + (WindowsContent.Content as myTable).myLessons.IndexOf(outtemp) * 2,
                                ColOffset + i].Value = outtemp[i].CollectionOfItems[0].AllAttr;
                            continue;
                        }
                        if (outtemp[i].CollectionOfItems.Count == 2)
                        {
                            if (outtemp[i].CollectionOfItems[0].AllAttr != "")
                                Sheet.Cells[RowOffset + (WindowsContent.Content as myTable).myLessons.IndexOf(outtemp) * 2,
                                      ColOffset + i].Value = outtemp[i].CollectionOfItems[0].AllAttr;
                            if (outtemp[i].CollectionOfItems[1].AllAttr != "")
                                Sheet.Cells[RowOffset + (WindowsContent.Content as myTable).myLessons.IndexOf(outtemp) * 2 + 1,
                                    ColOffset + i].Value = outtemp[i].CollectionOfItems[1].AllAttr;
                        }
                    }
                ExcelApp.Visible = true;
            }
            catch(Exception exc)
            {
                if (exc.HResult == -2147467261)
                {
                    MessageBox.Show("Нет расписания для печати", "Ошибка", MessageBoxButton.OK);
                    return;
                }
                
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Open_Edit OE = new Open_Edit();
            myTable NewTab = null;
            if (OE.ShowDialog() == false)
                return;
            // считать id расписания
            id_rasp_year = OE.id_rasp_year;
            // считываем данные расписания и заносим в новую таблицу 
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetRasps, Db.myConnection))
            {
                SC.CommandText += Db.SQLCommands.Params.ForGetRasps.id;
                SC.Parameters.AddWithValue("@id", id_rasp_year);
                using (SqlDataReader SDR = SC.ExecuteReader())
                {
                    if (SDR.Read())
                        NewTab = new myTable(
                          SDR["name_grup"] as string,
                          SDR["fak_name"] as string,
                          Convert.ToInt32(SDR["k_ft"]),
                          Convert.ToInt32(SDR["god_nabora"]),
                          Convert.ToInt32(SDR["vid_podg"]),
                          Convert.ToInt32(SDR["kod_spec"]),
                          Convert.ToInt32(SDR["uch_god"]),
                          !Convert.ToBoolean(SDR["n_sem"]),
                          Convert.ToInt32(SDR["kod_grup"]),
                          id_rasp_year);
                    else
                    {
                        MessageBox.Show("Расписание не найдено", "Уведомление", MessageBoxButton.OK);
                    }

                }
            }


            // записываем расписание в таблицу
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetFromRaspDiscipSpDayVWRaspTime, Db.myConnection))
            {
                myCell buf;
                SC.Parameters.AddWithValue("@id_rasp_year", id_rasp_year);
                using (SqlDataReader SDR = SC.ExecuteReader())
                {
                    while (SDR.Read())
                    {
                        #region 
                        //using(SqlCommand innerSC = new SqlCommand(Db.SQLCommands.GetDay,Db.myConnection))
                        //{
                        //    innerSC.Parameters.AddWithValue("@id_ned",SDR["d_ned"]);
                        //    SqlDataReader innerSDR = SC.ExecuteReader();
                        //    innerSDR.Read();
                        //    buf = NewTab.myLessons.First((x) => x.Day == innerSDR.GetString(0));
                        //}
                        #endregion
                        buf = NewTab.myLessons.First((x) => x.Day == SDR["day"].ToString())[Convert.ToInt32(SDR["Nom"]) - 1];
                        if (Convert.ToInt32(SDR["pr_ned"]) == 0)
                        {
                            buf.CollectionOfItems.Add(
                                new OneItem(Convert.ToInt32(SDR["k_discip"]),
                                Convert.ToInt32(SDR["id_nagruzka_vid"]),
                                Convert.ToInt32(SDR["id_aud"]),
                                Convert.ToInt32(SDR["TABN_S"]),
                                Convert.ToInt32(SDR["podgruppa"])));
                            continue;
                        }
                        if ((Convert.ToInt32(SDR["pr_ned"]) == 1 ||
                            Convert.ToInt32(SDR["pr_ned"]) == 2) && buf.CollectionOfItems.Count == 0)
                        {
                            buf.InnerDataGrid.RowHeight = 45.5;
                            buf.CollectionOfItems.Add(new OneItem());
                            buf.CollectionOfItems.Add(new OneItem());
                        }
                        buf.CollectionOfItems[Convert.ToInt32(SDR["pr_ned"]) - 1] =
                            new OneItem(Convert.ToInt32(SDR["k_discip"]),
                                Convert.ToInt32(SDR["id_nagruzka_vid"]),
                                Convert.ToInt32(SDR["id_aud"]),
                                Convert.ToInt32(SDR["TABN_S"]),
                                Convert.ToInt32(SDR["podgruppa"]));
                    }
                    foreach (Cells temp in NewTab.myDataGrid.Items)
                        for (int i = 0; i < 5; i++)
                        {
                            foreach (OneItem innertemp in (temp[i] as myCell).InnerDataGrid.Items)
                                innertemp.DrawItem();
                            (temp[i] as myCell).InnerDataGrid.Items.Refresh();
                        }
                }
            }
            ListOfTables.Add(NewTab);
            MenuItem menuItem = new MenuItem() { Header = NewTab.gruppa };
            menuItem.Click += new RoutedEventHandler(OpenWin);
            OpenedWins.Items.Add(menuItem);

            Button NewTabButton = new Button
            {
                Template = (ControlTemplate)Resources["IconForButtonTab"],
                Margin = new Thickness(0, 0, -13, 0),
                Content = NewTab.gruppa,
            };
            NewTabButton.Click += new RoutedEventHandler(OpenWin);
            MenuOfTabs.Children.Insert(MenuOfTabs.Children.Count, NewTabButton);
            OpenWin(menuItem);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Приложение для рабочего стола RASP_YEAR.exe. Разработал Симак А.С.\n" +
                "Движок расписания на сайте АСУ ДонНТУ. Разработал Овчинников Д.А.\n" +
                "\t\tЦИКТ 2019",
                "О приложении", MessageBoxButton.OK);
        }

        private void Insrtuction_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"ShablSprav\Инструкция.docx"));
        }
    }
    //public class OneDay
    //{
    //    public string Day { get; set; }
    //    public string FirstPair { get; set; }
    //    public string SecondPair { get; set; }
    //    public string ThirdPair { get; set; }
    //    public string FourthPare { get; set; }
    //    public string FifthPair { get; set; }
    //}
      
}
