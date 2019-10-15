using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using System.Threading;

namespace Raspisanie2019
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string login { get; set; }
        public string pass { get; set; }
        public LoginWindow() => InitializeComponent();
        public CheckMark CM;

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

            if (tbLogin.Text == "" || tbPass.Password == "")
            {
                tbError.Text = "Не все поля заполнены";
                tbError.Foreground = Brushes.DarkRed;
                tbError.ToolTip = new ToolTip() { Content = "Введены не все данные" };
                return;
            }
            #region
            //if(tbLogin.Text != "1")
            //{
            //    tbError.Text = "Пользователь не найден";
            //    return;
            //}
            //if (tbLogin.Text != "1")
            //{
            //    tbError.Text = "Неверный пароль";
            //    return;
            //}
            #endregion
            Db.login = tbLogin.Text;
            Db.password = tbPass.Password;
            if (!Db.OpenCon())
            {
                tbError.Text = "У Вас нет доступа к серверу";
                tbError.ToolTip = new ToolTip() { Content = "Неверный логин или пароль" };
                tbError.Foreground = Brushes.Red;
                return;
            }

            //Проверка доступа
            using (SqlCommand myCommand = new SqlCommand())
            {
                SqlDataReader mySDR;
                myCommand.CommandText = Db.SQLCommands.CheckRole;
                myCommand.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        new SqlParameter("@login", tbLogin.Text),
                        new SqlParameter("@nameOfExe", Process.GetCurrentProcess().MainModule.ModuleName)
                    });
                myCommand.Connection = Db.myConnection;
                //если запрос пустой, то доступа нет
                if (!(mySDR = myCommand.ExecuteReader()).HasRows)
                {
                    tbError.Text = "У Вас нет доступа к приложению";
                    tbError.ToolTip = new ToolTip() { Content = "Не назначена роль для приложения по данному логину" };
                    tbError.Foreground = Brushes.OrangeRed;
                    return;
                }
                //если запрос не пустой, то получаем коды роли, программы и факультета
                mySDR.Read();
                Db.id_role = Convert.ToInt32(mySDR["id_role"]);
                Db.id_program = Convert.ToInt32(mySDR["id_program"]);
                Db.kod_ft = Convert.ToInt32(mySDR["kod_ft"]);
                mySDR.Close();
                myCommand.CommandText = Db.SQLCommands.GetPermits;
                myCommand.Parameters.Clear();
                //Cчитываем пермиты
                myCommand.Parameters.AddRange(
                    new SqlParameter[]
                    {
                        new SqlParameter("@id_program", Db.id_program),
                        new SqlParameter("@id_role", Db.id_role)
                    });
                if (!(mySDR = myCommand.ExecuteReader()).HasRows)
                {
                    tbError.Text = "У Вас нет прав к приложению";
                    tbError.ToolTip = new ToolTip() { Content = "Не назначены права для приложения по данному логину" };
                    tbError.Foreground = Brushes.Orange;
                    return;
                }
                try {
                while (mySDR.Read())
                    Db.Permits.Add(
                        mySDR.GetInt32(0),
                        mySDR.GetString(1));
                }
                catch (Exception exc)
                { }
                tbError.Text = "Вход выполнен успешно";
                tbError.Foreground = Brushes.Green;
                // посмотреть mySDR.IsClose
                mySDR.Close();
            }
            tbLogin.IsEnabled = tbPass.IsEnabled = btnOk.IsEnabled = btnCancel.IsEnabled = false;
            DrawCheckMark();
        }

        /// <summary>
        /// Анимимированное появление галочки, как сигнала успешного входа
        /// </summary>
        void DrawCheckMark()
        {
            DispatcherTimer myDispatcherTimerGameProgress = new DispatcherTimer();
            myDispatcherTimerGameProgress.Tick += new EventHandler(AddFigure);
            myDispatcherTimerGameProgress.Start();
            CM = new CheckMark();
            LoginGrid.Children.Add(CM);
            
            CM.RenderTransform = new ScaleTransform(5, 5)
            {
                CenterX = VisualTreeHelper.GetOffset(CM).X + 15,
                CenterY = VisualTreeHelper.GetOffset(CM).Y + 15
            };
            
            Grid.SetRow(CM, 0);
            Grid.SetRowSpan(CM, 7);
            Grid.SetColumn(CM, 0);
            Grid.SetColumnSpan(CM, 2);
            PointAnimationUsingPath paup1 = new PointAnimationUsingPath
            {
                PathGeometry = CM.FrstRec
            };
            CM.FrstRec.Freeze();
            paup1.Duration = TimeSpan.FromSeconds(1.5);
            paup1.BeginTime = TimeSpan.FromSeconds(0.2);

            PointAnimationUsingPath paup2 = new PointAnimationUsingPath
            {
                PathGeometry = CM.SecRec
            };
            CM.SecRec.Freeze();
            paup2.Duration = TimeSpan.FromSeconds(1.5);
            paup2.BeginTime = TimeSpan.FromSeconds(0.2);

            PointAnimationUsingPath paup3 = new PointAnimationUsingPath
            {
                PathGeometry = CM.CM
            };
            CM.CM.Freeze();
            paup3.Duration = TimeSpan.FromSeconds(1.5);
            paup3.BeginTime = TimeSpan.FromSeconds(0.2);
            paup3.Completed += delegate (object sender, EventArgs e)
            {
                Thread.Sleep(2000);
                myDispatcherTimerGameProgress.Stop();
                Close();
            };
            CM.EllGeom1.BeginAnimation(EllipseGeometry.CenterProperty, paup1);
            CM.EllGeom2.BeginAnimation(EllipseGeometry.CenterProperty, paup2);
            CM.EllGeom3.BeginAnimation(EllipseGeometry.CenterProperty, paup3);
            //CM = null;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => 
            Application.Current.Shutdown();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Db.PingServ())
            {
                StatusOfServer.Fill = Brushes.Red;
                tbError.Text = "Сервер недоступен";
                tbError.ToolTip = new ToolTip() { Content = "Проверьте подключение к сети" };
                tbLogin.IsEnabled = btnOk.IsEnabled = tbPass.IsEnabled = false;
                return;
            }
            tbLogin.Focus();
            StatusOfServer.Fill = Brushes.Green;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer myDispatcherTimerGameProgress = new DispatcherTimer();
            myDispatcherTimerGameProgress.Tick += new EventHandler(AddFigure);
            myDispatcherTimerGameProgress.Start();
            CM = new CheckMark();
            LoginGrid.Children.Add(CM);
            CM.RenderTransform = new ScaleTransform(5, 5);
            PointAnimationUsingPath paup1 = new PointAnimationUsingPath
            {
                Duration = TimeSpan.FromSeconds(1),
                PathGeometry = CM.FrstRec,

            };
            CM.FrstRec.Freeze();


            PointAnimationUsingPath paup2 = new PointAnimationUsingPath
            {
                Duration = TimeSpan.FromSeconds(1),
                PathGeometry = CM.SecRec
            };
            CM.SecRec.Freeze();


            PointAnimationUsingPath paup3 = new PointAnimationUsingPath
            {
                PathGeometry = CM.CM,
                Duration = TimeSpan.FromSeconds(1)
            };
            CM.CM.Freeze();

            CM.EllGeom1.BeginAnimation(EllipseGeometry.CenterProperty, paup1);
            CM.EllGeom2.BeginAnimation(EllipseGeometry.CenterProperty, paup2);
            CM.EllGeom3.BeginAnimation(EllipseGeometry.CenterProperty, paup3);

            #region
            //RegisterName("Ellipse", CM.ELLIPSE);
            //Storyboard.SetTargetName(paup1, "Ellipse");
            //Storyboard.SetTarget(paup1, CM.EllGeom);
            //Storyboard.SetTargetProperty(paup1, new PropertyPath(EllipseGeometry.CenterProperty));
            //Storyboard storyboard1 = new Storyboard();
            //storyboard1.Children.Add(paup1);
            //storyboard1.Begin();
            //storyboard1.Children.Add(paup1);
            //PointAnimationUsingPath paup2 = new PointAnimationUsingPath();
            //paup2.PathGeometry = CM.SecRec;
            //PointAnimationUsingPath paup3 = new PointAnimationUsingPath();
            //paup3.PathGeometry = CM.CM;

            //Storyboard MyStoryboard = new Storyboard();
            //MyStoryboard.Children = new TimelineCollection { };
            #endregion
        }

        private void AddFigure(object sender, EventArgs e)
        {
            CM.PolyLine1.Points.Add(CM.EllGeom1.Center);
            CM.PolyLine2.Points.Add(CM.EllGeom2.Center);
            CM.PolyLine3.Points.Add(CM.EllGeom3.Center);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnOk_Click(null, null);
        }
    }
}
