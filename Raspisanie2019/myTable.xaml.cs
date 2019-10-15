using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Raspisanie2019
{
    /// <summary>
    /// Логика взаимодействия для myTable.xaml
    /// </summary>
    public partial class myTable : UserControl
    {
        myCell ClipBoard;
        public int k_fak;
        public string FAK;
        public int god_nabora;
        public int vid_podg;
        public int spec;
        public int npp_g;
        public int nomer_semestra;
        public int kod_grup;
        public int uch_god;
        public int kurs;
        public int id_rasp_year;
        public string gruppa;
        /// <summary>
        /// true -- расписание уже зарегистрировано
        /// false -- расписание еще не зарегистрировано
        /// </summary>
        public bool flag = false;
        public ObservableCollection<Cells> myLessons = new ObservableCollection<Cells>();
        public Dictionary<int, string> Discips = new Dictionary<int, string>(0);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gruppa"> название группы </param>
        /// <param name="FAK"> название факультета </param>
        /// <param name="k_fak"> код факультета </param>
        /// <param name="god_nabora"> год набора группы </param>
        /// <param name="vid_podg"> вид подготовки </param>
        /// <param name="spec"> код специальности(профиля) </param>
        /// <param name="uch_god"> номер учебного года</param>
        /// <param name="semestr"> номер семестра </param>
        /// <param name="kod_grup"> код группы </param>
        /// <param name="id_rasp_year"> идентификатор записи в RASP_YEAR</param>
        public myTable(string gruppa, string FAK, int k_fak, int god_nabora, int vid_podg, int spec, int uch_god, bool semestr, int kod_grup, int id_rasp_year = 0)
        {
            InitializeComponent();
            //myDataGrid.CommandBindings.Remove(new CommandBinding(ApplicationCommands.Copy));
            this.gruppa = gruppa;
            this.FAK = FAK;
            this.k_fak = k_fak;
            this.god_nabora = god_nabora;
            this.vid_podg = vid_podg;
            this.spec = spec;
            this.kod_grup = kod_grup;
            this.uch_god = uch_god;
            this.id_rasp_year = id_rasp_year;
            //this.flag = flag;
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.GetNPPG, Db.myConnection))
            {
                SC.Parameters.AddWithValue("@k_fak",k_fak);
                SC.Parameters.AddWithValue("@god_nabora", god_nabora);
                SC.Parameters.AddWithValue("@vid_podg", vid_podg);
                SC.Parameters.AddWithValue("@spec", spec);
                using (SqlDataReader SDR = SC.ExecuteReader())
                {
                    SDR.Read();
                    npp_g = SDR.GetInt32(0);
                }
            }
            //если semestr == true, то осенний
            nomer_semestra = 
                semestr ? 
                (uch_god - god_nabora) * 2 + 1 
                : (uch_god - god_nabora + 1) * 2;
            kurs = Convert.ToInt32(Math.Round((double)nomer_semestra / 2, MidpointRounding.AwayFromZero));
            //выбор дисциплин 
            using(SqlCommand SC = new SqlCommand(Db.SQLCommands.GetDiscips,Db.myConnection))
            {
                SC.Parameters.AddWithValue("@npp_g",npp_g);
                SC.Parameters.AddWithValue("@k_fak",k_fak);
                SC.Parameters.AddWithValue("@nomer_semestra",nomer_semestra);
                using (SqlDataReader SDR = SC.ExecuteReader())
                {
                    while (SDR.Read())
                        Discips.Add(Convert.ToInt32(SDR["k_discip"]),
                            string.Format("{0} ({1},{2},{3},{4}) - {5}",
                            SDR["name_rus"].ToString(),
                            (Convert.ToInt32(SDR["lekcii"]) != 0) ? "л." : "",
                            (Convert.ToInt32(SDR["prakt"]) != 0) ? "пр." : "",
                            (Convert.ToInt32(SDR["laborat"]) != 0) ? "лб." : "",
                            (Convert.ToInt32(SDR["k_proekt"]) != 0 ||
                            Convert.ToInt32(SDR["k_rab"]) != 0) ? "кп/р" : "",
                            Convert.ToInt32(SDR["k_discip"]))
                            );
                            //SDR["snam_rus"].ToString());
                        //залить куда-то предметы, чтоб отправлять их в форму выбора дисциплин +
                }
            }
            myLessons.Add(new Cells("Понедельник"));
            myLessons.Add(new Cells("Вторник"));
            myLessons.Add(new Cells("Среда"));
            myLessons.Add(new Cells("Четверг"));
            myLessons.Add(new Cells("Пятница"));
            myLessons.Add(new Cells("Суббота"));
            
            //myLessons[0].FirstPair.myGridCell.Background = Brushes.Black;
            myDataGrid.ItemsSource = myLessons;
        }

        public void Copy()
        {
            
        }

        public void Save()
        {
            // сохраняем регистрационные данные расписания, если оно не существует
            if(id_rasp_year == 0)
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.InsertRegData, Db.myConnection))
            {
                SC.Parameters.AddWithValue("@kod_grup",kod_grup);
                SC.Parameters.AddWithValue("@uch_god",uch_god);
                SC.Parameters.AddWithValue("@n_sem", (nomer_semestra + 1) % 2);
                SC.Parameters.AddWithValue("@vid_podg",vid_podg);
                SC.Parameters.AddWithValue("@kurs",kurs);
                SC.Parameters.AddWithValue("@npp_g",npp_g);
                SC.Parameters.AddWithValue("@k_ft",k_fak);
                int c = SC.ExecuteNonQuery();
                SC.CommandText = Db.SQLCommands.GetIdRaspYear;
                using (SqlDataReader SDR = SC.ExecuteReader())
                {
                    SDR.Read();
                    id_rasp_year = Convert.ToInt32(SDR["id"]);
                }
            }
            else
                // иначе удаляем все данные расписания из базы по id
                using (SqlCommand SC = new SqlCommand(Db.SQLCommands.DeleteAll, Db.myConnection))
                {
                    SC.Parameters.AddWithValue("@id_rasp_year", id_rasp_year);
                    int c = SC.ExecuteNonQuery();
                }

            // сохраняем данные расписания
            using (SqlCommand SC = new SqlCommand(Db.SQLCommands.InsertToRaspDiscip, Db.myConnection))
            {
                // заливаем параметры без значений, чтоб их можно было изменить
                // id_rasp_year у всех одинаковый, поэтому заливаем значение сейчас
                SC.Parameters.AddWithValue("@id_rasp_year", id_rasp_year);
                SC.Parameters.AddWithValue("@d_ned", null);
                SC.Parameters.AddWithValue("@pr_ned", null);
                SC.Parameters.AddWithValue("@i", null);
                SC.Parameters.AddWithValue("@vid_podg", null);
                SC.Parameters.AddWithValue("@codeAudit", null);
                SC.Parameters.AddWithValue("@codeDisc", null);
                SC.Parameters.AddWithValue("@codepodgr", null);
                // идем по дням недели
                foreach (var outtemp in myLessons)
                {
                    // день недели для пар одинаковый, поэтому можно его добавить вне цикла прохода по парам
                    SC.Parameters["@d_ned"].Value = outtemp.Day;
                    //SC.Parameters.Clear();
                    // идем по парам
                    for (int i = 0; i < 5; i++)
                    {
                        // идем по внутренности ячейки
                        foreach (var intemp in outtemp[i].CollectionOfItems)
                        {
                            // если хотя бы один атрибут отсуствует, то запись не добавляется
                            if (intemp.CodeDisc == 0 ||
                                intemp.CodeVid == 0 ||
                                intemp.CodeAudit == 0 ||
                                intemp.CodePrep == 0 ||
                                intemp.CodePodgr == -1)
                                continue;
                            //SC.Parameters.AddWithValue("@pr_ned", outtemp[i].CollectionOfItems.IndexOf(intemp));
                            SC.Parameters["@pr_ned"].Value = (outtemp[i].CollectionOfItems.Count == 2) ? outtemp[i].CollectionOfItems.IndexOf(intemp) + 1 : outtemp[i].CollectionOfItems.IndexOf(intemp);
                            //SC.Parameters.AddWithValue("@i",i);
                            SC.Parameters["@i"].Value = i + 1;
                            //SC.Parameters.AddWithValue("@vid_podg",intemp.CodeVid);
                            SC.Parameters["@vid_podg"].Value = intemp.CodeVid;
                            //SC.Parameters.AddWithValue("@codeAudit",intemp.CodeAudit);
                            SC.Parameters["@codeAudit"].Value = intemp.CodeAudit;
                            //SC.Parameters.AddWithValue("@codeDisc",intemp.CodeDisc);
                            SC.Parameters["@codeDisc"].Value = intemp.CodeDisc;
                            //SC.Parameters.AddWithValue("@codepodgr", intemp.CodePodgr);
                            SC.Parameters["@codepodgr"].Value = intemp.CodePodgr;
                            int buf = SC.ExecuteNonQuery();
                            // добавляем препода в RASP_PPS
                            using (SqlCommand SC2 = new SqlCommand(Db.SQLCommands.InsertToRaspPps, Db.myConnection))
                            {
                                SC2.Parameters.AddWithValue("@tabn_s", intemp.CodePrep);
                                SC2.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            MessageBox.Show("Расписание сохранено успешно", "Уведомление", MessageBoxButton.OK);
        }



        public void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                myCell bufCell = myLessons[myDataGrid.Items.IndexOf(myDataGrid.CurrentItem)][myDataGrid.CurrentColumn.DisplayIndex];

                //XmlReader OldCell = bufCell.DeepClone();
                AddDataToCell WinForAdding = new AddDataToCell(bufCell, Discips);
                //if (bufCell.PointToScreen(new Point(0d, 0d)).Y);
                if (bufCell.PointToScreen(new Point(0d, 0d)).X + WinForAdding.Width + bufCell.ActualWidth < SystemParameters.PrimaryScreenWidth)
                    WinForAdding.Left = bufCell.PointToScreen(new Point(0d, 0d)).X + bufCell.ActualWidth;
                else
                    WinForAdding.Left = bufCell.PointToScreen(new Point(0d, 0d)).X - WinForAdding.Width;
                if (bufCell.PointToScreen(new Point(0d, 0d)).Y - WinForAdding.Height / 3 + WinForAdding.Height < SystemParameters.WorkArea.Height)
                    WinForAdding.Top = bufCell.PointToScreen(new Point(0d, 0d)).Y - WinForAdding.Height / 3;
                else
                    WinForAdding.Top = SystemParameters.WorkArea.Height - WinForAdding.Height;
                WinForAdding.ShowDialog();
                #region
                //if (WinForAdding.ShowDialog() == false)
                //{
                //    RemoveLogicalChild(bufCell);
                //    bufCell = (myCell) XamlReader.Load(OldCell);
                //}

                //WinForAdding.LocalCell.InnerDataGrid.ColumnWidth = myDataGrid.ColumnWidth;

                //WinForAdding.LocalCell.InnerDataGrid.IsHitTestVisible = false;



                //switch (WinForAdding.LocalCell.CollectionOfItems.Count)
                //{
                //    case 1:
                //        WinForAdding.LocalCell.InnerDataGrid.RowHeight = myDataGrid.RowHeight;
                //        break;
                //    case 2:
                //        WinForAdding.LocalCell.InnerDataGrid.RowHeight = myDataGrid.RowHeight / 2;
                //        break;
                //}
                //bufCell = WinForAdding.LocalCell;
                //bufCell.Margin = new Thickness(0);
                //myLessons[myDataGrid.Items.IndexOf(myDataGrid.CurrentItem)][myDataGrid.CurrentColumn.DisplayIndex] = bufCell; 
                //myDataGrid.Items.Refresh();
                #endregion

                bufCell.InnerDataGrid.UnselectAllCells();
            }
            catch(Exception exc)
            {
                MessageBox.Show("Не выбрана ячейка", "Ошибка", MessageBoxButton.OK);
            }
        }
        

        private void CommandBinding_Executed_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            // Нужно делать полную копию элемента
            // внутри него коллекция, поэтому нужно делать полные копии и ее содержимого
            //Clipboard.SetDataObject(myDataGrid.SelectedCells, true);
            //Clipboard.SetDataObject(myDataGrid.SelectedValue, true);
            //Clipboard.SetDataObject(myLessons[myLessons.IndexOf(myDataGrid.CurrentItem as Cells)][myDataGrid.SelectedCells[0].Column.DisplayIndex],true);
            //ClipBoard = myLessons[myLessons.IndexOf(myDataGrid.CurrentItem as Cells)][myDataGrid.SelectedCells[0].Column.DisplayIndex];
            ClipBoard = myLessons[myLessons.IndexOf(myDataGrid.CurrentItem as Cells)][myDataGrid.SelectedCells[0].Column.DisplayIndex];
            //ClipBoard = (myCell) myDataGrid.CurrentColumn.OnCopyingCellClipboardContent(myLessons[myLessons.IndexOf(myDataGrid.CurrentItem as Cells)][myDataGrid.SelectedCells[0].Column.DisplayIndex]);
            
        }
        
        private void CommandBinding_Executed_Paste(object sender, ExecutedRoutedEventArgs e)
        {
            //myDataGrid.SelectedCells = Clipboard.GetDataObject();
            //myDataGrid.SelectedItem = Clipboard.GetDataObject();
            //myLessons[myLessons.IndexOf(myDataGrid.CurrentItem as Cells)][myDataGrid.SelectedCells[0].Column.DisplayIndex] = Clipboard.GetDataObject() as myCell;
            //myLessons[myLessons.IndexOf(myDataGrid.CurrentItem as Cells)][myDataGrid.SelectedCells[0].Column.DisplayIndex] = ClipBoard;
            //myDataGrid.CurrentColumn.OnPastingCellClipboardContent(myDataGrid.CurrentColumn.OnCopyingCellClipboardContent(myLessons[myLessons.IndexOf(myDataGrid.CurrentItem as Cells)][myDataGrid.SelectedCells[0].Column.DisplayIndex]), ClipBoard);
            
        }
    }
}
