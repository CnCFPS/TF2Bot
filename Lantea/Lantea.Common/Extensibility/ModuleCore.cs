﻿// -----------------------------------------------------------------------------
//  <copyright file="ModuleCore.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System.ComponentModel.Composition.Hosting;
	using IO;

	public abstract class ModuleCore : IModule
	{
		protected ModuleCore(IBotCore bot, Configuration config)
		{
			Bot = bot;
			Config = config;
		}

		#region Implementation of IModuleAttribute

		public abstract string Author { get; }

		public abstract string Description { get; }

		public abstract string Name { get; }

		public abstract string Version { get; }

		public abstract ModuleType ModuleType { get; }

		#endregion

		#region Implementation of IModule

		public IBotCore Bot { get; private set; }

		public Configuration Config { get; private set; }

		public virtual void Load()
		{
			var container = new CompositionContainer(new AssemblyCatalog(GetType().Assembly));
			
			var commands  = container.GetExportedValues<ICommand>();
			foreach (ICommand item in commands)
			{
				Bot.CommandManager.Register(item);
			}
		}

		#endregion
	}
}
