SELECT rm.ID, rm.SerialNo, rm.pno, rm.CreatedOn, rm.CreatedBy, rm.Type, sm.St_Description AS Status, 
(SELECT Section FROM App_SectionalHead_Master WHERE App_SectionalHead_Master.ID = rm.Section) AS Section,
CASE WHEN sm.St_Description = 'Pending with Sectional Head'
THEN

(SELECT Pno FROM App_External_Doc_Type WHERE App_External_Doc_Type.Pno = rm.Pno) WHEN sm.St_Description = 'Pending with Sectional Head'

THEN (SELECT Pno FROM App_External_Doc_Type WHERE App_External_Doc_Type.Pno = rm.Pno) ELSE 'No Details Available' 
END AS StatusDetails FROM App_External_Doc_Type AS rm LEFT JOIN App_StatusMaster AS sm ON sm.St_Code = rm.Status;
