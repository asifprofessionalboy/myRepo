in App_AttendanceDetails table has a column name DayDef in which values 
WD - working day
OD - off dat
HD - holiday
Hf - half day

 DECLARE @DynamicColumns NVARCHAR(MAX); DECLARE @SQLQuery NVARCHAR(MAX);

 SELECT @DynamicColumns = STRING_AGG(QUOTENAME(DayOfMonth), ',') 
 FROM ( SELECT DISTINCT DATEPART(DAY, ML.Dates) AS DayOfMonth FROM dbo.ListOfDaysByEngagementType('09', '2024') AS ML LEFT JOIN 
 App_AttendanceDetails AS ad ON ML.Dates = AD.Dates WHERE AD.VendorCode = '17201' AND DATEPART(MONTH, AD.Dates) = '09' AND DATEPART(YEAR, AD.Dates) = '2024' 
 ) AS Days;

 SET @SQLQuery = ' WITH AttendanceData AS ( SELECT DATEPART(DAY, ML.Dates) AS DayOfMonth, ad.WorkManSl AS WorkManSLNo, ad.WorkManName AS WorkManName,
 CASE WHEN (ML.EngagementType = AD.EngagementType AND Present = ''True'') THEN ''P'' ELSE ''A'' END AS Present, ad.EngagementType AS Eng_Type, DATEPART(MONTH, ML.Dates) AS Month,
 DATEPART(YEAR, ML.Dates) AS Year FROM dbo.ListOfDaysByEngagementType(''09'', ''2024'') AS ML LEFT JOIN App_AttendanceDetails AS ad ON ML.Dates = AD.Dates
 WHERE AD.VendorCode = ''17201'' AND DATEPART(MONTH, AD.Dates) = ''09'' AND DATEPART(YEAR, AD.Dates) = ''2024''  )

SELECT WorkManSLNo, WorkManName, Eng_Type, Month, Year, ' + @DynamicColumns + ' FROM AttendanceData PIVOT ( MAX(Present) FOR DayOfMonth IN (' + @DynamicColumns + ') )
AS PivotTable ORDER BY WorkManSLNo; ';
 EXEC sp_executesql @SQLQuery;

this query is working great just some thing
 modified it that 
 if my worker present is true and DayDef='WD' then print P means working day present
 and if my worker present is False and DayDef='WD' then print A means working day absent

and if my worker DayDef='OD' and present is true then print OP that means off day present

and if my worker DayDef='HD' and present is true  then print HP means holiday present
and if my worker DayDef='HD' and present is false  then print HD means holiday present
and if my worker DayDef='LV' and  then print LV
and of my worker DayDef='HF' and present is ture then print HF


