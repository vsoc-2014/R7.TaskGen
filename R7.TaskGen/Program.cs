using System;
using System.Collections.Generic;
using System.Linq;

using Gtk;
using Mono.Unix;

namespace R7.TaskGen
{
    static class Program
    {
		/// <summary>
		/// Program entry point
		/// </summary>
		/// <param name="args">
		/// A <see cref="System.String[]"/>
		/// </param>
        static void Main (string[] args)
        {
			// init gettext catalog
   			Catalog.Init ("taskgen", "./locale");

			// GTK main cycle
			Application.Init ();
        	MainForm win = new MainForm ();
        	win.Show ();
        	Application.Run ();
	    }

    } // class
} // namespace
