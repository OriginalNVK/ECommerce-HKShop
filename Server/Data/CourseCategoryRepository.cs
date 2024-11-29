using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    //this is primary constructor and available from .net 8
    //public class CourseCategoryRepository(OnlineCourseDbContext dbContext) : ICourseCategoryRepository
    //{ 

    //    private readonly OnlineCourseDbContext dbContext = dbContext;

    //    public List<CourseCategory> GetAll()
    //    {
    //        var data = dbContext.CourseCategories.ToList();
    //        return data;
    //    }

    //    public CourseCategory? GetById(int id)
    //    {
    //        // This is the sync method will try to find the record with its primary key 
    //        var data = dbContext.CourseCategories.Find(id);
    //        return data;
    //    }
    //}

    // implement the async methods
    public class CourseCategoryRepository(OnlineCourseDbContext dbContext) : ICourseCategoryRepository
    {
        private readonly OnlineCourseDbContext dbContext = dbContext;

        public Task<List<CourseCategory>> GetAllAsync()
        {
            var data = dbContext.CourseCategories.ToListAsync();
            return data;
        }

        public Task<CourseCategory?> GetByIdAsync(int id)
        {
            var data = dbContext.CourseCategories.FindAsync(id).AsTask();
            return data;
        }
    }
}
