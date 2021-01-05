// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace DataLayer.EfClasses
{
    public class BookAuthor
    {
        private BookAuthor() { }

        internal BookAuthor(Book book, Author author, byte order)
        {
            Book = book;
            Author = author;
            Order = order;
        }

        public int BookId { get; private set; }
        public int AuthorId { get; private set; }
        public byte Order { get; private set; }

        //-----------------------------
        //Relationships

        public Book Book { get; private set; }
        public Author Author { get; private set; }
    }
}