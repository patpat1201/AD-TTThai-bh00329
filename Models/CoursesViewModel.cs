using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TrainingFPT.Validations;

namespace TrainingFPT.Models
{
    public class CoursesViewModel
    {
        public List<CourseDetail> CourseDetailList { get; set; }
    }
    public class CourseDetail
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Choose Status, please")]
        public int CategoryId { get; set; }
        public string Name { get; set; }

        [AllowNull]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Choose File, please")]
        [AllowExtensionFile(new string[] { ".png", ".jpg", ".jpeg" })]
        [AllowMaxSizeFile(5 * 1024 * 1024)]
        public IFormFile PosterImage { get; set; }

        // view ten anh
        
        [Required(ErrorMessage = "Choose Status, please")]
        public string Status { get; set; }
        public int Like { get; set; }
        public int Star { get; set; }

        [AllowNull]
        public string? PosterNameImage { get; set; }
        [AllowNull]
        public DateTime? CreatedAt { get; set; }

        [AllowNull]
        public DateTime? UpdatedAt { get; set; }

        [AllowNull]
        public DateTime? DeletedAt { get; set; }

        [AllowNull]
        public string? NameCategory { get; set; }
     
    }
}
