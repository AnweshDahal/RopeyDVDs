#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.DBContext;
using RopeyDVDs.Models;
using RopeyDVDs.Models.ViewModels;
using RopeyDVDs.Models.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RopeyDVDs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(Roles = "Manager")]
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
                ViewData["Error"] = "This copy is already on loan";
                return View("Views/Shared/ValidationError.cshtml");
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
                ViewData["Error"] = "You have reached your maximum number of loans!";
                return View("Views/Shared/ValidationError.cshtml");
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
                ViewData["Error"] = "The selected DVD Category is Age Restricted!";
                return View("Views/Shared/ValidationError.cshtml");
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

            var dvdTitle = _context.DVDTitle.Find(_context.DVDCopy.Find(saveLoanRequestModel.DVDCopyNumber).DVDNumber);
            var dvdCategory = _context.DVDCategory.Find(dvdTitle.CategoryNumber);
            var reqMember = _context.Member.Find(saveLoanRequestModel.MemberNumber);
            ReciptViewModel recipt = new ReciptViewModel()
            {
                ReciptNumber = String.Format("{0}-{1}-{2}-{3}", loan.Id, loan.LoanTypeNumber, loan.CopyNumber, loan.MemberNumber),
                DVDNumber = dvdTitle.ID.ToString(),
                DVDTitle = dvdTitle.Title,
                DVDCategory = dvdCategory.CategoryDescription,
                DateOut = loan.DateOut.ToShortDateString().ToString(),
                DateDue = loan.DateDue.ToShortDateString().ToString(),
                LoanDuration = _context.LoanType.Find(saveLoanRequestModel.LoanTypeNumber).LoanDuration.ToString(),
                StandardCharge = dvdTitle.StandardCharge.ToString(),
                TotalCharge = (dvdTitle.StandardCharge * _context.LoanType.Find(saveLoanRequestModel.LoanTypeNumber).LoanDuration).ToString(),
                MemberName = String.Format("{0} {1}", reqMember.MemberFirstName, reqMember.MemberLastName),
                MemberAddress = reqMember.MemberAddress,
            };
            return View("Views/Loans/Recipt.cshtml", recipt);
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
                Text = m.Id.ToString()
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

        public async Task<IActionResult> LoanCheckout(Loan loanModel)
        {
            var loanDetails = (from loan in _context.Loan
                               join member in _context.Member on loan.MemberNumber equals member.Id
                               join membership in _context.MembershipCategory on member.MembershipCategoryNumber equals membership.Id
                               join loanType in _context.Loan on loan.LoanTypeNumber equals loanType.Id
                               join copy in _context.DVDCopy on loan.CopyNumber equals copy.Id
                               join dvdTitle in _context.DVDTitle on copy.DVDNumber equals dvdTitle.ID
                               where loan.Id == loanModel.Id
                               select new
                               {   
                                   LoanNumber = loanModel.Id,
                                   CopyNumber = loan.CopyNumber,
                                   DateOut = loan.DateOut,
                                   DateDue = loan.DateDue,
                                   DateReturned = loanModel.DateReturned,
                                   MemberName = member.MemberFirstName + " " + member.MemberLastName,
                                   Membership = membership.MembersgipCategoryDescription,
                                   LoanType = loanType.LoanType,
                                   DVDTitle = dvdTitle.Title,
                                   StandardCharge = dvdTitle.StandardCharge,
                                   PenaltyCharge = dvdTitle.PenaltyCharge,
                               }).FirstOrDefault();
            //Console.Write("======================");
            //System.Diagnostics.Debug.WriteLine("This is the loan detail loan type" + loanModel.LoanType);
            //Console.Write("======================");
            return View(loanDetails);    
                
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
                //return RedirectToAction("LoanCheckout", new { loanModel = loan });
                //return RedirectToAction("LoanCheckout");
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
