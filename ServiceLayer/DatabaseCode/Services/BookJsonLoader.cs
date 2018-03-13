// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.EfClasses;
using Newtonsoft.Json;
using ServiceLayer.DatabaseCode.Dtos;

namespace ServiceLayer.DatabaseCode.Services
{
    public static class BookJsonLoader
    {
        private const decimal DefaultBookPrice = 40;    //Any book without a price is set to this value

        public static IEnumerable<Book> LoadBooks(string fileDir, string fileSearchString)
        {
            var filePath = GetJsonFilePath(fileDir, fileSearchString);
            var jsonDecoded = JsonConvert.DeserializeObject<ICollection<BookInfoJson>>(File.ReadAllText(filePath));

            var authorDict = new Dictionary<string,Author>();
            foreach (var bookInfoJson in jsonDecoded)
            {
                foreach (var author in bookInfoJson.authors)
                {
                    if (!authorDict.ContainsKey(author))
                        authorDict[author] = new Author(author);
                }
            }

            return jsonDecoded.Select(x => CreateBookWithRefs(x, authorDict));
        }


        //--------------------------------------------------------------
        //private methods
        private static Book CreateBookWithRefs(BookInfoJson bookInfoJson, Dictionary<string, Author> authorDict)
        {
            var authors = bookInfoJson.authors.Select(x => authorDict[x]).ToList();
            var book = new Book(bookInfoJson.title, 
                bookInfoJson.description, 
                DecodePubishDate(bookInfoJson.publishedDate),
                bookInfoJson.publisher, 
                ((decimal?)bookInfoJson.saleInfoListPriceAmount) ?? DefaultBookPrice, 
                bookInfoJson.imageLinksThumbnail,
                authors);

            if (bookInfoJson.averageRating != null)
                CalculateReviewsToMatch((double)bookInfoJson.averageRating, (int)bookInfoJson.ratingsCount).ToList()
                    .ForEach(x => book.AddReview(x, null, "anonymous"));

            return book;
        }

        /// <summary>
        /// This create the right number of NumStars that add up to the average rating
        /// </summary>
        /// <param name="averageRating"></param>
        /// <param name="ratingsCount"></param>
        /// <returns></returns>
        private static List<int> CalculateReviewsToMatch(double averageRating, int ratingsCount)
        {
            var numStars = new List<int>();
            var currentAve = averageRating;
            for (int i = 0; i < ratingsCount; i++)
            {
                numStars.Add( (int)( currentAve > averageRating ? Math.Truncate(averageRating) : Math.Ceiling(averageRating)));
                currentAve = numStars.Average();
            }
            return numStars;
        }

        private static DateTime DecodePubishDate(string publishedDate)
        {
            var split = publishedDate.Split('-');
            switch (split.Length)
            {
                case 1:
                    return new DateTime(int.Parse(split[0]), 1, 1);
                case 2:
                    return new DateTime(int.Parse(split[0]), int.Parse(split[1]), 1);
                case 3:
                    return new DateTime(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
            }

            throw new InvalidOperationException($"The json publishedDate failed to decode: string was {publishedDate}");
        }

        private static string GetJsonFilePath(string fileDir, string searchPattern)
        {
            var fileList = Directory.GetFiles(fileDir, searchPattern);

            if (fileList.Length == 0)
                throw new FileNotFoundException($"Could not find a file with the search name of {searchPattern} in directory {fileDir}");

            //If there are many then we take the most recent
            return fileList.ToList().OrderBy(x => x).Last();
        }
    }
}