using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;

namespace TaskManagement.Controllers;
[Authorize(Roles = "Admin,Super Admin,Employee")]
public class TaskAssignedController : Controller
{
    private readonly TaskDbContext _dbContext;

    public TaskAssignedController(TaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        var assignedTasks = _dbContext.AssignedTasks.Include("Task")
            .Include("User").OrderBy(u=>u.User.Name).ToList();

        return View(assignedTasks);
    }

    public IActionResult Create()
    {
        var model= new AssignedTask
        {
            AssignedDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(7), // Default due date is 7 days from now
            Users = _dbContext.Employees.OrderBy(u=>u.Name).ToList(),
            Tasklist = _dbContext.Tasks.OrderBy(t=>t.Title).ToList()
        };
        return View(model);
    }
    [HttpPost]
    public IActionResult Create(AssignedTask assignedTask, List<int> Tasklist)
    {
        if (ModelState.IsValid)
        {
            int result = 0;
            foreach (var taskId in Tasklist)
            {
                var addnew= new AssignedTask
                {
                    UserId = assignedTask.UserId,
                    AssignedDate = assignedTask.AssignedDate,
                    DueDate = assignedTask.DueDate,
                    SubmitDate = assignedTask.SubmitDate,
                    Status = assignedTask.Status,
                    Remarks = assignedTask.Remarks
                };
                var task = _dbContext.Tasks.Find(taskId);
                if (task != null)
                {
                    addnew.Task = task;
                    addnew.TaskId = task.Id;
                }
                _dbContext.AssignedTasks.Add(addnew);
              
            }
            result = _dbContext.SaveChanges();
            if (result> 0)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Failed to create assigned task. Please try again.");

        }
        else
        {
            var message = string.Join(" | ", ModelState.Values
    .SelectMany(v => v.Errors)
    .Select(e => e.ErrorMessage));
            ModelState.AddModelError(" ", message);
        }
        assignedTask.Users = _dbContext.Employees.OrderBy(u => u.Name).ToList();
        assignedTask.Tasklist = _dbContext.Tasks.OrderBy(t => t.Title).ToList();
        return View(assignedTask);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var assignedTask = _dbContext.AssignedTasks.Find(id);
        if (assignedTask == null)
        {
            return NotFound();
        }
        assignedTask.Users = _dbContext.Employees.OrderBy(u => u.Name).ToList();
        assignedTask.Tasklist = _dbContext.Tasks.OrderBy(t => t.Title).ToList();
        return View(assignedTask);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(AssignedTask assignedTask)
    {
        if (ModelState.IsValid)
        {
            _dbContext.AssignedTasks.Update(assignedTask);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        assignedTask.Users = _dbContext.Employees.OrderBy(u => u.Name).ToList();
        assignedTask.Tasklist = _dbContext.Tasks.OrderBy(t => t.Title).ToList();
        return View(assignedTask);
    }

    [Authorize(Roles = "Admin,Super Admin")]
    public IActionResult Delete(int id)
    {
        var assignedTask = _dbContext.AssignedTasks.Include(at => at.Task).Include(at => at.User).FirstOrDefault(at => at.Id == id);
        if (assignedTask == null)
        {
            return NotFound();
        }
        return View(assignedTask);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var assignedTask = _dbContext.AssignedTasks.Find(id);
        if (assignedTask == null)
        {
            return NotFound();
        }
        _dbContext.AssignedTasks.Remove(assignedTask);
        _dbContext.SaveChanges();
        return RedirectToAction("Index");
    }

}
