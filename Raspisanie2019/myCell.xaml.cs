using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Логика взаимодействия для myCell.xaml
    /// </summary>

    public partial class myCell : UserControl
    {
        //public int id_rasp_year;
        //public int id_nagruzka_vid;
        //public int id_aud;
        //public int k_discip;

        public ObservableCollection<OneItem> CollectionOfItems = new ObservableCollection<OneItem>(/*new List<OneItem> {new OneItem()}*/);
        public myCell()
        {
            InitializeComponent();
            InnerDataGrid.ItemsSource = CollectionOfItems;
        }

        //public myCell Clone()
        //{
        //    myCell myClone = (myCell)MemberwiseClone();
        //    myClone.InnerDataGrid.RowHeight = InnerDataGrid.RowHeight;
        //    myClone.CollectionOfItems = new ObservableCollection<OneItem>(from temp in CollectionOfItems select (OneItem)temp.Clone());
        //    return myClone;
        //}
        
        //public XmlReader DeepClone()
        //{
        //    using (var myMS = new MemoryStream())
        //    {
        //        var myBF = new BinaryFormatter();
        //        myBF.Serialize(myMS, this);
        //        myMS.Position = 0;
        //        return (myCell)myBF.Deserialize(myMS);
        //    }
        //    return XmlReader.Create(new StringReader(XamlWriter.Save(this)));
        //}

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderThickness = new Thickness(1);
            BorderBrush = Brushes.Blue;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderThickness = new Thickness(0);
            BorderBrush = Brushes.White;
        }

    }
}