using JSON_DAL;
using Newtonsoft.Json;
using PhotosManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoManager.Models
{
    public class Photo : Record
    {
        const string Avatars_Folder = @"/App_Assets/Photos/";
        const string Default_Avatar = @"no_image.png";

        [Display(Name = "Titre"), Required(ErrorMessage = "Obligatoire")]
        public string Title { get; set; }
        [Display(Name = "Description"), Required(ErrorMessage = "Obligatoire")]
        public string Description { get; set; }
        public int OwnerId { get; set; }

        [ImageAsset(Avatars_Folder, Default_Avatar)]
        public string Path { get; set; }
        public DateTime Date { get; set; }
        public bool Shared { get; set; }
        [JsonIgnore]
        public User Owner => DB.Users.Get(OwnerId);
        [JsonIgnore]
        public List<Like> Likes => DB.Likes.ToList().Where(l => l.PhotoId == Id).ToList();
    }
}