using Autofac;
using ReactiveUI;
using System;
using WpfGroupFinder.Logic;
using WpfGroupFinder.Models;
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
			containerBuilder.RegisterType<UpdateTimer>().As<IUpdateTimer>().SingleInstance();
			containerBuilder.RegisterType<GroupParser>().As<IGroupParser>();
			containerBuilder.RegisterType<FileHandler>().As<IFileHandler>();
			containerBuilder.RegisterType<RaidCategorizer>().As<IRaidCategorizer>();
			containerBuilder.RegisterType<MainWindow>();
			containerBuilder.RegisterType<MainViewModel>();

			containerBuilder.RegisterInstance(new Languages("Deutsch", "de", "de"));
			containerBuilder.RegisterInstance(new Languages("English", "en", "us"));

			var container = containerBuilder.Build();
			var app = new App();
			app.InitializeComponent();
			var mainWindow = container.Resolve<MainWindow>();
			app.Run(mainWindow);
		}
	}
}