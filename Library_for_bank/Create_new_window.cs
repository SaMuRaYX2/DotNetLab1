using MySql.Data.MySqlClient;
using MyWpfLibrary;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Library_for_bank
{
    public class Create_new_window
    {
        public List<UIElement> Elements_old { get; private set; }
        public List<UIElement> Elements_new { get; private set; }
        public Grid MyGrid { get; private set; }
        public int id_user { get; private set; }
        private Timer background_timer;
        private int row_count;
        private int column_count;
        private bool isTimerFinished = false;
        private Timer timer_reload_menu;
        private bool isTimerReloadFinished = false;
        public TextBlock text_balance { get; private set; }
        public TextBlock text_withdrawal { get; private set; }
        public TextBlock text_enrollment { get; private set; }
        public TextBlock text_transfer { get; private set; }
        public Transfer transfer { get; private set; }
        public Enrollment enrollment { get; private set; }
        public string choosen_funk { get; private set; } = string.Empty;
        public Create_new_window(List<UIElement> elements, Grid myGrid, int id_user)
        {
            Elements_old = new List<UIElement>();
            Elements_new = new List<UIElement>();
            Elements_old = elements;
            this.id_user = id_user;
            MyGrid = myGrid;
            row_count = MyGrid.RowDefinitions.Count;
            column_count = MyGrid.ColumnDefinitions.Count;
            Success_Window();
            
        }

        public void Transfer_cash(decimal cash, string number)
        {
            DB db = new DB();
            string query = "update users set balance = balance + @s1 where number_telephone like @s2";
            try
            {
                db.openConnection();
                using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
                {
                    cmd.Parameters.Add("@s1", MySqlDbType.Decimal).Value = cash;
                    cmd.Parameters.Add("@s2", MySqlDbType.VarChar).Value = "%" + number + "%";
                    int result = cmd.ExecuteNonQuery();
                    if(result > 0)
                    {
                        MessageBox.Show("Баланс успішно обнолений :)", "Операція успішна", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Баланс не обнолений :(", "Операція не успішна", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                db.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Text_transfer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isTimerReloadFinished = true;
            Action<decimal, string> action = new Action<decimal, string>(Transfer_cash);
            transfer = new Transfer(action,ref isTimerReloadFinished);
            transfer.Show();
            Application.Current.MainWindow.Hide();
            choosen_funk = "transfer";
        }

        
        public void Add_cash(decimal cash, int id_user)
        {
            DB dB = new DB();
            string query = "update users set balance = balance + @s1 where id = @s2";
            try
            {
                dB.openConnection();
                using (MySqlCommand cmd = new MySqlCommand(query, dB.getConnection()))
                {
                    cmd.Parameters.Add("@s1", MySqlDbType.Decimal).Value = cash;
                    cmd.Parameters.Add("@s2", MySqlDbType.Decimal).Value = id_user;
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Баланс успішно обнолений :)", "Операція успішна", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Баланс не обнолений :(", "Операція не успішна", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                dB.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Text_enrollment_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isTimerReloadFinished = true;
            Action<decimal, int> action = new Action<decimal, int>(Add_cash);
            enrollment = new Enrollment(action,id_user,ref isTimerReloadFinished);
            enrollment.Show();
            Application.Current.MainWindow.Hide();
            choosen_funk = "enrollment";
        }

        private void Text_withdrawal_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void Button_exit_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RestoreSomeElement();
            TextBlock button_exit = sender as TextBlock;
            button_exit.MouseDown -= Button_exit_MouseDown;
        }

        public async void Success_Window()
        {
            TextBlock successTextBlock = new TextBlock();
            Grid.SetRow(successTextBlock, 0);
            Grid.SetColumn(successTextBlock, 0);
            Grid.SetRowSpan(successTextBlock, row_count / 2);
            Grid.SetColumnSpan(successTextBlock, column_count);
            successTextBlock.Text = "Authentication is succesfull :)";
            successTextBlock.FontSize = 44;
            successTextBlock.FontWeight = FontWeights.UltraBold;
            successTextBlock.Foreground = Brushes.White;
            successTextBlock.FontStyle = FontStyles.Italic;
            successTextBlock.TextDecorations = TextDecorations.Underline;
            successTextBlock.TextAlignment = TextAlignment.Center;
            successTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            successTextBlock.Tag = "authentication_block";
            successTextBlock.Margin = new Thickness(0, 200, 0, 0);
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Red,
                BlurRadius = 60,
                ShadowDepth = 0,
                Opacity = 1
            };
            successTextBlock.Effect = dropShadowEffect; 
            MyGrid.Children.Add(successTextBlock);
            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "LoadingBar";
            progressBar.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            progressBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            progressBar.Width = MyGrid.ActualWidth - 100;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Height = 25;
            progressBar.Orientation = Orientation.Horizontal;
            progressBar.Foreground = Brushes.Green;
            progressBar.Background = Brushes.DarkGray;
            Grid.SetRow(progressBar, row_count / 2);
            Grid.SetColumn(progressBar, 0);
            Grid.SetRowSpan(progressBar, row_count / 2);
            Grid.SetColumnSpan(progressBar, column_count);
            progressBar.Value = 0;
            
            MyGrid.Children.Add(progressBar);
            background_timer = new Timer(Second_step_Authentication, MyGrid, 1000, Timeout.Infinite);
            for(int i = 0; i <= 100; i++)
            {
                progressBar.Value = i;
                await Task.Delay(10);
            }
        }

        public void Second_step_Authentication(object state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if(state is Grid myGrid)
                {
                    for (int i = myGrid.Children.Count - 1; i >= 0; i--)
                    {
                        if (myGrid.Children[i] is TextBlock textBlock)
                        {
                            if(textBlock.Tag != null)
                            {
                                if (textBlock.Tag.ToString() == "authentication_block")
                                {
                                    myGrid.Children.Remove(textBlock);
                                }
                            }
                        }
                        else if (myGrid.Children[i] is ProgressBar progress)
                        {
                            if(progress.Name != null)
                            {
                                if (progress.Name == "LoadingBar")
                                {
                                    myGrid.Children.Remove(progress);
                                }
                            }
                        }
                    }
                    
                }

                isTimerFinished = true;
                StartUsingBank();
            });
            

        }
        public void StartUsingBank()
        {
            if (isTimerFinished == true)
            {
                Image background = MyGrid.FindName("background_image") as Image;
                ImageBehavior.SetAnimatedSource(background, null);
                background.Stretch = Stretch.UniformToFill;

                if (background != null)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri("pack://application:,,,/Library_for_bank;component/image/background_image.jpg", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    background.Source = image;
                }
            }
            Image img_logout = new Image();
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("pack://application:,,,/Library_for_bank;component/image/icon_logout.png", UriKind.RelativeOrAbsolute);
            img.EndInit();
            img_logout.Source = img;
            img_logout.Margin = new Thickness(8,5,0,5);
            img_logout.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            img_logout.Stretch = Stretch.Uniform;
            Grid.SetRow(img_logout, 1);
            Grid.SetColumn(img_logout, 14);
            Grid.SetRowSpan(img_logout, 1);
            Grid.SetColumnSpan(img_logout, 1);
            MyGrid.Children.Add(img_logout);
            Elements_new.Add(img_logout);
            img_logout.MouseDown += Img_logout_MouseDown;
            Border border_balance = new Border();
            border_balance.CornerRadius = new CornerRadius(20);
            border_balance.BorderThickness = new Thickness(2);
            border_balance.BorderBrush = Brushes.White;
            border_balance.Background = Brushes.Black;
            border_balance.Opacity = 0.9;
            border_balance.Effect = new DropShadowEffect
            {
                Color = Colors.White,
                BlurRadius = 60,
                ShadowDepth = 0,
                Opacity = 1
            };
            border_balance.Margin = new Thickness(0, 15, 0, 15);
            Panel.SetZIndex(border_balance, 1);
            Grid.SetRow(border_balance, 0);
            Grid.SetColumn(border_balance, 9);
            Grid.SetColumnSpan(border_balance, 4);
            Grid.SetRowSpan(border_balance, 2);
            Border border_withdrawal = new Border
            {
                Background = Brushes.Black,
                CornerRadius = new CornerRadius(20),
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.White,
                Opacity = 0.9,
                Effect = new DropShadowEffect
                {
                    Color = Colors.White,
                    BlurRadius = 60,
                    ShadowDepth = 0,
                    Opacity = 1
                },
                Margin = new Thickness(8,0,8,0)
            };
            Panel.SetZIndex(border_withdrawal, 1);
            Grid.SetRow(border_withdrawal, 4);
            Grid.SetColumn(border_withdrawal, 2);
            Grid.SetRowSpan(border_withdrawal, 4);
            Grid.SetColumnSpan(border_withdrawal, 5);
            Border border_enrollment = new Border
            {
                Background = Brushes.Black,
                CornerRadius = new CornerRadius(20),
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.White,
                Opacity = 0.9,
                Effect = new DropShadowEffect
                {
                    Color = Colors.White,
                    BlurRadius = 60,
                    ShadowDepth = 0,
                    Opacity = 1
                },
                Margin = new Thickness(28, 0, 8, 8)
            };
            Panel.SetZIndex(border_enrollment, 1);
            Grid.SetRow(border_enrollment, 4);
            Grid.SetColumn(border_enrollment, 7);
            Grid.SetRowSpan(border_enrollment, 2);
            Grid.SetColumnSpan(border_enrollment, 5);
            Border border_transfer = new Border
            {
                Background = Brushes.Black,
                CornerRadius = new CornerRadius(20),
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.White,
                Opacity = 0.9,
                Effect = new DropShadowEffect
                {
                    Color = Colors.White,
                    BlurRadius = 60,
                    ShadowDepth = 0,
                    Opacity = 1
                },
                Margin = new Thickness(28, 8, 8, 0)
            };
            Panel.SetZIndex(border_transfer, 1);
            Grid.SetRow(border_transfer, 6);
            Grid.SetColumn(border_transfer, 7);
            Grid.SetRowSpan(border_transfer, 2);
            Grid.SetColumnSpan(border_transfer, 5);
            MyGrid.Children.Add(border_transfer);
            MyGrid.Children.Add(border_enrollment);
            MyGrid.Children.Add(border_withdrawal);
            MyGrid.Children.Add(border_balance);
            Elements_new.Add(border_transfer);
            Elements_new.Add(border_enrollment);
            Elements_new.Add(border_withdrawal);
            Elements_new.Add(border_balance);
            text_balance = new TextBlock
            {
                Foreground = Brushes.White,
                Text = "my_balance",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 20,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.Bold,
                Effect = new DropShadowEffect
                {
                    Color = Colors.DarkRed,
                    BlurRadius = 1,
                    ShadowDepth = 10,
                    Opacity = 1
                },
                Margin = new Thickness(20, 0, 0, 0)
            };
            Panel.SetZIndex(text_balance, 2);
            Grid.SetRow(text_balance, 0);
            Grid.SetColumn(text_balance, 9);
            Grid.SetRowSpan(text_balance, 2);
            Grid.SetColumnSpan(text_balance, 4);
            text_withdrawal = new TextBlock
            {
                Foreground = Brushes.White,
                Text = "Зняття",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 50,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.Bold,
                Effect = new DropShadowEffect
                {
                    Color = Colors.White,
                    BlurRadius = 4,
                    ShadowDepth = 10,
                    Opacity = 1
                },
                
            };
            Panel.SetZIndex(text_withdrawal, 2);
            Grid.SetRow(text_withdrawal, 4);
            Grid.SetColumn(text_withdrawal, 2);
            Grid.SetRowSpan(text_withdrawal, 4);
            Grid.SetColumnSpan(text_withdrawal, 5);
            text_enrollment = new TextBlock
            {
                Foreground = Brushes.White,
                Text = "Зарахування",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 45,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.Bold,
                Effect = new DropShadowEffect
                {
                    Color = Colors.White,
                    BlurRadius = 4,
                    ShadowDepth = 10,
                    Opacity = 1
                },
                
                Margin = new Thickness(20, 0, 0, 16)
            };
            Panel.SetZIndex(text_enrollment, 2);
            Grid.SetRow(text_enrollment, 4);
            Grid.SetColumn(text_enrollment, 7);
            Grid.SetRowSpan(text_enrollment, 2);
            Grid.SetColumnSpan(text_enrollment, 5);
            text_transfer = new TextBlock
            {
                Foreground = Brushes.White,
                Text = "Переказ",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                FontSize = 50,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.Bold,
                Effect = new DropShadowEffect
                {
                    Color = Colors.White,
                    BlurRadius = 4,
                    ShadowDepth = 10,
                    Opacity = 1
                },
                
                Margin = new Thickness(20,0,0,0)
            };
            
            Panel.SetZIndex(text_transfer, 2);
            Grid.SetRow(text_transfer, 6);
            Grid.SetColumn(text_transfer, 7);
            Grid.SetRowSpan(text_transfer, 2);
            Grid.SetColumnSpan(text_transfer, 5);

            text_balance.Name = "textblock_balance";
            text_enrollment.Name = "textblock_enrollment";
            text_withdrawal.Name = "textblock_withdrawal";
            text_transfer.Name = "textblock_transfer";
            text_enrollment.Background = Brushes.DarkRed;
            text_withdrawal.Background = Brushes.DarkRed;
            text_transfer.Background = Brushes.DarkRed;
            text_withdrawal.Focusable = true;
            text_transfer.Focusable = true;
            text_enrollment.Focusable = true;
            text_withdrawal.IsHitTestVisible = true;
            text_transfer.IsHitTestVisible = true;
            text_transfer.IsHitTestVisible = true;
            
            
            MyGrid.Children.Add(text_transfer);
            MyGrid.Children.Add(text_enrollment);
            MyGrid.Children.Add(text_withdrawal);
            MyGrid.Children.Add(text_balance);
            MyGrid.InvalidateVisual();
            MyGrid.UpdateLayout();
            text_withdrawal.MouseDown += Text_withdrawal_MouseDown;
            text_enrollment.MouseDown += Text_enrollment_MouseDown;
            text_transfer.MouseDown += Text_transfer_MouseDown;
            Elements_new.Add(text_transfer);
            Elements_new.Add(text_enrollment);
            Elements_new.Add(text_withdrawal);
            Elements_new.Add(text_balance);
            
            timer_reload_menu = new Timer(Reload_Bank, MyGrid, 0, 5000);
        }
        public void Reload_Bank(object sender)
        {
            if (choosen_funk == "enrollment" || choosen_funk == "transfer" || choosen_funk == "withdrawal")
            {
                if (choosen_funk == "enrollment")
                {
                    isTimerReloadFinished = enrollment.IsTimerReloadFinished;
                }
                else if (choosen_funk == "transfer")
                {
                    isTimerReloadFinished = transfer.IsTimerReloadFinished;
                }
                else if (choosen_funk == "withdrawal")
                {

                }
            }
            if (isTimerReloadFinished == false)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DB db = new DB();
                    db.openConnection();
                    string query = "select (balance) from users where id = @s1";
                    using (MySqlCommand cmd = new MySqlCommand(query, db.getConnection()))
                    {
                        cmd.Parameters.Add("@s1", MySqlDbType.Int32).Value = id_user;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                text_balance.Text = reader.GetDecimal("balance").ToString();
                            }
                        }
                    }
                    db.closeConnection();

                    MyGrid.InvalidateVisual();
                });
            }
            
        }
        private void Img_logout_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RestoreSomeElement();
        }

        public void RestoreSomeElement()
        {
            foreach (UIElement element in Elements_new)
            {
                //if (element is FrameworkElement child)
                //{
                //    string name = element.GetValue(FrameworkElement.NameProperty) as string;
                //    if (!string.IsNullOrEmpty(name))
                //    {
                //        MyGrid.UnregisterName(name);
                //    }

                //}
                MyGrid.Children.Remove(element);
            }
            foreach (UIElement element in Elements_old)
            {
                MyGrid.Children.Add(element);
            }
            Image image = MyGrid.FindName("background_image") as Image;
            if(image != null)
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri("pack://application:,,,/Library_for_bank;component/image/Background_gif.gif", UriKind.RelativeOrAbsolute);
                img.EndInit();
                ImageBehavior.SetAnimatedSource(image, img);
            }
            timer_reload_menu.Dispose();
        }
    }
}
