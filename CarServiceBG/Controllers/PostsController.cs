using AutoMapper;
using CarService.Business.Abstract;
using CarService.DataAccess.Database;
using CarService.Entities.Entities;
using CarServiceBG.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly CarServiceDbContext _context;
        private readonly IProductService _productService;
        private readonly ICarService _carService;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public PostsController(CarServiceDbContext context, IProductService productService,
            ICarService carService, IPhotoService photoService, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _productService = productService;
            _carService = carService;
            _photoService = photoService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            return Ok(await _context.Posts.ToListAsync());
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ActionName("GetNoteById")]
        public async Task<IActionResult> GetPostById([FromRoute] Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(Post post)
        {
            post.Id = Guid.NewGuid();
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromBody] Post post)
        {
            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Text = post.Text;
            existingPost.ImageUrl = post.ImageUrl;
            existingPost.DateTime = post.DateTime;

            await _context.SaveChangesAsync();
            return Ok(existingPost);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
