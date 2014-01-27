// -----------------------------------------------------------------------------
//  <copyright file="RawMessageEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;

	public class RawMessageEventArgs : EventArgs
	{
		public RawMessageEventArgs(string message)
		{
			Message = message;
		}

		/// <summary>
		/// Gets a <see cref="T:System.String" /> value representing the message received from the IRC server.
		/// </summary>
		public string Message { get; set; }
	}
}
