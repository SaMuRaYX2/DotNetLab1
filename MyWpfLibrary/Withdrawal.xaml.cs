using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
    /// Interaction logic for Withdrawal.xaml
    /// </summary>
    public partial class Withdrawal : Window
    {
        public bool IsTimerReloadFinished { get; private set; } = false;
        public decimal history_cash { get; private set; } = 0.0m;
        public decimal withdrawal_cash { get; private set; }

        public Action<decimal, int, int, Image> action { get; private set; }
        public Action<decimal, Image,int> switch_terminal { get; private set; }
        public int id_user { get; set; }
        public int id_bank { get; set; }


        public Withdrawal(Action<decimal, int, int, Image> action, int id_user, int id_bank, Action<decimal, Image,int> switch_ter)
        {
            InitializeComponent();
            exit_from_window.MouseDown += Exit_from_window_MouseDown;
            enter_value.TextChanged += Enter_value_TextChanged;
            enter_value.GotFocus += Enter_value_GotFocus;
            pay.MouseDown += Pay_MouseDown;
            this.action = action;
            this.id_user = id_user;
            this.id_bank = id_bank;
            switch_terminal = switch_ter;
            switch_bank.MouseDown += Switch_bank_MouseDown;
        }

        private void Switch_bank_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (switch_terminal != null)
            {
                switch_terminal(withdrawal_cash,switch_bank,id_bank);
            }
        }

        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(withdrawal_cash != 0.0m)
            {
                if(action != null)
                {
                    action(withdrawal_cash, id_user, id_bank, switch_bank);
                    history_cash += withdrawal_cash;
                }
            }
            else
            {
                MessageBox.Show("Ви ввели не правильні значення для суми та номера телефона", "Неправильне введення!!!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Enter_value_GotFocus(object sender, RoutedEventArgs e)
        {
            if(enter_value.Text == "Enter_value")
            {
                enter_value.Text = "";
            }
        }

        

        private void Enter_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsRightValue(enter_value.Text))
            {
                enter_value.Foreground = Brushes.Black;
                withdrawal_cash = decimal.Parse(enter_value.Text);
            }
            else
            {
                if (enter_value.Text == "Enter_value" || enter_value.Text == "")
                {
                    enter_value.Foreground = Brushes.Black;
                    withdrawal_cash = 0.0m;
                }
                else
                {
                    enter_value.Foreground = Brushes.DarkRed;
                    withdrawal_cash = 0.0m;
                }
            }
        }

        public bool IsRightValue(string value)
        {
            if (decimal.TryParse(value, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void Exit_from_window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"Сума, яка була знятта з рахунку становить {history_cash}", "Сума зняття", MessageBoxButton.OK, MessageBoxImage.Information);
            IsTimerReloadFinished = false;
            Application.Current.MainWindow.Show();
            this.Close();

        }
    }
}
