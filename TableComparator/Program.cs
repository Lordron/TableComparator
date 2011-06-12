using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql;

namespace TableComparator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Table Comparator v1.0";
            AppDomain.CurrentDomain.UnhandledException +=
                (o, e) => CrashReport(e.ExceptionObject.ToString());
        }

        static void CrashReport(string message)
        {
            Console.WriteLine("<<ERROR>>");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
