( SELECT '202406' AS procMonth,WO_NO,(SELECT TOP 1 CASE WHEN COUNT(*) > 0 THEN 'Y' ELSE 'N' END FROM App_Online_Wages_Details a1 INNER JOIN App_Online_Wages a2 
    ON a2.V_CODE = a1.VendorCode AND a2.PROC_MONTH = a1.PROC_MONTH AND a2.STATUS = 'Request Closed' WHERE a1.VendorCode = '11408' AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS Wages )
       OR (
       (SELECT TOP 1 CASE WHEN COUNT(*) > 0 THEN 'Y' ELSE 'N' END FROM App_Online_Wages_Details_Supplement a1 INNER JOIN App_Online_WagesSupplement a2 
    ON a2.V_CODE = a1.VendorCode AND a2.PROC_MONTH = a1.PROC_MONTH AND a2.STATUS = 'Request Closed' WHERE a1.VendorCode = '11408' AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS Wages ),
