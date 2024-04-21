using Microsoft.Data.SqlClient;

namespace TrainingFPT.Models.Queries
{
    public class CourseQuery
    {
        public List<CourseDetail> GetAllCourses(string? keyword, string? filterStatus)
        {
            string dataKeyword = "%" + keyword + "%";
            List<CourseDetail> courses = new List<CourseDetail>();
            Dictionary<int, string> categoryNames = new Dictionary<int, string>();
           
            using (SqlConnection conn = Database.GetSqlConnection())
            {
                string sqlQuery = string.Empty;
                if (filterStatus != null)
                {
                    sqlQuery = "SELECT * FROM [Courses] WHERE [NameCourse] LIKE @keyword AND [DeletedAt] IS NULL AND [Status] = @status";
                }
                else
                {
                    sqlQuery = "SELECT * FROM [Courses] WHERE [NameCourse] LIKE @keyword AND [DeletedAt] IS NULL";
                }
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@keyword", dataKeyword ?? DBNull.Value.ToString());
                if (filterStatus != null)
                {
                    cmd.Parameters.AddWithValue("@status", filterStatus ?? DBNull.Value.ToString());
                }


                conn.Open();
                using (SqlCommand cmdCategories = new SqlCommand("SELECT Id, Name FROM Categories", conn))
                {
                    using (SqlDataReader readerCategories = cmdCategories.ExecuteReader())
                    {
                        while (readerCategories.Read())
                        {
                            categoryNames.Add(Convert.ToInt32(readerCategories["Id"]), readerCategories["Name"].ToString());
                        }
                    }
                }
                conn.Close();


                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CourseDetail courseDetail = new CourseDetail();
                        courseDetail.Id = Convert.ToInt32(reader["Id"]);
                        courseDetail.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        courseDetail.Name = reader["NameCourse"].ToString();
                        courseDetail.Description = reader["Description"].ToString();
                        courseDetail.PosterNameImage = reader["Image"].ToString();
                        courseDetail.Status = reader["Status"].ToString();
                        courseDetail.Like = Convert.ToInt32(reader["LikeCourse"]);
                        courseDetail.Star = Convert.ToInt32(reader["StarCourse"]);
                        courseDetail.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
                        courseDetail.UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedAt"]) : DateTime.MinValue;
                        courseDetail.DeletedAt = reader["DeletedAt"] != DBNull.Value ? Convert.ToDateTime(reader["DeletedAt"]) : DateTime.MinValue;
                        if (categoryNames.ContainsKey(courseDetail.CategoryId))
                        {
                            courseDetail.NameCategory = categoryNames[courseDetail.CategoryId];
                        }
                        courses.Add(courseDetail);

                    }
                    conn.Close();
                }
                
            }
            return courses;
        }
        public int InsertItemCourse(
            int categoryId,
            string nameCourse,
            string description,
            string image,
            string status,
            int like,
            int star
        )
        {
            int lastIdInsert = 0; // id cua category vua dc them moi
            string sqlQuery = "INSERT INTO [Courses]([CategoryId],[NameCourse],[Description],[Image],[Status],[LikeCourse], [StarCourse], [CreatedAt]) VALUES(@categoryId, @nameCourse, @description, @image, @status, @like, @star, @createdAt) SELECT SCOPE_IDENTITY()";
            // SELECT SCOPE_IDENTITY() : lay ra id vua dc them moi

            using (SqlConnection connection = Database.GetSqlConnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                cmd.Parameters.AddWithValue("@nameCourse", nameCourse ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@description", description ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@image", image ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@status", status ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@like", like);
                cmd.Parameters.AddWithValue("@Star", star);
                cmd.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                lastIdInsert = Convert.ToInt32(cmd.ExecuteScalar());
                connection.Close();
            }

            // lastIdInsert tra ve lon hon 0 insert thanh cong va nguoc lai
            return lastIdInsert;
        }
        public List<CourseDetail> GetCategory()
        {
            List<CourseDetail> courses = new List<CourseDetail>();
            using (SqlConnection conn = Database.GetSqlConnection())
            {
                string sqlQuery = "SELECT * FROM [Categories] WHERE [DeletedAt] IS NULL";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) // Kiểm tra xem có dữ liệu để đọc hay không
                    {
                        CourseDetail courseDetail = new CourseDetail();
                        courseDetail.NameCategory = reader["Name"].ToString();
                        courseDetail.CategoryId = Convert.ToInt32(reader["Id"]);
                        courses.Add(courseDetail);

                    }
                }
                conn.Close();
            }
            return courses;
        }
        public CourseDetail GetDataCourseById(int id = 0)
        {
            CourseDetail courseDetail = new CourseDetail();
            using (SqlConnection connection = Database.GetSqlConnection())
            {
                string sqlQuery = "SELECT * FROM [Courses] WHERE [Id] = @id AND [DeletedAt] IS NULL";
                connection.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                cmd.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courseDetail.Id = Convert.ToInt32(reader["Id"]);
                        courseDetail.Name = reader["NameCourse"].ToString();
                        courseDetail.Description = reader["Description"].ToString();
                        courseDetail.PosterNameImage = reader["Image"].ToString();
                        courseDetail.Status = reader["Status"].ToString();
                        courseDetail.Like = Convert.ToInt32(reader["LikeCourse"]);
                        courseDetail.Star = Convert.ToInt32(reader["StarCourse"]);
                    }
                    connection.Close(); // ngat ket noi
                }
            }
            return courseDetail;
        }
        public bool UpdateCourseById(
            int categoryId,
            string nameCourse,
            string description,
            string image,
            string status,
            int like,
            int star,
            int id
        )
        {
            bool checkUpdate = false;
            using (SqlConnection connection = Database.GetSqlConnection())
            {
                string sqlUpdate = "UPDATE [Courses] SET [CategoryId] = @categoryId,[NameCourse] = @nameCourse, [Description] = @description, [Image] = @image, [Status] = @status, [LikeCourse] = @like, [StarCourse] = @star, [UpdatedAt] = @updatedAt WHERE [Id] = @id AND [DeletedAt] IS NULL";
                connection.Open();
                SqlCommand cmd = new SqlCommand( sqlUpdate, connection );
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                cmd.Parameters.AddWithValue("@nameCourse", nameCourse ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@description", description ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@image", image ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@status", status ?? DBNull.Value.ToString());
                cmd.Parameters.AddWithValue("@like", like);
                cmd.Parameters.AddWithValue("@star", star);
                cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                connection.Close();
                checkUpdate = true;
            }
            return checkUpdate;
        }
        public bool DeleteItemCourse(int id = 0)
        {
            bool statusDelete = false;
            using (SqlConnection connection = Database.GetSqlConnection())
            {
                string sqlQuery = "UPDATE [Courses] SET [DeletedAt] = @deletedAt WHERE [Id] = @id";
                SqlCommand cmd = new SqlCommand(sqlQuery, connection);
                connection.Open();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@deletedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();
                statusDelete = true;
                connection.Close();
            }
            // false : ko xoa dc - true : xoa thanh cong
            return statusDelete;
        }
    }
}
