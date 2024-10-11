using Org.BouncyCastle.Math.EC;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;
using System.Net;
using System.Net.Mail;
using Library_for_bank;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace Terminal_MyBankWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public string choosen_service { get; set; }
        public string number { get; set; }
        public int pin { get; set; }
        public int code { get; set; }
        public int id_user { get; set; }
        public List<UIElement> list_element { get; private set; }
        public AutomatedTellerMachine machine;
       
        
        public MainWindow()
        {
            InitializeComponent();
            //list_element = new List<UIElement>();
            button_exit.MouseDown += Button_exit_MouseDown;
            background_image.Loaded += Start_Program;
            admin_panel.MouseDown += Admin_panel_MouseDown;
            machine = new AutomatedTellerMachine();
            foreach(var element in MyGrid.Children)
            {
                if(element is TextBox textBox)
                {
                    if (textBox.Tag.ToString() == "number_of_card" || textBox.Tag.ToString() == "pin_of_card" || textBox.Tag.ToString() == "code_authentication")
                    {
                        textBox.TextChanged += TextBox_TextChanged;
                        textBox.LostFocus += TextBox_LostFocus;
                    }
                }
            }
            get_sms.Click += Try_authentication;
            number_card.Focus();
        }

        public void Email_code(object sender, CodeEvent e)
        {
            try
            {
                e.del();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка з відправкою смс на пошту {e.user.email}, помилка пов'язана з відправкою на domain {e.email_recipient}", $"Exception", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }
        
        public int CountDigits(string text)
        {
            int count = 0;
            foreach(var c in text)
            {
                if (char.IsDigit(c))
                {
                    count++;
                }
            }
            return count;
        }

        private void Try_authentication(object sender, RoutedEventArgs e)
        {
            var serviceItem = service_of_authentication.SelectedItem as ComboBoxItem;
            if(serviceItem != null)
            {
                choosen_service = serviceItem.Content.ToString();
            }
            int count = CountDigits(number_card.Text);
            if(count != 16)
            {
                MessageBox.Show("Enter right number of card, number of card must to have 16 digit!!!", "Error of number card", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            number = number_card.Text;
            int temp_pin;
            int.TryParse(pin_card.Text, out temp_pin);
            pin = temp_pin;
            string query = "select id from users where number_card = @s1 and pin = @s2";
            DB db = new DB();
            db.openConnection();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
                {
                    cmd.Parameters.AddWithValue("@s1", number);
                    cmd.Parameters.AddWithValue("@s2", pin);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id_user = reader.GetInt32(reader.GetOrdinal("id"));
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка під час отримання id користувача!", "Скоро ми це виправимо", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            db.closeConnection();
            Random rnd = new Random();
            code = rnd.Next(1000,10000);
            string request = $"Нікому не показуйте цей код, який вам потрібно для аунтифікації в банківську систему для наступних маніпуляцій з вашою картою, упевніться, що це ваша карта {number}\nВаш код аунтифікації {code}";

            if (test_to_myLove(db))
            {
                request = $"Привіт котику люблю тебе :), це твій код аунтифікації в банківську систему MyBank {code}, а це твій номер карти {number}, нікому не говори цей код";
            }
            machine.CodeEvent += Email_code;
            machine.Send_code_authentification(request, choosen_service,id_user);
            machine.CodeEvent -= Email_code;
            code_authentication.Focus();
        }
        public bool test_to_myLove(DB db)
        {
            string query = "select id from users where name = @s1 and surname = @s2 and fatherly = @s3";
            db.openConnection();
            int temp_id = 0;
            using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
            {
                cmd.Parameters.AddWithValue("@s1", "Юліана");
                cmd.Parameters.AddWithValue("@s2", "Липська");
                cmd.Parameters.AddWithValue("@s3", "Андріївна");
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        temp_id = reader.GetInt32("id");
                    }
                }
            }
            db.closeConnection();
            return temp_id == id_user;
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox some_code)
            {
                if(some_code.Tag.ToString() == "code_authentication")
                {
                    int temp_code;
                    bool test = int.TryParse(some_code.Text, out temp_code);
                    if (test && temp_code == code)
                    {
                        list_element = new List<UIElement>();
                        //MessageBox.Show("Authentication is succesfull :)", "Authentication", MessageBoxButton.OK, MessageBoxImage.Information);
                        for (int i = MyGrid.Children.Count - 1; i >= 0; i--)
                        {
                            if (MyGrid.Children[i] is TextBlock textBlock)
                            {
                                if (textBlock.Tag != null)
                                {
                                    if (textBlock.Tag.ToString() == "mybankchapter1" || textBlock.Tag.ToString() == "mybankchapter2" || textBlock.Tag.ToString() == "exit_block")
                                    {
                                        continue;
                                    }
                                }
                                list_element.Add(MyGrid.Children[i]);
                                MyGrid.Children.Remove(MyGrid.Children[i]);
                            }
                            else if (MyGrid.Children[i] is Button button)
                            {
                                list_element.Add(MyGrid.Children[i]);
                                MyGrid.Children.Remove(MyGrid.Children[i]);
                            }
                            else if (MyGrid.Children[i] is ComboBox comboBox)
                            {
                                list_element.Add(MyGrid.Children[i]);
                                MyGrid.Children.Remove(MyGrid.Children[i]);
                            }
                            else if (MyGrid.Children[i] is Border border)
                            {
                                if(border.Tag != null)
                                {
                                    if(border.Tag.ToString() == "border_exit_block")
                                    {
                                        continue;
                                    }
                                }
                                list_element.Add(MyGrid.Children[i]);
                                MyGrid.Children.Remove(border);
                            }
                            else if (MyGrid.Children[i] is TextBox textBox)
                            {
                                list_element.Add(textBox);
                                MyGrid.Children.Remove(textBox);
                            }
                        }
                        Create_new_window new_step = new Create_new_window(list_element, MyGrid, id_user);
                        
                    }
                    else
                    {
                        MessageBox.Show($"Authentication error :(", "Authentication", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            if(sender is TextBox text_box)
            {
                if (!text_box.Text.All(char.IsDigit))
                {
                    text_box.Foreground = Brushes.White;
                    text_box.Clear();
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (sender is TextBox text_box)
            {
                if (text_box.Tag.ToString() == "code_authentication")
                {
                    if (text_box.Text.Length == 4)
                    {
                        get_sms.Focus();
                    }
                }
                else if (text_box.Tag.ToString() == "number_of_card")
                {
                    if (text_box.Text.Length == 16)
                    {
                        pin_card.Focus();
                    }
                }
                else if (text_box.Tag.ToString() == "pin_of_card")
                {
                    if (text_box.Text.Length == 4)
                    {
                        get_sms.Focus();
                    }
                }
                if (string.IsNullOrEmpty(text_box.Text))
                {
                    text_box.Foreground = Brushes.White;
                    return;
                }
                if (!text_box.Text.All(char.IsDigit))
                {
                    text_box.Foreground = Brushes.Red;
                }
                else
                {
                    text_box.Foreground = Brushes.White;

                }
            }
        }


        private void Admin_panel_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Authentication_admin authentication_ = new Authentication_admin();
            authentication_.Show();
            this.Hide();
        }

        private void Button_exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach(Window window in Application.Current.Windows)
            {
                window.Close();
            }
        }
        private void Start_Program(object sender, RoutedEventArgs e)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Image/Background_gif.gif", UriKind.RelativeOrAbsolute);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(background_image, image);
        }
    }
}