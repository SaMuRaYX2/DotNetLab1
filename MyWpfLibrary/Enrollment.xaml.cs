using Org.BouncyCastle.Crypto.Modes.Gcm;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyWpfLibrary
{
    /// <summary>
    /// Interaction logic for Enrollment.xaml
    /// </summary>
    public partial class Enrollment : Window
    {
        public decimal cash_enrollment { get; private set; }
        public Action<decimal, int> add_cash { get; private set; }
        public int id_user { get; private set; }
        public decimal history_cash { get; private set; }
        public bool IsTimerReloadFinished { get; private set; }
        public Enrollment(Action<decimal,int> action, int id_user, bool IsTimerFinished)
        {
            InitializeComponent();
            enter_value.TextChanged += Enter_value_TextChanged;
            enter_value.LostFocus += Enter_value_LostFocus;
            enter_value.GotFocus += Enter_value_GotFocus;
            pay.MouseDown += Pay_MouseDown;
            cash_enrollment = (decimal)0;
            exit_from_window.MouseDown += Exit_from_window_MouseDown;
            add_cash = action;
            this.id_user = id_user;
            IsTimerReloadFinished = IsTimerFinished;
        }

        private void Enter_value_GotFocus(object sender, RoutedEventArgs e)
        {
            if(enter_value.Text == "Enter_value")
            {
                enter_value.Text = "";
            }
            
        }

        private void Exit_from_window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"Сума, яка була поповненна на карту становить {history_cash}", "Сума поповнення", MessageBoxButton.OK, MessageBoxImage.Information);
            IsTimerReloadFinished = false;
            this.Close();

        }

        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (add_cash != null)
            {
                add_cash(cash_enrollment, id_user);
            }
            history_cash += cash_enrollment;
            cash_enrollment = (decimal)0;
        }

        private void Enter_value_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(enter_value.Text,out _) || enter_value.Text == "Enter_value")
            {
                enter_value.Text = "Enter_value";
                cash_enrollment = 0.0m;
            }
            else
            {
                cash_enrollment = decimal.Parse(enter_value.Text);
            }
            
        }

        private void Enter_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!decimal.TryParse(enter_value.Text,out _))
            {
                if (enter_value.Text == "Enter_value" || enter_value.Text == "")
                {
                    enter_value.Foreground = Brushes.Black;
                }
                else
                {
                    enter_value.Foreground = Brushes.DarkRed;
                }
            }
            else
            {
                enter_value.Foreground = Brushes.Black;    
            }
        }
    }
}
