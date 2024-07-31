using AutoMapper;
using TestConsoleApplication.Services.FileControl.Models;
using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplication.Services.Mapper
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<ImportBook, Book>();
        }
    }
}
