// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Configuration;
using GenericServices.Internal;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

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
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task<IStatusGeneric> SaveChangesWithOptionalValidationAsync(this DbContext context,
            bool shouldValidate, IGenericServicesConfig config)
        {
            return shouldValidate
                ? await context.SaveChangesWithValidationAsync(config).ConfigureAwait(false)
                : await context.SaveChangesWithExtrasAsync(config).ConfigureAwait(false);
        }

        /// <summary>
        /// This SaveChanges, with a boolean to decide whether to validate or not
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shouldValidate"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IStatusGeneric SaveChangesWithOptionalValidation(this DbContext context,
            bool shouldValidate, IGenericServicesConfig config)
        {
            return shouldValidate
                ? context.SaveChangesWithValidation(config)
                : context.SaveChangesWithExtras( config);
        }

        /// <summary>
        /// This will validate any entity classes that will be added or updated
        /// If the validation does not produce any errors then SaveChangesAsync will be called 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <returns>List of errors, empty if there were no errors</returns>
        public static async Task<IStatusGeneric> SaveChangesWithValidationAsync(this DbContext context, IGenericServicesConfig config = null)
        {
            var status = context.ExecuteValidation();
            return !status.IsValid
                ? status
                : await context.SaveChangesWithExtrasAsync(config, true);
        }

        //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
        //for why I call DetectChanges before ChangeTracker, and why I then turn ChangeTracker.AutoDetectChangesEnabled off/on around SaveChanges

        /// <summary>
        /// This will validate any entity classes that will be added or updated
        /// If the validation does not produce any errors then SaveChanges will be called 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <returns>List of errors, empty if there were no errors</returns>
        public static IStatusGeneric SaveChangesWithValidation(this DbContext context, IGenericServicesConfig config = null)
        {
            var status = context.ExecuteValidation();
            return !status.IsValid 
                ? status 
                : context.SaveChangesWithExtras(config, true);
        }

        //-----------------------------------------------------------------
        //private methods

        private static IStatusGeneric SaveChangesWithExtras(this DbContext context, 
            IGenericServicesConfig config, bool turnOffChangeTracker = false)
        {
            var status = config?.BeforeSaveChanges != null
                ? config.BeforeSaveChanges(context)
                : new StatusGenericHandler();
            if (!status.IsValid)
                return status;

            if (turnOffChangeTracker)
                context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                //This handles SaveChangesExceptionHandlers that can fix the exception, and the SaveChanges i tried again
                do
                {
                    try
                    {
                        context.SaveChanges();
                        break; //This breaks out of the do/while
                    }
                    catch (Exception e)
                    {
                        var exStatus = config?.SaveChangesExceptionHandler(e, context);
                        if (exStatus == null)
                            throw;       //error wasn't handled, so rethrow
                        status.CombineStatuses(exStatus);
                    }
                } while (status.IsValid);
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return status;
        }

        private static async Task<IStatusGeneric> SaveChangesWithExtrasAsync(this DbContext context, 
            IGenericServicesConfig config, bool turnOffChangeTracker = false)
        {
            var status = config?.BeforeSaveChanges != null
                ? config.BeforeSaveChanges(context)
                : new StatusGenericHandler();
            if (!status.IsValid)
                return status;

            if (turnOffChangeTracker)
                context.ChangeTracker.AutoDetectChangesEnabled = false;
            
            try
            {
                //This handles SaveChangesExceptionHandlers that can fix the exception, and the SaveChanges i tried again
                do
                {
                    try
                    {
                        await context.SaveChangesAsync().ConfigureAwait(false);
                        break; //This breaks out of the do/while
                    }
                    catch (Exception e)
                    {
                        var exStatus = config?.SaveChangesExceptionHandler(e, context);
                        if (exStatus == null) 
                            throw;       //error wasn't handled, so rethrow
                        status.CombineStatuses(exStatus);
                    }
                } while (status.IsValid);
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
            var entriesToCheck = context.ChangeTracker.Entries()
                .Where(e =>
                    (e.State == EntityState.Added) ||
                    (e.State == EntityState.Modified)).ToList(); //This is needed, otherwise you get a "collection has changed" exception
            foreach (var entry in entriesToCheck)
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

            status.Header = null; //reset the header, as could cause incorrect error message
            return status;
        }
    }
}