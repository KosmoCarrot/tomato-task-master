using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TomatoTaskMaster.Model;

namespace TomatoTaskMaster.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<ObservableCollection<Task>>>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage<ObservableCollection<Task>> msg)
        {
            switch (msg.Notification)
            {
                case "OpenTomatoWindow":
                    TomatoWindow tomatoWindow;

                    if (Application.Current.Windows.OfType<TomatoWindow>().Any())
                        tomatoWindow = Application.Current.Windows.OfType<TomatoWindow>().First();
                    else
                        tomatoWindow = new TomatoWindow();

                    Close();
                    tomatoWindow.Show();
                    break;

                case "ShowTasksForDate":
                    lstNewTasks.ItemsSource = msg.Content;
                    break;
            }
        }
    }
}
