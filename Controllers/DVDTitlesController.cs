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
            return View(await _context.DVDTitle.ToListAsync());
        }

        /**
         * Get DVD Titles, Producer, Studio, Cast of all
         * DVDs with the DVD in increasing order of DateReleased,
         * Show titles of each DVD and the names of all cast member in 
         * increasing order in relation to the last name of the cast member
         */
        public async Task<IActionResult> DVDDetails()
        {
            //var dvd = await _context.DVDTitle
            //    .Join(
            //        _context.Producer,
            //        dvd => dvd.ProducerNumber,
            //        producer => producer.Id,
            //        (dvd, producer) => new
            //        {
            //            ProducerName = producer.ProducerName
            //        }
            //    ).Join(
            //        _context.Studio,
            //        dvd => dvd.StudioNumber,
            //        studio => studio.ID,
            //        (dvd, studio) => new
            //        {
            //            StudioName = studio.StudioName,
            //        }
            //    ).ToListAsync();
            var dvd = await _context.DVDTitle.ToListAsync();
            var producer = await _context.Producer.ToListAsync(); ;
            var studio = await _context.Studio.ToListAsync();
            var castMember = await _context.CastMember
                .Join(
                    _context.Actor, 
                    cast => cast.ActorNumber,
                    actor => actor.Id,
                    (cast, actor) => new
                    {
                        FirstName = actor.ActorFirstName,
                        LastName = actor.ActorSurName,
                    }

                ).ToListAsync();
            return View();
        }

        // GET: DVDTitles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitle
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

            ViewBag.DVDCategory = dvdCategory;
            ViewBag.Studio = studio;
            ViewBag.Producer = producer;
            return View();
        }

        // POST: DVDTitles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProducerNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dVDTitle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            ViewBag.DVDCategory = dvdCategory;
            ViewBag.Studio = studio;
            ViewBag.Producer = producer;
            return View(dVDTitle);
        }

        // POST: DVDTitles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProducerNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dVDTitle)
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
    }
}
