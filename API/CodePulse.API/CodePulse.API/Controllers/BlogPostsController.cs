using Azure.Core;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostsController(IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
        }
        // POST: {apibaseurl}/api/blogposts
        [HttpPost]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            var blogPost = new BlogPost
            {
                Title = request.Title,
                Author = request.Author,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                UrlHandle = request.UrlHandle,
                IsVisible = request.IsVisible,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory =  await categoryRepository.GetByIdAsync(categoryGuid);

                if(existingCategory is not null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.CreateAsync(blogPost);

            // Convert domain model to dto
            var response = new BlogPostDTO
            {
                Author = blogPost.Author,
                Content = blogPost.Content,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                IsVisible = blogPost.IsVisible,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Id = blogPost.Id,
                Title = blogPost.Title,
                Categories = blogPost.Categories.Select(x=> new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            return Ok(response);
        }

        // GET: {apibaseurl}/api/blogposts
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await blogPostRepository.GetAllAsync();

            // convert domain model to dto

            var response = new List<BlogPostDTO>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(new BlogPostDTO
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    Author = blogPost.Author,
                    Content = blogPost.Content,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    IsVisible = blogPost.IsVisible,
                    Categories = blogPost.Categories.Select(x => new CategoryDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    }).ToList()
                });
            }

            return Ok(response);
        }

        // GET: {apibaseurl}/api/blogposts/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBLogPostById([FromRoute] Guid id)
        {
            // get the blogpost fom repository
            var blogPost = await blogPostRepository.GetByIdAsync(id);

            if (blogPost is null)
            {
                return NotFound();
            }

            // convert domain model to DTO
            var response = new BlogPostDTO
            {
                Title = blogPost.Title,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            return Ok(response);
        }

        // PUT: {apibaseurl}/api/blogposts/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, UpdateBlogPostRequestDTO request)
        {
            // convert DTO to domain model
            var blogPost = new BlogPost
            {
                Id = id,
                Title = request.Title,
                Author = request.Author,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                UrlHandle = request.UrlHandle,
                IsVisible = request.IsVisible,
                Categories = new List<Category>()
            };

            // foreach 
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetByIdAsync(categoryGuid);

                if (existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            // call repository to upate blogpost domain model
            var updatedBlogPost = await blogPostRepository.UpdateAsync(blogPost);

            if (updatedBlogPost == null)
            {
                return NotFound();
            }

            // convert domain model back to dto
            var response = new BlogPostDTO
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            return  Ok(response);
        }
    }
}
