using AiReviewHub.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Feedbacks.Commands.CreateFeedback
{
    public class CreateFeedbackProfile : Profile
    {
        public CreateFeedbackProfile()
        {
            CreateMap<Feedback, CreateFeedbackResult>()
                .ForMember(dest => dest.Content,
                    opt => opt.MapFrom(src => src.Content.Value))
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
