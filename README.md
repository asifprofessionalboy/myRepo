select W.LocationCode,newid() as ID,CAST(W.WeeklyAllowance AS float) as Weekly_allowance,W.WorkManSl,W.WorkManCategory,W.PFNo,W.ESINo,W.WorkManName,W.MonthWage,W.YearWage,
W.AadharNo,W.VendorCode,W.VendorName,W.WorkOrderNo,W.TotPaymentDays,W.holiday,W.NetWagesAmt,W.BasicWages,W.DAWages,

(select WR.DEPT_CODE from App_WorkOrder_Reg WR where WR.V_CODE = W.VendorCode and WR.WO_NO = W.WorkOrderNo) as Department,W.OtherAllow, isnull

( (Select  O.bank_statement_sl_no from App_Wages_BankStatement O where O.VendorCode = W.VendorCode and O.MonthWage = W.MonthWage and O.YearWage = W.YearWage
and O.WorkOrderNo = W.WorkOrderNo and O.WorkManName = W.WorkManName and O.LocationCode=W.LocationCode and O.AadharNo=W.AadharNo  ) ,'') as bank_statement_sl_no,

(Select O.Paid_Amount from App_Online_Wages_Details O where O.VendorCode = W.VendorCode and O.MonthWage = W.MonthWage and O.YearWage = W.YearWage and O.WorkOrderNo = W.WorkOrderNo 
and O.WorkManName = W.WorkManName and O.LocationCode=W.LocationCode) as Paid_Amount, 

(Select O.Unpaid_Amount from App_Online_Wages_Details O
where O.VendorCode = W.VendorCode and O.MonthWage = W.MonthWage and O.YearWage = W.YearWage and O.WorkOrderNo = W.WorkOrderNo
and O.WorkManName = W.WorkManName and O.LocationCode=W.LocationCode) as Unpaid_Amount,

(Select O.Total_Amount from App_Online_Wages_Details O
where O.VendorCode = W.VendorCode and O.MonthWage = W.MonthWage and O.YearWage = W.YearWage and O.WorkOrderNo = W.WorkOrderNo
and O.WorkManName = W.WorkManName and O.LocationCode=W.LocationCode) as Total_Amount,TotalWages,W.PFAmt,W.ESIAmt,W.OtherDeduAmt from App_WagesDetailsJharkhand W 
where W.MonthWage = '11' and W.YearWage='2024' and W.VendorCode = '10482' order by W.WorkManName 
