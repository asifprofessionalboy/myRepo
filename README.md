SELECT 
    rm.ID,
    rm.SerialNo,
    rm.pno,
    rm.CreatedOn,
    rm.CreatedBy,
    rm.Type,
    sm.St_Description AS Status,
    (SELECT Section FROM App_SectionalHead_Master WHERE App_SectionalHead_Master.ID = rm.Section) AS Section,
    CASE 
        WHEN sm.St_Description = 'Pending with Sectional Head' THEN 
            (SELECT SectionHead FROM App_SectionalHead_Master WHERE App_SectionalHead_Master.ID = rm.Section)
        WHEN sm.St_Description = 'Approved' THEN 
            (SELECT ApproverName FROM Approval_Details WHERE Approval_Details.pno = rm.pno)
        WHEN sm.St_Description = 'Rejected' THEN 
            (SELECT RejectReason FROM Rejection_Details WHERE Rejection_Details.pno = rm.pno)
        ELSE 
            'No Details Available'
    END AS StatusDetails
FROM 
    App_External_Doc_Type AS rm
LEFT JOIN 
    App_StatusMaster AS sm 
ON 
    sm.St_Code = rm.Status;
