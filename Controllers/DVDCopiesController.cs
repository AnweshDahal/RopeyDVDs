#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.DBContext;
using RopeyDVDs.Models;

namespace RopeyDVDs.Controllers
{
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
            return View(await _context.DVDCopy.ToListAsync());
        }

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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dVDCopy == null)
            {
                return NotFound();
            }

            var recentLoan = from loan in _context.Loan
                             join member in _context.Member on loan.MemberNumber equals member.Id
                             join membership in _context.MembershipCategory on member.MembershipCategoryNumber equals membership.Id
                             join loanType in _context.Loan on loan.LoanTypeNumber equals loanType.Id
                             join copy in _context.DVDCopy on loan.CopyNumber equals copy.Id
                             join dvdTitle in _context.DVDTitle on copy.DVDNumber equals dvdTitle.ID
                             where loan.CopyNumber == id
                             orderby loan.DateOut descending

                             select new
                             {
                                 MemberName = member.MemberFirstName + " " + member.MemberLastName,
                                 DateDue = loan.DateDue,
                                 LoanType = loanType.LoanType,
                                 Membership = membership.MembersgipCategoryDescription,
                                 DVDTitle = dvdTitle.Title,
                                 StandardCharge = dvdTitle.StandardCharge,
                                 PenaltyCharge = dvdTitle.PenaltyCharge,
                                 DateReturned = loan.DateReturned,
                                 Loan = loan
                             };


            //var recentLoan = (from loan in _context.Loan
            //                  join member in _context.Member on loan.MemberNumber equals member.Id
            //                  join membership in _context.MembershipCategory on member.MembershipCategoryNumber equals membership.Id
            //                  join loanType in _context.Loan on loan.LoanTypeNumber equals loanType.Id
            //                  join copy in _context.DVDCopy on loan.CopyNumber equals copy.Id
            //                  join dvdTitle in _context.DVDTitle on copy.DVDNumber equals dvdTitle.ID
            //                  where loan.Id == id
            //                  select new
            //                  {
            //                      LoanNumber = id,
            //                      CopyNumber = loan.CopyNumber,
            //                      DateOut = loan.DateOut,
            //                      DateDue = loan.DateDue,
            //                      DateReturned = loan.DateReturned,
            //                      MemberName = member.MemberFirstName + " " + member.MemberLastName,
            //                      Membership = membership.MembersgipCategoryDescription,
            //                      LoanType = loanType.LoanType,
            //                      DVDTitle = dvdTitle.Title,
            //                      StandardCharge = dvdTitle.StandardCharge,
            //                      PenaltyCharge = dvdTitle.PenaltyCharge,
            //                  });

            return View(recentLoan);
        }

        // GET: DVDCopies/Create
        public IActionResult Create()
        {
            var dvdNumber = _context.DVDTitle.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ID.ToString()
            });

            ViewBag.DVDNumber = dvdNumber;
            return View();
        }

        // POST: DVDCopies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DVDNumber,DatePurchased")] DVDCopy dVDCopy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDCopy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            // The following retrieval requires await however the IDE displays the
            // GetAwaiter not defined error
            var dvdTitle = _context.DVDTitle.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.ID.ToString()
            });

            if (dVDCopy == null)
            {
                return NotFound();
            }

            ViewBag.DVDTitle = dvdTitle;
            return View(dVDCopy);
        }

        // POST: DVDCopies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DVDNumber,DatePurchased")] DVDCopy dVDCopy)
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
    }
}
