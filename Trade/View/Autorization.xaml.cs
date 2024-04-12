using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Trade.View;
using static System.Net.Mime.MediaTypeNames;

namespace Trade.View
{

    public partial class Autorization : Window
    {
        private readonly double letterWidth = 40;
        private readonly Random random = new Random();
        private readonly DataBase.TradeEntities entities;
        private DataBase.User user;
        private bool isRequareCaptch;
        private string captchaCode;
        private readonly string captchaSymbols = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890";
        private int charStartPosition;
        public Autorization()
        {
            InitializeComponent();
            random = new Random(Environment.TickCount);
            entities = new DataBase.TradeEntities();

        }
        private void OnSignIn(object sender, RoutedEventArgs e)
        {
            if (isRequareCaptch && captchaCode.ToLower() == tbCaptcha.Text.Trim().ToLower())
            {

                return;
            }
            string login = tbLogin.Text.Trim();
            string password = tbPassword.Password.Trim();
            if (login.Length < 1 || password.Length < 1)
            {
                MessageBox.Show("Необходимо ввести логин и пароль");
                return;
            }

            user = entities.Users.Where(u => u.UserLogin == login && u.UserPassword == password).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("Некорректный логин/пароль");
                generateCaptcha();
                return;
            }
            if (isRequareCaptch)
            {
                isRequareCaptch = false;
            }
            switch (user.Role.RoleName)
            {
                case "Администратор":
                    ProductView productView = new ProductView(entities, user);
                    productView.Owner = this;
                    productView.Show();
                    break;
                case "Менеджер":
                    break;
                case "Клиент":
                    break;
            }

        }
        private void generateCaptcha()
        {
            captchaCode = getNewCaptchaCode();
            for (int i = 0; i < captchaCode.Length; i++)
            {
                AddCharToCanvas(i, captchaCode[i]);

            }
            GenerageNoize();
        }
        private string getNewCaptchaCode()
        {
            canvas.Children.Clear();

            string code = "";
            for (int i = 0; i < 4; i++)
            {
                code += captchaSymbols[random.Next(captchaSymbols.Length)];
            }
            return code;
        }
        private void AddCharToCanvas(int index, char ch)
        {
            Label label = new Label();
            label.Content = ch.ToString();
            label.FontSize = random.Next(18, 24);
            label.FontWeight = FontWeights.Black;
            label.Foreground = GetRandomBrush();
            label.Width = letterWidth;
            label.Height = 60;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.RenderTransformOrigin = new Point(0.5, 0.5);
            label.RenderTransform = new RotateTransform(random.Next(-20, 15));


            canvas.Children.Add(label);

            int startPosition = (int)((canvas.ActualWidth / 2) - (30 * 4 / 2));
            Canvas.SetLeft(label, startPosition + (index * letterWidth));
            Canvas.SetTop(label, random.Next(0, 10));

        }
        private void GenerageNoize()
        {
            for (int i = 1; i < 100; i++)
            {
                double x = random.NextDouble() * canvas.ActualWidth;
                double y = random.NextDouble() * canvas.ActualHeight;
                int radius = random.Next(2, 5);
                Ellipse ellipse = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = GetRandomBrush((byte)random.Next(100, 180)),
                    Stroke = Brushes.Transparent,
                };
                canvas.Children.Add(ellipse);
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);



            }
            int lineCount = random.Next(3, 8);
            for (int i = 0; i < lineCount; i++)
            {
                Line line = new Line();


                line.X1 = random.Next(100, 120);
                line.Y1 = random.Next(10, 54);
                line.X2 = random.Next(260, 280);
                line.Y2 = random.Next(10, 54);
                line.Stroke = GetRandomBrush();
                line.StrokeThickness = random.Next(2, 4);
                canvas.Children.Add(line);
            }
        }
        private SolidColorBrush GetRandomBrush(byte alpha = 255)
        {
            return new SolidColorBrush(Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)));
        }
    }
}
