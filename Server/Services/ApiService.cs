using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Projekt.Shared.Entities;
using Projekt.Server.Helpers;
using BCryptNet = BCrypt.Net.BCrypt;
using Projekt.Shared.Models;
using System.Linq;

namespace Projekt.Server.Services
{
    public class ApiService : IApiService
    {
        private readonly StudyContext _context;
        private readonly IConfiguration _config;

        public ApiService(StudyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthenticateResponse> Login(AuthenticateRequest request)
        {
            User foundUser = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == request.Login);
            if (foundUser == null || !BCryptNet.Verify(request.Password, foundUser.Password))
            {
                return null;
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, foundUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, foundUser.Login),
                new Claim(ClaimTypes.Role, foundUser.Role?.Name ?? "")
            };
            IConfiguration jwtConfig = _config.GetSection("JwtToken");
            JwtSecurityToken token = AuthHelper.GetJwtToken(
                jwtConfig["SigningKey"],
                jwtConfig["Issuer"],
                jwtConfig["Audience"],
                TimeSpan.FromMinutes(double.Parse(jwtConfig["TimeoutMinutes"])),
                claims);

            return new AuthenticateResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
            };
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<List<CourseModel>> GetCourses(int categoryId, int userId)
        {
            List<Course> courses = await _context.Course.Include(c => c.Category).Where(c => categoryId == 0 || c.Category.CategoryId == categoryId).Include(c => c.Items).ThenInclude(i => i.Questions).ThenInclude(q => q.Answers).ToListAsync();

            foreach (Course course in courses)
            {
                course.Points = await GetCoursePoints(course, userId);
                course.Completed = course.Items.Count == await _context.SentItem.Include(si => si.Item).ThenInclude(i => i.Course).Include(si => si.User).Where(si => si.User.UserId == userId && si.Item.Course.CourseId == course.CourseId).CountAsync();
            }
            return courses.Select(c => new CourseModel() { CourseId = c.CourseId, Name = c.Name, Description = c.Description, CategoryId = c.Category?.CategoryId ?? 0, CategoryName = c.Category?.Name ?? "", Points = c.Points, MaxPoints = c.MaxPoints, Completed = c.Completed }).ToList();
        }

        public async Task<CourseModel> GetCourse(int id, int userId)
        {
            Course course = await _context.Course.Include(c => c.Category).Include(c => c.Items).ThenInclude(i => i.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null)
            {
                return null;
            }
            if (userId > 0)
            {
                course.Points = await GetCoursePoints(course, userId);
            }
            return new CourseModel() { CourseId = course.CourseId, Name = course.Name, Description = course.Description, CategoryId = course.Category?.CategoryId ?? 0, CategoryName = course.Category?.Name ?? "", Items = course.Items.Select(i => (ItemModel)GetItemModel(i)).ToList(), Points = course.Points, MaxPoints = course.MaxPoints, Completed = course.Completed };
        }

        public async Task<CourseModel> AddCourse(CourseModel course)
        {
            Course newCourse = new() { Name = course.Name, Description = course.Description, Items = course.Items.Select(i => (Item)GetItemModel(i)).ToList() };
            newCourse.Category = course.CategoryId > 0 ? await _context.Category.FirstOrDefaultAsync(c => c.CategoryId == course.CategoryId) : !string.IsNullOrWhiteSpace(course.CategoryName) ? new Category() { Name = course.CategoryName } : null;
            await _context.Course.AddAsync(newCourse);
            await _context.SaveChangesAsync();
            return new CourseModel() { CourseId = newCourse.CourseId, Name = newCourse.Name, Description = newCourse.Description, CategoryId = newCourse.Category?.CategoryId ?? 0, CategoryName = newCourse.Category?.Name ?? "", Items = newCourse.Items.Select(i => (ItemModel)GetItemModel(i)).ToList(), MaxPoints = newCourse.MaxPoints };
        }

        public async Task<CourseModel> EditCourse(CourseModel course)
        {
            Course foundCourse = await _context.Course.Include(c => c.Category).Include(c => c.Items).FirstOrDefaultAsync(c => c.CourseId == course.CourseId);
            if (foundCourse == null)
            {
                return null;
            }
            foundCourse.Name = course.Name;
            foundCourse.Description = course.Description;
            foundCourse.Category = course.CategoryId > 0 ? await _context.Category.FirstOrDefaultAsync(c => c.CategoryId == course.CategoryId) : !string.IsNullOrWhiteSpace(course.CategoryName) ? new Category() { Name = course.CategoryName } : null;

            List<Item> items = foundCourse.Items;
            List<Item> itemsToRemove = items.Where(ir => !course.Items.Any(i => i.ItemId == ir.ItemId)).ToList();
            items.RemoveAll(ir => itemsToRemove.Any(i => i.ItemId == ir.ItemId));
            RemoveItems(itemsToRemove);

            foreach (ItemModel item in course.Items)
            {
                Item editedItem = items.FirstOrDefault(i => i.ItemId == item.ItemId);
                if (editedItem != null && editedItem.ItemId != 0)
                {
                    editedItem.Name = item.Name;
                    editedItem.Description = item.Description;
                    editedItem.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    Item newItem = new() { Course = foundCourse, Name = item.Name, Description = item.Description, UpdatedDate = DateTime.UtcNow };
                    foundCourse.Items.Add(newItem);
                }
            }

            _context.Course.Update(foundCourse);
            await _context.SaveChangesAsync();
            return new CourseModel() { CourseId = foundCourse.CourseId, Name = foundCourse.Name, Description = foundCourse.Description, CategoryId = foundCourse.Category?.CategoryId ?? 0, CategoryName = foundCourse.Category?.Name ?? "", Items = foundCourse.Items.Select(i => (ItemModel)GetItemModel(i)).ToList(), MaxPoints = foundCourse.MaxPoints };
        }

        public async Task<bool> DeleteCourse(int id)
        {
            Course course = await _context.Course.Include(c => c.Items).FirstOrDefaultAsync(i => i.CourseId == id);
            if (course == null)
            {
                return false;
            }
            RemoveItems(course.Items);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ItemModel> GetItem(int id)
        {
            Item item = await _context.Item.Include(i => i.Course).Include(i => i.Questions).FirstOrDefaultAsync(c => c.ItemId == id);
            if (item == null)
            {
                return null;
            }

            item.PrevItemId = (await _context.Item.Include(i => i.Course).OrderBy(i => i.ItemId).Where(i => i.Course.CourseId == item.Course.CourseId).LastOrDefaultAsync(i => i.ItemId < item.ItemId))?.ItemId ?? 0;
            item.NextItemId = await GetNextItem(item);

            return new ItemModel() { ItemId = item.ItemId, Name = item.Name, Description = item.Description, Content = item.Content, ContentPoints = item.ContentPoints, QuestionsPoints = item.Questions.Sum(q => q.Points), QuestionsCount = item.Questions.Count, UpdatedDate = item.UpdatedDate, PrevItemId = item.PrevItemId, NextItemId = item.NextItemId };
        }

        public async Task<ItemModel> EditItem(ItemModel item)
        {
            Item foundItem = await _context.Item.Include(i => i.Questions).FirstOrDefaultAsync(c => c.ItemId == item.ItemId);
            if (foundItem == null)
            {
                return null;
            }
            foundItem.UpdatedDate = DateTime.UtcNow;
            foundItem.Name = item.Name;
            foundItem.Description = item.Description;
            foundItem.ContentPoints = item.ContentPoints;
            foundItem.Content = item.Content;
            _context.Item.Update(foundItem);

            await _context.SaveChangesAsync();
            item.UpdatedDate = foundItem.UpdatedDate;
            item.QuestionsCount = foundItem.Questions.Count;
            return item;
        }

        public async Task<List<QuestionModel>> GetQuestions(int id)
        {
            Item item = await _context.Item.Include(c => c.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(c => c.ItemId == id);
            if (item == null)
            {
                return null;
            }
            return item.Questions.Select(q => new QuestionModel() { QuestionId = q.QuestionId, Description = q.Description, Points = q.Points, Open = q.Open, Answers = q.Answers, ItemId = item.ItemId }).ToList();
        }

        public async Task<List<QuestionModel>> AddQuestions(int id, List<QuestionModel> questions)
        {
            Item item = await _context.Item.Include(c => c.Questions).FirstOrDefaultAsync(c => c.ItemId == id);
            if (item == null)
            {
                return null;
            }
            questions = questions.Select(q => { q.QuestionId = 0; return q; }).ToList();
            item.UpdatedDate = DateTime.UtcNow;
            item.Questions.AddRange(questions.Select(q => new Question() { Description = q.Description, Points = q.Points, Open = q.Open, Answers = q.Answers }));
            _context.Item.Update(item);
            await _context.SaveChangesAsync();
            return item.Questions.Select(q => new QuestionModel() { QuestionId = q.QuestionId, Description = q.Description, Points = q.Points, Open = q.Open, Answers = q.Answers, ItemId = item.ItemId }).ToList();
        }

        public async Task<List<QuestionModel>> EditQuestions(int id, List<QuestionModel> questions)
        {
            questions = questions.Select(q => { if (q.IsNew) { q.QuestionId = 0; } return q; }).ToList();
            Item item = await _context.Item.FirstOrDefaultAsync(i => i.ItemId == id);
            if (item == null)
            {
                return null;
            }
            List<Question> foundQuestions = await _context.Question.Include(q => q.Answers).Where(q => q.Item.ItemId == id).ToListAsync();
            if (foundQuestions == null)
            {
                return null;
            }
            item.UpdatedDate = DateTime.UtcNow;

            List<Question> questionsToRemove = foundQuestions.Where(fq => !questions.Any(q => fq.QuestionId == q.QuestionId)).ToList();
            if (questionsToRemove.Count > 0)
            {
                foundQuestions.RemoveAll(fq => !questions.Any(q => fq.QuestionId == q.QuestionId));
                List<int> ids = questionsToRemove.Select(q => q.QuestionId).ToList();
                IEnumerable<SentQuestion> sentQuestions = await _context.SentQuestion.Include(sq => sq.Question).Include(sq => sq.Answers).Where(sq => ids.Contains(sq.Question.QuestionId)).ToListAsync();
                _context.Answer.RemoveRange(questionsToRemove.SelectMany(q => q.Answers).Concat(sentQuestions.SelectMany(sq => sq.Answers)));
                _context.SentQuestion.RemoveRange(sentQuestions);
                _context.Question.RemoveRange(questionsToRemove);
            }

            List<Question> newQuestions = new();
            foreach (QuestionModel question in questions)
            {
                Question foundQuestion = foundQuestions.FirstOrDefault(fq => fq.QuestionId == question.QuestionId);

                if (foundQuestion != null && foundQuestion.QuestionId != 0)
                {
                    bool match(Answer fa) => !question.Answers.Any(a => a.AnswerId == fa.AnswerId);
                    _context.Answer.RemoveRange(foundQuestion.Answers.Where(match));

                    foreach (Answer answer in question.Answers)
                    {
                        Answer foundAnswer = foundQuestion.Answers.FirstOrDefault(a => a.AnswerId == answer.AnswerId);
                        if (foundAnswer == null)
                        {
                            foundQuestion.Answers.Add(answer);
                        }
                        else
                        {
                            foundAnswer.Correct = answer.Correct;
                            foundAnswer.Name = answer.Name;
                        }
                    }
                }

                if (foundQuestion == null || foundQuestion.QuestionId == 0)
                {
                    newQuestions.Add(new Question() { Description = question.Description, Points = question.Points, Open = question.Open, Answers = question.Answers, Item = item });
                }
                else
                {
                    foundQuestion.Description = question.Description;
                    foundQuestion.Open = question.Open;
                    foundQuestion.Points = question.Points;
                }
            }
            if (newQuestions.Count > 0)
            {
                await _context.Question.AddRangeAsync(newQuestions);
            }
            if (foundQuestions.Count > 0)
            {
                _context.Question.UpdateRange(foundQuestions);
            }
            await _context.SaveChangesAsync();
            return foundQuestions.Concat(newQuestions).Select(q => new QuestionModel() { QuestionId = q.QuestionId, Description = q.Description, Points = q.Points, Open = q.Open, Answers = q.Answers, ItemId = item.ItemId }).ToList(); ;
        }

        public async Task<SentItem> AddSentItem(SentItem item, int userId)
        {
            SentItem foundItem = await _context.SentItem.Include(si => si.Item).Include(si => si.User).FirstOrDefaultAsync(si => si.Item.ItemId == item.Item.ItemId && si.User.UserId == userId);
            if (foundItem != null)
            {
                return foundItem;
            }
            item.SentDate = DateTime.UtcNow;
            item.Item = await _context.Item.FirstOrDefaultAsync(i => i.ItemId == item.Item.ItemId);
            item.User = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (item.Item == null || item.User == null)
            {
                return null;
            }
            await _context.SentItem.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<List<SentQuestionModel>> GetSentQuestions(int id, int userId)
        {
            SentItem item = await _context.SentItem.Include(si => si.User).Include(si => si.Item).Include(si => si.SentQuestions).ThenInclude(sq => sq.Answers).Include(si => si.SentQuestions).ThenInclude(sq => sq.Question).ThenInclude(q => q.Answers).FirstOrDefaultAsync(si => si.Item.ItemId == id && si.User.UserId == userId);
            if (item == null)
            {
                return null;
            }
            return item.SentQuestions.Select(sq => new SentQuestionModel()
            {
                QuestionId = sq.Question.QuestionId,
                ItemId = item.Item.ItemId,
                Open = sq.Question.Open,
                Answers = sq.Answers,
                Points = GetAnswersPoints(sq)
            }).ToList();
        }

        public async Task<List<SentQuestionModel>> AddSentQuestions(List<SentQuestionModel> sentQuestions, int userId)
        {
            SentItem item = await _context.SentItem.Include(si => si.SentQuestions).ThenInclude(sq => sq.Question).Include(si => si.Item).ThenInclude(i => i.Course).Include(si => si.User).FirstOrDefaultAsync(i => i.Item.ItemId == sentQuestions[0].ItemId && i.User.UserId == userId);
            if (item == null)
            {
                return null;
            }
            IEnumerable<int> sentQuestionIds = sentQuestions.Select(sq => sq.QuestionId);
            List<Question> questions = await _context.Question.Where(q => sentQuestionIds.Contains(q.QuestionId)).ToListAsync();
            if (questions.Count == 0)
            {
                return null;
            }

            sentQuestions[0].NextItemId = await GetNextItem(item.Item);

            List<SentQuestion> newQuestions = new();
            for (int i = 0; i < sentQuestions.Count; i++)
            {
                if (!item.SentQuestions.Any(q => q.Question.QuestionId == sentQuestions[i].QuestionId))
                {
                    newQuestions.Add(new()
                    {
                        Question = questions.FirstOrDefault(q => q.QuestionId == sentQuestions[i].QuestionId),
                        Answers = sentQuestions[i].Answers.Select(a => { a.AnswerId = 0; return a; }).ToList()
                    });
                }
            }
            item.SentQuestions.AddRange(newQuestions);
            await _context.SaveChangesAsync();

            for (int i = 0; i < sentQuestions.Count; i++)
            {
                if (!item.SentQuestions.Any(q => q.Question.QuestionId == sentQuestions[i].QuestionId))
                {
                    sentQuestions[i].Answers = newQuestions[i].Answers;
                }
            }
            return sentQuestions;
        }

        public async Task<List<SentItem>> GetCourseStats(int courseId)
        {
            return await _context.SentItem.Include(si => si.SentQuestions).Include(si => si.User).Include(si => si.Item).ThenInclude(i => i.Course).ThenInclude(c => c.Category).Where(si => si.Item.Course.CourseId == courseId).OrderByDescending(si => si.SentDate).ToListAsync();
        }

        private async Task<int> GetCoursePoints(Course course, int userId)
        {
            int points = 0;

            List<int> itemIds = course.Items.Select(i => i.ItemId).ToList();
            List<SentItem> items = await _context.SentItem.Include(si => si.Item).Include(si => si.User).Include(si => si.SentQuestions).ThenInclude(sq => sq.Question).Include(si => si.SentQuestions).ThenInclude(sq => sq.Answers).Where(si => si.User.UserId == userId && itemIds.Contains(si.Item.ItemId)).ToListAsync();

            foreach (Item item in course.Items)
            {
                SentItem sentItem = items.FirstOrDefault(i => i.Item.ItemId == item.ItemId);
                if (sentItem != null)
                {
                    points += item.ContentPoints;
                }
                foreach (Question question in item.Questions)
                {
                    if (sentItem != null)
                    {
                        SentQuestion sentQuestion = sentItem.SentQuestions.FirstOrDefault(q => q.Question.QuestionId == question.QuestionId);
                        points += GetAnswersPoints(sentQuestion);
                    }
                }
            }

            return points;
        }

        private static int GetAnswersPoints(SentQuestion question)
        {
            int points = 0;

            if (question != null)
            {
                if (question.Question.Open)
                {
                    if (question.Question.Answers.Any(a => a.Name.ToLower() == question.Answers[0].Name.ToLower()))
                    {
                        points += question.Question.Points;
                    }
                }
                else if (question.Answers.Count == question.Question.Answers.Where(a => a.Correct).Count() && !question.Question.Answers.Where(a => question.Answers.Any(sa => sa.Name == a.Name)).Any(a => !a.Correct))
                {
                    points += question.Question.Points;
                }
            }
            return points;
        }

        private async Task<int> GetNextItem(Item item)
        {
            int id = (await _context.Item.Include(i => i.Course).Where(i => i.Course.CourseId == item.Course.CourseId).FirstOrDefaultAsync(i => i.ItemId > item.ItemId))?.ItemId ?? 0;
            return id;
        }

        private static object GetItemModel(object item)
        {
            if (item is Item i)
            {
                return new ItemModel() { ItemId = i.ItemId, Name = i.Name, Description = i.Description, Content = i.Content, ContentPoints = i.ContentPoints, QuestionsPoints = i.Questions != null ? i.Questions.Sum(q => q.Points) : 0 };
            }
            if (item is ItemModel j)
            {
                return new Item() { ItemId = j.ItemId, Name = j.Name, Description = j.Description, Content = j.Content, ContentPoints = j.ContentPoints };
            }
            return null;
        }

        private async void RemoveItems(List<Item> items)
        {
            IEnumerable<int> ids = items.Select(i => i.ItemId);
            List<SentItem> sentItems = await _context.SentItem.Include(i => i.Item).Where(si => ids.Contains(si.Item.ItemId)).ToListAsync();
            IEnumerable<int> sentItemIds = sentItems.Select(si => si.SentItemId);
            List<SentQuestion> sentQuestions = await _context.SentQuestion.Include(sq => sq.SentItem).Include(sq => sq.Answers).Where(sq => sentItemIds.Contains(sq.SentItem.SentItemId)).ToListAsync();
            List<Question> questions = await _context.Question.Include(q => q.Item).Include(q => q.Answers).Where(q => ids.Contains(q.Item.ItemId)).ToListAsync();
            IEnumerable<Answer> answers = questions.SelectMany(q => q.Answers).Concat(sentQuestions.SelectMany(sq => sq.Answers));

            _context.Answer.RemoveRange(answers);
            _context.SentQuestion.RemoveRange(sentQuestions);
            _context.SentItem.RemoveRange(sentItems);
            _context.Question.RemoveRange(questions);
            _context.Item.RemoveRange(items);
        }
    }

    public interface IApiService
    {
        Task<AuthenticateResponse> Login(AuthenticateRequest request);
        Task<List<Category>> GetCategories();
        Task<List<CourseModel>> GetCourses(int categoryId, int userId);
        Task<CourseModel> GetCourse(int id, int userId = 0);
        Task<CourseModel> AddCourse(CourseModel course);
        Task<CourseModel> EditCourse(CourseModel course);
        Task<bool> DeleteCourse(int id);
        Task<ItemModel> GetItem(int id);
        Task<ItemModel> EditItem(ItemModel item);
        Task<List<QuestionModel>> GetQuestions(int id);
        Task<List<QuestionModel>> AddQuestions(int id, List<QuestionModel> questions);
        Task<List<QuestionModel>> EditQuestions(int id, List<QuestionModel> questions);
        Task<SentItem> AddSentItem(SentItem item, int userId);
        Task<List<SentQuestionModel>> GetSentQuestions(int id, int userId);
        Task<List<SentQuestionModel>> AddSentQuestions(List<SentQuestionModel> sentQuestions, int userId);
        Task<List<SentItem>> GetCourseStats(int courseId);
    }
}
