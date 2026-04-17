using System.Diagnostics;
using Get_It_Done.Models;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

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

        [HttpPost]
        public IActionResult DeleteAll()
        {
            foreach(var task in tasks)
            {
                task.IsDeleteAll = true;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCompleted()
        {
            foreach(var task in tasks.Where(t=> t.IsCompleted).ToList())
            {
                task.IsDeleted = true;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult FinishAll()
        {
            foreach(var task in tasks)
            {
                task.IsCompleted = true;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ExportJson()
        {
            var data = tasks.Where(t => !t.IsDeleted).ToList();

            var json = System.Text.Json.JsonSerializer.Serialize(data);

            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "tasks.json");
        }

        [HttpPost]
        public IActionResult ExportCsv()
        {
            var data = tasks.Where(t => !t.IsDeleted).ToList();

            var csv = new System.Text.StringBuilder();

            csv.AppendLine("ID, Task, IsCompleted, IsPriority, DeadLine");

            foreach(var d in data)
            {
                csv.AppendLine($"{d.TaskId}, {d.Task}, {d.IsCompleted}, {d.IsPriority}, {d.DeadLine}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "tasks.csv");
        }

        [HttpPost]
        public IActionResult ExportPdf()
        {
            var data = tasks.Where(t => !t.IsDeleted).ToList();

            using(var stream = new MemoryStream())
            {
                using (var writer = new PdfWriter(stream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);
                        {
                            document.Add(new Paragraph("Task List"));
                            foreach (var t in data)
                            {
                                document.Add(new Paragraph(
                                    $"{t.Task} | Completed: {t.IsCompleted} | Priority: {t.IsPriority} | Deadline: {t.DeadLine}"));
                            }
                        }
                    }
                }                    
                return File(stream.ToArray(), "application/pdf", "tasks.pdf");
            }
        }
    }
}
