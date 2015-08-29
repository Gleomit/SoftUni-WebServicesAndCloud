namespace News.Services.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NewsBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }
    }
}