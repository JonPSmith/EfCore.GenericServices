// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace GenericServices.Configuration
{
    /// <summary>
    /// This provides a per-DTO/ViewModel configuation source. This part is the part that doesn't need Generics
    /// </summary>
    public abstract class PerDtoConfig
    {
        //--------------------------------------------------
        //Control of (CRUD) Create, Read Update and Delete methods

        /// <summary>
        /// This allows you to specify the exact constructor, static method or AutoMapper to create/fill the entity:
        /// - use constructor: "ctor(n)", where n is the number of parameters the ctor has
        /// - static method: use MethodName, e.g. "CreateBook" (can have num params too, e.g. CreateBook(3))
        /// - AutoMapper: CrudValues.UseAutoMapper
        /// </summary>
        public virtual string CreateMethod { get; } = null;

        /// <summary>
        /// This allows you to specify the exact method or AutoMapper that can be used to update the entity
        /// The options are:
        /// - Method: use MethodName, e.g. "AddReview" (can have num params too, e.g. AddReview(3))
        /// - AutoMapper: CrudValues.UseAutoMapper
        /// </summary>
        public virtual string UpdateMethod { get; } = null;

        //------------------------------------------------------
        //Misc

        /// <summary>
        /// The default SaveChanges doesn't validate any entities written to the database (it assumes the front-end has validated it)
        /// By default the normal SaveChanges (non-validation) method  is used, but that can be overridden globally in the GenericServicesConfig class. 
        /// Setting this to true/false will override everything so that you can set validation on/off just the methods in this DTO.
        /// </summary>
        public virtual bool? UseSaveChangesWithValidation { get; } = null;


    }
}