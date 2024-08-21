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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ListingsController(IListingsService listingsService, IWebHostEnvironment webHostEnvironment)
        {
            _listingsService = listingsService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Listings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _listingsService.GetAll();
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Listings/Details/5
        //public async task<iactionresult> details(int? id)
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
