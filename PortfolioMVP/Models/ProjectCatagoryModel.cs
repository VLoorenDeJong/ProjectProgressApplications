﻿using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioMVP.Models
{
    public class ProjectCatagoryModel
    {
        public string CatagoryTitle { get; set; }
        public ProjectModel ProjectCatagory { get; set; }
        public List<ProjectModel> ProjectsInCatagory { get; set; }
    }
}
