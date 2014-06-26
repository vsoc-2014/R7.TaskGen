//  
//  Utils.cs
//  
//  Author:
//       redhound <>
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
using System.Reflection;

namespace R7.TaskGen
{
	public static class Utils
	{
		public static string FormatTitle ()
		{
			// get assembly name and version and put it on title
			AssemblyName n = Assembly.GetEntryAssembly ().GetName ();
			return string.Format ("{0} v{1}.{2}.{3}", 
				n.Name, n.Version.Major, n.Version.Minor, n.Version.Build);
		}

		#region Platform flags

		public static bool OnUnix
		{
			get
			{
				return Environment.OSVersion.Platform == PlatformID.Unix;
			}
		}

		public static bool OnWindows
		{
			get
			{
				return Environment.OSVersion.Platform <= PlatformID.WinCE;
			}
		}

		public static bool OnMac
		{
			get
			{
				return Environment.OSVersion.Platform == PlatformID.MacOSX;
			}
		}

		#endregion
	}
}

