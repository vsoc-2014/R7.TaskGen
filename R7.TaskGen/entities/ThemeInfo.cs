using System;
using PetaPoco;

namespace R7.TaskGen
{
	[TableName("Themes")]
	[PrimaryKey("ThemeID")]
	[ExplicitColumns]
	public class ThemeInfo
	{
		public ThemeInfo ()
		{
		}

		[Column("ThemeID")]
		public int ThemeID { get; set; }

		[Column("DisciplineID")]
		public int DisciplineID { get; set; }

		[AutoJoin]
		public DisciplineInfo Discipline { get; set; }

		[Column("Order")]
		public int Order { get; set; }

		[Column("Title")]
		public string Title { get; set; }

		[Column("Description")]
		public string Description { get; set; }

	}
}

