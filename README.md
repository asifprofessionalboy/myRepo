string str_exit = "3/6/2024 12:00:00 AM";

// Parse the string into a DateTime object
DateTime dateTime = DateTime.Parse(str_exit);

// Format the date to "MM/dd/yyyy" and include the hour
string str_exit_acc = $"{dateTime:MM/dd/yyyy h}";
Console.WriteLine(str_exit_acc);
