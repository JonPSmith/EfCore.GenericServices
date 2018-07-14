# Release Notes

## 1.2.1

- New Feature: Added SqlErrorHandler to configuration and called in SaveChangesWithValidation/Async.
Allows you to intercept an exception in SaveChanges and do things like capture 
SQL errors and turn them into user-friendly error messages.
- New Feature: `bool DirectAccessValidateOnSave` to GenericServicesConfig to globally 
configure all direct CreateAndSave/UpdateAndSave/DeleteAndSave to use validation.
- New Feature: `bool DtoAccessValidateOnSave` to GenericServicesConfig to globally 
configure all via DTO  CreateAndSave/UpdateAndSave to use validation.
- New Feature: New GenericServicesSimpleSetup DI setup method that takes 
in a IGenericServicesConfig parameter

## 1.1.0

- Package: Updated to NET Core 2.1
- Minor breaking change: Now uses Display attribute to override name returned in CRUD messages (for localization)

## 1.0.0 EfCore.GenericServices & EfCore.GenericServices.AspNetCore

- First release

## Not yet implemented/tested

- I have implemented but not unit tested the handling of multiple DbContexts