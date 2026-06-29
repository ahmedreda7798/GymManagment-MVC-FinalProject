using AutoMapper;
using GymManagment.BLL.ViewModels.MemberShipViewModels;
using GymManagment.BLL.ViewModels.MemberViewModels;
using GymManagment.BLL.ViewModels.PlanViewModels;
using GymManagment.BLL.ViewModels.SessionViewModels;
using GymManagment.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<Source, Destination>()
        //.ForMember(dest => dest.DestinationProperty, opt => opt.MapFrom(src => src.SourceProperty))
        //.AfterMap((src ,dest) => 
        //{
           //dest.Property = src.Property;
           //dest.Property = src.Property;
           //.....
        //});
        MapMember();
        MapSession();
        MapPlan();
        MapMemberShip();
    }
    private void MapMember()
    {
        CreateMap<Member, MemberViewModel>()
           .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} -{src.Address.City}"))
           .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
           .AfterMap((src, dest) =>
           {
               if (src.Memberships != null)
               {
                   var activeMembership = src.Memberships
                       .FirstOrDefault(ms => ms.EndDate > DateTime.Now);

                   if (activeMembership != null)
                   {
                       dest.PlanName = activeMembership.Plan?.Name;
                       dest.MemberShipStartDate = activeMembership.CreatedAt.ToShortDateString();
                       dest.MemberShipEndDate = activeMembership.EndDate.ToShortDateString();
                   }
               }
           });

        CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

        CreateMap<Member, MemberToUpdateViewModel>()
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City));

        CreateMap<MemberToUpdateViewModel, Member>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.Photo, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                dest.Address.BuildingNumber = src.BuildingNumber;
                dest.Address.Street = src.Street;
                dest.Address.City = src.City;
                //dest.UpdatedAt = DateTime.Now;
            });

        CreateMap<CreateMemberViewModel, Member>()
            .ForMember(dest => dest.Address, opt =>
            opt.MapFrom(src => new Address()
            {
                BuildingNumber = src.BuildingNumber,
                Street = src.Street,
                City = src.City
            }))
            .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));

    }
    private void MapSession()
    {
        CreateMap<CreateSessionViewModel, Session>();
        CreateMap<Trainer, TrainerSelectViewModel>();
        CreateMap<Category, CategorySelectViewModel>();

        CreateMap<Session, SessionViewModel>()
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
    }
    private void MapPlan()
    {
        CreateMap<Plan, UpdatePlanViewModel>()
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name));

        CreateMap<UpdatePlanViewModel, Plan>()
            .ForMember(dest => dest.Name, opt => opt.Ignore());
    }
    private void MapMemberShip()
    {
        CreateMap<Membership, MemberShipViewModel>()
            .ForMember(dist => dist.MemberName, opt => opt.MapFrom(src => src.Member.Name))
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt));
        
        CreateMap<MemberShipViewModel, Membership>();
        CreateMap<Member, MemberSelectListViewModel>();
        CreateMap<Plan, PlanSelectListViewModel>();
         
    }
}
