# Alice Identity Service

Alice Identity Service is an OAuth 2 / OpenID Connect service built with
[ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
and [IdentityServer4](http://docs.identityserver.io/en/latest/index.html).

## Create Signing Certificate

    openssl req -x509 -newkey rsa:4096 -sha256 -keyout AliceIdService.key -out AliceIdService.crt -days 3650
    openssl pkcs12 -export -out AliceIdService.pfx -inkey AliceIdService.key -in AliceIdService.crt -certfile AliceIdService.crt

## Generate DB Scripts

    dotnet ef migrations add -c ConfigurationDbContext ConfigurationDbSchema
    dotnet ef migrations add -c PersistedGrantDbContext PersistedGrantDbSchema
    dotnet ef migrations script -c ConfigurationDbContext -o Scripts/ConfigurationDb.sql
    dotnet ef migrations script -c PersistedGrantDbContext -o Scripts/PersistedGrantDb.sql