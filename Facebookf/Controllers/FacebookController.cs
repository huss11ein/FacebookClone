using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Facebookf.Models;
using Facebookf.ViewModels;

namespace Facebookf.Controllers
{
    public class FacebookController : Controller
    {
        private FacebookEntities2 db = new FacebookEntities2();
        public ActionResult feed()
        {
            var Acccpted = db.FriendRequests.Where(m => m.SenderID == 1 || m.RecieverID == 1 && m.IsApproved == 1).ToList();
            var OldPosts = db.POSTS.Where(m => m.UserID == 2).ToList();
            POST post = new POST();
            User user = db.Users.Where(m => m.UserID == 2).FirstOrDefault();
            var imgsrc = "~/img/Default.jpg";
            if (user.UserImage != null)
            {
                var base64 = Convert.ToBase64String(user.UserImage);
                imgsrc = string.Format("data:image/jpg;base64,{0}", base64);

            }
            ViewBag.ImageSrc = imgsrc;
            ViewOldPostsAndAdd VOPAA = new ViewOldPostsAndAdd()
            {
                OldPosts = OldPosts,
                UserLikes = db.Likes.Where(m => m.UserID == 2 && m.POST.UserID == 2).ToList(),
                post = post,
                User = user,
                Accepted = Acccpted
            };
            return View(VOPAA);
        }

        public ActionResult LoginAndRegister()
        {


            return View();
        }


        public ActionResult Login(User user) {
            var usr = db.Users.Single(u => u.UserName == user.UserName && u.Password == user.Password);
            if (usr != null)
            {
                Session["UserID"] = usr.UserID;


                return RedirectToAction("feed");
            }
            else
            {
                ModelState.AddModelError("", "User Name or Password is wrong.");
                return RedirectToAction("LoginAndRegister");
            }
        }
        public ActionResult Index(string search)
        {
            int numericValue;
            bool isNumber = int.TryParse(search, out numericValue);

            if (isNumber)
            {
                int IntSearch = Int32.Parse(search);
                return Json(db.Users.Where(x => x.PhoneNumber == IntSearch || search == null).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(db.Users.Where(x => x.UserEmail == search || search == null).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult WriteComment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                return RedirectToAction("feed");
            }
            return RedirectToAction("feed");
        }

        public String DecideLikeOrDisLike(int PostID, bool status)
        {
            var Toggle = false;
            Like like = db.Likes.FirstOrDefault(m => m.PostID == PostID && m.UserID == 5);
            POST post = db.POSTS.FirstOrDefault(m => m.PostID == PostID);
            if (like == null)//The user does not press like or dislike before 
            {
                like = new Like();
                like.UserID = 2;
                like.PostID = PostID;
                like.LikeOrDislike = status;
                if (status == true)//Decide user pressed like or dislike---true=like
                {
                    if (post.LikesNumber == null)
                    {
                        post.LikesNumber = 1;
                        post.DisLikesNumber = post.DisLikesNumber ?? 0;
                    } // if dislikenumber !=null so put zero in it 
                    else
                        post.LikesNumber += 1;
                }
                else                 //Decide user pressed like or dislike---false=Dislike
                {
                    if (post.DisLikesNumber == null)
                    {
                        post.DisLikesNumber = 1;
                        post.LikesNumber = post.LikesNumber ?? 0;
                    } // if likenumber !=null so put zero in it 
                    else
                        post.DisLikesNumber += 1;
                }
                db.Likes.Add(like);
            }
            else
            {
                Toggle = true;
            }
            if (Toggle)
            {
                like.LikeOrDislike = status;
                if (status == true)
                {
                    post.LikesNumber += 1;
                    post.DisLikesNumber -= 1;

                }
                else
                {
                    post.DisLikesNumber += 1;
                    post.LikesNumber -= 1;

                }


            }
            db.SaveChanges();
            return post.LikesNumber + "/" + post.DisLikesNumber;

        }
        public ActionResult Like(int id, bool status)
        {
            var result = DecideLikeOrDisLike(id, status);  // calling and sending data to  like function using Db
            return Content(result);
        }

        public ActionResult Profile(int id)
        {
            var OldPosts = db.POSTS.Where(m => m.UserID == id && m.Publicity == 0).ToList();
            var imgsrc = "~/img/Default.jpg";
            User user = db.Users.Where(m => m.UserID == id).FirstOrDefault();
            if (user.UserImage != null)
            {
                var base64 = Convert.ToBase64String(user.UserImage);
                imgsrc = string.Format("data:image/jpg;base64,{0}", base64);

            }
            ViewBag.ImageSrc = imgsrc;
            
            
            FriendRequest FR = db.FriendRequests.Where(m => (m.SenderID == 1 && m.RecieverID == 2) || (m.SenderID == 2 && m.RecieverID == 1)).SingleOrDefault();
            ViewOldPostsAndAdd VOPAA = new ViewOldPostsAndAdd()
            {
                OldPosts = OldPosts,
                UserLikes = db.Likes.Where(m => m.UserID == id && m.POST.UserID == id).ToList(),
                User = user,
                FR = FR

            };

            return View(VOPAA);
        }
        public ActionResult AcceptRequest(int SenderID,int RecieverID )
        {

            return RedirectToAction("Profile");
        }
      //  public ActionResult DeclineRequest() { }

        public ActionResult AddFriend(int senderID, int recieverID)
        {
            FriendRequest FR = new FriendRequest();
            FR.SenderID = senderID;
            FR.RecieverID = recieverID;
            FR.IsApproved = 0;
            db.FriendRequests.Add(FR);
            db.SaveChanges();
            return RedirectToAction("profile");
        }
        public ActionResult WritePost(HttpPostedFileBase image, ViewOldPostsAndAdd VOPAA)
        {
            if (image != null)
            {
                VOPAA.post.PostImage = new byte[image.ContentLength];
                image.InputStream.Read(VOPAA.post.PostImage, 0, image.ContentLength);

            }
            DateTime x = DateTime.Now;
            VOPAA.post.PostDate = x;
            VOPAA.post.UserID = 2;
            db.POSTS.Add(VOPAA.post);
            db.SaveChanges();
            return RedirectToAction("feed");


        }



    }
}
