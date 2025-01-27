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
            WHEN AD.DayDef = ''HD'' THEN ''H''  -- Holiday
            WHEN AD.DayDef = ''LV'' THEN ''L''  -- Leave
            WHEN AD.DayDef = ''P'' THEN ''P''  -- Present
            ELSE ''A''                          -- Absent
        END AS Status,
        CASE WHEN AD.DayDef = ''HD'' THEN 1 ELSE 0 END AS Holiday,
        CASE WHEN AD.DayDef = ''LV'' THEN 1 ELSE 0 END AS LeaveCount,
        CASE WHEN AD.DayDef = ''P'' THEN 1 ELSE 0 END AS PresentCount,
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
    SUM(Holiday) AS TotalHolidays,
    SUM(LeaveCount) AS TotalLeaves,
    SUM(PresentCount) AS TotalPresent,
    SUM(TotalDays) AS TotalDays
FROM AttendanceData
PIVOT (
    MAX(Status) FOR DayOfMonth IN (' + @DynamicColumns + ')
) AS PivotTable
ORDER BY WorkManSLNo;
';

-- Step 3: Execute the SQL query
EXEC sp_executesql @SQLQuery;
