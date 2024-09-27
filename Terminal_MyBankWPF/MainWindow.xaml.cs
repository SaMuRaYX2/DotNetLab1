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

namespace Terminal_MyBankWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            button_exit.MouseDown += Button_exit_MouseDown;
            background_image.Loaded += Start_Program;
            admin_panel.MouseDown += Admin_panel_MouseDown;
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