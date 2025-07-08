using System;
using sample_cli_todo.Services;
using sample_cli_todo.Models;
using System.Linq;

namespace sample_cli_todo
{
    class Program
    {
        static void Main(string[] args)
        {
            var todoService = new TodoService();
            var emailService = new EmailService();
            bool exit = false;
            while (!exit)
            {
                // Show the list of to-dos on the main screen
                var todos = todoService.GetAll()
                    .OrderBy(t => t.IsCompleted) // Not completed first, then completed
                    .ThenBy(t => t.IsCompleted ? t.CompletedAt ?? DateTime.MaxValue : DateTime.MinValue) // For completed, order by CompletedAt; for not completed, order by Id
                    .ToList();
                Console.WriteLine("\n==== To-Do List ====");
                if (todos.Count == 0)
                {
                    Console.WriteLine("No to-dos yet.");
                }
                else
                {
                    for (int i = 0; i < todos.Count; i++)
                    {
                        var todo = todos[i];
                        string status = todo.IsCompleted ? "[X]" : "[ ]";
                        if (todo.IsCompleted)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        Console.WriteLine($"{i + 1}. {status} {todo.Title}");
                        Console.ResetColor();
                    }
                    Console.WriteLine("\nEnter the number of a to-do to mark it as completed, or press Enter to continue.");
                    var markInput = Console.ReadLine();
                    if (int.TryParse(markInput, out int markIndex) && markIndex > 0 && markIndex <= todos.Count)
                    {
                        var todo = todos[markIndex - 1];
                        if (!todo.IsCompleted)
                        {
                            todoService.MarkComplete(todo.Id);
                            Console.WriteLine($"Marked '{todo.Title}' as completed.");
                        }
                        else
                        {
                            Console.WriteLine("That to-do is already completed.");
                        }
                        continue;
                    }
                }
                Console.WriteLine("\n==== Menu ====");
                Console.WriteLine("1. List all to-dos");
                Console.WriteLine("2. Add a to-do");
                Console.WriteLine("3. Update a to-do");
                Console.WriteLine("4. Delete a to-do");
                Console.WriteLine("5. Toggle a to-do's completion");
                Console.WriteLine("6. Email all not-completed to-dos");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        // todoService.ListTodos();
                        Console.WriteLine("[List to-dos]");
                        break;
                    case "2":
                        Console.Write("Enter title: ");
                        var title = Console.ReadLine();
                        Console.Write("Enter description: ");
                        var description = Console.ReadLine();
                        todoService.AddTodo(title, description);
                        Console.WriteLine("To-do added.");
                        break;
                    case "3":
                        Console.WriteLine("Enter the number of the to-do to update:");
                        var updateInput = Console.ReadLine();
                        if (int.TryParse(updateInput, out int updateIndex) && updateIndex > 0 && updateIndex <= todos.Count)
                        {
                            var todo = todos[updateIndex - 1];
                            Console.WriteLine($"Current title: {todo.Title}");
                            Console.Write("Enter new title (leave blank to keep current): ");
                            var newTitle = Console.ReadLine();
                            Console.WriteLine($"Current description: {todo.Description}");
                            Console.Write("Enter new description (leave blank to keep current): ");
                            var newDescription = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newTitle))
                                todo.Title = newTitle;
                            if (!string.IsNullOrWhiteSpace(newDescription))
                                todo.Description = newDescription;
                            // Save changes
                            todoService.UpdateTodo(todo);
                            Console.WriteLine("To-do updated.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid to-do number.");
                        }
                        break;
                    case "4":
                        Console.WriteLine("Enter the number of the to-do to delete:");
                        var deleteInput = Console.ReadLine();
                        if (int.TryParse(deleteInput, out int deleteIndex) && deleteIndex > 0 && deleteIndex <= todos.Count)
                        {
                            var todo = todos[deleteIndex - 1];
                            todoService.DeleteTodo(todo.Id);
                            Console.WriteLine($"Deleted to-do: '{todo.Title}'.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid to-do number.");
                        }
                        break;
                    case "5":
                        Console.WriteLine("Enter the number of the to-do to toggle completion:");
                        var toggleInput = Console.ReadLine();
                        if (int.TryParse(toggleInput, out int toggleIndex) && toggleIndex > 0 && toggleIndex <= todos.Count)
                        {
                            var todo = todos[toggleIndex - 1];
                            todoService.ToggleComplete(todo.Id);
                            Console.WriteLine($"Toggled completion for '{todo.Title}'. Now marked as {(todo.IsCompleted ? "completed" : "not completed")}.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid to-do number.");
                        }
                        break;
                    case "6":
                        // emailService.SendAllNotCompletedEmail();
                        Console.WriteLine("[Email not-completed to-dos]");
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
            Console.WriteLine("Goodbye!");
        }
    }
}