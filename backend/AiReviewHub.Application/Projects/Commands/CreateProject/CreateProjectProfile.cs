using AiReviewHub.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.CreateProject
{
    public class CreateProjectProfile : Profile
    {
        public CreateProjectProfile()
        {
            CreateMap<Project, CreateProjectResult>();
        }
    }
}
