DECLARE @DynamicColumns NVARCHAR(MAX); 
DECLARE @SQLQuery NVARCHAR(MAX);

SELECT @DynamicColumns = STRING_AGG(QUOTENAME(DayOfMonth), ',') 
FROM ( 
    SELECT DISTINCT DATEPART(DAY, ML.Dates) AS DayOfMonth 
    FROM dbo.ListOfDaysByEngagementType('09', '2024') AS ML 
    LEFT JOIN App_AttendanceDetails AS ad ON ML.Dates = AD.Dates 
    WHERE AD.VendorCode = '17201' 
    AND DATEPART(MONTH, AD.Dates) = '09' 
    AND DATEPART(YEAR, AD.Dates) = '2024' 
) AS Days;

SET @SQLQuery = ' 
WITH AttendanceData AS ( 
    SELECT DATEPART(DAY, ML.Dates) AS DayOfMonth, 
           ad.WorkManSl AS WorkManSLNo, 
           ad.WorkManName AS WorkManName, 
           CASE 
               WHEN (ML.EngagementType = AD.EngagementType AND Present = ''True'') THEN ''P'' 
               ELSE ''A'' 
           END AS Present, 
           ad.EngagementType AS Eng_Type, 
           DATEPART(MONTH, ML.Dates) AS Month, 
           DATEPART(YEAR, ML.Dates) AS Year 
    FROM dbo.ListOfDaysByEngagementType(''09'', ''2024'') AS ML 
    LEFT JOIN App_AttendanceDetails AS ad ON ML.Dates = AD.Dates 
    WHERE AD.VendorCode = ''17201'' 
    AND DATEPART(MONTH, AD.Dates) = ''09'' 
    AND DATEPART(YEAR, AD.Dates) = ''2024''
)

SELECT WorkManSLNo, 
       WorkManName, 
       Eng_Type, 
       Month, 
       Year, 
       ' + @DynamicColumns + ',
       SUM(CASE WHEN Present = ''P'' THEN 1 ELSE 0 END) AS TotalPresent
FROM AttendanceData
PIVOT ( 
    MAX(Present) FOR DayOfMonth IN (' + @DynamicColumns + ') 
) AS PivotTable
GROUP BY WorkManSLNo, WorkManName, Eng_Type, Month, Year
ORDER BY WorkManSLNo;';

EXEC sp_executesql @SQLQuery;
