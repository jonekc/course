using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Projekt.Shared.Models;
using Projekt.Server.Services;
using System.Security.Claims;
using Projekt.Server.Helpers;
using System.Collections.Generic;
using Projekt.Shared.Entities;

namespace Projekt.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IApiService _apiService;

        public ApiController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticateRequest request)
        {
            AuthenticateResponse token = await _apiService.Login(request);
            if (token == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(token);
        }

        [Authorize]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _apiService.GetCategories());
        }

        [Authorize]
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] int categoryId)
        {
            return Ok(await _apiService.GetCourses(categoryId, AuthHelper.GetUserId((ClaimsIdentity)User.Identity)));
        }

        [Authorize]
        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            CourseModel course = await _apiService.GetCourse(id, AuthHelper.GetUserId((ClaimsIdentity)User.Identity));
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("courses")]
        public async Task<IActionResult> AddCourse(CourseModel course)
        {
            CourseModel newCourse = await _apiService.AddCourse(course);
            return CreatedAtAction(nameof(AddCourse), newCourse);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("courses")]
        public async Task<IActionResult> EditCourse(CourseModel course)
        {
            CourseModel editedCourse = await _apiService.EditCourse(course);
            if (editedCourse == null)
            {
                return NotFound();
            }
            return Ok(editedCourse);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            bool deleted = await _apiService.DeleteCourse(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize]
        [HttpGet("courses/items/{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            ItemModel item = await _apiService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("courses/items")]
        public async Task<IActionResult> EditItem(ItemModel item)
        {
            ItemModel editedItem = await _apiService.EditItem(item);
            if (editedItem == null)
            {
                return NotFound();
            }
            return Ok(editedItem);
        }

        [Authorize]
        [HttpGet("courses/items/questions/{id}")]
        public async Task<IActionResult> GetQuestions(int id)
        {
            List<QuestionModel> questions = await _apiService.GetQuestions(id);
            if (questions == null)
            {
                return NotFound();
            }
            return Ok(questions);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("courses/items/{id}/questions")]
        public async Task<IActionResult> AddQuestions(int id, List<QuestionModel> questions)
        {
            List<QuestionModel> newQuestions = await _apiService.AddQuestions(id, questions);
            if (newQuestions == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(AddQuestions), newQuestions);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("courses/items/{id}/questions")]
        public async Task<IActionResult> EditQuestions(int id, List<QuestionModel> questions)
        {
            List<QuestionModel> editedQuestions = await _apiService.EditQuestions(id, questions);
            if (editedQuestions == null)
            {
                return NotFound();
            }
            return Ok(editedQuestions);
        }

        [Authorize]
        [HttpPost("courses/send")]
        public async Task<IActionResult> AddSentItem(SentItem item)
        {
            SentItem newItem = await _apiService.AddSentItem(item, AuthHelper.GetUserId((ClaimsIdentity)User.Identity));
            if (newItem == null)
            {
                return NotFound();
            }
            newItem.User.Password = "a";
            return CreatedAtAction(nameof(AddSentItem), newItem);
        }

        [Authorize]
        [HttpGet("courses/send/questions/{itemId}")]
        public async Task<IActionResult> GetSentQuestions(int itemId)
        {
            List<SentQuestionModel> questions = await _apiService.GetSentQuestions(itemId, AuthHelper.GetUserId((ClaimsIdentity)User.Identity));
            if (questions == null)
            {
                return NotFound();
            }
            return Ok(questions);
        }

        [Authorize]
        [HttpPost("courses/send/questions")]
        public async Task<IActionResult> AddSentQuestions(List<SentQuestionModel> questions)
        {
            List<SentQuestionModel> newQuestions = await _apiService.AddSentQuestions(questions, AuthHelper.GetUserId((ClaimsIdentity)User.Identity));
            if (newQuestions == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(AddSentQuestions), newQuestions);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("courses/stats/{id}")]
        public async Task<IActionResult> GetCourseStats(int id)
        {
            List<SentItem> items = await _apiService.GetCourseStats(id);
            return Ok(items);
        }
    }
}