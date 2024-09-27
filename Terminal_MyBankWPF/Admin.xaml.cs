using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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

namespace Terminal_MyBankWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public MainWindow window_start { get; private set; }
        public Admin(MainWindow mainWindow)
        {
            InitializeComponent();
            window_start = mainWindow;
            this.Closed += Admin_Closed;
            foreach(var textBox in GetAllTextBoxes(this))
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                if(textBox.Tag.ToString() == "pid" || textBox.Tag.ToString() == "number_telephone" || textBox.Tag.ToString() == "pin" || textBox.Tag.ToString() == "telephone" || textBox.Tag.ToString() == "age" || textBox.Tag.ToString() == "sex")
                {
                    textBox.Text = textBox.Tag.ToString();
                }
                else
                {

                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if(textBox != null)
            {
                if(textBox.Tag.ToString() == "pid" || textBox.Tag.ToString() == "number_telephone" || textBox.Tag.ToString() == "pin" || textBox.Tag.ToString() == "telephone" || textBox.Tag.ToString() == "age" || textBox.Tag.ToString() == "sex")
                {
                    textBox.Text = "";
                }
            }
        }

        private void Admin_Closed(object? sender, EventArgs e)
        {
            window_start.Show();
            this.Close();
        }
        public IEnumerable<TextBox> GetAllTextBoxes (DependencyObject parent)
        {
            if(parent == null)
            {
                yield break;
            }
            List<TextBox> textBoxes = new List<TextBox>();
            for(int i=0;i< VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if(child is TextBox textBox)
                {
                    yield return textBox;
                }
                foreach(var childofChild in GetAllTextBoxes(child))
                {
                    yield return childofChild;
                }
            }
            
        }
    }
}
