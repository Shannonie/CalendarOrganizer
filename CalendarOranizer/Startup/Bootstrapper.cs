using Autofac;
using CalendarOrganizer.DataAccess;
using CalendarOrganizer.UI.Data.Lookups;
using CalendarOrganizer.UI.Data.Repositories;
using CalendarOrganizer.UI.View.Services;
using CalendarOrganizer.UI.ViewModel;
using Prism.Events;

namespace CalendarOrganizer.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<TaskOrganizeDbContext>().AsSelf();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<TaskViewModel>().Keyed<IDetailViewModel>(nameof(TaskViewModel));
            builder.RegisterType<TaskTypeViewModel>().Keyed<IDetailViewModel>(nameof(TaskTypeViewModel));
            builder.RegisterType<DateTasksViewModel>().Keyed<IDetailViewModel>(nameof(DateTasksViewModel));

            builder.RegisterType<MessageDialogServices>().As<IMessageDialogServices>();
            builder.RegisterType<LookUpDbItems>().AsImplementedInterfaces();
            builder.RegisterType<TaskRepository>().As<ITaskRepository>();
            builder.RegisterType<TaskTypeRepository>().As<ITaskTypeRepository>();
            builder.RegisterType<DateTasksRepository>().As<IDateTasksRepository>();

            return builder.Build();
        }
    }
}
