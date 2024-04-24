using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject_TailTales.Models
{
    public class MyUserData
    {
        public List<Pet> MyPets { get; set; }
        public List<Memo> MyMemo { get; set; }
        public List<AlbumFoto> MyAlbum { get; set; }
    }
}