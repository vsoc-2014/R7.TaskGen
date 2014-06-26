using System;
using PetaPoco;

namespace R7.TaskGen
{
	[TableName("Disciplines")]
	[PrimaryKey("DisciplineID")]
	[ExplicitColumns]
	public class DisciplineInfo
	{
		public DisciplineInfo()
		{
		}

		[Column("DisciplineID")]
		public int DisciplineID { get; set; }

		[Column("Semester")]
		public int Semester { get; set; }

		[Column("Title")]
		public string Title { get; set; }
	}
}

