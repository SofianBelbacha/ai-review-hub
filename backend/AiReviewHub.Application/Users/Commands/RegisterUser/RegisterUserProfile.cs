using AiReviewHub.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Users.Commands.RegisterUser
{
    public class RegisterUserProfile : Profile
    {
        public RegisterUserProfile()
        {
            CreateMap<User, RegisterUserResult>()
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.Plan,
                    opt => opt.MapFrom(src => src.Plan.ToString()));
        }
    }
}
