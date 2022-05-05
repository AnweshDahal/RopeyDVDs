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
    public class DVDTitlesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public DVDTitlesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: DVDTitles
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.DVDTitle.Include(d => d.DVDCategory).Include(d => d.Producer).Include(d => d.Studio);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: DVDTitles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitle
                .Include(d => d.DVDCategory)
                .Include(d => d.Producer)
                .Include(d => d.Studio)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (dVDTitle == null)
            {
                return NotFound();
            }

            return View(dVDTitle);
        }

        // GET: DVDTitles/Create
        public IActionResult Create()
        {
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategory, "Id", "Id");
            ViewData["ProducerNumber"] = new SelectList(_context.Producer, "Id", "Id");
            ViewData["StudioNumber"] = new SelectList(_context.Studio, "ID", "ID");
            return View();
        }

        // POST: DVDTitles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProducerNumber,CategoryNumber,StudioNumber,Title,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dVDTitle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategory, "Id", "Id", dVDTitle.CategoryNumber);
            ViewData["ProducerNumber"] = new SelectList(_context.Producer, "Id", "Id", dVDTitle.ProducerNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studio, "ID", "ID", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // GET: DVDTitles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitle.FindAsync(id);
            var dvdCategory = _context.DVDCategory.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CategoryDescription
            });

            var studio = _context.Studio.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.StudioName
            });

            var producer = _context.Producer.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ProducerName
            });

            if (dVDTitle == null)
            {
                return NotFound();
            }
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategory, "Id", "Id", dVDTitle.CategoryNumber);
            ViewData["ProducerNumber"] = new SelectList(_context.Producer, "Id", "Id", dVDTitle.ProducerNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studio, "ID", "ID", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // POST: DVDTitles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProducerNumber,CategoryNumber,StudioNumber,Title,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dVDTitle)
        {
            if (id != dVDTitle.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dVDTitle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DVDTitleExists(dVDTitle.ID))
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
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategory, "Id", "Id", dVDTitle.CategoryNumber);
            ViewData["ProducerNumber"] = new SelectList(_context.Producer, "Id", "Id", dVDTitle.ProducerNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studio, "ID", "ID", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // GET: DVDTitles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitle
                .Include(d => d.DVDCategory)
                .Include(d => d.Producer)
                .Include(d => d.Studio)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (dVDTitle == null)
            {
                return NotFound();
            }

            return View(dVDTitle);
        }

        // POST: DVDTitles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDTitle = await _context.DVDTitle.FindAsync(id);
            _context.DVDTitle.Remove(dVDTitle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDTitleExists(int id)
        {
            return _context.DVDTitle.Any(e => e.ID == id);
        }


        public async Task<IActionResult> SelectActor(Actor actor)
        {
            ViewData["ActorSurName"] = new SelectList(_context.Set<Actor>(), "ActorSurName", "ActorSurName", actor.ActorSurName);
            return View();
        }
        public async Task<IActionResult> ShowDVDsofActors()
        {
            string actorName = Request.Form["actorList"].ToString();
            var data = from castmembers in _context.CastMember
                       join actor in _context.Actor on castmembers.ActorNumber equals actor.Id
                       where actor.ActorSurName == actorName
                       join dvdtitle in _context.DVDTitle
                       on castmembers.DVDNumber equals dvdtitle.ID
                       select new
                       {
                           Title = dvdtitle.Title,
                           Cast = from casts in dvdtitle.CastMembers
                                  join actor in _context.Actor on casts.ActorNumber equals actor.Id
                                  group actor by new { casts.DVDNumber } into g
                                  select
                                       String.Join(", ", g.OrderBy(c => c.ActorSurName).Select(x => (x.ActorFirstName + " " + x.ActorSurName))),
                       };
            return View(data);
        }

        public async Task<IActionResult> ShowDVDCopiesofActors()
        {
            string actorName = Request.Form["actorList"].ToString();

            var loanedCopies = (from loan in _context.Loan
                                where loan.DateReturned == null
                                select loan.CopyNumber).Distinct();


            var data = from castmembers in _context.CastMember
                       join actor in _context.Actor on castmembers.ActorNumber equals actor.Id
                       where actor.ActorSurName == actorName
                       join dvdtitle in _context.DVDTitle
                       on castmembers.DVDNumber equals dvdtitle.ID
                       select new
                       {
                           Title = dvdtitle.Title,
                           Cast = from casts in dvdtitle.CastMembers
                                  join actor in _context.Actor on casts.ActorNumber equals actor.Id
                                  group actor by new { casts.DVDNumber } into g
                                  select
                                       String.Join(", ", g.OrderBy(c => c.ActorSurName).Select(x => (x.ActorFirstName + " " + x.ActorSurName))),
                           NumberOfCopies = (from copy in _context.DVDCopy
                                             where !(loanedCopies).Contains(copy.Id)
                                             where copy.DVDNumber == dvdtitle.ID
                                             select copy).Count()
                       };

            return View(data);
        }
    }
}
