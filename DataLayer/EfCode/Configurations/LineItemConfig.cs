// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    public class LineItemConfig : IEntityTypeConfiguration<LineItem>
    {
        public void Configure
            (EntityTypeBuilder<LineItem> entity)
        {
            entity.HasOne(p => p.ChosenBook)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict); //#A
        }

        /**************************************************
        #A I add the OnDelete method onto the end of defining a relationship. I set it to restrict, which will stop the LineItem from being deleted, and hence EF Core will throw an exception if a Book entity class is deleted and a LineItem is linked to that specific book
         * *************************************************/
    }
}