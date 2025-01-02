select rm.ID,rm.SerialNo,rm.pno,rm.CreatedOn,rm.CreatedBy,rm.Type,sm.St_Description as Status,

(select Section from App_SectionalHead_Master where App_SectionalHead_Master.ID = rm.Section) as Section 

from App_External_Doc_Type as rm left join App_StatusMaster as sm on sm.St_Code = rm.Status

where status case when sm.St_Description='Pending with Sectional Head' then ()
