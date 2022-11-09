using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebookf.Models;

namespace Facebookf.ViewModels
{
    public class ViewOldPostsAndAdd 
    {
        public POST post { get; set; }
        public User User { get; set; }
        public User userSearch { get; set; }
        public IEnumerable<POST> OldPosts { get; set; }
        public IEnumerable<Like> UserLikes { get; set; }
        public IEnumerable<FriendRequest> Accepted { get; set; }
        public IEnumerable<FriendRequest> Pended { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public FriendRequest FR { get; set; }

    }

}