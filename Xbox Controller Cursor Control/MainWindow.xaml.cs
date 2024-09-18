using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace Xbox_Controller_Mouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XController xController;
        public MainWindow()
        {
            InitializeComponent();
            lblStatus.Content = "Controller not connected";
            lblStatus.Foreground = Brushes.Red;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            // Configurez un Timer ou un autre mécanisme pour mettre à jour l'interface utilisateur
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Mettez à jour toutes les secondes
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void Init()
        {
            xController = new XController();
            Task.Run(() => xController.Start());
        }

        private void UpdateStatus()
        {
            // Exécuter sur le thread principal pour mettre à jour l'interface utilisateur
            if (xController.success)
            {
                lblStatus.Content = "Controller connected";
                lblStatus.Foreground = Brushes.Green;
            }
            else
            {
                lblStatus.Content = "Controller not connected";
                lblStatus.Foreground = Brushes.Red;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}