using System;
using PetaPoco;

namespace R7.TaskGen
{
	[TableName("Users")]
	[PrimaryKey("UserID")]
	[ExplicitColumns]
	public class UserInfo
	{
		public UserInfo ()
		{
		}

		[Column("UserID")]
		public int UserID { get; set; }

		[Column("Name")]
		public string Name { get; set; }

		public static bool IsValidName (string name)
		{
			name = name.Replace ("  ", " ");
			string[] nameParts = name.Split (new char[] { ' ' });
			
			if (nameParts.Length != 3) 
				return false;
			else if (nameParts[0].Length < 2 || nameParts[1].Length < 2 || nameParts[2].Length < 2)
				return false;
			else
				return true;
		}
	}
}

