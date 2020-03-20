using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Windows;

namespace Joseph.ServiceTrayMonitor
{
    /// <summary>
    /// ServiceSelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServiceSelectWindow
    {
        public MainWindow parentWindow;
        public ObservableCollection<ServiceController> services;

        public ServiceSelectWindow(MainWindow parentWindow)
        {
            this.parentWindow = parentWindow;
            InitializeComponent();
            services = ServiceController.GetServices().ToList().ToObservableCollection();
            servicesListView.DataContext = services;
        }



        private void OnWindowClosed(object sender, EventArgs e)
        {
            parentWindow.serviceSelectWindow = null;
            parentWindow.Show();
            parentWindow.Topmost = true;
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            parentWindow.TrayDisplayName = ((ServiceController)servicesListView.SelectedItem)
                ?.DisplayName ?? string.Empty;
            parentWindow.ServiceName = ((ServiceController) servicesListView.SelectedItem)
                ?.ServiceName ?? string.Empty;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e) => Close();
    }
}
