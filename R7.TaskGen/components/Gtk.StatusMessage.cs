using System;

namespace Gtk
{
	public class StatusMessage
	{
		private Label label;
		private Image image;

		public StatusMessage (Gtk.Label label, Gtk.Image image)
		{
			this.label = label;
			this.image = image;
		}

		public void Message (string message, string stockId)
		{
			label.LabelProp = label.TooltipText = message;
			image.Pixbuf = image.RenderIcon (stockId, IconSize.Menu, string.Empty);
		}

		public void Message (string message, string tooltip, string stockId)
		{
			label.LabelProp = message;
			label.TooltipText = tooltip;
			image.Pixbuf = image.RenderIcon (stockId, IconSize.Menu, string.Empty);
		}

	} // class
} // namespace

