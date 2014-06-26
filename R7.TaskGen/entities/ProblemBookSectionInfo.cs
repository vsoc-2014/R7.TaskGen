using System;
using PetaPoco;

namespace R7.TaskGen
{
	[TableName("ProblemBookSections")]
	[PrimaryKey("ProblemBookSectionID")]
	[ExplicitColumns]
	public class ProblemBookSectionInfo
	{
		public ProblemBookSectionInfo ()
		{
		}

		[Column("ProblemBookSectionID")]
		public int ProblemBookSectionID { get; set; }

		[Column("EndPage")]
		public int EndPage { get; set; }

		[Column("Order")]
		public int Order { get; set; }

		[Column("StartPage")]
		public int StartPage { get; set; }

		[Column("Title")]
		public string Title { get; set; }

		[Column("ProblemBookID")]
		public int ProblemBookID { get; set; }

		[AutoJoin]
		public ProblemBookInfo ProblemBook { get; set; }
	}
}

