# Alice Identity Service

Alice Identity Service is an OAuth 2 / OpenID Connect service built with
[ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
and [IdentityServer4](http://docs.identityserver.io/en/latest/index.html).

## Database Initialization

    dotnet ef migrations add -c ConfigurationDbContext ConfigurationDbSchema
    dotnet ef migrations add -c PersistedGrantDbContext PersistedGrantDbSchema
    dotnet ef migrations script -c ConfigurationDbContext -o Scripts/ConfigurationDb.sql
    dotnet ef migrations script -c PersistedGrantDbContext -o Scripts/PersistedGrantDb.sql