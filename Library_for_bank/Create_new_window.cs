using FluentTextTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
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
        public List<UIElement> Elements { get; private set; }
        public Grid MyGrid { get; private set; }
        private Timer background_timer;
        private int row_count;
        private int column_count;
        private bool isTimerFinished = false;
        public Create_new_window(List<UIElement> elements, Grid myGrid)
        {
            Elements = new List<UIElement>();
            Elements = elements;
            MyGrid = myGrid;
            row_count = MyGrid.RowDefinitions.Count;
            column_count = MyGrid.ColumnDefinitions.Count;
            Success_Window();
            
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
            successTextBlock.Foreground = Brushes.Black;
            successTextBlock.FontStyle = FontStyles.Italic;
            successTextBlock.TextDecorations = TextDecorations.Underline;
            successTextBlock.TextAlignment = TextAlignment.Center;
            successTextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            successTextBlock.Tag = "authentication_block";
            successTextBlock.Margin = new Thickness(0, 200, 0, 0);
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.White,
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
            background_timer = new Timer(Second_step_Authentication, MyGrid, 4500, Timeout.Infinite);
            for(int i = 0; i <= 100; i++)
            {
                progressBar.Value = i;
                await Task.Delay(40);
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
                    image.UriSource = new Uri("pack://application:,,,/Library_for_bank;component/Image/background_image.jpg", UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    background.Source = image;
                }
            }
        }
        public void RestoreSomeElement()
        {
            foreach(UIElement element in Elements)
            {
                MyGrid.Children.Add(element);
            }
        }
    }
}
