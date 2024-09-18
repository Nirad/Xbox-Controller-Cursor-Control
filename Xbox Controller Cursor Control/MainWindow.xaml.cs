
using System.Windows;

using System.Windows.Input;
using System.Windows.Media;

using System.Windows.Threading;

namespace Xbox_Controller_Mouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XController xController = new XController();
        public MainWindow()
        {
            InitializeComponent();
            lblStatus.Content = "Controller not connected";
            lblStatus.Foreground = Brushes.Red;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }


        private void Init()
        {
            xController.ControllerStatusChanged += OnControllerStatusChanged!;
            Task.Run(() => xController.Start());
        }

        private void OnControllerStatusChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(UpdateStatus); // Met à jour l'UI sur le thread principal
        }
        private void UpdateStatus()
        {
            // Exécuter sur le thread principal pour mettre à jour l'interface utilisateur
            if (xController.isConnected)
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            xController.moveStep = (int)sliderMouseSptep.Value;
        }

        private void sliderScrollSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            xController.WHEEL_DELTA = (int)sliderScrollSpeed.Value;
        }
    }
}