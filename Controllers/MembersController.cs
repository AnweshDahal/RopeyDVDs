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
    public class MembersController : Controller
    {
        private readonly ApplicationDBContext _context;

        public MembersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            return View(await _context.Member.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == id);
            var differenceDate = DateTime.Now.AddDays(-31);
            var data = from members in _context.Member
                       join
            loan in _context.Loan on members.Id equals loan.MemberNumber
                       where loan.DateOut >= differenceDate
                       where members.Id == id
                       join dvdcopy in _context.DVDCopy on loan.CopyNumber equals dvdcopy.Id
                       join dvdtitle in _context.DVDTitle on dvdcopy.DVDNumber equals dvdtitle.ID
                       select new
                       {
                           Member = members.MemberFirstName + " " + members.MemberLastName,
                           Loan = loan.Id,
                           CopyNumber = dvdcopy.Id,
                           Title = dvdtitle.Title,
                       };
            if (member == null)
            {
                return NotFound();
            }

            return View(data);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            var membershipCategory = _context.MembershipCategory.Select( a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.MembersgipCategoryDescription
            });
            ViewBag.MembershipCategory = membershipCategory;
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberLastName,MemberFirstName,MemberAddress,MemberDOB,MembershipCategoryNumber")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            var membershipCategory = _context.MembershipCategory.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.MembersgipCategoryDescription
            });
            if (member == null)
            {
                return NotFound();
            }
            ViewBag.MembershipCategory = membershipCategory;
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberLastName,MemberFirstName,MemberAddress,MemberDOB,MembershipCategoryNumber")] Member member)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
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
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Member.FindAsync(id);
            _context.Member.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.Id == id);
        }
    }
}
