USE [master]
GO
/****** Object:  Database [WikiDumpDB]    Script Date: 3/28/2021 2:24:08 AM ******/
CREATE DATABASE [WikiDumpDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'WikiDumpDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER01\MSSQL\DATA\WikiDumpDB.mdf' , SIZE = 3874816KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'WikiDumpDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER01\MSSQL\DATA\WikiDumpDB_log.ldf' , SIZE = 2760704KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [WikiDumpDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [WikiDumpDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [WikiDumpDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [WikiDumpDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [WikiDumpDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [WikiDumpDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [WikiDumpDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [WikiDumpDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [WikiDumpDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [WikiDumpDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [WikiDumpDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [WikiDumpDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [WikiDumpDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [WikiDumpDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [WikiDumpDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [WikiDumpDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [WikiDumpDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [WikiDumpDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [WikiDumpDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [WikiDumpDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [WikiDumpDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [WikiDumpDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [WikiDumpDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [WikiDumpDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [WikiDumpDB] SET RECOVERY FULL 
GO
ALTER DATABASE [WikiDumpDB] SET  MULTI_USER 
GO
ALTER DATABASE [WikiDumpDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [WikiDumpDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [WikiDumpDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [WikiDumpDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [WikiDumpDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [WikiDumpDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'WikiDumpDB', N'ON'
GO
ALTER DATABASE [WikiDumpDB] SET QUERY_STORE = OFF
GO
USE [WikiDumpDB]
GO
/****** Object:  Table [dbo].[WikiPage]    Script Date: 3/28/2021 2:24:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WikiPage](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PeriodHour] [int] NOT NULL,
	[PageLanguage] [nvarchar](50) NOT NULL,
	[PageDomain] [nvarchar](50) NOT NULL,
	[PageDesign] [nvarchar](50) NOT NULL,
	[PageTitle] [nvarchar](350) NOT NULL,
	[CountViews] [int] NOT NULL,
	[TotalResponseSize] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spWikiPage_GetFirstQuery]    Script Date: 3/28/2021 2:24:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description: Query to Display Max Viewed Count
--				per Language and Domain (grouped by hours)
-- =============================================
CREATE PROCEDURE [dbo].[spWikiPage_GetFirstQuery]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT w.PeriodHour, c.PageLanguage, c.PageDomain, c.CountViews
	FROM dbo.WikiPage as w
	CROSS APPLY
		(SELECT TOP 1 PeriodHour, PageLanguage, PageDomain, SUM(CountViews) as CountViews
		FROM dbo.WikiPage
		WHERE PeriodHour = w.PeriodHour
		GROUP BY PeriodHour, PageLanguage, PageDomain
		ORDER BY 1, 4 DESC) as c;
END
GO
/****** Object:  StoredProcedure [dbo].[spWikiPage_GetSecondQuery]    Script Date: 3/28/2021 2:24:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description: Query to Display Max Viewed Count
--				per Page Title (grouped by hours)
-- =============================================
CREATE PROCEDURE [dbo].[spWikiPage_GetSecondQuery]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT w.PeriodHour, c.PageTitle, c.CountViews
	FROM dbo.WikiPage as w
	CROSS APPLY
		(SELECT TOP 1 PeriodHour, PageTitle, SUM(CountViews) as CountViews
		FROM dbo.WikiPage
		WHERE PeriodHour = w.PeriodHour
		GROUP BY PeriodHour, PageTitle
		ORDER BY 1, 3 DESC) as c;
END
GO
/****** Object:  StoredProcedure [dbo].[spWikiPage_Truncate]    Script Date: 3/28/2021 2:24:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Truncates the Table WikiPage.
-- =============================================
CREATE PROCEDURE [dbo].[spWikiPage_Truncate]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	TRUNCATE TABLE dbo.WikiPage;
END
GO
USE [master]
GO
ALTER DATABASE [WikiDumpDB] SET  READ_WRITE 
GO
