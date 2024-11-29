using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Data
{
    // this is the sync methods
    //public  interface ICourseCategoryRepository
    //{
    //    CourseCategory? GetById(int id);

    //    List<CourseCategory> GetAll();
    //}

    // this is the async methods
    public interface ICourseCategoryRepository
    {
        Task<CourseCategory?> GetByIdAsync(int id);

        Task<List<CourseCategory>> GetAllAsync();
    }
}
