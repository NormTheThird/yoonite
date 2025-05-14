using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Yoonite.Common.Models;
using Yoonite.Data;

namespace Yoonite.Service.Services
{
    public interface IMapperService
    {
        T Map<U, T>(U source);
        T Map<U, T>(U source, T destination);
        List<T> MapToList<U, T>(List<U> source);
    }

    public class MapperService : IMapperService
    {
        public T Map<U, T>(U source)
        {
            try
            {
                var mapper = CreateMap<U, T>();
                return mapper.Map<U, T>(source);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T Map<U, T>(U source, T destination)
        {
            try
            {
                var mapper = CreateMap<U, T>();
                return mapper.Map<U, T>(source, destination);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T> MapToList<U, T>(List<U> source)
        {
            try
            {
                var mapper = CreateMap<U, T>();
                return mapper.Map<List<U>, List<T>>(source);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static IMapper CreateMap<U, T>()
        {
            // Account Mappings
            if (typeof(U) == typeof(Account) && typeof(T) == typeof(AccountModel))
                return MapAccountToAccountModel().CreateMapper();
            else if (typeof(U) == typeof(AccountModel) && typeof(T) == typeof(Account))
                return MapAccountModelToAccount().CreateMapper();

            // Project Mappings
            if (typeof(U) == typeof(Project) && typeof(T) == typeof(ProjectModel))
                return MapProjectToProjectModel().CreateMapper();

            // Skill Mappings
            else if (typeof(U) == typeof(Skill) && typeof(T) == typeof(SkillModel))
                return MapSkillToSkillModel().CreateMapper();
            else if (typeof(U) == typeof(SkillModel) && typeof(T) == typeof(Skill))
                return MapSkillModelToSkill().CreateMapper();

            else
                throw new ApplicationException("No mapping configuration exists [" + typeof(T).ToString() + "]");
        }

        // Account mapping
        private static MapperConfiguration MapAccountToAccountModel()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<Storage, StorageModel>();
                config.CreateMap<Address, AddressModel>();
                config.CreateMap<Account, AccountModel>()
                      .ForMember(dest => dest.ProfileImageStorage, opt => opt.MapFrom(src => src.Storage ?? null));

            });
        }
        private static MapperConfiguration MapAccountModelToAccount()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<AddressModel, Address>()
                      .ForMember(dest => dest.Id, opt => opt.Ignore())
                      .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.Address1 ?? ""))
                      .ForMember(dest => dest.Address2, opt => opt.MapFrom(src => src.Address2 ?? ""))
                      .ForMember(dest => dest.DateCreated, opt => opt.Ignore());
                config.CreateMap<AccountModel, Account>()
                      .ForMember(dest => dest.Id, opt => opt.Ignore())
                      .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                      .ForMember(dest => dest.Email, opt => opt.Ignore())
                      .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? ""))
                      .ForMember(dest => dest.AltPhoneNumber, opt => opt.MapFrom(src => src.AltPhoneNumber ?? ""))
                      .ForMember(dest => dest.FacebookUrl, opt => opt.MapFrom(src => src.FacebookUrl ?? ""))
                      .ForMember(dest => dest.TwitterUrl, opt => opt.MapFrom(src => src.TwitterUrl ?? ""))
                      .ForMember(dest => dest.LinkedInUrl, opt => opt.MapFrom(src => src.LinkedinUrl ?? ""))
                      .ForMember(dest => dest.InstagramUrl, opt => opt.MapFrom(src => src.InstagramUrl ?? ""))
                      .ForMember(dest => dest.WebsiteUrl, opt => opt.MapFrom(src => src.WebsiteUrl ?? ""))
                      .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio ?? ""))
                      .ForMember(dest => dest.RefreshTokenMinutes, opt => opt.Ignore())
                      .ForMember(dest => dest.Password, opt => opt.Ignore())
                      .ForMember(dest => dest.IsSystemAdmin, opt => opt.Ignore())
                      .ForMember(dest => dest.DateCreated, opt => opt.Ignore());
            });
        }

        // Project mapping
        private static MapperConfiguration MapProjectToProjectModel()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<Skill, SkillModel>();
                config.CreateMap<Project, ProjectModel>();
            });
        }

        // Skills mapping
        private static MapperConfiguration MapSkillToSkillModel()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<Skill, SkillModel>();
            });
        }
        private static MapperConfiguration MapSkillModelToSkill()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<SkillModel, Skill>()
                      .ForMember(dest => dest.Id, opt => opt.Ignore())
                      .ForMember(dest => dest.DateCreated, opt => opt.Ignore());
            });
        }
    }
}