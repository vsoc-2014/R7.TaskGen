-- Tabless

CREATE TABLE [Users] (
	[UserID] INTEGER PRIMARY KEY NOT NULL,
	[Name] TEXT UNIQUE NOT NULL 
)

CREATE TABLE [Disciplines] (
	[DisciplineID] INTEGER PRIMARY KEY NOT NULL,
	[Title] TEXT NOT NULL,
	[Semester] INTEGER NOT NULL 
)

CREATE TABLE [Themes] (
	[ThemeID] INTEGER PRIMARY KEY NOT NULL,
	[Title] TEXT NOT NULL,
	[Order] INTEGER,
	[DisciplineID] INTEGER NOT NULL,
	[IsEnabled] BOOL NOT NULL DEFAULT (1), 
	[Description] TEXT
)

CREATE TABLE [ProblemBooks] (
	[ProblemBookID] INTEGER PRIMARY KEY NOT NULL,
	[Title] TEXT NOT NULL,
	[Author] TEXT NOT NULL,
	[Year] INTEGER NOT NULL,
	[Designation] TEXT NOT NULL,
	[File] TEXT,
	[PagesMinus] INTEGER DEFAULT (0),
	[PagesOnSheet] INTEGER DEFAULT (1) 
)

CREATE TABLE [ProblemBookSections] (
	[ProblemBookSectionID] INTEGER PRIMARY KEY NOT NULL,
	[Title] TEXT NOT NULL,
	[Order] INTEGER NOT NULL,
	[ProblemBookID] INTEGER NOT NULL,
	[startPage] INTEGER NOT NULL,
	[endPage] INTEGER NOT NULL
)

CREATE TABLE [ThemeSectionPairs] (
	[ThemeSectionPairID] INTEGER PRIMARY KEY NOT NULL,
	[ThemeID] INTEGER NOT NULL,
	[ProblemBookSectionID] INTEGER NOT NULL 
)

CREATE TABLE [Tasks] (
	[TaskID] INTEGER PRIMARY KEY NOT NULL, 
	[Order] INTEGER,
	[Title] TEXT,
	[ProblemBookSectionID] INTEGER NOT NULL,
	[Page] INTEGER NOT NULL,
	[Parts] INTEGER NOT NULL,
	[Description] TEXT,
	[Complexity] INTEGER NOT NULL,
	[Variants] INTEGER NOT NULL DEFAULT (1) 
)

-- Views-

CREATE VIEW [vw_ThemeSectionPairs] AS  
	SELECT 
		Themes.ThemeID, 
		Themes.Title AS Theme, 
		PBS.ProblemBookSectionID, 
		PBS.Title AS Section, 
		PB.Author as Book
 	FROM ThemeSectionPairs AS TSP 
 		INNER JOIN Themes 
 			ON TSP.ThemeID = Themes.ThemeID 
 		INNER JOIN ProblemBookSections AS PBS 
 			ON TSP.ProblemBookSectionID = PBS.ProblemBookSectionID 
 		INNER JOIN ProblemBooks AS PB 
 			ON PBS.ProblemBookID = PB.ProblemBookID
 			