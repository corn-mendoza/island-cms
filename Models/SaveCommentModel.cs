using System;
using System.ComponentModel.DataAnnotations;

namespace cms_mvc.Models
{
    public class SaveCommentModel
    {
        [Required(ErrorMessage = "Post ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        [StringLength(128, ErrorMessage = "Author name cannot exceed 128 characters")]
        public string CommentAuthor { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string CommentEmail { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(512, ErrorMessage = "URL cannot exceed 512 characters")]
        public string CommentUrl { get; set; }

        [Required(ErrorMessage = "Comment body is required")]
        [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters")]
        public string CommentBody { get; set; }
    }
}