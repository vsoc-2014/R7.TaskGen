//
//  DataProvider.cs
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
using PetaPoco;
using System.IO;

namespace R7.TaskGen
{
	public class DataProvider
	{
		private static string ConnectionString
		{
			get
			{
				return string.Format ("URI=file:{0}/r7.taskgen/taskgen.sqlite",
				                      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			}
		}
		static DataProvider()
		{
			String dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			dir = dir +"/r7.taskgen";
			String dir1 = dir+"/books";
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
				File.Copy ("/usr/local/lib/r7.taskgen/App_Data/taskgen.sqlite", dir);
				File.Copy ("/usr/local/lib/r7.taskgen/R7.TaskGen.exe.config", dir);
			}

			if (!Directory.Exists (dir1))
			{
				Directory.CreateDirectory (dir1);

				File.Copy ("/usr/local/lib/r7.taskgen/books/chulyukov_va.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/abramyan_me_linq.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/informatics.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/yurkin_ag.djvu", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/abramyan_me.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/zlatopolsky_dm.djvu", dir1);
			}

			/*String dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			dir = dir +"/r7.taskgen";
			String dir1 = dir+"/books";
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);

				File.Copy ("/usr/local/lib/r7.taskgen/App_Data/taskgen.sqlite", dir);
				File.Copy ("/usr/local/lib/r7.taskgen/R7.TaskGen.exe.config", dir);

			}

			if (!Directory.Exists (dir1))
			{
				Directory.CreateDirectory (dir1);

				File.Copy ("/usr/local/lib/r7.taskgen/books/chulyukov_va.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/abramyan_me_linq.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/informatics.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/yurkin_ag.djvu", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/abramyan_me.pdf", dir1);
				File.Copy ("/usr/local/lib/r7.taskgen/books/zlatopolsky_dm.djvu", dir1);

			}*/
		}

		private static Database db = null;
		internal static Database Database
		{
			get
			{
				// return db ?? (db = new Database (ConnectionString, "SQLite"));
				return db ?? (db = new Database (ConnectionString, Mono.Data.Sqlite.SqliteFactory.Instance));
				// with this, we can remove system.data section from app.config,
				// but is is platform-dependable

			}
		}

	} // class
} // namespace