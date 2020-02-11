# TabletArtco

OS: Windows 10 Pro 64bit
IDE : Visual Studio 2019 Community
Language : C#, XML
Framework : Xamarin.Android
Target Framework : Android 9.0(Pie)

Coding Conventions
  Coding conventions serve the following purposes:

  - They create a consistent look to the code, so that readers can focus on content, not layout.
  - They enable readers to understand the code more quickly by making assumptions based on previous experience.
  - They facilitate copying, changing, and maintaining the code.
  - They demonstrate C# best practices.

  The guidelines in this topic are used by Microsoft to develop samples and documentation.

  # Naming Conventions

  Do use PascalCasing for class names and method names.

      public class ClientActivity
      {
          public void ClearStatistics()
          {
              //...
          }
          public void CalculateStatistics()
          {
              //...
          }
      }

  Do use camelCasing for method arguments and local variables.

      public class UserLog
      {
          public void Add(LogEvent logEvent)
          {
              int itemCount = logEvent.Items.Count;
              // ...
          }
      }

  Do not use Hungarian notation or any other type identification in identifiers

      // Correct
      int counter;
      string name;

      // Avoid
      int iCounter;
      string strName;

  Do not use Screaming Caps for constants or readonly variables

      // Correct
      public static const string ShippingType = "DropShip";

      // Avoid
      public static const string SHIPPINGTYPE = "DropShip";

  Avoid using Abbreviations. Exceptions: abbreviations commonly used as names, such as Id, Xml, Ftp, Uri

      // Correct
      UserGroup userGroup;
      Assignment employeeAssignment;

      // Avoid
      UserGroup usrGrp;
      Assignment empAssignment;

      // Exceptions
      CustomerId customerId;
      XmlDocument xmlDocument;
      FtpHelper ftpHelper;
      UriPart uriPart;

  Do not use Underscores in identifiers. Exception: you can prefix private static variables with an underscore.

      // Correct
      public DateTime clientAppointment;
      public TimeSpan timeLeft;

      // Avoid
      public DateTime client_Appointment;
      public TimeSpan time_Left;

      // Exception
      private DateTime _registrationDate;

  Do use predefined type names instead of system type names like Int16, Single, Uint64, etc

      // Correct
      string firstName;
      int lastIndex;
      bool isSaved;

      // Avoid
      String firstName;
      Int32 lastIndex;
      Boolean isSaved;

  Do use implicit type var for local variable declarations. Exception: primitive types(int, string, double, etc) use predefined names.

      var stream = File.Create(path);
      var customers = new Dictionary();

      // Exceptions
      int index = 100;
      string timeSheet;
      bool isCompleted;

  Do use noun or noun phrases to name a class

      public class Employee
      {
      }
      public class BusinessLocation
      {
      }
      public class DocumentCollection
      {
      }

  Do vertically align curly brackets

      // Correct
      class Program
      {
          static void Main(string[] args)
          {
          }
      }

  Do declare all member variables at the top of a class, with static variables at the very top

      // Correct
      public class Account
      {
          public static string BankName;
          public static decimal Reserves;

          public string Number {get; set;}
          public DateTime DateOpened {get; set;}
          public DateTime DateClosed {get; set;}
          public decimal Balance {get; set;}

          // Constructor
          public Account()
          {
              // ...
          }
      }

  Do not explicitly specify a type of an enum or values of enums

      // Don't
      public enum Direction : long
      {
          North = 1,
          East = 2,
          South = 3,
          West = 4
      }

      // Correct
      public enum Direction
      {
          North,
          East,
          South,
          West
      }

  Do not suffix enum names with Enum

      // Don't
      public enum CoinEnum
      {
          Penny,
          Nickel,
          Dime,
          Quarter,
          Dollar
      }

      // Correct
      public enum Coin
      {
          Penny,
          Nickel,
          Dime,
          Quarter,
          Dollar
      }

  # Layout Conventions

  Good layout uses formatting to emphasize the structure of your code and to make the code easier to read. Microsoft examples and samples conform to the following conventions:

  - Use the default Code Editor settings (smart indenting, four-character indents, tabs saved as spaces). For more information, see [Options, Text Editor, C#, Formatting](https://docs.microsoft.com/ko-kr/visualstudio/ide/reference/options-text-editor-csharp-formatting).
  - Write only one statement per line.
  - Write only one declaration per line.
  - If continuation lines are not indented automatically, indent them one tab stop (four spaces).
  - Add at least one blank line between method definitions and property definitions.
  - Use parentheses to make clauses in an expression apparent, as shown in the following code.

      if ((val1 > val2) && (val1 > val3))
      {
          // Take appropriate action.
      }

  # Commenting Conventions

  - Place the comment on a separate line, not at the end of a line of code.
  - Begin comment text with an uppercase letter.
  - End comment text with a period.
  - Insert one space between the comment delimiter (//) and the comment text, as shown in the following example.

      // The following declaration creates a query. It does not run
      // the query.

  - Do not create formatted blocks of asterisks around comments.
