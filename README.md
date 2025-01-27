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


    --for holiday calculation

    Select datepart(d, ML.Dates) as Dates,ML.EngagementType, ad.WorkManSl as WorkManSLNo, ad.WorkManName as WorkManName, ad.DayDef as DayDef, 
                           case when( ML.EngagementType = AD.EngagementType and Present = 'True') then 1 else 0 end as Present , 
                           ad.EngagementType as Eng_Type
                            from dbo.ListOfDaysByEngagementType('09','2024') as ML 
                          left join App_AttendanceDetails as ad on ML.Dates = AD.Dates 
                           where AD.VendorCode ='17201'  and DATEPART(month, AD.Dates)='9' and DATEPART(year, AD.Dates)='2024' 
                            group by ML.Dates,ML.EngagementType,AD.EngagementType,AD.WorkManSl,ad.WorkManName,AD.Present,AD.WorkOrderNo,ad.DayDef 
                           order by ML.Dates,AD.WorkManSl
