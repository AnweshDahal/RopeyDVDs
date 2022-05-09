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

        [Authorize(Roles = "Manager")]
        public IActionResult DVDDescription()
        {
            var data = from dvdtitles in _context.DVDTitle
                       orderby dvdtitles.DateReleased
                       select new
                       {
                           Title = dvdtitles.Title,
                           Producer = dvdtitles.Producer.ProducerName.ToString(),
                           Studio = dvdtitles.Studio.StudioName.ToString(),
                           Cast = from castmember in dvdtitles.CastMembers
                                  join actor in _context.Actor on castmember.ActorNumber equals actor.Id
                                  orderby actor.ActorSurName descending
                                  select new
                                  {
                                      Name = String.Join(" ", new string[] { actor.ActorSurName.ToString(), actor.ActorFirstName.ToString() }),
                                  }
                       };

            ViewData["DVDDescription"] = data;

            return View("Views/DVDTitles/DVDDetails.cshtml");
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
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategory, "Id", "CategoryDescription");
            ViewData["ProducerNumber"] = new SelectList(_context.Producer, "Id", "ProducerName");
            ViewData["StudioNumber"] = new SelectList(_context.Studio, "ID", "StudioName");
            return View();
        }

        public IActionResult ProducerCreate()
        {
            return View();
        }

        // POST: Producers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProducerCreate([Bind("Id,ProducerName")] Producer producer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(producer);
        }

        // GET: CastMembers/Create
        public IActionResult CastMemberCreate(int? id)
        {
            if (id == null) return NotFound();

            ViewData["DVDTitle"] = _context.DVDTitle.Find(id);
            ViewData["ActorNumber"] = new SelectList(_context.Actor, "Id", "ActorSurName");
            return View();
        }

        // POST: CastMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CastMemberCreate([Bind("ID,DVDNumber,ActorNumber")] CastMember castMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(castMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CastMemberCreate));
            }
            return View(castMember);
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
                return RedirectToAction(nameof(CastMemberCreate), new { id=dVDTitle.ID});
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

        [Authorize(Roles = "Assistant, Manager")]
        public async Task<IActionResult> SelectActor(Actor actor)
        {
            ViewData["ActorSurName"] = new SelectList(_context.Set<Actor>(), "ActorSurName", "ActorSurName", actor.ActorSurName);
            return View();
        }

        [Authorize(Roles = "Assistant, Manager")]
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

        [Authorize(Roles = "Assistant, Manager")]
        // GET: DVDCategories/Create
        public IActionResult CategoryCreate()
        {
            return View();
        }

        // POST: DVDCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryCreate([Bind("Id,CategoryDescription,AgeRestricted")] DVDCategory dVDCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(dVDCategory);
        }

        public IActionResult StudioCreate()
        {
            return View();
        }

        // POST: Studios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudioCreate([Bind("ID,StudioName")] Studio studio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(studio);
        }


        // GET: Actors/Create
        public IActionResult ActorCreate()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActorCreate([Bind("Id,ActorSurName,ActorFirstName")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ActorCreate));
            }
            return View(actor);
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
