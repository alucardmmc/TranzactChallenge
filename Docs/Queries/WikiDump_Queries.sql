/* Drop Table WikiPage */
DROP TABLE dbo.WikiPage;

/* Create Table WikiPage */
CREATE TABLE dbo.WikiPage (
	ID int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	PeriodHour int NOT NULL,
	PageLanguage nvarchar(50) NOT NULL,
	PageDomain nvarchar(50) NOT NULL,
	PageDesign nvarchar(50) NOT NULL,
	PageTitle nvarchar(350) NOT NULL,
	CountViews int NOT NULL,
	TotalResponseSize int NOT NULL
);

/* FIRST QUERY */
SELECT TOP 1 WITH TIES PeriodHour, PageLanguage, PageDomain, SUM(CountViews) as ViewCount
FROM dbo.WikiPage
GROUP BY PeriodHour, PageLanguage, PageDomain
ORDER BY ROW_NUMBER() OVER (PARTITION BY PeriodHour ORDER BY SUM(CountViews) DESC);

/* FIRST QUERY WITH CROSS APPLY - 40 SECS*/
SELECT DISTINCT w.PeriodHour, c.PageLanguage, c.PageDomain, c.ViewCount
FROM dbo.WikiPage as w
CROSS APPLY
	(SELECT TOP 1 PeriodHour, PageLanguage, PageDomain, SUM(CountViews) as ViewCount
	FROM dbo.WikiPage
	WHERE PeriodHour = w.PeriodHour
	GROUP BY PeriodHour, PageLanguage, PageDomain
	ORDER BY 1, 4 DESC) as c;

/* SECOND QUERY */
SELECT TOP 1 WITH TIES PeriodHour, PageTitle, SUM(CountViews) as ViewCount
FROM dbo.WikiPage
GROUP BY PeriodHour, PageTitle
ORDER BY ROW_NUMBER() OVER (PARTITION BY PeriodHour ORDER BY SUM(CountViews) DESC);

/* SECOND QUERY WITH CROSS APPLY - 1.20 Min*/
SELECT DISTINCT w.PeriodHour, c.PageTitle, c.ViewCount
FROM dbo.WikiPage as w
CROSS APPLY
	(SELECT TOP 1 PeriodHour, PageTitle, SUM(CountViews) as ViewCount
	FROM dbo.WikiPage
	WHERE PeriodHour = w.PeriodHour
	GROUP BY PeriodHour, PageTitle
	ORDER BY 1, 3 DESC) as c;

/* TRUNCATE */
TRUNCATE TABLE dbo.WikiPage;

/* CHECK TABLE HAS NOTHING */
SELECT TOP 100 *
FROM dbo.WikiPage
ORDER BY ID DESC;

SELECT TOP 100 *
FROM dbo.WikiPage;

SELECT DISTINCT PageDomain
FROM dbo.WikiPage;
