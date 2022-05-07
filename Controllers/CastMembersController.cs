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
    public class CastMembersController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CastMembersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: CastMembers
        public async Task<IActionResult> Index()
        {
            return View(await _context.CastMember.ToListAsync());
        }

        // GET: CastMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var castMember = await _context.CastMember
                .FirstOrDefaultAsync(m => m.ID == id);
            if (castMember == null)
            {
                return NotFound();
            }

            return View(castMember);
        }

        // GET: CastMembers/Create
        public IActionResult Create()
        {
            ViewData["ActorNumber"] = new SelectList(_context.Actor, "Id", "ActorSurName");
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "ID", "Title");
            return View();
        }

        // POST: CastMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DVDNumber,ActorNumber")] CastMember castMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(castMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(castMember);
        }

        // GET: CastMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var castMember = await _context.CastMember.FindAsync(id);
            if (castMember == null)
            {
                return NotFound();
            }
            return View(castMember);
        }

        // POST: CastMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,DVDNumber,ActorNumber")] CastMember castMember)
        {
            if (id != castMember.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(castMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CastMemberExists(castMember.ID))
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
            return View(castMember);
        }

        // GET: CastMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var castMember = await _context.CastMember
                .FirstOrDefaultAsync(m => m.ID == id);
            if (castMember == null)
            {
                return NotFound();
            }

            return View(castMember);
        }

        // POST: CastMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var castMember = await _context.CastMember.FindAsync(id);
            _context.CastMember.Remove(castMember);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CastMemberExists(int id)
        {
            return _context.CastMember.Any(e => e.ID == id);
        }
    }
}
