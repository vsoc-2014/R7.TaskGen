using System;
using System.Collections.Generic;

using Mono.Unix;
using PetaPoco;

namespace R7.TaskGen
{
	public class TaskOrderComparer : IComparer<TaskInfo>
	{
		public int Compare (TaskInfo x, TaskInfo y)
		{
			if (x.ComplexityLevel > y.ComplexityLevel)
				return 1;
			else if (x.ComplexityLevel < y.ComplexityLevel)
				return -1;
			else
				if (x.ProblemBookSectionId > y.ProblemBookSectionId)
				return 1;
			if (x.ProblemBookSectionId < y.ProblemBookSectionId)
				return -1;
			else
				if (x.Order > y.Order)
					return 1; 
				else if (x.Order < y.Order)
					return -1;
				else
					return 0;
		}
	}

	public enum TaskComplexity {
		Low, Normal, Higher, High
	}

	[TableName("Tasks")]
	[PrimaryKey("TaskID")]
	[ExplicitColumns]
	public class TaskInfo
	{
		
		public TaskComplexity ComplexityLevel
		{
			get {
				return GetComplexityLevel(this.Complexity);
			}
		}
		
		public static TaskComplexity GetComplexityLevel (int compl)
		{
			if (compl <= 3)
				return TaskComplexity.Low;
			else if (compl >= 7 && compl <= 8)
				return TaskComplexity.Higher;
			else if (compl >= 9)
				return TaskComplexity.High;
			else
				return TaskComplexity.Normal;
		}
		
		public TaskInfo ()
		{
		}

		[Column("TaskID")]
		public int TaskID { get; set; }

		[Column("Complexity")]
		public int Complexity { get; set; }

		[Column("Variants")]
		public int Variants { get; set; }

		[Column("Parts")]
		public int Parts { get; set; }

		[Column("Order")]
		public int Order { get; set; }

		[Column("Page")]
		public int Page { get; set; }

		[Column("Title")]
		public string Title { get; set; }

		[Column("Description")]
		public string Description { get; set; }
			
		[Column("ProblemBookSectionId")]
		public int ProblemBookSectionId { get; set; }

		[AutoJoin]
		public ProblemBookSectionInfo ProblemBookSection { get; set; }

		#region Additional

		public int SelectedVariant { get; set; }

		public string Designation 
		{
			get { return ProblemBookSection.ProblemBook.Designation; }
		}

		public int SectionOrder
		{
			get { return ProblemBookSection.Order; }
		}


		#endregion

		public static char VariantLetter (int n)
		{
			// TODO: Need DB refactoring!
			// THINK: variant letters depends on taskbook language, but when we get rid of taskbooks, there be no need in this
			string vletters = "абвгдежзиклмнопрстуфхцчшэюя";
			return vletters[n - 1];
		}
		
		public override string ToString ()
		{
			object section;
			string templateV1;
			string templateVm;
			
			// TODO: Need DB refactoring!

			if (Designation == "Аб")
			{
				section = Title;
				templateV1 = "{0} {1} {2}";
				templateVm = "{0} {1} {2} {3})";
			} else
			{
				section = ProblemBookSection.Order;
				templateV1 = "{0} {1}.{2}";
				templateVm = "{0} {1}.{2} {3})";
			}
		
			if (Variants == 1)
				return string.Format (templateV1, Designation, section, Order);
			else
				return string.Format (templateVm, 
					Designation, section, Order, VariantLetter (SelectedVariant));
		
		}

		public string ToDetailString ()
		{
			var book = ProblemBookSection.ProblemBook;

			if (book.Designation == "Зл")
				return string.Format (
					Catalog.GetString ("Task book:\n{0} {1}\nTask: {2}.{3}{4} {8}\nComplexity: {7:F1}\nPage: {5}\n{6}"),
					book.Author, book.Year, SectionOrder, Order, 
					(Variants > 1) ? " " + TaskInfo.VariantLetter (SelectedVariant) + ")" : string.Empty, 
					Page, (!string.IsNullOrEmpty (Description)) ? Catalog.GetString ("Task text:") + " " + Description + "\n" : string.Empty, 
					Complexity / 10.0f, 
					!string.IsNullOrEmpty (Title) ? "\n" + Title : "");
			else if (book.Designation == "Яг")
				return string.Format (
					Catalog.GetString ("Task book:\n{0} {1}\nTask: {6} {2}{3}\nComplexity: {5:F1}\nPage: {4}"),
					book.Author, book.Year, Order, 
					(Variants > 1) ? " " + TaskInfo.VariantLetter (SelectedVariant) + ")" : string.Empty, 
					Page, Complexity / 10.0f, Title);
			else
				return string.Format (
					Catalog.GetString("Task book:\n{0} {1}\nTask: {7} {2}{3}\nComplexity: {6:F1}\nPage: {4}\n{5}"),
					book.Author, book.Year, Order, 
					(Variants > 1) ? " " + TaskInfo.VariantLetter (SelectedVariant) + ")" : string.Empty, 
					Page, (!string.IsNullOrEmpty (Description)) ? Catalog.GetString("Task text:") + " " + Description + "\n" : string.Empty, 
					Complexity / 10.0f, Title );
			
		}
		
	} // class
} // namespace