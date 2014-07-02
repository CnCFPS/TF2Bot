// -----------------------------------------------------------------------------
//  <copyright file="TF2Bot.cs" company="CnCFPS">
//      Copyright (c) CnCFPS.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TF2Bot.Core
{
    using Atlantis.Net.Irc;
    using Lantea.Common.Extensibility;
    using Lantea.Common.IO;

    // ReSharper disable InconsistentNaming

    public class TF2Bot : IModule
    {
        #region Implementation of IModule

        public string Author
        {
            get { return "Zack Loveless"; }
        }

        public string Description
        {
            get { return "A TF2 server regulator."; }
        }

        public string Name
        {
            get { return "TF2Bot"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        public void Dispose()
        {
        }

        public void Initialize(Block config, IrcClient client)
        {
            // 
        }

        #endregion
    }

    // ReSharper restore InconsistentNaming
}
