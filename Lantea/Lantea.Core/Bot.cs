﻿// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Atlantis.IO;
	using Atlantis.Linq;
	using Atlantis.Net.Irc;
	using Common.Extensibility;
	using Common.IO;

	// ReSharper disable InconsistentNaming
	public class Bot : IBotCore, IModuleLoader
	{
		private void LoadIRC()
		{
			Block connection = Config.GetBlock("connection");
			if (connection == null)
			{
				throw new Exception("No connection block found.");
			}

			String clientNick = connection.Get("nick", "Lantea");

			Client = new IrcClient(clientNick)
			{
				Host = connection.Get("server", "127.0.0.1"),
				Port = connection.Get("port", 6667),
				RealName = connection.Get("name", clientNick),
			};
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (!disposing) return;

			if (Client != null) Client.Dispose();
		}

		#endregion

		#region Implementation of IBotCore

		public IrcClient Client { get; private set; }

		public Configuration Config { get; private set; }

		public ILog Log { get; private set; }

		public IModuleLoader ModuleLoader
		{
			get { return this; }
		}

		// ReSharper disable AssignNullToNotNullAttribute
		public void Initialize(String configFile)
		{
			Config = new Configuration();
			Config.Load(configFile);

			LoadIRC();

			var loc = Assembly.GetEntryAssembly().Location;
			String path = Path.GetDirectoryName(loc);
			String moduleDirectory = Path.Combine(path, "Extensions");

			Block modules = Config.GetBlock("modules");
			if (modules != null)
			{
				moduleDirectory = modules.Get("directory", moduleDirectory);
			}

			// moduleDirectory = Path.Combine(path, moduleDirectory);
			String logPath  = Path.Combine(path, "logs");

			if (!Directory.Exists(logPath))
			{
				Directory.CreateDirectory(logPath);
			}

			Log = new MultiLog(new ConsoleLog(Console.Write), new AutoFileLog(logPath));

			Client.Start();

			ModuleLoader.ModulesLoadedEvent += OnModulesLoaded;
			ModuleLoader.LoadModules(moduleDirectory);
		}

		private void OnModulesLoaded(object sender, ModulesLoadedEventArgs args)
		{
			Log.Info("Modules loaded.");
		}

		public void Load(String moduleName)
		{
		}

		#endregion

		#region Implementation of IModuleLoader

		public event EventHandler<ModulesLoadedEventArgs> ModulesLoadedEvent;

		public IEnumerable<IModule> Modules { get; private set; }

		public void LoadModules(String directory)
		{
			if (File.Exists(directory))
			{
				throw new DirectoryNotFoundException(String.Format("Unable to load modules from the specified path: {0}\nA file was given.", directory));
			}

			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException(String.Format("Unable to load modules from the specified path: {0}\nThe directory does not exist.", directory));
			}

			var catalog   = new DirectoryCatalog(directory);
			var container = new CompositionContainer(catalog);

			container.ComposeExportedValue<IBotCore>(this);
			container.ComposeExportedValue(Config);

			var composedModules   = container.GetExports<IModule, IModuleAttribute>().ToList();
			List<IModule> modules = new List<IModule>();

			Log.Info("Loading modules.");
			foreach (var item in composedModules)
			{
				IModule value = item.Value;
				value.Load();

				Log.InfoFormat("Loaded: {0} (v{1}) by {2}", item.Metadata.Name, item.Metadata.Version, item.Metadata.Author);

				modules.Add(value);
			}

			Modules = modules;
			ModulesLoadedEvent.Raise(this, new ModulesLoadedEventArgs(Modules));
		}

		#endregion
	}
	// ReSharper restore InconsistentNaming
}