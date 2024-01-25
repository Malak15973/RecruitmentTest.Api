using AutoMapper;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Jobs;
using RecruitmentTest.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, SelectListDto>().ReverseMap();
            CreateMap<JobSkill, SelectListDto>().
                ForMember(des => des.Name, src => src.MapFrom(src => src.Skill.Name)).
                ForMember(des => des.Id, src => src.MapFrom(src => src.Skill.Id)).
                ReverseMap();
            CreateMap<JobResponsability, SelectListDto>().
                ForMember(des => des.Name, src => src.MapFrom(src => src.Responsability.Name)).
                ForMember(des => des.Id, src => src.MapFrom(src => src.Responsability.Id)).
                ReverseMap();
            CreateMap<Job, JobDto>().
                ForMember(des => des.IsOpen, src => src.
                MapFrom(src => (src.JobApplicationUsers.Count < src.MaxApplicants) && (DateTime.Now >= src.ValidFrom && DateTime.Now <= src.ValidTo))).
                ReverseMap();

        }
    }
}
