using FileApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FileApp.Data;

namespace FileApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            this._context = context;

        }
        public IActionResult Index()
        {
            var customers = _context.customers.ToList();
            return View(customers);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(CustomerModel customers)
        {
            if (ModelState.IsValid)
            {
                if (customers.Picture != null)
                {
                    // First, save the customer to get the ID
                    _context.customers.Add(customers);
                    _context.SaveChanges();

                    // Now, handle the picture upload with the generated ID
                    string fileName = Path.GetFileName(customers.Picture.FileName);
                    string extension = Path.GetExtension(fileName).ToLowerInvariant();
                    string isExist = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Pictures");
                    if (!Directory.Exists(isExist))
                    {
                        Directory.CreateDirectory(isExist);
                    }
                    string filePath = Path.Combine(isExist, customers.Id + extension);
                    if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                    {
                        ModelState.AddModelError("Picture", "Only .jpg, .png, and .jpeg files are allowed.");
                        // We should probably remove the customer we just created if the picture is invalid
                        _context.customers.Remove(customers);
                        _context.SaveChanges();
                        return View(customers);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        customers.Picture.CopyTo(stream);
                    }
                    customers.PicturePath = "/Pictures/" + customers.Id + extension;

                    // Update the customer with the PicturePath
                    _context.customers.Update(customers);
                    if (_context.SaveChanges() > 0)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    _context.customers.Add(customers);
                    if (_context.SaveChanges() > 0)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
                ModelState.AddModelError(" ", message);
            }

            return View(customers);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            { return BadRequest(); }
            var obj = _context.customers.Find(id);

            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(CustomerModel customers)
        {
            if (ModelState.IsValid)
            {
                if (customers.Picture != null)
                {
                    string fileName = Path.GetFileName(customers.Picture.FileName);
                    string extension = Path.GetExtension(fileName).ToLower();
                    string oldpath = customers.PicturePath;

                    string oldpathcombine = Directory.GetCurrentDirectory() + "/wwwroot" + oldpath;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Pictures", customers.Id + extension);
                    if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                    {
                        ModelState.AddModelError("Picture", "Only .jpg, .png, and .jpeg files are allowed.");
                        return View(customers);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        customers.Picture.CopyTo(stream);
                    }
                    if (System.IO.File.Exists(oldpathcombine))
                    {
                        System.IO.File.Delete(oldpathcombine);
                    }
                    customers.PicturePath = "/Pictures/" + customers.Id + extension;
                }
                else
                {
                    customers.PicturePath = customers.PicturePath;
                }
                _context.customers.Update(customers);
                if (_context.SaveChanges() > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
                ModelState.AddModelError(" ", message);
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _context.customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var customers = _context.customers.Find(id);
            if (customers == null) return NotFound();

            if (!string.IsNullOrEmpty(customers.PicturePath))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",customers.PicturePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.customers.Remove(customers);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}