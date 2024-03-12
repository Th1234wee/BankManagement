using BankClassLibrary;

namespace BankManagement
{
    internal class Program
    {
        static List<BankAccount> accounts = new List<BankAccount>();
        static void Main(string[] args)
        {
            Console.WriteLine("Bank Accounts!");
            Console.WriteLine();
            if (CreateAccounts("create.txt") == false) return;
            Console.WriteLine();
            ViewAccounts();
            while (true)
            {
                Console.WriteLine();
                Console.Write("Account Number: ");
                string no = Console.ReadLine() ?? "";
                var found = accounts.FirstOrDefault(a => a.Number == no);
                if (found == null)
                {
                    Console.WriteLine($" No account with number, {no}");
                    continue;
                }
                ActionAccount(found);
                Console.WriteLine();
                ViewAccounts();
                Console.WriteLine();
                Console.WriteLine("Press any key for another account or Esc to go stop");
                if (Console.ReadKey().Key == ConsoleKey.Escape) break;
            }
        }
        static void ViewSuccess(string text, bool isNewLine = false)
        {
            ConsoleColor temp = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.BackgroundColor = temp;
            if (isNewLine) Console.WriteLine();
        }
        static void ViewFail(string text, bool isNewLine = false)
        {
            ConsoleColor temp = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.BackgroundColor = temp;
            if (isNewLine) Console.WriteLine();
        }
        static void ViewAccounts()
        {
            Console.WriteLine("[Viewing accounts]");
            if (accounts.Any())
            {
                Console.WriteLine($"{"Number",-15} {"Holder",-15} {"Balance($)",12} {"Trans.",6}");
                Console.WriteLine(new string('=', (15 + 1 + 15 + 1 + 12 + 1 + 6)));
            }
            foreach (var acc in accounts)
            {
                Console.WriteLine($"{acc.Number,-15} {acc.Holder,-15} {acc.Balance,12:N2} {acc.Transactions.Count(),6}");
            }
        }
        static bool CreateAccounts(string file)
        {
            if (!File.Exists(file))
            {
                ViewFail($"The data file, {file}, does not exist.", true);
                return false;
            }
            Console.WriteLine("Creating accounts...");
            int success = 0;
            int fail = 0;
            foreach (var line in File.ReadAllLines(file))
            {
                var arr = line.Split('/');
                if (arr.Length < 2) { fail++; continue; }
                if (double.TryParse(arr[1], out var balance) == false) { fail++; continue; }
                try
                {
                    accounts.Add(BankAccount.Create(arr[0], balance));
                    success++;
                }
                catch (Exception ex)
                {
                    ConsoleColor temp = Console.BackgroundColor;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($" =>Failed to create account> {ex.Message}");
                    Console.BackgroundColor = temp;
                    fail++;
                }
            }
            ViewSuccess($"=>Succeded:{success}, Failed:{fail}", true);
            return accounts.Count > 0;
        }
        static void ActionAccount(BankAccount found)
        {
            Console.WriteLine();
            Console.WriteLine($"[Acting on the account {found.Number}]");
            string[] options = { "Viewing", "Deposit", "Withdraw", "Reset", "Close", "Return" };
            while (true)
            {
                Console.Write("Options>> ");
                for (int index = 0; index < options.Length; index++)
                {
                    if (index > 0) Console.Write(", ");
                    Console.Write($" {index + 1,2}-{options[index]}");
                }
                Console.WriteLine();
                int choose = 0;
                while (true)
                {
                    Console.Write(" Choose: ");
                    if (int.TryParse(Console.ReadLine(), out choose) == false
                    || choose < 1 || options.Length < choose)
                    {
                        ViewFail(" =>Invalid chosen option", true);
                        continue;
                    }
                    choose--;
                    break;
                }
                if (choose == options.Length - 1) break;
                switch (choose)
                {
                    case 0: ViewDetails(found); break;
                    case 1: MakeDeposit(found); break;
                    case 2: MakeWithdrawal(found); break;
                    case 3: MakeReset(found); break;
                    case 4: MakeClose(found); break;
                };
                Console.WriteLine();
            }
        }
        static void ViewDetails(BankAccount acc)
        {
            Console.WriteLine();
            Console.WriteLine("[Viewing an account]");
            Console.WriteLine($"Number : {acc.Number}");
            Console.WriteLine($"Holder : {acc.Holder}");
            Console.WriteLine($"Balance($) : {acc.Balance}");
            Console.WriteLine($"Transactions: {"Date",-20} {"Type",-10} {"Amount($)",12} {"Note",-30}");
            Console.WriteLine($" {new string('-', 20 + 1 + 10 + 1 + 12 + 1 + 30)}");
            foreach (var tran in acc.Transactions)
            {
                Console.WriteLine($"{"",14}{tran.Date,-20} {tran.Type.ToString(),-10} {tran.Amount,12:N2} {tran.Note} ");
            }
        }
        static void MakeDeposit(BankAccount acc)
        {
            Console.WriteLine();
            Console.WriteLine("[Depositing]");
            double amount = 0;
            while (true)
            {
                Console.Write(" Amount: ");
                if (double.TryParse(Console.ReadLine(), out amount) == false)
                {
                    ViewFail(" =>Invalid amount", true);
                    continue;
                }
                break;
            }
            Console.Write(" note : ");
            var note = Console.ReadLine() ?? "";
            try
            {
                acc.Deposit(amount, note);
                ViewSuccess(" =>Successfully", true);
            }
            catch (Exception ex)
            {
                ViewFail($" =>Failed to deposit> {ex.Message.Trim()}", true);
            }
        }
        static void MakeWithdrawal(BankAccount acc)
        {
            Console.WriteLine();
            Console.WriteLine("[Withdrawing]");
            double amount = 0;
            while (true)
            {
                Console.Write(" Amount: ");
                if (double.TryParse(Console.ReadLine(), out amount) == false)
                {
                    ViewFail(" =>Invalid amount", true);
                    continue;
                }
                break;
            }
            Console.Write(" note : ");
            var note = Console.ReadLine() ?? "";
            try
            {
                acc.WithDraw(amount, note);
                ViewSuccess(" =>Successfully", true);
            }
            catch (Exception ex)
            {
                ViewFail($" =>Failed to withdraw> {ex.Message}", true);
            }
        }
        static void MakeReset(BankAccount acc)
        {
            Console.WriteLine();
            Console.WriteLine("[Resetting]");
            Console.Write(" note : ");
            var note = Console.ReadLine() ?? "";
            try
            {
                acc.Reset(note);
                ViewSuccess(" =>Successfully", true);
            }
            catch (Exception ex)
            {
                ViewFail($" =>Failed to reset> {ex.Message}", true);
            }
        }
        static void MakeClose(BankAccount acc)
        {
            Console.WriteLine();
            Console.WriteLine("[Closing]");
            Console.Write(" note : ");
            var note = Console.ReadLine() ?? "";
            try
            {
                acc.Close(note);
                ViewSuccess(" =>Successfully", true);
            }
            catch (Exception ex)
            {
                ViewFail($" =>Failed to close> {ex.Message}", true);
            }
        }
    }
}