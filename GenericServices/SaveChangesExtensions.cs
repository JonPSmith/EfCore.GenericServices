// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Internal;
using Microsoft.EntityFrameworkCore;

namespace GenericServices
{
    /// <summary>
    /// This static class contains the extension methods for saving data with validation
    /// </summary>
    public static class SaveChangesExtensions
    {
        //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
        //for why I call DetectChanges before ChangeTracker, and why I then turn ChangeTracker.AutoDetectChangesEnabled off/on around SaveChanges

        /// <summary>
        /// This SaveChangesAsync, with a boolean to decide whether to validate or not
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shouldValidate"></param>
        /// <returns></returns>
        public static async Task<IStatusGeneric> SaveChangesWithOptionalValidationAsync(this DbContext context,
            bool shouldValidate)
        {
            if (shouldValidate)
                return await context.SaveChangesWithValidationAsync().ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return new StatusGenericHandler();
        }

        /// <summary>
        /// This SaveChanges, with a boolean to decide whether to validate or not
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shouldValidate"></param>
        /// <returns></returns>
        public static IStatusGeneric SaveChangesWithOptionalValidation(this DbContext context,
            bool shouldValidate)
        {
            if (shouldValidate)
                return context.SaveChangesWithValidation();
            context.SaveChanges();
            return new StatusGenericHandler();
        }

        /// <summary>
        /// This will validate any entity classes that will be added or updated
        /// If the validation does not produce any errors then SaveChangesAsync will be called 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>List of errors, empty if there were no errors</returns>
        public static async Task<IStatusGeneric> SaveChangesWithValidationAsync(this DbContext context)
        {
            var status = context.ExecuteValidation();
            if (!status.IsValid) return status;

            context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return status;
        }

        //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
        //for why I call DetectChanges before ChangeTracker, and why I then turn ChangeTracker.AutoDetectChangesEnabled off/on around SaveChanges

        /// <summary>
        /// This will validate any entity classes that will be added or updated
        /// If the validation does not produce any errors then SaveChanges will be called 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>List of errors, empty if there were no errors</returns>
        public static IStatusGeneric SaveChangesWithValidation(this DbContext context)
        {
            var status = context.ExecuteValidation();
            if (!status.IsValid) return status;

            context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                context.SaveChanges();
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return status;
        }

        private static IStatusGeneric ExecuteValidation(this DbContext context)
        {
            var status = new StatusGenericHandler();
            foreach (var entry in
                context.ChangeTracker.Entries()
                    .Where(e =>
                        (e.State == EntityState.Added) ||
                        (e.State == EntityState.Modified)))
            {
                var entity = entry.Entity;
                status.Header = entity.GetType().GetNameForClass();
                var valProvider = new ValidationDbContextServiceProvider(context);
                var valContext = new ValidationContext(entity, valProvider, null);
                var entityErrors = new List<ValidationResult>();
                if (!Validator.TryValidateObject(
                    entity, valContext, entityErrors, true))
                {
                    status.AddValidationResults(entityErrors);
                }
            }

            return status;
        }
    }
}