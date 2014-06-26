//
//  MainForm.cs
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
//`
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
#undef TRACE
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;
using Gtk;
using Mono.Unix;

namespace R7.TaskGen
{
	public partial class MainForm : Gtk.Window
	{
		private SizeGroup sizegroupButtons;
		private StatusMessage statusMessage;

		public MainForm () : base(Gtk.WindowType.Toplevel)
		{
			Build ();
			Initialize ();
		}

		protected void Initialize ()
		{
			// add application name and 
			Title = Utils.FormatTitle ();

			LoadThemes ();
			LoadUsers ();

			labelName.TooltipText = cbeName.TooltipText;
			labelTheme.TooltipText = comboTheme.TooltipText;

			statusMessage = new StatusMessage (lblStatus, imgStatus);

			statusMessage.Message (Catalog.GetString ("Enter your name, theme and press \"My Variant\" button"), Stock.DialogInfo);
		}

		protected void LoadThemes ()
		{
			// load all themes
			lsThemes = DataProvider.Database.Fetch<ThemeInfo> (
				"SELECT * FROM Themes WHERE IsEnabled = 1 ORDER BY [Order] ASC"
			);

			// add 'not selected' item to theme combo
			comboTheme.AppendText (Catalog.GetString ("-- Not selected --"));
			comboTheme.Active = 0;

			// fill other themes
			foreach (var theme in lsThemes)
				comboTheme.AppendText (theme.Title);
		}

		protected void LoadUsers ()
		{
			// load all users
			lsUsers = DataProvider.Database.Fetch<UserInfo> ("SELECT * FROM Users");

			// Fill user name combobox
			var userNames = new ListStore (typeof(string));		

			foreach (UserInfo u in lsUsers)
			{
				cbeName.AppendText (u.Name);
				userNames.AppendValues (u.Name);
			}
			cbeName.Active = -1;

			cbeName.Entry.Completion = new EntryCompletion ();
			cbeName.Entry.Completion.Model = userNames;
			cbeName.Entry.Completion.TextColumn = 0;

			cbeName.Entry.Activated += new EventHandler (OnCbeNameEntryActivated);
		}

		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs a)
		{
			Gtk.Application.Quit ();
			a.RetVal = true;
		}

		/*
		private void AddTags (TextView tv)
		{
			TextBuffer b = tv.Buffer;
			b.TagTable.Add(new TextTag("bold"));
		}*/


		private int UniqueInt2 (string text)
		{
			var uid = 0;
			var m = 1;

			foreach (var t in text)
			{
				uid += m * t;
				m = m * 2;
			}

			return  uid;
		}

		// TODO: Make UniqueInt culture-independent

		private int UniqueInt (string text)
		{
			long uid = 0;
		
			int m = int.MaxValue / 32; // начальный множитель
			int t_mid = 'А' + ('Я' - 'А') / 2; // средний символ алфавита
			foreach (char t in text)
			{
				uid += m * (t - t_mid) + m / 2;
				// русская А
				m = m / 2;
			}
			return (int)(uid % int.MaxValue);
		}

		private string Mix (string s1, string s2, string s3)
		{
			string s = string.Empty;
			int len = s1.Length + s2.Length + s3.Length;
			int i = 0;
			int i1 = 0, i2 = 0, i3 = 0;
			
			while (i < len)
			{
				
				if (i1 < s1.Length)
				{
					s += s1 [i1];
					i1++;
					i++;
				}
				if (i2 < s2.Length)
				{
					s += s2 [i2];
					i2++;
					i++;
				}
				if (i3 < s3.Length)
				{
					s += s3 [i3];
					i3++;
					i++;
				}
			}
			
			return s;
		}

		private bool IsSomethingNear (bool[] a, bool x, int index, int delta)
		{
			int first = (index - delta >= 0) ? index - delta : 0;
			int last = (index + delta < a.Length) ? index + delta : a.Length - 1;
			
			for (int i = first; i <= last; i++)
			{
				int ii = i % a.Length;
				if (a [ii] == x)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Convert selected char in a string to uppercase
		/// </summary>
		/// <returns>
		/// string
		/// </returns>
		/// <param name='s'>
		/// input string
		/// </param>
		/// <param name='n'>
		/// character index
		/// </param>/
		private string CharToUpper (string s, int n)
		{
			char[] cs = s.ToCharArray ();
			cs [n] = cs [n].ToString ().ToUpper () [0];
			return new string (cs);
		}

		private List<TaskInfo> lsTasks;
		private	List<TaskInfo> lsTasksSelected;
		private List<ThemeInfo> lsThemes;
		private List<UserInfo> lsUsers;

		protected virtual void OnActionGenerateActivated (object sender, System.EventArgs e)
		{
			#region 

			// TODO: Do not correct entered name, just normalize it internally

			// TODO: Valid name regex (culture- or option- dependent)

			// TODO: Name normalization regex (remove spaces, ё => e)

			var sName = cbeName.ActiveText.ToLower ().Trim ();
			
			// remove duplicate whitespace
			sName = Regex.Replace (sName, @"\s+", " ");
			
			
			// filter invalid symbols
			// [^а-я-\s] - with "-"
			sName = Regex.Replace (sName, @"[^а-яё\s]", "", RegexOptions.IgnoreCase);
			
			if (!UserInfo.IsValidName (sName))
			{
				statusMessage.Message (
					Catalog.GetString ("Name was entered incorrectly!"),
					Catalog.GetString ("Name must consists from three parts - lastname (family name), firstname and othername, separated with spaces."),
					Stock.DialogError
				);

				cbeName.ErrorBell ();
				return;
			}

			var sNameParts = sName.Split (' ');

			sName = CharToUpper (sNameParts [0], 0) + " " + CharToUpper (sNameParts [1], 0) + " " + CharToUpper (sNameParts [2], 0);
			
			RememberUser (sName);

			// THINK: RU locale-specific
			// replace 'Ё' after remember, so it is equivalent to 'Е'
			sName = sName.Replace ("ё", "е").Replace ("Ё", "Е");
			
			sNameParts = sName.ToUpper ().Split (' ');
			//cbeName.Entry.Text = sName;
			
			//sName = sName.ToUpper();
		
			#endregion
			
			var uniqueInt = UniqueInt (
				Mix (sNameParts [0], sNameParts [1], sNameParts [2]));
		
			var rnd = new Random (uniqueInt);
			
			// roll dice three times
			rnd.Next ();
			rnd.Next ();
			rnd.Next (); // new version
						
			//int nTasksToSelect = 10;
			//int iGrouping = 2;
			//int ToSumComplexity = 10 * 5 - 3; // примерно 10 задач средней сложности (5) минус 5 (чуть меньше половинной сложности)

			if (comboTheme.Active == 0)
			{
				statusMessage.Message (
					Catalog.GetString ("You must choose a theme!"),
					Catalog.GetString ("You must choose a theme of work to generate variant for it."),	
					Stock.DialogError
				);

				comboTheme.ErrorBell ();
				return;
			}

			lsTasks = new List<TaskInfo> (100);
			lsTasksSelected = new List<TaskInfo> (10);
			
			ThemeInfo theme = null;
			foreach (ThemeInfo t in lsThemes)
			{
				if (t.Title == comboTheme.ActiveText)
				{
					theme = t;
					break;
				}
			}

			if (theme == null)
			{
				statusMessage.Message (
					Catalog.GetString ("Unexpected error!"),
					Catalog.GetString ("Unexpected error occured! Application restart recommended."),
					Stock.DialogError
				);

				comboTheme.ErrorBell ();
				return;
			}

			lsTasks = DataProvider.Database.Fetch<TaskInfo,ProblemBookSectionInfo,ProblemBookInfo> (
				"SELECT TA.*, PBS.*, PB.* FROM Tasks AS TA " +
				"INNER JOIN ThemeSectionPairs AS TSP " +
				"ON TA.ProblemBookSectionID = TSP.ProblemBookSectionID " +
				"INNER JOIN ProblemBookSections AS PBS " +
				"ON TA.ProblemBookSectionID = PBS.ProblemBookSectionID " +
				"INNER JOIN ProblemBooks AS PB " +
				"ON PBS.ProblemBookID = PB.ProblemBookID " +
				"WHERE TSP.ThemeID = @0 ORDER BY TA.[Order] ASC", theme.ThemeID);

			var nTasks = lsTasks.Count;

			if (nTasks == 0)
			{
				statusMessage.Message (
					Catalog.GetString ("No tasks for choosen theme!"),
					Catalog.GetString ("No tasks for choosen theme found in the database"),
					Stock.DialogWarning
				);
				
				comboTheme.ErrorBell ();
				return;
			}
						
			// массив маски выбранных задач
			// если taskMask[i] = true, задача выбрана
			var taskMask = new bool[nTasks];
			for (var i = 0; i < taskMask.Length; i++)
				taskMask [i] = false;
			
			var easyTasks = SelectTasks (2, 1, 3, taskMask, rnd);
			var hardTasks = SelectTasks (2, 9, 10, taskMask, rnd);
			var complexTasks = SelectTasks (2, 7, 8, taskMask, rnd);
			var commonTasks = SelectTasks (
				10 - complexTasks.Count - hardTasks.Count - easyTasks.Count, 
				4, 6, taskMask, rnd);
			
			lsTasksSelected.AddRange (easyTasks);
			lsTasksSelected.AddRange (hardTasks);
			lsTasksSelected.AddRange (complexTasks);
			lsTasksSelected.AddRange (commonTasks);
					
			lsTasksSelected.Sort (new TaskOrderComparer ());

			while (table2.Children.Length > 0)
				table2.Remove (table2.Children [0]);
						
			// CHECK: what is it for?
			if (table2.Children.Length < lsTasksSelected.Count)
			{
				for (var i = 0; i < lsTasksSelected.Count; i++)
				{
					var button = new Button ();
					table2.Add (button);
					
					var tableChild = table2 [button] as Table.TableChild;
					tableChild.XOptions = tableChild.YOptions = AttachOptions.Expand | AttachOptions.Fill;
					 
					var x = (uint)(i % 5) * 2 + 1;
					var y = (uint)(i / 5);

					tableChild.LeftAttach = x;
					tableChild.RightAttach = tableChild.LeftAttach + 1;
					tableChild.TopAttach = y;
					tableChild.BottomAttach = tableChild.TopAttach + 1;
					
					// add label
					var label = new Label ();
					table2.Add (label);
					
					tableChild = table2 [label] as Table.TableChild;

					label.UseMarkup = true;
					label.LabelProp = string.Format ("<b>{0,2}:</b>", i + 1);
					label.SetAlignment (1, 0.5f);
				
					tableChild.LeftAttach = x - 1;
					tableChild.RightAttach = tableChild.LeftAttach + 1;
					tableChild.TopAttach = y;
					tableChild.BottomAttach = tableChild.TopAttach + 1;
					
					label.Show ();
					button.Show ();
				} // for
			}

			statusMessage.Message (
				string.Format (Catalog.GetString ("{0} tasks choosen"), lsTasksSelected.Count),
				Stock.DialogInfo
			);

			sizegroupButtons = new SizeGroup (SizeGroupMode.Horizontal);
				
			// fill task buttons
			var ti = 0;
			foreach (var w in table2.Children)
			{
				if (w is Button)
				{
					// FIXME: Sort order is inverse!
					// must be: TaskInfo t = lsTasksSelected [ti++];

					var task = lsTasksSelected [lsTasksSelected.Count - ++ti];
					
					var button = w as Button;
					button.Label = task.ToString ();
					button.TooltipText = task.ToDetailString ();
					button.BorderWidth = 2;
					button.WidthRequest = 100;
					button.FocusOnClick = true;
					button.Clicked += new EventHandler (OnTaskButtonClicked);

					sizegroupButtons.AddWidget (button);

					#region Button coloring (disabled)

					// Button coloring is disabled - not supported by all GTK themes

					/* // color by complexity
					Gdk.Color color = new Gdk.Color (192, 255, 192);
					if (t.ComplexityLevel == TaskComplexity.Low)
						color = new Gdk.Color (255, 255, 255);
					else if (t.ComplexityLevel == TaskComplexity.Higher)
						color = new Gdk.Color (255, 255, 192);
					else if (t.ComplexityLevel == TaskComplexity.High)
						color = new Gdk.Color (255, 192, 192);
					ModifyButtonBg (b, color); 
					*/
					
					/* // color by taskbook
					Gdk.Color color = new Gdk.Color (200, 255, 200);
					if (t.Designation == "Зл")
						color = new Gdk.Color (200, 255, 200);
					else if (t.Designation == "Аб")
						color = new Gdk.Color (255, 255, 255);
					else if (t.Designation == "Юр")
						color = new Gdk.Color (200, 200, 255);
					
					ModifyWidgetBg (rb, color); 
					*/

					#endregion 
				}
			}
		
			// display theme description on label and make label visible
			labelThemeDesc.LabelProp = "<i>" + theme.Description + "</i>";
			labelThemeDesc.TooltipText = theme.Description;
			labelThemeDesc.Visible = true;

			// make task table and checkbutton visible
			table2.Visible = true;
			chkOpenBook.Visible = true;

			// dump generated varian to textview
			textview1.Buffer.Text = 
				string.Format (Catalog.GetString ("Variant for:\n{0}\n{1}"), sName, TasksToText (lsTasksSelected));
		}

		private List<TaskInfo> SelectTasks (int nTasksToSelect, int minCompl, int maxCompl, bool[] taskMask, Random rnd)
		{
			var nIter = 0;
			var nTasksSelected = 0;
			var iGrouping = 3; 

			var tasksSelected = new List<TaskInfo> (nTasksToSelect);

			while (nTasksSelected < nTasksToSelect)
			{
				if (++nIter > lsTasks.Count)
				{
					// decrease non-heaping requirements
					iGrouping = 2;
				}       

				if (++nIter > 2 * lsTasks.Count)
				{
					// decrease non-heaping requirements more 
					iGrouping = 1;
				}       

				// аварийный выход - нужное количество не найдено
				if (nIter > 10 * lsTasks.Count)
					break;

				var n = rnd.Next (lsTasks.Count);
				// реализуем заданную не-кучность,
				// проверяя наличие ранее выбранных задач рядом с текущей

				if (lsTasks [n].Complexity <= maxCompl && lsTasks [n].Complexity >= minCompl)
					if (!IsSomethingNear (taskMask, true, n, iGrouping))
					{
						nTasksSelected++;
					
						taskMask [n] = true;

						lsTasks [n].SelectedVariant = rnd.Next (lsTasks [n].Variants) + 1;
						tasksSelected.Add (lsTasks [n]);

					}

			} // while

			return tasksSelected;
		}

		#region Old SelectTasks algorithm

		// THINK: A way to invoke old generation algorithm

		private List<TaskInfo> SelectTasksOld (int nTasksToSelect, int minCompl, int maxCompl, bool[] taskMask, Random rnd)
		{
			var nIter = 0;
			var nTasksSelected = 0;
			var iGrouping = 2;
			
			var tasksSelected = new List<TaskInfo> (10);
			
			while (nTasksSelected < nTasksToSelect)
			{
				if (++nIter > 2 * lsTasks.Count)
				{
					// снижаем требования к не-кучности при угрозе зацикливания

					// TODO: graduate decrease grouping

					iGrouping = 1;
					// аварийный выход - нужное количество не найдено
					if (nIter > 10 * lsTasks.Count)
						break;
				}
				
				var n = rnd.Next (lsTasks.Count);
				// реализуем заданную не-кучность,
				// проверяя наличие ранее выбранных задач рядом с текущей
				
				if (lsTasks [n].Complexity <= maxCompl && lsTasks [n].Complexity >= minCompl)
				if (!IsSomethingNear (taskMask, true, n, iGrouping))
				{
					nTasksSelected++;
					//SumComplexity += lsTasks[n].Complexity;
						
					taskMask [n] = true;
						
					lsTasks [n].SelectedVariant = rnd.Next (lsTasks [n].Variants) + 1;
					tasksSelected.Add (lsTasks [n]);
					
				}
			} // while
			
			return tasksSelected;
			
		}

		#endregion

		protected void OnTaskButtonClicked (object sender, EventArgs e)
		{
			var button = sender as Button;
			var table = button.Parent as Table;

			// table[sender]


			for (var i = 0; i < table.Children.Length; i++)
			{
				if (table.Children [i] is Button)
				{
					if (table.Children [i] == button)
					{
						var task = lsTasksSelected [lsTasksSelected.Count - i / 2 - 1];
						var book = task.ProblemBookSection.ProblemBook;

						textview1.Buffer.Clear ();
						textview1.Buffer.Text = task.ToDetailString ();
							
						if (chkOpenBook.Active)
						{
							var helpViewer = new Process ();

							if (!string.IsNullOrWhiteSpace (task.Description) &&
								task.Description.Trim ().StartsWith ("<"))
							{
								// if description in XML format

								var tmpFileName = System.IO.Path.GetTempFileName () + ".xml";
								var template = "<?xml version='1.0' encoding='UTF-8'?>\n" +
									"<!DOCTYPE section PUBLIC \"-//OASIS//DTD DocBook XML V4.5//EN\" \"docbookV4.5/docbookx.dtd\" []>\n" +
								// add XSL stylesheet to DocBook, if not help viewer (like yelp) is present
									(string.IsNullOrEmpty (AppConfig.HelpViewerApp) ?
								// TODO: relative stylesheet link
										 "<?xml-stylesheet href=\"/home/redhound/Projects/TaskGen/TaskGen/docbook-simple.xsl\" type=\"text/xsl\"?>\n" : "") + 
									"<section lang=\"ru-RU\"><title>[TITLE]</title>[CONTENT]</section>";
					
								template = template.Replace ("[TITLE]", task.Title + " " + task.Order);
								template = template.Replace ("[CONTENT]", task.Description);
				
								File.WriteAllText (tmpFileName, template);

								helpViewer.StartInfo.FileName = 
									!string.IsNullOrEmpty (AppConfig.HelpViewerApp) ? AppConfig.HelpViewerApp : AppConfig.BrowserApp;

								helpViewer.StartInfo.Arguments = tmpFileName;
							} 
							else
							{
								helpViewer.StartInfo.FileName = "evince";
								helpViewer.StartInfo.Arguments = 
									// CHECK: Must quote filename?
									string.Format ("--page-label={0} {1}",
                                        task.Page / book.PagesOnSheet + book.PagesMinus, 
                                        System.IO.Path.Combine (AppConfig.ApplicationData, "books", book.File)
									);
							}
							
							helpViewer.Start ();
						}

						return;

					} // if "found"
				} // if "Button"
			} // for table.Children
		}

		/*
		bool firstTimeExpose = true;

		protected virtual void OnExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			if (firstTimeExpose)
			{
				firstTimeExpose = false;
				OnInit ();
			}
		}*/

		/// <summary>
		/// 
		/// </summary>
		protected void RememberUser (string userName)
		{
			if (UserInfo.IsValidName (userName))
			{
				try
				{
					var user = new UserInfo ();
					user.Name = userName;
					DataProvider.Database.Insert (user);
								
					// вставляем новое имя в начало списка
					lsUsers.Insert (0, user);
					cbeName.PrependText (user.Name);
					cbeName.Active = 0;
					
					// adding new item to completition
					(cbeName.Entry.Completion.Model as ListStore).AppendValues (user.Name);
				} catch
				{
					// Error: User may already exists,
					// so we do not need to add him twice
				} 
			}
		}

		protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
		{
			Gtk.Application.Quit ();
		}

		protected virtual void OnAboutAction1Activated (object sender, System.EventArgs e)
		{
			var about = new AboutDialog ();

			about.Authors = new string[] {
				Catalog.GetString("Roman M. Yagodin - concept and programming")
			};
			about.Artists = new string[] {
				Catalog.GetString("IconCubic <iconcubic.deviantart.com> - dices icons clipart")
			};

			about.Copyright = string.Format ("All right reserved (c) 2010-{0} Roman M. Yagodin", DateTime.Now.Year);
			about.Website = "https://github.com/roman-yagodin/r7-taskgen";
			about.ProgramName = "R7.TaskGen";
			about.Logo = Gdk.Pixbuf.LoadFromResource ("R7.TaskGen.icons.taskgen-d8.png");

			about.WrapLicense = true;

			about.License = string.Format (Catalog.GetString (
				"Author: Roman M. Yagodin <roman.yagodin@gmail.com>\n\n" +
				"Copyright (c) 2010-{0} by Roman M. Yagodin\n\n" +
				"This program is free software: you can redistribute it and/or modify " +
				"it under the terms of the GNU General Public License as published by " +
				"the Free Software Foundation, either version 3 of the License, or " +
				"(at your option) any later version.\n\n" +
				"This program is distributed in the hope that it will be useful, " +
				"but WITHOUT ANY WARRANTY; without even the implied warranty of " + 
				"MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the " +
				"GNU General Public License for more details.\n\n" +
				"You should have received a copy of the GNU General Public License " +
				"along with this program. If not, see <http://www.gnu.org/licenses/>."), DateTime.Now.Year);

			about.Comments = Catalog.GetString ("Task variants generator for use with learning courses, that uses " +
				"student's name as a seed to combine randomly selected tasks in student-specific variant");

			about.Run ();
			about.Destroy ();
		}

		protected virtual void OnActionHelpActivated (object sender, System.EventArgs e)
		{
			Process.Start (AppConfig.HelpViewerApp, 
			               System.IO.Path.Combine (
				"help",
				Catalog.GetString ("help_en.xml")));
		}

		private void ModifyWidgetBg (Widget w, Gdk.Color normalBgColor)
		{
			// TODO: check values of color components

			int red = normalBgColor.Red;
			int green = normalBgColor.Green;
			int blue = normalBgColor.Blue;
			
			// red >>= 8;
			red /= 255;
			green /= 255;
			blue /= 255;
			
			Gdk.Color prelightBgColor = new Gdk.Color ((byte)Math.Min (red + red / 10, 255),
			                                           (byte)Math.Min (green + green / 10, 255),
			                                           (byte)Math.Min (blue + blue / 10, 255)
			);
			

			Gdk.Color activeBgColor = new Gdk.Color ((byte)Math.Max (red - red / 10, 0), 
			                                         (byte)Math.Max (green - green / 10, 0),
			                                         (byte)Math.Max (blue - blue / 10, 0)
			);
			
			/* Gdk.Color.Zero;*/
		
			//Gtk.Widget parent = widget;
			//for (int i = 0; i < bbx.Children.Length; i++) {
			//Gtk.Widget w = bbx.Children[childNum];
			
			//if (w is Gtk.Button)
			//{
			w.ModifyBg (StateType.Normal, normalBgColor);
			w.ModifyBg (StateType.Prelight, prelightBgColor);
			w.ModifyBg (StateType.Active, activeBgColor);
			w.ModifyBg (StateType.Insensitive, prelightBgColor);
			//}
			//}
		}

		/// <summary>
		/// Taskses to text.
		/// </summary>
		/// <returns>
		/// The to text.
		/// </returns>
		/// <param name='tasks'>
		/// Tasks.
		/// </param>
		private string TasksToText (List<TaskInfo> tasks)
		{
			string result = string.Empty;
			
			for (int i = 0; i < tasks.Count; i++)
				result += string.Format ("{0}: {1}\n", i + 1, tasks [i]);
			
			return result;
		}

		/// <summary>
		/// Copy textview content to a clipboard
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnActionCopyActivated (object sender, System.EventArgs e)
		{
			if (lsTasksSelected != null)
			if (lsTasksSelected.Count > 0)
			{
				Gtk.Clipboard cb = Gtk.Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", true));
				cb.Text = TasksToText (lsTasksSelected);
				cb.Store ();
			}
		}
		//bool firstTimeThemeChange = true;
		protected void OnComboThemeChanged (object sender, System.EventArgs e)
		{
			if (comboTheme.Active > 0)
			{
				// Delete default "-- Not selected --" item from combobox
				//if (firstTimeThemeChange) comboTheme.RemoveText(0);
				//firstTimeThemeChange = false;

				OnActionGenerateActivated (sender, e);
			} else
			{
				lblStatus.LabelProp = Catalog.GetString ("You must choose a theme!");
				lblStatus.TooltipText = Catalog.GetString (
					"You must choose a theme of work to generate variant for it.");
				imgStatus.Pixbuf = this.RenderIcon (Gtk.Stock.DialogError, IconSize.Menu, "");
				comboTheme.ErrorBell ();
				return;
			}
		}

		protected void OnCbeNameEntryActivated (object sender, System.EventArgs e)
		{
			// Tip: Activated event fires then user hit Enter!
			OnActionGenerateActivated (sender, e);
		}

		int prevActive = -1;

		protected void OnCbeNameChanged (object sender, System.EventArgs e)
		{
			#if TRACE

			Console.WriteLine (cbeName.Active);

			#endif
			
			if (cbeName.Active != -1 && cbeName.Active != prevActive)
			{
				// Then user must be selected something from list.
				// Tip: Active = -1 then user now typing
				OnActionGenerateActivated (sender, e);
			}
			prevActive = cbeName.Active;
		}

		protected void OnOpenTaskbookFolderActionActivated (object sender, EventArgs e)
		{
			Process.Start (AppConfig.StarterApp, 
			               System.IO.Path.Combine (AppConfig.ApplicationData, "books"));
		}
		#region TEST
		protected void OnButton3Clicked (object sender, EventArgs e)
		{
			Test2 ();
		}

		private void Test2 ()
		{

			var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(
				System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);

			Console.WriteLine (config.FilePath);


		//	AppConfig.Platform = "TestTestTest";
		//	AppConfig.Save ();

		}

		private void Test ()
		{
			using (var sr = new StreamReader("docbook-simple.xsl"))
			{
				using (var xmlr = XmlReader.Create(sr))
				{
					// TODO: Replace with XslCompiledTransform
					var xslt = new XslTransform ();
					try
					{
						xslt.Load (xmlr);
						xslt.Transform ("sample.xml", "sample.html");
					} catch (Exception ex)
					{
						Console.WriteLine (ex.Message);
					}

				}
			}
		}
		#endregion
	}
}
