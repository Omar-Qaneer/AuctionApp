using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionApp.Data;
using AuctionApp.Models;
using AuctionApp.Data.Services;

namespace AuctionApp.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IListingsService _listingsService;
        private readonly IBidsService _bidsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ListingsController(IListingsService listingsService, IWebHostEnvironment webHostEnvironment, IBidsService bidsService)
        {
            _listingsService = listingsService;
            _webHostEnvironment = webHostEnvironment;
            _bidsService = bidsService;
        }

        // GET: Listings
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            var applicationDbContext = _listingsService.GetAll();

            int pageSize = 3;
            if (!string.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(a => a.Title.Contains(searchString));
                return View(await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l => l.IsSold == false).AsNoTracking(), pageNumber ?? 1, pageSize));
            }

            return View(await PaginatedList<Listing>.CreateAsync(applicationDbContext/*.Where(l => l.IsSold == false)*/.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? itemid)
        {
            if (itemid == null)
            {
                return NotFound();
            }

            var listing = await _listingsService.GetById(itemid);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // get: listings/create
        public IActionResult create()
        {
            return View();
        }

        // post: listings/create
        // to protect from overposting attacks, enable the specific properties you want to bind to.
        // for more details, see http://go.microsoft.com/fwlink/?linkid=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingVM listing)
        {
            if (listing.Image != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                string fileName = Path.GetFileName(listing.Image.FileName);
                string filePath = Path.Combine(uploadDir, fileName);
                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    listing.Image.CopyTo(fileStream);
                }

                var listObj = new Listing
                {
                    Title = listing.Title,
                    Description = listing.Description,
                    Price = listing.Price,
                    IdentityUserId = listing.IdentityUserId,
                    ImagePath = Path.Combine("Images", fileName),
                };
                await _listingsService.Add(listObj);
                return RedirectToAction("Index");
            }
            return View(listing);
        }
        [HttpPost]
        public async Task<ActionResult> AddBid([Bind("Id, Price, ListingId, IdentityUserId")] Bid bid)
        {
            if (ModelState.IsValid)
            {
                await _bidsService.Add(bid);
            }
            var listing = await _listingsService.GetById(bid.ListingId);
            listing.Price = bid.Price;
            await _listingsService.SaveChanges();

            return View("Details", listing);
        }

        public async Task<ActionResult> CloseBidding(int id)
        {
            var listing = await _listingsService.GetById(id);
            listing.IsSold = true;
            await _listingsService.SaveChanges();
            return View("Details", listing);
        }

        //// get: listings/edit/5
        //public async task<iactionresult> edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return notfound();
        //    }

        //    var listing = await _context.listings.findasync(id);
        //    if (listing == null)
        //    {
        //        return notfound();
        //    }
        //    viewdata["identityuserid"] = new selectlist(_context.users, "id", "id", listing.identityuserid);
        //    return view(listing);
        //}

        //// post: listings/edit/5
        //// to protect from overposting attacks, enable the specific properties you want to bind to.
        //// for more details, see http://go.microsoft.com/fwlink/?linkid=317598.
        //[httppost]
        //[validateantiforgerytoken]
        //public async task<iactionresult> edit(int id, [bind("id,title,description,price,imagepath,issold,identityuserid")] listing listing)
        //{
        //    if (id != listing.id)
        //    {
        //        return notfound();
        //    }

        //    if (modelstate.isvalid)
        //    {
        //        try
        //        {
        //            _context.update(listing);
        //            await _context.savechangesasync();
        //        }
        //        catch (dbupdateconcurrencyexception)
        //        {
        //            if (!listingexists(listing.id))
        //            {
        //                return notfound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return redirecttoaction(nameof(index));
        //    }
        //    viewdata["identityuserid"] = new selectlist(_context.users, "id", "id", listing.identityuserid);
        //    return view(listing);
        //}

        //// get: listings/delete/5
        //public async task<iactionresult> delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return notfound();
        //    }

        //    var listing = await _context.listings
        //        .include(l => l.user)
        //        .firstordefaultasync(m => m.id == id);
        //    if (listing == null)
        //    {
        //        return notfound();
        //    }

        //    return view(listing);
        //}

        //// post: listings/delete/5
        //[httppost, actionname("delete")]
        //[validateantiforgerytoken]
        //public async task<iactionresult> deleteconfirmed(int id)
        //{
        //    var listing = await _context.listings.findasync(id);
        //    if (listing != null)
        //    {
        //        _context.listings.remove(listing);
        //    }

        //    await _context.savechangesasync();
        //    return redirecttoaction(nameof(index));
        //}

        //private bool listingexists(int id)
        //{
        //    return _context.listings.any(e => e.id == id);
        //}
    }
}
