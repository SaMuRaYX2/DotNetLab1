﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata.Ecma335;
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
using Library_for_bank;
using MySql.Data.MySqlClient;
using Mysqlx.Session;

namespace Terminal_MyBankWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class Admin : Window
    {
        
        public MainWindow window_start { get; private set; }
        public List<UserBankArgs> Users { get; private set; }
        string name;
        string surname;
        string fatherly;
        string number_telephone;
        int pin;
        string telephone;
        int age;
        string sex;
        int number_card;
        public Admin(MainWindow mainWindow)
        {
            InitializeComponent();
            window_start = mainWindow;
            this.Closed += Admin_Closed;
            this.Loaded += Admin_Loaded;
            add_user.Click += Add_user_Click;
        }

        private void Add_user_Click(object sender, RoutedEventArgs e)
        {
            bool test_to_fill_all_textBox = true;
            
            foreach(var textBox in GetAllTextBoxes(this))
            {
                if(textBox.Text == "pib" || textBox.Text == "telephone_number" || textBox.Text == "pin" || textBox.Text == "telephone" || textBox.Text == "age" || textBox.Text == "sex" || textBox.Text == "")
                {
                    test_to_fill_all_textBox = false;
                    textBox.Background = Brushes.DarkRed;
                }
            }
            if (test_to_fill_all_textBox == false)
            {
                MessageBox.Show("Ви не заповнили всі поля для створення пользувателя", "Заповніть поля які виділенні червоним", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                foreach(var textBox in GetAllTextBoxes(this))
                {
                    if(textBox.Tag.ToString() == "pib")
                    {
                        string[] parts = textBox.Text.Split(' ');
                        surname = parts[0];
                        name = parts[1];
                        fatherly = parts[2];
                    }
                    else if(textBox.Tag.ToString() == "telephone_number")
                    {
                        number_telephone = textBox.Text;
                    }
                    else if(textBox.Tag.ToString() == "pin")
                    {
                        int.TryParse(textBox.Text, out pin);
                    }
                    else if(textBox.Tag.ToString() == "telephone")
                    {
                        telephone = textBox.Text;
                    }
                    else if(textBox.Tag.ToString() == "age")
                    {
                        int.TryParse(textBox.Text, out age);
                    }
                    else if(textBox.Tag.ToString() == "sex")
                    {
                        sex = textBox.Text;
                    }

                }
                Random random = new Random();
                DB db = new DB();
                number_card = random.Next(1000000, 9000000);
                bool test = Test_to_unique_number_card(number_card, db);
                while ((test = Test_to_unique_number_card(number_card, db)))
                {
                    number_card = random.Next(1000000, 9000000);
                }
                db.Start_Fill_DataBase += Filling_DataBase;
                db.Start_Filling(name, surname, fatherly, number_telephone, pin, telephone, age, sex, number_card);
            }
        }

        private bool Test_to_unique_number_card(int number, DB db)
        {
            string query = "SELECT (number_card) from users";
            List<int> existingNumbers = new List<int>();
            using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
            {
                db.openConnection();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingNumbers.Add(reader.GetInt32("number_card"));
                    }
                }
            }
            db.closeConnection();
            return existingNumbers.Contains(number);
        }
        private void Filling_DataBase(object? sender, UserBankArgs e)
        {
            DB db = sender as DB;
            string query = "INSERT INTO users (name,surname,fatherly,number_telephone,pin,telephone,age,sex,number_card) VALUES (@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9)";

            MySqlCommand cmd = new MySqlCommand(query, db.getConnection());
            cmd.Parameters.Add("@s1", MySqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@s2", MySqlDbType.VarChar).Value = surname;
            cmd.Parameters.Add("@s3", MySqlDbType.VarChar).Value = fatherly;
            cmd.Parameters.Add("@s4", MySqlDbType.VarChar).Value = number_telephone;
            cmd.Parameters.Add("@s5", MySqlDbType.Int32).Value = pin;
            cmd.Parameters.Add("@s6", MySqlDbType.VarChar).Value = telephone;
            cmd.Parameters.Add("@s7", MySqlDbType.Int32).Value = age;
            cmd.Parameters.Add("@s8", MySqlDbType.VarChar).Value = sex;
            cmd.Parameters.Add("@s9", MySqlDbType.Int32).Value = number_card;
            db.openConnection();
            int result = cmd.ExecuteNonQuery();
            db.closeConnection();
            if(result > 0)
            {
                void Result()
                {
                    MessageBox.Show("Дані були завантаженні на сервер","Все добре, без поимилок!!!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                db.Result_of_INSERT(Result);
            }
            else
            {
                void Result()
                {
                   MessageBox.Show("Дані не були завантаженні на сервер", "Є помилки!!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                db.Result_of_INSERT(Result);
            }
        }
        
        
        private void Admin_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in GetAllTextBoxes(this))
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#801e1e1a"));
            }
            if (textBox != null)
            {
                if(textBox.Text == "pib" || textBox.Text == "telephone_number" || textBox.Text == "pin" || textBox.Text == "telephone" || textBox.Text == "age" || textBox.Text == "sex" || textBox.Text == "")
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
                textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#801e1e1a"));
            }
            if (textBox != null)
            {
                if(textBox.Text == "pib" || textBox.Text == "telephone_number" || textBox.Text == "pin" || textBox.Text == "telephone" || textBox.Text == "age" || textBox.Text == "sex")
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
