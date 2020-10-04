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
            CreateMap<EditUserInputModel, User>();
            CreateMap<User, UserViewModel>();
            CreateMap<ApiScopeInputModel, ApiScope>();
            CreateMap<ApiScope, ApiScopeInputModel>();
            CreateMap<IdentityResourceInputModel, IdentityResource>();
            CreateMap<IdentityResource, IdentityResourceInputModel>();
            CreateMap<IdentityResourceClaimInputModel, IdentityResourceClaim>();
            CreateMap<IdentityResourceClaim, IdentityResourceClaimInputModel>();

            CreateMap<ClientInputModel, Client>().ForMember(c => c.Id, opt => opt.Ignore());
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
                .ConvertUsing(source => source == null || source.Count == 0 ? "" : source[0].RedirectUri);
            CreateMap<string, List<ClientRedirectUri>>()
                .ConvertUsing((source, dest) =>
                {
                    if (dest == null) dest = new List<ClientRedirectUri>();

                    if (!string.IsNullOrEmpty(source))
                    {
                        if (dest.Count == 0)
                            dest.Add(new ClientRedirectUri
                            {
                                RedirectUri = source
                            });
                        else
                            dest[0].RedirectUri = source;

                    }
                    return dest;
                });

            CreateMap<List<ClientCorsOrigin>, string>()
                .ConvertUsing(source => source == null || source.Count == 0 ? "" : source[0].Origin);
            CreateMap<string, List<ClientCorsOrigin>>()
                .ConvertUsing((source, dest) =>
                {
                    if (dest == null) dest = new List<ClientCorsOrigin>();

                    if (!string.IsNullOrEmpty(source))
                    {
                        if (dest.Count == 0)
                            dest.Add(new ClientCorsOrigin
                            {
                                Origin = source
                            });
                        else
                            dest[0].Origin = source;

                    }
                    return dest;
                });
        }
    }
}
