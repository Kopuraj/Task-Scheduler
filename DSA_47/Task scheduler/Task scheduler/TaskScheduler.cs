using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Task_scheduler
{
    class TaskSchedulerApp
    {
        static List<TaskItem> taskList = new List<TaskItem>();
        static string filePath = "tasks.json"; // File to store tasks
        static bool isRunning = true; // Control background notifications

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\t\t\t\t\t\t\tWELCOME TO TASK SCHEDULER");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\t\t\t\t\t\t========================================");
            Console.ResetColor();
            LoadTasks(); // Load tasks from file

            while (true)
            {
                Console.WriteLine("\nTask Scheduler - Choose an option:");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. Update Task");
                Console.WriteLine("3. Delete Task");
                Console.WriteLine("4. View Tasks");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");

                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    SaveTasks();
                    return;
                }

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid input! Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1: AddTask(); break;
                    case 2: UpdateTask(); break;
                    case 3: DeleteTask(); break;
                    case 4: ViewTasks(); break;
                    case 5:
                        isRunning = false;
                        SaveTasks();
                        return;
                    default: Console.WriteLine("Invalid choice! Try again."); break;
                }
            }
        }

        static void AddTask()
        {
            Console.Write("Enter Task Name: ");
            string name = Console.ReadLine();
            if (CheckExit(name)) return;

            Console.Write("Enter Due Date (yyyy-MM-dd): ");
            string dueDateInput = Console.ReadLine();
            if (CheckExit(dueDateInput)) return;

            if (!DateTime.TryParse(dueDateInput, out DateTime dueDate))
            {
                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
                return;
            }

            Console.Write("Enter Due Time (HH:mm): ");
            string dueTimeInput = Console.ReadLine();
            if (CheckExit(dueTimeInput)) return;

            if (!TimeSpan.TryParse(dueTimeInput, out TimeSpan dueTime))
            {
                Console.WriteLine("Invalid time format. Please use HH:mm.");
                return;
            }

            Console.Write("Enter Priority (1-High, 2-Medium, 3-Low): ");
            string priorityInput = Console.ReadLine();
            if (CheckExit(priorityInput)) return;

            if (!int.TryParse(priorityInput, out int priority) || priority < 1 || priority > 3)
            {
                Console.WriteLine("Invalid priority input.");
                return;
            }

            taskList.Add(new TaskItem(name, dueDate, dueTime, priority));
            SaveTasks();
            Console.WriteLine("Task added successfully!");
        }


        static void UpdateTask()
        {
            Console.Write("Enter the Task Name to Update: ");
            string name = Console.ReadLine();
            if (CheckExit(name)) return;

            TaskItem task = taskList.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                Console.WriteLine("Task not found!");
                return;
            }

            Console.Write("Enter New Task Name (or press Enter to keep the same): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName)) task.Name = newName;

            Console.Write("Enter New Due Date (yyyy-MM-dd) or press Enter to keep the same: ");
            string newDateStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDateStr) && DateTime.TryParse(newDateStr, out DateTime newDueDate))
                task.DueDate = newDueDate;

            Console.Write("Enter New Due Time (HH:mm) or press Enter to keep the same: ");
            string newTimeStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(newTimeStr) && TimeSpan.TryParse(newTimeStr, out TimeSpan newDueTime))
                task.DueTime = newDueTime;

            Console.Write("Enter New Priority (1-High, 2-Medium, 3-Low) or press Enter to keep the same: ");
            string newPriorityStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPriorityStr) && int.TryParse(newPriorityStr, out int newPriority))
                task.Priority = newPriority;

            SaveTasks();
            Console.WriteLine("Task updated successfully!");
        }

        static void DeleteTask()
        {
            Console.Write("Enter the Task Name to Delete: ");
            string name = Console.ReadLine();
            if (CheckExit(name)) return;

            TaskItem task = taskList.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                Console.WriteLine("Task not found!");
                return;
            }

            taskList.Remove(task);
            SaveTasks();
            Console.WriteLine("Task deleted successfully!");
        }

      
      

        static void ViewTasks()
        {
            if (taskList.Count == 0)
            {
                Console.WriteLine("No tasks available.");
                return;
            }

            Console.WriteLine("\nWhich sorting method do you prefer?");
            Console.WriteLine("1. Bubble Sort");
            Console.WriteLine("2. Merge Sort");
            Console.WriteLine("3. Quick Sort");
            Console.Write("Enter your choice: ");

            string input = Console.ReadLine();
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > 3)
            {
                Console.WriteLine("Invalid choice! Defaulting to Merge Sort.");
                choice = 2; // Default to Merge Sort
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            switch (choice)
            {
                case 1:
                    BubbleSort(taskList);
                    break;
                case 2:
                    MergeSort(taskList, 0, taskList.Count - 1);
                    break;
                case 3:
                    QuickSort(taskList, 0, taskList.Count - 1);
                    break;
            }

            stopwatch.Stop();
            double elapsedTime = stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000;
            Console.WriteLine($"\nSorting Execution Time: {elapsedTime:F6} ms (for {taskList.Count} tasks)");

            Console.WriteLine("\nTask List:");
            foreach (var task in taskList)
               
            {
                
                Console.WriteLine(task);
                
            }
            


        }


        static void SaveTasks()
        {
            string json = JsonSerializer.Serialize(taskList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        static void LoadTasks()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                taskList = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
        }

        static bool CheckExit(string input) => input.ToLower() == "exit";

       
        //Bubble sort
        static void BubbleSort(List<TaskItem> list)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (CompareTasks(list[j], list[j + 1]) > 0)
                    {
                        (list[j], list[j + 1]) = (list[j + 1], list[j]); // Swap
                    }
                }
            }
        }

       
        //Merge sort
        static void MergeSort(List<TaskItem> list, int left, int right)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;
                MergeSort(list, left, middle);
                MergeSort(list, middle + 1, right);
                Merge(list, left, middle, right);
            }
        }

        static void Merge(List<TaskItem> list, int left, int middle, int right)
        {
            int n1 = middle - left + 1;
            int n2 = right - middle;
            var leftList = new List<TaskItem>();
            var rightList = new List<TaskItem>();

            for (int i = 0; i < n1; i++)
                leftList.Add(list[left + i]);
            for (int j = 0; j < n2; j++)
                rightList.Add(list[middle + 1 + j]);

            int i1 = 0, i2 = 0, k = left;
            while (i1 < n1 && i2 < n2)
            {
                if (CompareTasks(leftList[i1], rightList[i2]) <= 0)
                {
                    list[k++] = leftList[i1++];
                }
                else
                {
                    list[k++] = rightList[i2++];
                }
            }

            while (i1 < n1) list[k++] = leftList[i1++];
            while (i2 < n2) list[k++] = rightList[i2++];
        }

        //Quick sort
        static void QuickSort(List<TaskItem> list, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(list, low, high);
                QuickSort(list, low, pi - 1);
                QuickSort(list, pi + 1, high);
            }
        }

        static int Partition(List<TaskItem> list, int low, int high)
        {
            TaskItem pivot = list[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (CompareTasks(list[j], pivot) < 0)
                {
                    i++;
                    (list[i], list[j]) = (list[j], list[i]); // Swap
                }
            }

            (list[i + 1], list[high]) = (list[high], list[i + 1]); // Swap pivot
            return i + 1;
        }


        static int CompareTasks(TaskItem task1, TaskItem task2)
        {
            if (task1.DueDate < task2.DueDate) return -1;
            if (task1.DueDate > task2.DueDate) return 1;
            if (task1.DueTime < task2.DueTime) return -1;
            if (task1.DueTime > task2.DueTime) return 1;
            return task1.Priority.CompareTo(task2.Priority);
        }


    }
}
