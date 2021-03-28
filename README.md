# TranzactChallenge
This is a repo for a Coding Assignment.

## What is the programming challenge about?

### Challenge Context

The Wikimedia Foundation provides all pageviews for Wikipedia site since 2015. 
These pageviews can be download in gzip format, and are aggregated per hour per page.
The Hourly Dump is around 50 mb, and the unzipped file is between 100 MB and 250 MB.

Technical documentation of the service: [Wikimedia Documentation](https://wikitech.wikimedia.org/wiki/Analytics/Data_Lake/Traffic/Pageviews)

Sample Link: [Wikimedia Dumps 2015](https://dumps.wikimedia.org/other/pageviews/2015/2015-05/)

### Challenge Result

For this Challenge, we need to:

- Create a command line application using **C#** or **Python**.
- Get the Data from the last 5 hours.
- Display the First Query: Language and Domain with the Max **Viewed Count** (Grouped by **Hours**).
- Display the Second Query: Page Title with the Max **Viewed Count** including all languages (Grouped by **Hours**).

---

## Solution

### Applications Used

To code the Challenge I used:

- Visual Studio 2019
- Microsoft SQL Server Developer

## Explaining the Solution

The project is structured into 2 sections:

- **Model**: Data structure
- **Application**: Classes that control the application.

The project also uses the following:

- **Configuration File**
- **MS SQL Server Database**
- **NuGET Packages**

### Model:

For the **Model** section, we have 2 classes.

- **Time**: Class that contains the year, month, day and hour.
- **WikiPage**: Class that contains the structure of the WikiPage.

### Application:

For the **Application** section, we have 7 classes.

- **Helper**: Class that gets different data for other classes.
- **FileDirectory**: Class that gets the zipped and unzipped files. It also creates the folder where we are going to store the data.
- **FileDownload**: Class that downloads the required files.
- **FileDecompress**: Class that decompresses the required files.
- **WikiDump**: Class that creates a WikiPage. It also has the logic that Wikipedia uses for the **Domain_code**.
- **DataAccess**: Class that has Access to the Database to Insert and Query data.
- **ConsolePrint**: Class that displays the Queries into the Console.

### Configuration File:

Here we use an **App.config** file to store 2 values:

- **Connection String**: We store the connection string to our database. 
- **DataDumpFolder**: The folder where we are going to store the files from Wikipedia.

### MS SQL Server:

In SQL Server we created a **"WikiDumpDB"** Database with 1 Table and 3 Stored Procedures.

#### Table

The table created is **dbo.WikiPage** with the following columns:

|    Column Name    | Data Type     | Constraint |
|:-----------------:|---------------|------------|
| ID                | int           | PK         |
| PeriodHour        | int           |            |
| PageLanguage      | nvarchar(50)  |            |
| PageDomain        | nvarchar(50)  |            |
| PageDesign        | nvarchar(50)  |            |
| PageTitle         | nvarchar(350) |            |
| CountViews        | int           |            |
| TotalResponseSize | int           |            |

**Q**: Why use nvarchar?

- Nvarchar is used because the data of the WikiDumps has different encoding according to the language of the Page Title. We can see this in the image below:

![PageTitle_Issue](https://user-images.githubusercontent.com/21108071/112761444-03ca9400-8fc1-11eb-8b6c-0deb568cf842.JPG)

#### Stored Procedures

The 3 Stored Procedures created are:

- **[dbo].[spWikiPage_GetFirstQuery]**: Returns the First Query of the Challenge.
- **[dbo].[spWikiPage_GetSecondQuery]**: Returns the Second Query of the Challenge.
- **[dbo].[spWikiPage_Truncate]**: Truncates the **dbo.WikiPage** for a new execution.

### NuGet Packages:

This project uses the following packages:

- **ConsoleTables**: Displays objects as Tables in a Console Application.
- **System.Configuration.ConfigurationManager**: Provides support for Configuration files.
- **System.Data.SqlClient**: Data Access Driver for SQL Server.

---

## Issues

### Domain Code

If we check the [Documentation](https://wikitech.wikimedia.org/wiki/Analytics/Data_Lake/Traffic/Pageviews), we can see that the Domain Code is very complex.
From the Domain Code, we can get 3 important pieces of information:

- **Page Language**: The language used for the Wikipedia page.
- **Page Domain**: The Wikipedia Domain of the page.
- **Page Design**: If the page is on **Mobile** or **Desktop**.

These differents are separated by a **"."**. Knowing this, we can classify the Domain Code in 3 different forms:

- **CASE 1**: When the Domain Code can be split into 1.

  - **Unique Case**: Language
  
    - Language is -> **Language**
    - Domain is   -> **Wikipedia**
    - Design is   -> **Desktop**

- **CASE 2**: When the Domain Code can be split into 2.

  - **CASE 2.1**: Language.Mobile
  
    - Language is -> **Language**
    - Domain is -> **Wikipedia**
    - Design is -> **Mobile**
  
  - **CASE 2.2**: Language.Domain
  
    - Language is -> **Language**
    - Domain is -> **Domain**
    - Design is -> **Desktop**
    
  - **CASE 2.3**: Wikimedia.Domain
  
    - Language is -> **English**
    - Domain is -> **Wikimedia**
    - Design is -> **Desktop**
    
- **CASE 3**: When the Domain Code can be split into 3.

  - **CASE 3.1**: Language.Mobile.Domain
  
    - Language is -> **Language**
    - Domain is -> **Domain**
    - Design is -> **Mobile**
    
  - **CASE 3.2**: Wikimedia.Mobile.Domain
  
    - Language is -> **English**
    - Domain is -> **Wikimedia**
    - Design is -> **Mobile**

**Q.**: How do we know if the Language is replaced with the Wikimedia code?

When the first part of the DomainCode has one of the following:

- commons | meta | incubator | species | strategy | outreach | usability | quality

**Q.**: How do we know the correct Domain of the page?

We use the following table:

| Page Domain Code | Domain Name         |
|:----------------:|---------------------|
|                  | Wikipedia           |
| b                | WikiBooks           |
| d                | Wiktionary          |
| f                | WikiMediaFoundation |
| n                | WikiNews            |
| q                | WikiQuote           |
| s                | WikiSource          |
| v                | WikiVersity         |
| voy              | WikiVoyage          |
| w                | MediaWiki           |
| wd               | WikiData            |
| m                | WikiMedia           |

### Insert Large Data into Table

Each file has aprox. **7 million** rows. We can't really use a simple INSERT for this operation, because it would insert each row individually:

```SQL
INSERT INTO mytable (column)
VALUES (data)
```

For this operation, we are going to use **SQLBulkCopy**. With this, we can send a large amount of data into a SQL Server Database efficiently.
In  the code below, we have a method **InsertWikiPages** that has as a parameter a **DataTable**.
We connect to the Database and create a Transaction to BulkCopy our **DataTable** into the table **WikiPage**.

```C#
private void InsertWikiPages(DataTable table)
{
  using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("WikiDumpDB")))
  {
    connection.Open();
    using (SqlTransaction transaction = connection.BeginTransaction())
    {
      using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
      {
        try
        {
          bulkCopy.DestinationTableName = "WikiPage";
          bulkCopy.WriteToServer(table);
          transaction.Commit();
        }
        catch (Exception e)
        {
          Console.WriteLine("Commit Exception Type: {0}", e.GetType());
          Console.WriteLine("  Message: {0}", e.Message);
          try
          {
            transaction.Rollback();
          }
          catch (Exception ex)
          {
            Console.WriteLine("Rollback Exception Type", ex.ToString());
            Console.WriteLine("  Message: {0}", ex.Message);
          }
        }
      }
    }
  }
}
```

---

## How can we run the Application?

1. Create the Database

In this repo, you can get the Database Script [here](https://github.com/alucardmmc/TranzactChallenge/tree/master/Docs/Queries).

Run the script on your MS SQL Server.

2. Modify the App.config file

Taking a look at the **App.config** file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<connectionStrings configSource="Connections.config">
	</connectionStrings>
	<appSettings>
		<add key="DataDumpFolder" value="D:\Projects\Projects_ASP.NET\TranzactChallenge\DataDump"/>
	</appSettings>
</configuration>
```

We can see that theres 2 things we need to create or modify.

- Create a **Connection.config** file. This file should look like this:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<connectionStrings>
		<add name="WikiDumpDB" connectionString="Your connection string here" providerName="System.Data.SqlClient"/>
	</connectionStrings>
</configuration>
```

- Modify the **DataDumpFolder** with your own path. This path is where all the WikiDump files will be downloaded.

```xml
	<appSettings>
		<add key="DataDumpFolder" value="Your Path here"/>
	</appSettings>
```
















