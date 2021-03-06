﻿// -----------------------------------------------------------------------------
//  <copyright file="ITcpClient.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Net
{
	public interface ITcpClient
	{
		bool Connected { get; }

		bool DataAvailable { get; }

		bool EndOfStream { get; }

		void Connect(string host, int port);

		void Close();

		string ReadLine();

		string ReadAll();

		void Write(string format, params object[] args);

		void WriteLine(string format, params object[] args);
	}
}
