using System;
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
using System.Diagnostics;

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
        string number_card;
        string email;
        public Admin(MainWindow mainWindow)
        {
            InitializeComponent();

            window_start = mainWindow;
            this.Closed += Admin_Closed;
            this.Loaded += Admin_Loaded;
            add_user.Click += Add_user_Click;
            go_to_terminal.MouseDown += Go_to_terminal_MouseDown;
        }

        private void Go_to_terminal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Users\Sam\source\repos\MyBank\MyBankTerminal\bin\Debug\MyBankTerminal.exe";
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
        }

        private void Add_user_Click(object sender, RoutedEventArgs e)
        {
            bool test_to_fill_all_textBox = true;
            foreach (var textBox in GetAllTextBoxes(this))
            {
                if (textBox.Text == "pib" || textBox.Text == "telephone_number" || textBox.Text == "pin" || textBox.Text == "telephone" || textBox.Text == "age" || textBox.Text == "sex" || textBox.Text == "" || textBox.Text == "email")
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
                foreach (var textBox in GetAllTextBoxes(this))
                {
                    if (textBox.Tag.ToString() == "pib")
                    {
                        string[] parts = textBox.Text.Split(' ');
                        surname = parts[0];
                        name = parts[1];
                        fatherly = parts[2];
                    }
                    else if (textBox.Tag.ToString() == "telephone_number")
                    {
                        number_telephone = textBox.Text;
                    }
                    else if (textBox.Tag.ToString() == "pin")
                    {
                        int.TryParse(textBox.Text, out pin);
                    }
                    else if (textBox.Tag.ToString() == "telephone")
                    {
                        telephone = textBox.Text;
                    }
                    else if (textBox.Tag.ToString() == "age")
                    {
                        int.TryParse(textBox.Text, out age);
                    }
                    else if (textBox.Tag.ToString() == "sex")
                    {
                        sex = textBox.Text;
                    }
                    else if(textBox.Tag.ToString() == "email")
                    {
                        email = textBox.Text;
                    }
                }
                Random random = new Random();
                DB db = new DB();
                decimal balance = 0.000m;
                
                number_card = GenerateRandomStringNumber(16);
                bool test;
                while (test = Test_to_unique_number_card(number_card, db))
                {
                    number_card = GenerateRandomStringNumber(16);
                }
                db.Start_Fill_DataBase += Filling_DataBase;
                db.Start_Filling(name, surname, fatherly, number_telephone, pin, telephone, age, sex, number_card, balance,email);
            }
        }
        public string GenerateRandomStringNumber(int length)
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(0, 10));
            }
            return sb.ToString();
        }
        private bool Test_to_unique_number_card(string number, DB db)
        {
            string query = "select number_card from users";
            List<string> existingNumbers = new List<string>();
            using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
            {
                db.openConnection();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingNumbers.Add(reader.GetString("number_card"));
                    }
                }
            }
            db.closeConnection();   
            return existingNumbers.Contains(number);
        }
        private void Filling_DataBase(object? sender, UserBankArgs e)
        {
            DB db = sender as DB;
            string query = "INSERT INTO users (name,surname,fatherly,number_telephone,pin,telephone,age,sex,number_card,balance,address_email) VALUES (@s1,@s2,@s3,@s4,@s5,@s6,@s7,@s8,@s9,@s10,@s11)";
            
            MySqlCommand cmd = new MySqlCommand(query, db.getConnection());
            cmd.Parameters.Add("@s1", MySqlDbType.VarChar).Value = e.Name;
            cmd.Parameters.Add("@s2", MySqlDbType.VarChar).Value = e.Surname;
            cmd.Parameters.Add("@s3", MySqlDbType.VarChar).Value = e.Fatherly;
            cmd.Parameters.Add("@s4", MySqlDbType.VarChar).Value = e.Number_telephone;
            cmd.Parameters.Add("@s5", MySqlDbType.Int32).Value = e.Pin;
            cmd.Parameters.Add("@s6", MySqlDbType.VarChar).Value = e.Telephone;
            cmd.Parameters.Add("@s7", MySqlDbType.Int32).Value = e.Age;
            cmd.Parameters.Add("@s8", MySqlDbType.VarChar).Value = e.Sex;
            cmd.Parameters.Add("@s9", MySqlDbType.VarChar).Value = e.Number_card;
            cmd.Parameters.Add("@s10", MySqlDbType.Decimal).Value = e.balance;
            cmd.Parameters.Add("@s11", MySqlDbType.VarChar).Value = e.email;
            db.openConnection();
            int result = cmd.ExecuteNonQuery();
            db.closeConnection();
            if(result > 0)
            {
                void Result()
                {
                    MessageBox.Show("Дані були завантаженні на сервер","Все добре, без помилок!!!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if(textBox.Text == "pib" || textBox.Text == "telephone_number" || textBox.Text == "pin" || textBox.Text == "telephone" || textBox.Text == "age" || textBox.Text == "sex" || textBox.Text == "" || textBox.Text == "email")
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
                if(textBox.Text == "pib" || textBox.Text == "telephone_number" || textBox.Text == "pin" || textBox.Text == "telephone" || textBox.Text == "age" || textBox.Text == "sex" || textBox.Text == "email")
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
