using System.Diagnostics;
using Get_It_Done.Models;
using Microsoft.AspNetCore.Mvc;

namespace Get_It_Done.Controllers
{
    public class HomeController : Controller
    {
        //Static list for now
        private static List<TaskItem> tasks = new List<TaskItem>();

        public IActionResult Index(string? filter = "All")
        {
            var filteredTask = tasks
                .Where(t =>
                {
                    return filter switch
                    {
                        "Completed" => t.IsCompleted && !t.IsDeleted,
                        "Pending" => !t.IsCompleted && !t.IsDeleted,
                        "Priority" => t.IsPriority && !t.IsDeleted,
                        "Trash" => t.IsDeleted,
                        _ => !t.IsDeleted
                    };
                })
                .OrderByDescending(t => t.IsPriority)
                .ThenByDescending(t => t.CreatedAt)
                .ToList();

            var vm = new TaskItem
            {
                Tasks = filteredTask, 
                CurrentFilter = filter
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddTask(string title, bool isPriority, DateTime? deadline)
        {
            
            tasks.Add(new TaskItem
            {
                TaskId = tasks.Count + 1,
                Task = title,
                IsPriority = isPriority,
                DeadLine = deadline             
            });
            return RedirectToAction("Index");
        }

        public IActionResult DeleteTask(int id)
        {
            var task = tasks.FirstOrDefault(t => t.TaskId == id);
            if (task != null)
            {
                task.IsDeleted = true;              
            }
            return RedirectToAction("Index");
        }

        public IActionResult ToggleComplete(int id)
        {
            var task = tasks.FirstOrDefault(t => t.TaskId == id);
            if(task != null)          
                task.IsCompleted = !task.IsCompleted;
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult UpdateTask(int id)
        {
            var task = tasks.FirstOrDefault(t => t.TaskId == id);
            return View(task);
        }

        [HttpPost]
        public IActionResult UpdateTask(TaskItem item)
        {
            var task = tasks.FirstOrDefault(t => t.TaskId == item.TaskId);
            if (task != null)
            {
                task.Task = item.Task;
                task.IsPriority = item.IsPriority;
                task.DeadLine = item.DeadLine;
            }

            return RedirectToAction("Index");
        }
    }
}
