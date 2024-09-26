using AutoMapper;
using CollegeApp.Model;
using Microsoft.IdentityModel.Tokens;

namespace CollegeApp.Configurations
{
    public class AutoMapperConfig : Profile
    {

        public AutoMapperConfig() {

            //CreateMap<Student, StudentDTO>();
            //CreateMap<StudentDTO, Student>();

            //Configuration for different property name
            //CreateMap<StudentDTO, Student>().ForMember(item => item.Name, option => option
            //                                .MapFrom(x=>x.StudentName))
            //                                .ReverseMap();

            //Ignore some property
            //CreateMap<StudentDTO, Student>().ReverseMap().ForMember(item => item.Name, option => option.Ignore());


            //Configuration for transforming specific property
            //CreateMap<StudentDTO, Student>().ReverseMap()
            //                                .ForMember(item => item.Email, option => option.MapFrom(item=> string.IsNullOrEmpty(item.Email) ? "No email Found" :  item.Email))
            //                                .AddTransform<string>(item => string.IsNullOrEmpty(item) ? "No email Found" : item);

            CreateMap<StudentDTO, Student>().ReverseMap();
        }
    }
}
