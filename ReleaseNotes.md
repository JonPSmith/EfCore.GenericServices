# Release Notes

## 1.1.0

- Package: Updated to NET Core 2.1
- Minor breaking change: Now uses Display attribute to override name returned in CRUD messages (for localization)


## 1.0.0 EfCore.GenericServices & EfCore.GenericServices.AspNetCore

- First release


## Not yet implemented/tested

- You cannot currently create a nested DTO. This means your DTOs cannot contain DTOs.
- I have implemented but not unit tested the handling of multiple DbContexts