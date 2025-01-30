DECLARE @DynamicColumns NVARCHAR(MAX);
DECLARE @SQLQuery NVARCHAR(MAX);

-- Get the distinct day numbers for the given month and year
SELECT @DynamicColumns = STRING_AGG(QUOTENAME(DayOfMonth), ',') 
FROM (
    SELECT DISTINCT DATEPART(DAY, ML.Dates) AS DayOfMonth
    FROM dbo.ListOfDaysByEngagementType('09', '2024') AS ML
    LEFT JOIN App_AttendanceDetails AS ad ON ML.Dates = AD.Dates
    WHERE AD.VendorCode = '17201'
    AND DATEPART(MONTH, AD.Dates) = '09'
    AND DATEPART(YEAR, AD.Dates) = '2024'
) AS Days;

-- Build the dynamic pivot query
SET @SQLQuery = '
WITH AttendanceData AS (
    SELECT 
        DATEPART(DAY, ML.Dates) AS DayOfMonth,
        ad.WorkManSl AS WorkManSLNo,
        ad.WorkManName AS WorkManName,
        ad.EngagementType AS Eng_Type,
        DATEPART(MONTH, ML.Dates) AS Month,
        DATEPART(YEAR, ML.Dates) AS Year,
        CASE 
            WHEN AD.DayDef = ''WD'' AND AD.Present = ''True'' THEN ''P''  -- Working Day Present
            WHEN AD.DayDef = ''WD'' AND AD.Present = ''False'' THEN ''A'' -- Working Day Absent
            WHEN AD.DayDef = ''OD'' AND AD.Present = ''True'' THEN ''OP'' -- Off Day Present
            WHEN AD.DayDef = ''HD'' AND AD.Present = ''True'' THEN ''HP'' -- Holiday Present
            WHEN AD.DayDef = ''HD'' AND AD.Present = ''False'' THEN ''HD'' -- Holiday Absent
            WHEN AD.DayDef = ''LV'' THEN ''LV'' -- Leave
            WHEN AD.DayDef = ''HF'' AND AD.Present = ''True'' THEN ''HF'' -- Half-Day Present
        END AS AttendanceStatus
    FROM dbo.ListOfDaysByEngagementType(''09'', ''2024'') AS ML
    LEFT JOIN App_AttendanceDetails AS ad ON ML.Dates = AD.Dates
    WHERE AD.VendorCode = ''17201''
    AND DATEPART(MONTH, AD.Dates) = ''09''
    AND DATEPART(YEAR, AD.Dates) = ''2024''
)

SELECT WorkManSLNo, WorkManName, Eng_Type, Month, Year, ' + @DynamicColumns + '
FROM AttendanceData
PIVOT (
    MAX(AttendanceStatus) FOR DayOfMonth IN (' + @DynamicColumns + ')
) AS PivotTable
ORDER BY WorkManSLNo;
';

-- Execute the dynamic SQL query
EXEC sp_executesql @SQLQuery;
