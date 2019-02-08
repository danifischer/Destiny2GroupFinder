using Autofac;
using ReactiveUI;
using System;
using WpfGroupFinder.ViewModels;

namespace WpfGroupFinder
{
	public static class BootStrapper
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterType<MessageBus>().As<IMessageBus>().SingleInstance();
			containerBuilder.RegisterType<MainWindow>();
			containerBuilder.RegisterType<MainViewModel>();

			var container = containerBuilder.Build();
			var app = new App();
			var mainWindow = container.Resolve<MainWindow>();
			app.Run(mainWindow);
		}
	}
}