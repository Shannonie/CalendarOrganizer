using System;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using CalendarOrganizer.UI.Startup;
using ControlzEx.Theming;

namespace CalendarOrganizer.UI
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // using lib:Autofac for dependency injection
            Bootstrapper bootstrapper = new Bootstrapper();
            IContainer container = bootstrapper.Bootstrap();
            MainWindow mainWindow = container.Resolve<MainWindow>();

            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, 
            DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
