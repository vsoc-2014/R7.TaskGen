using System;

namespace R7.TaskGen
{
	public class ThemeSectionPairInfo
	{
		public ThemeSectionPairInfo ()
		{
		}
		
		private int themeSectionPairId;
		private int themeId;
		private int problemBookSectionId;
		
		public int ProblemBookSectionID {
			get {
				return this.problemBookSectionId;
			}
			set {
				problemBookSectionId = value;
			}
		}

		public int ThemeID {
			get {
				return this.themeId;
			}
			set {
				themeId = value;
			}
		}

		public int ThemeSectionPairID {
			get {
				return this.themeSectionPairId;
			}
			set {
				themeSectionPairId = value;
			}
		}		
	}
}

