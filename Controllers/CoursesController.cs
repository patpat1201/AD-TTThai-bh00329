using Microsoft.AspNetCore.Mvc;
using TrainingFPT.Helpers;
using TrainingFPT.Models.Queries;
using TrainingFPT.Models;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TrainingFPT.Controllers
{
    public class CoursesController : Controller
    {
        [HttpGet]
        public IActionResult Index(string SearchString, string Status)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionUsername")))
            {
                return RedirectToAction(nameof(LoginController.Index), "Login");
            }

            CoursesViewModel courseViewModel = new CoursesViewModel();
            courseViewModel.CourseDetailList = new List<CourseDetail>();
            var dataCategory = new CourseQuery().GetAllCourses(SearchString, Status);
            foreach (var item in dataCategory)
            {
                courseViewModel.CourseDetailList.Add(new CourseDetail
                {
                    Id = item.Id,
                    Name = item.Name,
                    NameCategory = item.NameCategory,
                    Description = item.Description,
                    PosterNameImage = item.PosterNameImage,
                    Status = item.Status,
                    Like = item.Like,
                    Star = item.Star,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt
                }); 
            }
            ViewData["keyword"] = SearchString;
            ViewBag.Status = Status;
            return View(courseViewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            CoursesViewModel coursesViewModel = new CoursesViewModel();
            coursesViewModel.CourseDetailList = new List<CourseDetail>();
            var dataCourse = new CourseQuery().GetCategory();
            foreach (var item in dataCourse)
            {
                coursesViewModel.CourseDetailList.Add(new CourseDetail
                {
                    CategoryId = item.CategoryId,
                    NameCategory = item.NameCategory

                });
            }
            IEnumerable<CourseDetail> courseDetails = coursesViewModel.CourseDetailList;
            ViewBag.coursesViewModel = courseDetails;
            CourseDetail model = new CourseDetail();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CourseDetail course, IFormFile PosterImage)
        {
            if (ModelState.IsValid)
            {
                // khong co loi tu phia nguoi dung
                // upload file va lay dc ten file save database
                string filePosterImage = UploadFileHelper.UploadFile(PosterImage);
                try
                {
                    int idInsetCate = new CourseQuery().InsertItemCourse(course.CategoryId, course.Name, course.Description, filePosterImage, course.Status, course.Like, course.Star);
                    if (idInsetCate > 0)
                    {
                        TempData["saveStatus"] = true;
                    }
                    else
                    {
                        TempData["saveStatus"] = false;
                    }
                }
                catch(Exception ex) 
                {
                    //return Ok(ex.Message);
                    TempData["saveStatus"] = false;
                }
                return RedirectToAction(nameof(CategoryController.Index), "Courses");
            }
            CoursesViewModel coursesViewModel = new CoursesViewModel();
            coursesViewModel.CourseDetailList = new List<CourseDetail>();
            var dataCourse = new CourseQuery().GetCategory();
            foreach (var item in dataCourse)
            {
                coursesViewModel.CourseDetailList.Add(new CourseDetail
                {
                    CategoryId = item.CategoryId,
                    NameCategory = item.NameCategory

                });
            }
            IEnumerable<CourseDetail> courseDetails = coursesViewModel.CourseDetailList;
            ViewBag.coursesViewModel = courseDetails;
    
            return View(course);
        }
        [HttpGet]
        public IActionResult Edit(int id = 0)
        {
            CoursesViewModel coursesViewModel = new CoursesViewModel();
            coursesViewModel.CourseDetailList = new List<CourseDetail>();
            var dataCourse = new CourseQuery().GetCategory();
            foreach (var item in dataCourse)
            {
                coursesViewModel.CourseDetailList.Add(new CourseDetail
                {
                    CategoryId = item.CategoryId,
                    NameCategory = item.NameCategory

                });
            }
            IEnumerable<CourseDetail> courseDetails = coursesViewModel.CourseDetailList;
            ViewBag.coursesViewModel = courseDetails;
            CourseDetail courseDetail = new CourseQuery().GetDataCourseById(id);
            return View(courseDetail);
        }
        [HttpPost]
        public IActionResult Edit(CourseDetail courseDetail, IFormFile PosterImage)
        {
            try
            {
                var detail = new CourseQuery().GetDataCourseById(courseDetail.Id);
                string uniquePosterImage = detail.PosterNameImage; // lay lai ten anh cu truoc khi thay anh moi (neu co)
                // nguoi dung co muon thay anh poster category hay ko?
                if (courseDetail.PosterImage != null)
                {
                    // co muon thay doi anh
                    uniquePosterImage = UploadFileHelper.UploadFile(PosterImage);
                }
                bool update = new CourseQuery().UpdateCourseById(
                    courseDetail.CategoryId,
                    courseDetail.Name,
                    courseDetail.Description,
                    uniquePosterImage,
                    courseDetail.Status,
                    courseDetail.Like,
                    courseDetail.Star,
                    courseDetail.Id
                    );

                if (update)
                {
                    TempData["updateStatus"] = true;
                }
                else
                {
                    TempData["updateStatus"] = false;
                }
                return RedirectToAction(nameof(CategoryController.Index), "Courses");
            }
            catch (Exception ex)
            {
                return View(courseDetail);
            }
        }
        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            bool del = new CourseQuery().DeleteItemCourse(id);
            if (del)
            {
                TempData["statusDel"] = true;
            }
            else
            {
                TempData["statusDel"] = false;
            }
            return RedirectToAction(nameof(CoursesController.Index), "Courses");
        }
    }
    
}
