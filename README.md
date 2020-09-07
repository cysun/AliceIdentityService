# Alice Identity Service

Alice Identity Service is an OAuth 2 / OpenID Connect service built with
[ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
and [IdentityServer4](http://docs.identityserver.io/en/latest/index.html).

## Create Signing Certificate

    openssl req -new -x509 -nodes -newkey rsa:4096 -sha256 -keyout ais.key -out ais.crt -days 3650
    openssl pkcs12 -export -out ais.pfx -inkey ais.key -in ais.crt

## Generate DB Scripts

    dotnet ef migrations add -c AppDbContext InitialSchema
    dotnet ef migrations add -c AppConfigurationDbContext ConfigurationDbSchema
    dotnet ef migrations add -c PersistedGrantDbContext PersistedGrantDbSchema
    dotnet ef migrations script -c AppDbContext -o Scripts/CreateSchema.sql
    dotnet ef migrations script -c AppConfigurationDbContext -o Scripts/ConfigurationDb.sql
    dotnet ef migrations script -c PersistedGrantDbContext -o Scripts/PersistedGrantDb.sql