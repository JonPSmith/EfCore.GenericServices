// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfClasses
{
    public class Book
    {
        public const int PromotionalTextLength = 200;
        private HashSet<BookAuthor> _authorsLink;

        //-----------------------------------------------
        //relationships

        //Use uninitialised backing fields - this means we can detect if the collection was loaded
        private HashSet<Review> _reviews;

        //-----------------------------------------------
        //ctors

        private Book() { }

        public Book(string title, string description, DateTime publishedOn, 
            string publisher, decimal price, string imageUrl, ICollection<Author> authors)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title)); 

            Title = title;
            Description = description;
            PublishedOn = publishedOn;
            Publisher = publisher;
            ActualPrice = price;
            OrgPrice = price;
            ImageUrl = imageUrl;
            _reviews = new HashSet<Review>();       //We add an empty list on create. I allows reviews to be added when building test data

            if (authors == null || !authors.Any())
                throw new ArgumentException("You must have at least one Author for a book", nameof(authors));
            byte order = 0;
            _authorsLink = new HashSet<BookAuthor>(authors.Select(a => new BookAuthor(this, a, order++)));
        }

        public static IStatusGeneric<Book> CreateBook(string title, string description, DateTime publishedOn,
            string publisher, decimal price, string imageUrl, ICollection<Author> authors)
        {
            var status = new StatusGenericHandler<Book>();
            if (string.IsNullOrWhiteSpace(title))
                status.AddError("The book title cannot be empty.");

            var book = new Book
            {
                Title = title,
                Description = description,
                PublishedOn = publishedOn,
                Publisher = publisher,
                ActualPrice = price,
                OrgPrice = price,
                ImageUrl = imageUrl,
                _reviews = new HashSet<Review>()       //We add an empty list on create. I allows reviews to be added when building test data
            };
            if (authors == null)
                throw new ArgumentNullException(nameof(authors));

            byte order = 0;
            book._authorsLink = new HashSet<BookAuthor>(authors.Select(a => new BookAuthor(book, a, order++)));
            if (!book._authorsLink.Any())
                status.AddError("You must have at least one Author for a book.");

            return status.SetResult(book);
        }

        public int BookId { get; private set; }
        [Required(AllowEmptyStrings = false)]
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; private set; }
        public decimal OrgPrice { get; private set; }
        public decimal ActualPrice { get; private set; }

        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; private set; }

        public string ImageUrl { get; private set; }

        public IEnumerable<Review> Reviews => _reviews?.ToList();
        public IEnumerable<BookAuthor> AuthorsLink => _authorsLink?.ToList();

        public void UpdatePublishedOn(DateTime publishedOn)
        {
            PublishedOn = publishedOn;
        }

        public void AddReview(int numStars, string comment, string voterName, 
            DbContext context = null) 
        {
            if (_reviews != null)    
            {
                _reviews.Add(new Review(numStars, comment, voterName));   
            }
            else if (context == null)
            {
                throw new ArgumentNullException(nameof(context), 
                    "You must provide a context if the Reviews collection isn't valid.");
            }
            else if (context.Entry(this).IsKeySet)  
            {
                context.Add(new Review(numStars, comment, voterName, BookId));
            }
            else                                     
            {                                        
                throw new InvalidOperationException("Could not add a new review.");  
            }
        }

        public void RemoveReview(Review review)                          
        {
            if (_reviews == null)
                throw new NullReferenceException("You must use .Include(p => p.Reviews) before calling this method.");

            _reviews.Remove(review); 
        }

        public IStatusGeneric AddPromotion(decimal actualPrice, string promotionalText)                  
        {
            var status = new StatusGenericHandler();
            if (string.IsNullOrWhiteSpace(promotionalText))
            {
                status.AddError("You must provide some text to go with the promotion.", nameof(PromotionalText));
                return status;
            }

            ActualPrice = actualPrice;  
            PromotionalText = promotionalText;

            status.Message = $"The book's new price is ${actualPrice:F}.";

            return status; 
        }

        public void RemovePromotion() 
        {
            ActualPrice = OrgPrice; 
            PromotionalText = null; 
        }
    }

}