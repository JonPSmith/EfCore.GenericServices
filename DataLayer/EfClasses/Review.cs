// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace DataLayer.EfClasses
{
    public class Review
    {
        public const int NameLength = 100;

        private Review() { }

        internal Review(int numStars, string comment, string voterName, int bookId = 0)
        {
            NumStars = numStars;
            Comment = comment;
            VoterName = voterName;
            BookId = bookId;
        }

        public int ReviewId { get; private set; }

        [MaxLength(NameLength)]
        public string VoterName { get; private set; }

        public int NumStars { get; private set; }
        public string Comment { get; private set; }

        //-----------------------------------------
        //Relationships

        public int BookId { get; private set; }
    }

}