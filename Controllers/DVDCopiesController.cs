#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.DBContext;
using RopeyDVDs.Models;

namespace RopeyDVDs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DVDCopiesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public DVDCopiesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: DVDCopies
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.DVDCopy.Include(d => d.DVDTitle);
            return View(await applicationDBContext.ToListAsync());
        }

        [Authorize(Roles = "Manager")]
        /**
         * The user select a dvd copy and find the details of the last loan
         * Displays
         *      * if Copy is still on loan
         *      * Member who borrowed it
         *      * Date out
         *      * Due Back
         *      * DVD title
         */
        public IActionResult DVDCopyStatus(int? id)
        {
            if (id == null) return NotFound();

            var everLoaned = from loan in _context.Loan
                             where loan.CopyNumber == id
                             select loan;

            if (everLoaned.Count() == 0) return RedirectToAction("Index");

            var data = from copy in _context.DVDCopy
                       join loan in _context.Loan on copy.Id equals loan.CopyNumber
                       join member in _context.Member on loan.MemberNumber equals member.Id
                       where copy.Id == id
                       select new
                       {
                           IsOnLoan = (loan.DateReturned == null),
                           Member = String.Join(" ", new string[] { loan.Member.MemberFirstName, loan.Member.MemberLastName }),
                           DateOut = loan.DateOut,
                           DateDue = loan.DateDue,
                           DateBack = loan.DateReturned,
                           Title = copy.DVDTitle.Title
                       };

            ViewData["DVDCopy"] = data.First();

            return View("Views/DVDCopies/Status.cshtml");
        }


        // GET: DVDCopies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopy
                .Include(d => d.DVDTitle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dVDCopy == null)
            {
                return NotFound();
            }

            return View(dVDCopy);
        }

        // GET: DVDCopies/Create
        public IActionResult Create()
        {
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "ID", "ID");
            return View();
        }

        // POST: DVDCopies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsDeleted,DVDNumber,DatePurchased")] DVDCopy dVDCopy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDCopy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "ID", "ID", dVDCopy.DVDNumber);
            return View(dVDCopy);
        }

        // GET: DVDCopies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopy.FindAsync(id);
            if (dVDCopy == null)
            {
                return NotFound();
            }
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "ID", "ID", dVDCopy.DVDNumber);
            return View(dVDCopy);
        }

        // POST: DVDCopies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsDeleted,DVDNumber,DatePurchased")] DVDCopy dVDCopy)
        {
            if (id != dVDCopy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dVDCopy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DVDCopyExists(dVDCopy.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "ID", "ID", dVDCopy.DVDNumber);
            return View(dVDCopy);
        }

        // GET: DVDCopies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopy
                .Include(d => d.DVDTitle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dVDCopy == null)
            {
                return NotFound();
            }

            return View(dVDCopy);
        }

        // POST: DVDCopies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDCopy = await _context.DVDCopy.FindAsync(id);
            _context.DVDCopy.Remove(dVDCopy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDCopyExists(int id)
        {
            return _context.DVDCopy.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateDeleteStatus()
        {
            DateTime dateLimit = DateTime.Now.AddDays(-365);

            List<DVDCopy> dVDCopies = (
                from dvdCopy in _context.DVDCopy.Where(d => d.IsDeleted == false && d.DatePurchased.CompareTo(dateLimit) < 0)
                select dvdCopy
                ).ToList();


            // updating the deleted status of the dvd copies
            foreach (DVDCopy dvdCopy in dVDCopies)
            {
                dvdCopy.IsDeleted = true;
            }

            _context.SaveChanges();

            var data = from dvdCopy in _context.DVDCopy.Where(d => d.IsDeleted == false && d.DatePurchased.CompareTo(dateLimit) < 0)
                       join loan in _context.Loan on dvdCopy.Id equals loan.CopyNumber
                       where loan.DateReturned != null
                       join dvdTitle in _context.DVDTitle on dvdCopy.DVDNumber equals dvdTitle.ID
                       select new
                       {
                           DVDname = dvdTitle.Title,
                           dateReleased = dvdTitle.DateReleased.ToShortDateString(),
                           datePurchased = dvdCopy.DatePurchased.ToShortDateString(),
                       };
            var modifiedData = data.GroupBy(d => d.DVDname).Select(g => g.FirstOrDefault()).ToList();

            return View("Views/DVDCopies/ViewOldDVDCopies.cshtml", modifiedData);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ViewOldDVDCopies()
        {
            DateTime dateLimit = DateTime.Now.AddDays(-365);

            var allDVDCopies = from dvdCopy in _context.DVDCopy.Where(d => d.IsDeleted == false && d.DatePurchased.CompareTo(dateLimit) < 0)
                               select dvdCopy;
            var dvdCopiesLoaned = from dvdCopies in _context.DVDCopy
                                  join loan in _context.Loan on dvdCopies.Id equals loan.CopyNumber
                                  select dvdCopies;
            var neverLoanedDVDCopies = allDVDCopies.Except(dvdCopiesLoaned).ToList();



            var list1 = from dvdCopy in _context.DVDCopy.Where(d => neverLoanedDVDCopies.Contains(d))
                        join dvdTitle in _context.DVDTitle on dvdCopy.DVDNumber equals dvdTitle.ID
                        select new
                        {
                            DVDname = dvdTitle.Title,
                            dateReleased = dvdTitle.DateReleased.ToShortDateString(),
                            datePurchased = dvdCopy.DatePurchased.ToShortDateString(),
                        };


            var loanedOldDVDCopies = (from dvdCopy in _context.DVDCopy.Where(d => d.IsDeleted == false && d.DatePurchased.CompareTo(dateLimit) < 0)
                                     join loan in _context.Loan on dvdCopy.Id equals loan.CopyNumber
                                     where loan.DateReturned != null
                                     join dvdTitle in _context.DVDTitle on dvdCopy.DVDNumber equals dvdTitle.ID
                                     select new
                                     {
                                         DVDname = dvdTitle.Title,
                                         dateReleased = dvdTitle.DateReleased.ToShortDateString(),
                                         datePurchased = dvdCopy.DatePurchased.ToShortDateString(),
                                     }).ToList();

            var mergedList = loanedOldDVDCopies.Concat(list1);

            var modifiedData = mergedList.GroupBy(d => d.DVDname).Select(g => g.FirstOrDefault()).ToList();


            return View(modifiedData);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ViewLast31DaysLoanedDVDCopies()
        {
            DateTime dateLimit = DateTime.Now.AddDays(-31);
            var booksLoanedLast31Days = from dvdCopy in _context.DVDCopy.Where(d => d.IsDeleted == false)
                       join loan in _context.Loan on dvdCopy.Id equals loan.CopyNumber
                       where (loan.DateReturned != null && loan.DateOut.CompareTo(dateLimit) >= 0)
                       join dvdTitle in _context.DVDTitle on dvdCopy.DVDNumber equals dvdTitle.ID
                       select new
                       {
                           DVDname = dvdTitle.Title,
                           dateReleased = dvdTitle.DateReleased.ToShortDateString(),
                           datePurchased = dvdCopy.DatePurchased.ToShortDateString(),
                       };
            return View(booksLoanedLast31Days);
        }
    }
}
