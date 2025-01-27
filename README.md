DECLARE @DynamicColumns NVARCHAR(MAX);
DECLARE @SQLQuery NVARCHAR(MAX);

-- Step 1: Generate the dynamic column list for days of the month
SELECT @DynamicColumns = STRING_AGG(QUOTENAME(DayOfMonth), ',') 
FROM (
    SELECT DISTINCT DATEPART(DAY, ML.Dates) AS DayOfMonth
    FROM dbo.ListOfDaysByEngagementType('09', '2024') AS ML
    LEFT JOIN App_AttendanceDetails AS AD ON ML.Dates = AD.Dates
    WHERE AD.VendorCode = '17201'
      AND DATEPART(MONTH, AD.Dates) = '09'
      AND DATEPART(YEAR, AD.Dates) = '2024'
) AS Days;

-- Step 2: Create the dynamic SQL query
SET @SQLQuery = '
WITH AttendanceData AS (
    SELECT 
        DATEPART(DAY, ML.Dates) AS DayOfMonth,
        AD.WorkManSl AS WorkManSLNo,
        AD.WorkManName AS WorkManName,
        CASE 
            WHEN (ML.EngagementType = AD.EngagementType AND AD.Present = ''True'') THEN ''P''
            WHEN (ML.EngagementType = AD.EngagementType AND AD.Present = ''False'' AND AD.IsHoliday = ''True'') THEN ''H''
            WHEN (ML.EngagementType = AD.EngagementType AND AD.Present = ''False'' AND AD.IsLeave = ''True'') THEN ''L''
            ELSE ''A'' 
        END AS Status,
        CASE WHEN AD.IsHoliday = ''True'' THEN 1 ELSE 0 END AS Holiday,
        CASE WHEN AD.IsLeave = ''True'' THEN 1 ELSE 0 END AS LeaveCount,
        CASE WHEN AD.Present = ''True'' THEN 1 ELSE 0 END AS PresentCount,
        1 AS TotalDays,
        AD.EngagementType AS Eng_Type,
        DATEPART(MONTH, ML.Dates) AS Month,
        DATEPART(YEAR, ML.Dates) AS Year
    FROM dbo.ListOfDaysByEngagementType(''09'', ''2024'') AS ML
    LEFT JOIN App_AttendanceDetails AS AD ON ML.Dates = AD.Dates
    WHERE AD.VendorCode = ''17201''
      AND DATEPART(MONTH, AD.Dates) = ''09''
      AND DATEPART(YEAR, AD.Dates) = ''2024''
)
SELECT 
    WorkManSLNo, 
    WorkManName, 
    Eng_Type, 
    Month, 
    Year, 
    ' + @DynamicColumns + ',
    SUM(Holiday) AS Holiday,
    SUM(LeaveCount) AS Leave,
    SUM(PresentCount) AS TotalPresent,
    SUM(TotalDays) AS Total
FROM AttendanceData
PIVOT (
    MAX(Status) FOR DayOfMonth IN (' + @DynamicColumns + ')
) AS PivotTable
ORDER BY WorkManSLNo;
';

-- Step 3: Execute the dynamic SQL
EXEC sp_executesql @SQLQuery;
