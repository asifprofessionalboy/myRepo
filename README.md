SELECT 
    '202406' AS procMonth,
    WO_NO,
    -- Wage Details
    (SELECT TOP 1 
        CASE 
            WHEN COUNT(*) > 0 THEN 'Y' 
            ELSE 'N' 
        END 
     FROM App_Online_Wages_Details a1 
     INNER JOIN App_Online_Wages a2 
        ON a2.V_CODE = a1.VendorCode 
        AND a2.PROC_MONTH = a1.PROC_MONTH 
        AND a2.STATUS = 'Request Closed' 
     WHERE a1.VendorCode = '11408' 
       AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS Wages,
       
    -- Wage Supplement Details
    (SELECT TOP 1 
        CASE 
            WHEN COUNT(*) > 0 THEN 'Y' 
            ELSE 'N' 
        END 
     FROM App_Online_Wages_Details_Supplement a1 
     INNER JOIN App_Online_WagesSupplement a2 
        ON a2.V_CODE = a1.VendorCode 
        AND a2.PROC_MONTH = a1.PROC_MONTH 
        AND a2.STATUS = 'Request Closed' 
     WHERE a1.VendorCode = '11408' 
       AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS WageSupplement,

    -- PF Details
    (SELECT TOP 1 
        CASE 
            WHEN COUNT(*) > 0 THEN 'Y' 
            ELSE 'N' 
        END 
     FROM App_PF_ESI_Details a1 
     INNER JOIN App_PF_ESI_Summary a2 
        ON a2.VendorCode = a1.VendorCode 
        AND a2.PROC_MONTH = a1.PROC_MONTH 
        AND a2.Status = 'Request Closed' 
     WHERE a1.VendorCode = '11408' 
       AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS PF,

    -- PF Supplement Details
    (SELECT TOP 1 
        CASE 
            WHEN COUNT(*) > 0 THEN 'Y' 
            ELSE 'N' 
        END 
     FROM App_PF_ESI_Details_Supplement a1 
     INNER JOIN App_PF_ESI_Summary_Supplement a2 
        ON a2.VendorCode = a1.VendorCode 
        AND a2.PROC_MONTH = a1.PROC_MONTH 
        AND a2.Status = 'Request Closed' 
     WHERE a1.VendorCode = '11408' 
       AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS PFSupplement,

    -- ESI Details
    (SELECT TOP 1 
        CASE 
            WHEN COUNT(*) > 0 THEN 'Y' 
            ELSE 'N' 
        END 
     FROM App_PF_ESI_Details a1 
     INNER JOIN App_PF_ESI_Summary a2 
        ON a2.VendorCode = a1.VendorCode 
        AND a2.PROC_MONTH = a1.PROC_MONTH 
        AND a2.Status = 'Request Closed' 
     WHERE a1.VendorCode = '11408' 
       AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS ESI,

    -- ESI Supplement Details
    (SELECT TOP 1 
        CASE 
            WHEN COUNT(*) > 0 THEN 'Y' 
            ELSE 'N' 
        END 
     FROM App_PF_ESI_Details_Supplement a1 
     INNER JOIN App_PF_ESI_Summary_Supplement a2 
        ON a2.VendorCode = a1.VendorCode 
        AND a2.PROC_MONTH = a1.PROC_MONTH 
        AND a2.Status = 'Request Closed' 
     WHERE a1.VendorCode = '11408' 
       AND a1.WorkOrderNo = '2500011892' 
       AND a1.PROC_MONTH = '202406') AS ESISupplement,

    -- Labour License
    (SELECT TOP 1 
        CASE 
            WHEN COUNT(*) > 0 THEN 'Y' 
            ELSE 'N' 
        END 
     FROM App_LabourLicenseSubmission 
     WHERE Vcode = '11408' 
       AND CONVERT(INT, CONVERT(VARCHAR, DATEPART(YEAR, FromDate)) + CONVERT(VARCHAR, FORMAT(DATEPART(MONTH, FromDate), '00'))) <= '202406' 
       AND CONVERT(INT, CONVERT(VARCHAR, DATEPART(YEAR, ToDate)) + CONVERT(VARCHAR, FORMAT(DATEPART(MONTH, ToDate), '00'))) >= '202406') AS LL
FROM 
    App_Vendorwodetails 
WHERE 
    WO_NO = '2500011892';
