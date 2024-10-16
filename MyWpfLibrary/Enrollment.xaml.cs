﻿using Org.BouncyCastle.Crypto.Modes.Gcm;
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
        public bool IsTimerReloadFinished { get; private set; } = false;
        public Enrollment(Action<decimal,int> action, int id_user,ref bool IsTimerFinished)
        {
            InitializeComponent();
            enter_value.TextChanged += Enter_value_TextChanged;
            //enter_value.LostFocus += Enter_value_LostFocus;
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
            MyGrid.UpdateLayout();
            if(enter_value.Text == "Enter_value")
            {
                enter_value.Text = "";
            }
            else
            {
                if (!IsDigit(enter_value.Text))
                {
                    enter_value.Text = "";
                }
            }
            
        }

        public bool IsDigit(string number)
        {
            bool test = true;
            foreach(char c in number)
            {
                if (char.IsDigit(c))
                {
                    continue;
                }
                else
                {
                    test = false;
                }
            }
            return test;
        }
        private void Exit_from_window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"Сума, яка була поповненна на карту становить {history_cash}", "Сума поповнення", MessageBoxButton.OK, MessageBoxImage.Information);
            IsTimerReloadFinished = false;
            Application.Current.MainWindow.Show();
            this.Close();
            

        }

        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (cash_enrollment != 0.0m)
            {
                if (add_cash != null)
                {
                    add_cash(cash_enrollment, id_user);
                    history_cash += cash_enrollment;
                }
                
                
            }
            else
            {
                MessageBox.Show("Неправильно введене значення!!!", "WARNING VALUE", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                enter_value.Text = "Enter_value";
                
            }
        }

        

        private void Enter_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!decimal.TryParse(enter_value.Text,out _))
            {
                if (enter_value.Text == "Enter_value" || enter_value.Text == "")
                {
                    enter_value.Foreground = Brushes.Black;
                    cash_enrollment = 0.0m;
                    enter_value.Text = "";
                }
                else
                {
                    enter_value.Foreground = Brushes.DarkRed;
                    cash_enrollment = 0.0m;
                    
                }
            }
            else
            {
                cash_enrollment = decimal.Parse(enter_value.Text);
                enter_value.Foreground = Brushes.Black;
            }
        }
    }
}
