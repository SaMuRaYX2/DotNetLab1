using Library_for_bank;
using MySql.Data.MySqlClient;
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

namespace Terminal_MyBankWPF
{
    /// <summary>
    /// Interaction logic for Authentication_admin.xaml
    /// </summary>
    public partial class Authentication_admin : Window
    {
        public string name { get; private set; }
        public string password { get; private set; }
        public Authentication_admin()
        {
            InitializeComponent();
            exit_from_window.MouseDown += Exit_from_window_MouseDown;
            text_username.GotFocus += Text_GotFocus;
            text_username.TextChanged += Text_TextChanged;
            try_to_authentication.Click += Try_to_authentication_Click;
            password_show_and_hide.Click += Password_show_and_hide_Click;
        }

        private void Password_show_and_hide_Click(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (check.IsChecked == false)
            {
                check.Background = Brushes.Green;
                check.Content = "Скрити";
                passbox_password.Visibility = Visibility.Collapsed;
                textbox_password.Visibility = Visibility.Visible;
                textbox_password.Text = passbox_password.Password;
                textbox_password.Focus();
                textbox_password.CaretIndex = textbox_password.Text.Length;
                return;
            }
            else if (check.IsChecked == true)
            {
                check.Background = Brushes.Red;
                check.Content = "Показати";
                textbox_password.Visibility = Visibility.Collapsed;
                passbox_password.Visibility = Visibility.Visible;
                passbox_password.Password = textbox_password.Text;
                passbox_password.Focus();
                return;
            }
        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? box = sender as TextBox;
            if(box.Text != string.Empty)
            {

                name = box.Text;
                
                
            }
        }

        private void Try_to_authentication_Click(object sender, RoutedEventArgs e)
        {
            if (passbox_password.IsVisible == true && textbox_password.IsVisible == false)
            {
                if (passbox_password.Password != string.Empty)
                {
                    password = passbox_password.Password;
                }
            }
            else if (textbox_password.IsVisible == true && passbox_password.IsVisible == false)
            {
                if(textbox_password.Text != string.Empty)
                {
                    password = textbox_password.Text;
                }
            }
            if(name != string.Empty && password != string.Empty)
            {
                if (Check_right_data())
                {
                    Admin panel_admin = new Admin();
                    panel_admin.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Дані не вірні, або їх не було знайдено в базі даних!", "Неправильні дані", MessageBoxButton.OK, MessageBoxImage.Warning);
                    text_username.Focus();
                }
            }
        }

        private void Text_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox? box = sender as TextBox;
            if(box.Text == "Enter value")
            {
                box.Text = string.Empty;
            }
        }

        public bool Check_right_data()
        {
            bool exist = false;
            DB db = new DB();
            db.openConnection();
            try
            {
                string query = "select exists(select 1 from admins where name = @s1 and password = @s2)";
                using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
                {
                    cmd.Parameters.Add("@s1", MySqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@s2", MySqlDbType.VarChar).Value = password;
                    exist = Convert.ToBoolean(cmd.ExecuteScalar());
                    db.closeConnection();
                    return exist;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка під час роботи з базою даних помилка {ex.Message}","Помилка!!!",MessageBoxButton.OK,MessageBoxImage.Error);
                db.closeConnection();
                return exist;
                
            }
            
        }
        

        private void Exit_from_window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }
}
