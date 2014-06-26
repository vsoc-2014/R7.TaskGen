//
//  Configuration.cs
//
//  Author:
//       redhound <${AuthorEmail}>
//
//  Copyright (c) 2012 redhound
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Configuration;

using Nini.Config;

namespace R7.TaskGen
{
	// TODO: Make PetaPOCO read DbProviderFactories and connectionStrings from user.config (check first, maybe it already read)

	public class AppConfig
	{
		private static IConfigSource configSource;
		private static IConfig appSettings;

		static AppConfig ()
		{
			var userConfigFile = Path.Combine (ApplicationData, "user.config");

			if (!File.Exists (userConfigFile))
				File.Copy (DotNetConfigSource.GetFullConfigPath (), userConfigFile);

			configSource = new DotNetConfigSource(userConfigFile);
			appSettings = configSource.Configs ["appSettings"];
		}

		/// <summary>
		/// Saves changes to user.config
		/// </summary>
		public static void Save()
		{
			configSource.Save ();
		}

		public static string ApplicationData
		{
			get { return Path.Combine (
					Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "taskgen"); 
			}
		}

		public static string Platform 
		{
			get { return appSettings.Get("platform"); }
			set { appSettings.Set ("platform", value); }
		}

		public static bool OnUnix
		{
			get { return appSettings.Get("platform") == "Unix"; }
		}

		public static bool OnWindows
		{
			get { return appSettings.Get("platform") == "Windows"; }
		}

		public static string StarterApp
		{
			get { return appSettings.Get("starterApp"); }
		}

		public static string BrowserApp
		{
			get { return appSettings.Get("browserApp"); }
		}

		public static string HelpViewerApp
		{
			get { return appSettings.Get("helpViewerApp"); }
		}

	} // class
} // namespace

