﻿using JSON_DAL;
using PhotoManager.Models;
using System;

namespace PhotosManager.Models
{
    public sealed class DB
    {
        #region singleton setup
        private static readonly DB instance = new DB();
        public static DB Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion
        #region Repositories

        static public UsersRepository Users { get; set; }
            = new UsersRepository();

        static public LoginsRepository Logins { get; set; } 
            = new LoginsRepository();

        static public Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
            = new Repository<UnverifiedEmail>();

        static public Repository<RenewPasswordCommand> RenewPasswordCommands { get; set; }
            = new Repository<RenewPasswordCommand>();
        static public PhotosRepository Photos { get; set; }
            = new PhotosRepository();
        static public LikesRepository Likes { get; set; }
            = new LikesRepository();
        #endregion
    }
}