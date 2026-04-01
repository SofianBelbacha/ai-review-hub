using System;
using System.Collections.Generic;
using System.Text;

namespace AiReviewHub.Application.Projects.Commands.CreateProjects
{
    public class CreateProjectProfile : Profile
    {
        public CreateProjectProfile()
        {
            CreateMap<Project, CreateProjectResult>();
        }
    }
}
