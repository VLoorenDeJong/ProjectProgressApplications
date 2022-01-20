﻿using Microsoft.Extensions.Configuration;
using ProjectProgressLibrary.DataAccess;

namespace ProjectProgressLibrary.StartConfig
{
    public interface IStartConfig
    {
        bool DecideToShowPicture(string projectPictureFilePath);
        (IDataAccess database, string mainGoal) GetDbConfig(IConfiguration config, IDataAccess db, string pageName);
        (IDataAccess database, string mainGoal) GetProgressDbConfig(IConfiguration config, IDataAccess db, string pageName);
        (string ProjectPictureFilePath, bool ShowPicture) SetUpPictureShowing(string title, IDataAccess db, string rootFolderPath);
    }
}