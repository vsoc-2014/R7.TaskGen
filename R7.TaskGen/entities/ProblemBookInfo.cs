using System;
using PetaPoco;

namespace R7.TaskGen
{
	[TableName("ProblemBooks")]
	[PrimaryKey("ProblemBookID")]
	[ExplicitColumns]
	public class ProblemBookInfo
	{
		public ProblemBookInfo ()
		{
		}

		[Column("ProblemBookID")]
		public int ProblemBookID { get; set; }

		[Column("Author")]
		public string Author { get; set; }

		[Column("Designation")]
		public string Designation { get; set; }

		[Column("Title")]
		public string Title { get; set; }

		[Column("Year")]
		public int Year { get; set; }
		
		[Column("ProblemBookSectionOrder")]
		public int ProblemBookSectionOrder { get; set; }
		
		[Column("File")]
		public string File { get; set; }
		
		[Column("PagesOnSheet")]
		public int PagesOnSheet { get; set; }
		
		[Column("PagesMinus")]
		public int PagesMinus { get; set; }
	}
}

