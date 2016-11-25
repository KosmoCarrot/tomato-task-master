using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Windows;

namespace TomatoTaskMaster.View
{
    public partial class TomatoWindow 
    {
        public TomatoWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            switch (msg.Notification)
            {
                case "OpenMainWindow":
                    MainWindow mainWindow;

                    if (Application.Current.Windows.OfType<MainWindow>().Any())
                        mainWindow = Application.Current.Windows.OfType<MainWindow>().First();
                    else
                        mainWindow = new MainWindow();

                    Close();
                    mainWindow.Show();
                    break;

                case "SetTomatoWindowPosition":
                    Left = SystemParameters.PrimaryScreenWidth - Width;
                    break;
            }
        }
    }   
}
