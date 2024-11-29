using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Data;


namespace Service
{
    public interface ICourseCategoryService
    {
        Task<CourseCategoryModel> GetByIdAsync(int id);

        Task<CourseCategoryModel> GetCourseCategories();
    }

    public class CourseCategoryService : ICourseCategoryService
    {

        private readonly ICourseCategoryRepository categoryRepository;
        public CourseCategoryService(ICourseCategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public Task<CourseCategoryModel> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CourseCategoryModel> GetCourseCategories()
        {
            throw new NotImplementedException();
        }
    }
}
