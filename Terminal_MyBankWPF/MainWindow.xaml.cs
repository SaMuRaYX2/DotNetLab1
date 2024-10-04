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

namespace Terminal_MyBankWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public string choosen_service { get; set; }
        public int number { get; set; }
        public int pin { get; set; }
        public int code { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            button_exit.MouseDown += Button_exit_MouseDown;
            background_image.Loaded += Start_Program;
            admin_panel.MouseDown += Admin_panel_MouseDown;
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
            enter_to_interface.Click += Try_authentication;
        }

        public void Email_code(string request)
        {

        }
        

        private void Try_authentication(object sender, RoutedEventArgs e)
        {
            choosen_service = service_of_authentication.SelectedValue.ToString();
            int temp_number;
            int.TryParse(number_card.Text, out temp_number);
            number = temp_number;
            int temp_pin;
            int.TryParse(pin_card.Text, out temp_pin);
            pin = temp_pin;
            Random rnd = new Random();
            code = rnd.Next(1000,10000);
            string request = $"Нікому не показуйте цей код, який вам потрібно для аунтифікації в банківську систему для наступних маніпуляцій з вашою картою, упевніться, що це ваша карта {number}\nВаш код аунтифікації {code}";
            

        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox text_box)
            {
                if(!int.TryParse(text_box.Text, out _))
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
                if (string.IsNullOrEmpty(text_box.Text))
                {
                    text_box.Foreground = Brushes.White;
                    return;
                }
                if (!int.TryParse(text_box.Text, out _))
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
            Admin Panel_admin = new Admin(this);
            Panel_admin.Show();
            this.Hide();
        }

        private void Button_exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void Start_Program(object sender, RoutedEventArgs e)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("C:\\Users\\Sam\\Documents\\Labs\\DotNetLab1\\Image\\Background_gif.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(background_image, image);
        }
    }
}