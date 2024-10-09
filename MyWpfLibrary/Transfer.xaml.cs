using Org.BouncyCastle.Asn1.BC;
using System;
using System.Collections.Generic;
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

namespace MyWpfLibrary
{
    /// <summary>
    /// Interaction logic for Transfer.xaml
    /// </summary>
    public partial class Transfer : Window
    {
        public decimal cash_enrollment { get; private set; }
        public string number_telephone { get; private set; }
        public decimal history_cash { get; private set; }
        public Action<decimal,string> action { get; private set; }
        public bool IsTimerReloadFinished { get; private set; }
        public Transfer(Action<decimal,string> action, bool IsTimerFinished)
        {
            InitializeComponent();
            foreach (var element in MyGrid.Children)
            {
                if (element is FrameworkElement textBox)
                {
                    if (textBox.Name == "enter_number" || textBox.Name == "enter_value")
                    {
                        if (textBox is TextBox box)
                        {
                            box.TextChanged += Box_TextChanged;
                            box.LostFocus += Box_LostFocus;
                            box.GotFocus += Box_GotFocus;
                        }
                    }
                }
            }
            this.action = action;
            exit_from_window.MouseDown += Exit_from_window_MouseDown;
            pay.MouseDown += Pay_MouseDown;
            IsTimerReloadFinished = IsTimerFinished;
        }

        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(action != null)
            {
                action(cash_enrollment, number_telephone);
            }
            history_cash += cash_enrollment;
            cash_enrollment = (decimal)0;
        }

        private void Exit_from_window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"Сума, яка була поповненна на карту становить {history_cash}", "Сума поповнення", MessageBoxButton.OK, MessageBoxImage.Information);
            IsTimerReloadFinished = false;
            this.Close();
        }

        private void Box_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Enter_value" || textBox.Text == "Enter_number")
            {
                if (textBox.Text == "Enter_value")
                {
                    enter_value.Text = "";
                }
                else if (textBox.Text == "Enter_number")
                {
                    enter_number.Text = "";
                }
            }
        }

        private void Box_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Tag == "enter_value")
            {
                if (!decimal.TryParse(textBox.Text, out _) || textBox.Text == "Enter_value")
                {
                    textBox.Text = "Enter_value";
                    cash_enrollment = 0.0m;                    
                }
                else
                {
                    cash_enrollment = decimal.Parse(textBox.Text);
                }
            }
            else if(textBox.Tag == "enter_value")
            {
                if(!long.TryParse(textBox.Text, out _) || textBox.Text == "Enter_number")
                {
                    textBox.Text = "Enter_number";
                    number_telephone = "";
                }
                else
                {
                    int count = 0;
                    if(Right_number(textBox.Text, ref count) && count == 10)
                    {
                        number_telephone = textBox.Text;
                    }
                    else
                    {
                        MessageBox.Show("Номер введено не правильно!!!", "Неправильне введення", MessageBoxButton.OK, MessageBoxImage.Warning);
                        number_telephone = "";
                    }
                }
            }
            
        }
        public bool Right_number(string number, ref int count)
        {
            
            bool test = true;
            foreach (char c in number)
            {
                if (char.IsDigit(c))
                {
                    count++;
                }
                else
                {
                    test = false;
                    count++;
                }
            }
            return test;
        }
        public bool IsDigit(string c)
        {
            
            if (int.TryParse(c, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Tag == "enter_value")
            {
                if (!decimal.TryParse(textBox.Text, out _))
                {
                    if (textBox.Text == "Enter_value" || textBox.Text == "")
                    {
                        textBox.Foreground = Brushes.Black;
                    }
                    else
                    {
                        textBox.Foreground = Brushes.DarkRed;
                    }
                }
                else
                {
                    textBox.Foreground = Brushes.Black;
                }
            }
            else if(textBox.Tag == "enter_number")
            {

                if (IsDigit(textBox.Text) || textBox.Text == "Enter_number" || textBox.Text == "")
                {
                    textBox.Foreground = Brushes.Black;
                }
                else
                {
                    textBox.Foreground = Brushes.DarkRed;
                }
            }
        }

        
    }
}
