DECLARE @DynamicColumns NVARCHAR(MAX);
DECLARE @SQLQuery NVARCHAR(MAX);

-- Generate dynamic column names for days
SELECT @DynamicColumns = STRING_AGG(QUOTENAME(DayOfMonth), ',')
FROM (
    SELECT DISTINCT DATEPART(DAY, ML.Dates) AS DayOfMonth
    FROM dbo.ListOfDaysByEngagementType('09', '2024') AS ML
    LEFT JOIN App_AttendanceDetails AS ad
        ON ML.Dates = AD.Dates
    WHERE AD.VendorCode = '17201'
        AND DATEPART(MONTH, AD.Dates) = '09'
        AND DATEPART(YEAR, AD.Dates) = '2024'
) AS Days;

-- Build the main SQL query with additional calculations
SET @SQLQuery = '
WITH AttendanceData AS (
    SELECT 
        DATEPART(DAY, ML.Dates) AS DayOfMonth,
        ad.WorkManSl AS WorkManSLNo,
        ad.WorkManName AS WorkManName,
        CASE 
            WHEN (ML.EngagementType = AD.EngagementType AND Present = ''True'') THEN ''P''
            WHEN AD.DayDef = ''HD'' THEN ''HD''
            WHEN AD.DayDef = ''LV'' THEN ''LV''
            ELSE ''A''
        END AS DayDef,
        ad.EngagementType AS Eng_Type,
        DATEPART(MONTH, ML.Dates) AS Month,
        DATEPART(YEAR, ML.Dates) AS Year
    FROM dbo.ListOfDaysByEngagementType(''09'', ''2024'') AS ML
    LEFT JOIN App_AttendanceDetails AS ad
        ON ML.Dates = AD.Dates
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
    SUM(CASE WHEN DayDef = ''HD'' THEN 1 ELSE 0 END) AS Holiday,
    SUM(CASE WHEN DayDef = ''LV'' THEN 1 ELSE 0 END) AS Leave,
    SUM(CASE WHEN DayDef = ''P'' THEN 1 ELSE 0 END) AS TotalPresent,
    COUNT(*) AS Total
FROM AttendanceData
PIVOT (
    MAX(DayDef)
    FOR DayOfMonth IN (' + @DynamicColumns + ')
) AS PivotTable
GROUP BY WorkManSLNo, WorkManName, Eng_Type, Month, Year
ORDER BY WorkManSLNo;
';

-- Execute the dynamic SQL query
EXEC sp_executesql @SQLQuery;
