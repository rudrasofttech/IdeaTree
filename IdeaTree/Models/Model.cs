using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdeaTree.Models
{
    public class Member
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public String Phone { get; set; }
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }
        public string Country { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? PasswordGenerateDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; }
        public StatusType Status { get; set; }
        public MemberType MType { get; set; }
        public string Image { get; set; }
        [MaxLength(150)]
        public string FullName { get; set; }
        public string Bio { get; set; }
        public bool Newsletter { get; set; }

        public DateTime? LastLogon { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Member ModifiedBy { get; set; }
    }

    public class Idea
    {
        public int ID { get; set; }
        [MaxLength(250), Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public DateTime PostDate { get; set; }
        public Member PostedBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public Member ModifiedBy { get; set; }
        public StatusType Status { get; set; }

        public string Video { get; set; }
        public ICollection<Vote> votes  { get; set; }
        public ICollection<Comment> comments { get; set; }
        public ICollection<IdeaImages> IdeaImage { get; set; }
    }

    public class IdeaImages
    {
        [Key]
        public int IdeaImageId { get; set; }
        public int Id { get; set; }
        public  Idea Idea { get; set; }
        public string Image { get; set; }
    }
    public class Vote
    {
        public int ID { get; set; }
        public Member VotedBy { get; set; }
        public Idea VoteTo { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class Comment
    {
        public int ID { get; set; }
        public Member PostedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public String Content { get; set; }
        public Idea PostedTo { get; set; }
        public StatusType Status { get; set; }
    }

    public class Message
    {
        public int ID { get; set; }
        [DataType(DataType.EmailAddress)]
        public string ToEmail { get; set; }
        [MaxLength(100)]
        public string ToName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string FromEmail { get; set; }
        [MaxLength(100)]
        public string FromName { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        [MaxLength(100)]
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SentDate { get; set; }
    }


    public enum CategoryType
    {
        Device = 1
    }

    public enum StatusType
    {
        Unapproved = 1,
        Active = 2,
        Inactive = 3,
        Deleted = 4
    }

    public enum MemberType
    {
        Admin = 1,
        Member = 2
    }

    public class Country
    {
        public string Abbr { get; set; }
        public string Full { get; set; }
    }
}
