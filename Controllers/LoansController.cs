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
using RopeyDVDs.Models.ViewModels;

namespace RopeyDVDs.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDBContext _context;

        public LoansController(ApplicationDBContext context)
        {
            _context = context;
        }

        /*
         * 
         * This function Issues 
         * 
         */
        public IActionResult IssueLoan(int? id)
        {
            if (id == null) return NotFound();

            var DVDCopyLoanHistory = from loan in _context.Loan
                          where loan.CopyNumber == id
                          where loan.DateReturned == null
                          orderby loan.DateOut descending
                          select new
                          {
                              DateReturned = loan.DateReturned,
                          };

            if (DVDCopyLoanHistory.Count() > 0 && DVDCopyLoanHistory.First().DateReturned == null)
            {
                ViewData["Message"] = new
                {
                    Type = "Error",
                    Message = "This copy is already on loan"
                };

                return RedirectToAction("Index", "DVDCopies");
            }

            var members = _context.Member.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.MemberFirstName
            });

            var dvdCopy = from dvdCopies in _context.DVDCopy
                          join dvdTitle in _context.DVDTitle on dvdCopies.DVDNumber equals dvdTitle.ID
                          where dvdCopies.Id == id
                          select new
                          {
                              ID = dvdCopies.Id,
                              Title = dvdTitle.Title
                          };

            var loanType = _context.LoanType.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Type.ToString()
            });

            ViewData["Member"] = members;
            ViewData["LoanType"] = loanType;
            ViewData["DVDCopy"] = dvdCopy.FirstOrDefault();

            return View("Views/Loans/Issue.cshtml");
        }

        /**
         * This function allows a user to issue a DVD copy on loan
         * Check if the member issuing the loan is above 18 years
         * and the DVD is rated age restricted
         * Check if the member has not passed their loan number
         * 
         */
        // public async Task<IActionResult> Create([Bind("Id,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)
        [HttpPost]
        public IActionResult SaveLoan([Bind("ID", "DVDCopyNumber", "MemberNumber", "LoanTypeNumber")] SaveLoanRequestModel saveLoanRequestModel)
        {
            // Validations

            // Verifying the member's allowed loans
            var member = (from members in _context.Member
                         where members.Id == saveLoanRequestModel.MemberNumber
                         select new
                         {
                             Age = DateTime.Now.Year - members.MemberDOB.Year,
                             Type = members.MembershipCategoryNumber,
                             MaxLoan = members.MembershipCategory.MembershipCategoryTotalLoans
                         }).Take(1);

            var activeLoans = (from loans in _context.Loan
                               where loans.MemberNumber == saveLoanRequestModel.MemberNumber
                               where loans.DateReturned == null
                               select new
                               {
                                   ID = loans.Id
                               }).Count();

            if (Int32.Parse(activeLoans.ToString()) > Int32.Parse(member.First().MaxLoan.ToString())){
                return RedirectToAction("Index");
            }

            // Validating the age and the age restricted status of the DVD
            var copy = (from copies in _context.DVDCopy
                       where copies.Id == saveLoanRequestModel.DVDCopyNumber
                       select new
                       {
                           IsAgeRestricted = copies.DVDTitle.DVDCategory.AgeRestricted
                       }).Take(1);

            var IsAgeRestricted = copy.First().IsAgeRestricted;

            if (
                IsAgeRestricted == true
                && 
                Int32.Parse(member.First().Age.ToString()) < 18)
            {
                return RedirectToAction("Index", "DVDCopies");
            }

            // After all validations are passed creating a new loan issue entry in the database
            var LoanTypeDuration = (from loanTypes in _context.LoanType
                                    where loanTypes.Id == saveLoanRequestModel.LoanTypeNumber
                                    select new
                                    {
                                        Duration = loanTypes.LoanDuration,
                                    }).Take(1);

            DateTime DateDue = DateTime.Now.AddDays(Double.Parse(LoanTypeDuration.First().Duration.ToString()));

            Loan loan = new Loan()
            {
                LoanTypeNumber = Int32.Parse(saveLoanRequestModel.LoanTypeNumber.ToString()),
                CopyNumber = Int32.Parse(saveLoanRequestModel.DVDCopyNumber.ToString()),
                MemberNumber = Int32.Parse(saveLoanRequestModel.MemberNumber.ToString()),
                DateOut = DateTime.Now,
                DateDue = DateDue,
            };

            _context.Loan.Add(loan);
            _context.SaveChanges();
            return RedirectToAction("Index", "DVDCopies");
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            return View(await _context.Loan.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            // Loan Type Number
            var loanType = _context.LoanType.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Type
            });

            ViewBag.LoanType = loanType;
            // Copy Number
            var copyNumber = _context.DVDCopy.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.DVDNumber.ToString()
            });
            ViewBag.DVDCopy = copyNumber;
            // Member Number
            var member = _context.Member.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.MemberFirstName
            });
            ViewBag.Member = member;
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan.FindAsync(id);
            // Loan Type Number
            var loanType = _context.LoanType.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Type
            });

            // Copy Number
            var copyNumber = _context.DVDCopy.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.DVDNumber.ToString()
            });
            
            // Member Number
            var member = _context.Member.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.MemberFirstName
            });
            
            if (loan == null)
            {
                return NotFound();
            }

            ViewBag.LoanType = loanType;
            ViewBag.DVDCopy = copyNumber;
            ViewBag.Member = member;

            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)
        {
            if (id != loan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.Id))
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
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loan.FindAsync(id);
            _context.Loan.Remove(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loan.Any(e => e.Id == id);
        }
    }
}
