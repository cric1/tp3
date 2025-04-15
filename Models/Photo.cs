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
        const string Photos_Folder = @"/App_Assets/Photos/";
        const string Default_Photo = @"no_photo.png";
        public static string DefaultImage { get { return Photos_Folder + Default_Photo; } }

        [Display(Name = "Titre"), Required(ErrorMessage = "Obligatoire")]
        public string Title { get; set; }
        [Display(Name = "Description"), Required(ErrorMessage = "Obligatoire")]
        public string Description { get; set; }
        public int OwnerId { get; set; }

        [ImageAsset(Photos_Folder, Default_Photo)]
        public string Path { get; set; } = DefaultImage;
        public DateTime Date { get; set; }
        public bool Shared { get; set; }
        [JsonIgnore]
        public User Owner => DB.Users.Get(OwnerId);
        [JsonIgnore]
        public List<Like> Likes => DB.Likes.ToList().Where(l => l.PhotoId == Id).ToList();
    }
}