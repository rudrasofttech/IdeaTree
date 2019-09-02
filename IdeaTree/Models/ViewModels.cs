using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace IdeaTree.Models
{
    public class SignupModel
    {
        [Required]
        public string Name { get; set; }
        [Required, MaxLength(15)]
        public string Phone { get; set; }
        public bool Newsletter { get; set; }
    }

    public class LoginModel
    {
        [Required, MaxLength(15)]
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class ProfileModel
    {
        public int ID { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public bool Newsletter { get; set; }
    }

    public class CreateIdeaModel
    {
        [Required, MaxLength(100)]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public CategoryType Category { get; set; }
    }

    public class HomeModel
    {
        public List<Idea> Ideas { get; set; }
    }

    public class IdeaPublicModel
    {
        public Idea idea { get; set; }
        public int VoteCount { get; set; }
        public bool HasVoted { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Idea> OtherIdeasFromOwner { get; set; }
        public IdeaPublicModel()
        {
            Comments = new List<Comment>();
            OtherIdeasFromOwner = new List<Idea>();
        }
    }

}
