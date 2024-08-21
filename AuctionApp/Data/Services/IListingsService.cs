﻿using AuctionApp.Models;

namespace AuctionApp.Data.Services
{
    public interface IListingsService
    {
        IQueryable<Listing> GetAll();
        Task Add(Listing listing);
    }
}
