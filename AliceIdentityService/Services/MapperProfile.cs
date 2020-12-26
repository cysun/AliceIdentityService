using System;
using System.Collections.Generic;
using System.Linq;
using AliceIdentityService.Models;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace AliceIdentityService.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegistrationInputModel, User>();
            CreateMap<NewUserInputModel, User>();
            CreateMap<User, EditUserInputModel>();
            CreateMap<EditUserInputModel, User>().ForMember(i => i.Id, opt => opt.Ignore());
            CreateMap<User, UserViewModel>();

            CreateMap<ApiScopeInputModel, ApiScope>().ForMember(i => i.Id, opt => opt.Ignore());
            CreateMap<ApiScope, ApiScopeInputModel>();

            CreateMap<IdentityResourceInputModel, IdentityResource>().ForMember(i => i.Id, opt => opt.Ignore());
            CreateMap<IdentityResource, IdentityResourceInputModel>();

            CreateMap<IdentityResourceClaimInputModel, IdentityResourceClaim>();
            CreateMap<IdentityResourceClaim, IdentityResourceClaimInputModel>();

            CreateMap<ClientInputModel, Client>().ForMember(i => i.Id, opt => opt.Ignore());
            CreateMap<Client, ClientInputModel>();

            CreateMap<List<ClientGrantType>, string>()
                .ConvertUsing(source => source.Aggregate("", (result, t) => $"{result} {t.GrantType}"));
            CreateMap<string, List<ClientGrantType>>()
                .ConvertUsing((source, dest) =>
                {
                    if (source == null) source = "";
                    if (dest == null) dest = new List<ClientGrantType>();

                    var grantTypes = source.Split(" ");
                    var currentGrantTypes = dest.Select(t => t.GrantType).ToList();
                    var typesToRemove = currentGrantTypes.Except(grantTypes).ToHashSet();
                    var typesToAdd = grantTypes.Except(currentGrantTypes).ToList();
                    dest.RemoveAll(t => typesToRemove.Contains(t.GrantType));
                    dest.AddRange(typesToAdd.Select(t => new ClientGrantType
                    {
                        GrantType = t
                    }));
                    return dest;
                });

            CreateMap<List<ClientSecret>, string>().ConvertUsing(source => null);
            CreateMap<string, List<ClientSecret>>()
                .ConvertUsing((source, dest) =>
                {
                    if (!string.IsNullOrEmpty(source))
                    {
                        if (dest == null) dest = new List<ClientSecret>();
                        if (dest.Count == 0)
                            dest.Add(new ClientSecret
                            {
                                Value = source.Sha256()
                            });
                        else
                            dest[0].Value = source.Sha256();

                    }
                    return dest;
                });

            CreateMap<List<ClientRedirectUri>, string>()
                .ConvertUsing((source, dest) =>
                {
                    if (source == null || source.Count == 0) dest = "";
                    else
                    {
                        dest = source[0].RedirectUri;
                        for (int i = 1; i < source.Count; ++i)
                            dest += $", {source[i].RedirectUri}";
                    }
                    return dest;
                });
            CreateMap<string, List<ClientRedirectUri>>()
                .ConvertUsing((source, dest) =>
                {
                    if (dest == null) dest = new List<ClientRedirectUri>();
                    if (string.IsNullOrEmpty(source))
                    {
                        dest.Clear();
                        return dest;
                    }

                    var newUris = source.Split(',').Select(uri => uri.Trim()).ToHashSet();
                    var oldUris = dest.Select(uri => uri.RedirectUri).ToHashSet();
                    var uriToRemove = oldUris.Except(newUris);
                    var uriToAdd = newUris.Except(oldUris);
                    dest.RemoveAll(uri => uriToRemove.Contains(uri.RedirectUri));
                    dest.AddRange(uriToAdd.Select(uri => new ClientRedirectUri
                    {
                        RedirectUri = uri
                    }));
                    return dest;
                });

            CreateMap<List<ClientCorsOrigin>, string>()
                .ConvertUsing((source, dest) =>
                {
                    if (source == null || source.Count == 0) dest = "";
                    else
                    {
                        dest = source[0].Origin;
                        for (int i = 1; i < source.Count; ++i)
                            dest += $", {source[i].Origin}";
                    }
                    return dest;
                });
            CreateMap<string, List<ClientCorsOrigin>>()
                .ConvertUsing((source, dest) =>
                {
                    if (dest == null) dest = new List<ClientCorsOrigin>();
                    if (string.IsNullOrEmpty(source))
                    {
                        dest.Clear();
                        return dest;
                    }

                    var newOrigins = source.Split(',').Select(uri => uri.Trim()).ToHashSet();
                    var oldOrigins = dest.Select(uri => uri.Origin).ToHashSet();
                    var originsToRemove = oldOrigins.Except(newOrigins);
                    var originsToAdd = newOrigins.Except(oldOrigins);
                    dest.RemoveAll(origin => originsToRemove.Contains(origin.Origin));
                    dest.AddRange(originsToAdd.Select(origin => new ClientCorsOrigin
                    {
                        Origin = origin
                    }));
                    return dest;
                });
        }
    }
}
