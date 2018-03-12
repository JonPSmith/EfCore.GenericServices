// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;

namespace DataLayer.Dtos
{
    public struct OrderBooksDto
    {
        public int BookId { get; }
        public Book ChosenBook { get; }
        public short numBooks { get; }

        public OrderBooksDto(int bookId, Book chosenBook, short numBooks) : this()
        {
            BookId = bookId;
            ChosenBook = chosenBook ?? throw new ArgumentNullException(nameof(chosenBook));
            this.numBooks = numBooks;
        }
    }
}