string str_exit_acc = str_exit.Substring(0, 10);


input - 3/6/2024 12:00:00 AM
output - 3/6/2024 1



string str_exit = "3/6/2024 12:00:00 AM";

// Parse the string into a DateTime object
DateTime dateTime = DateTime.Parse(str_exit);

// Format the DateTime object to get only the date
string str_exit_acc = $"{dateTime:M/d/yyyy}";
Console.WriteLine(str_exit_acc);
