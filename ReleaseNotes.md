# Release Notes

## 1.1.1

- New Feature: Added SqlErrorHandler to configuration and called in SaveChangesWithValidation/Async.
Allows you to intercept `DbUpdateException` and turn SQL errors into user-friendly error messages.

## 1.1.0

- Package: Updated to NET Core 2.1
- Minor breaking change: Now uses Display attribute to override name returned in CRUD messages (for localization)

## 1.0.0 EfCore.GenericServices & EfCore.GenericServices.AspNetCore

- First release

## Not yet implemented/tested

- I have implemented but not unit tested the handling of multiple DbContexts