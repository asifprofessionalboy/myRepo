
Select Null as LocationCode,'All' as Location,0 as ord

union select distinct w.LOC_OF_WORK as LocationCode,l.Location as Location,1 from App_WorkOrder_Reg
w left join App_LocationMaster l on l.LocationCode=w.LOC_OF_WORK where w.V_CODE='17197' and w.LOC_OF_WORK is not null

union select distinct w.LocationCode as LocationCode,l.Location as Location,1 
from App_AttendanceDetails w left join App_LocationMaster l on l.LocationCode=w.LocationCode where
   w.VendorCode='17197' and DATEPART(month,Dates)='12' and DATEPART(YEAR,Dates)='2024' and w.LocationCode is not null 
   order by ord, Location
