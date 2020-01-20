using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QoutesAPI.Data;
using QoutesAPI.Models;

namespace QoutesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QoutesController : ControllerBase
    {
        private QoutesDbContext _qoutesDbContext;
        public QoutesController(QoutesDbContext qoutesDbContext)
        {
            _qoutesDbContext = qoutesDbContext;
        }

        // GET: api/Qoutes
        [HttpGet]
        [ResponseCache(Duration = 60)]
        [AllowAnonymous]
        public IActionResult Get(string sort)
        {
            IQueryable<Qoute> qoutes;
            switch (sort)
            {
                case "desc":
                    qoutes = _qoutesDbContext.Qoutes.OrderByDescending(q => q.CreatedAt);
                    break;
                case "asc":
                    qoutes = _qoutesDbContext.Qoutes.OrderBy(q => q.CreatedAt);
                    break;
                default:
                    qoutes = _qoutesDbContext.Qoutes;
                    break;
            }
            return Ok(qoutes);
        }

        // GET: api/Qoutes/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            Qoute qoute = _qoutesDbContext.Qoutes.Find(id);
            if (qoute == null)
            {
                return NotFound();
            }
            return Ok();
        }

        // POST: api/Qoutes
        [HttpPost]
        public IActionResult Post([FromBody] Qoute qoute)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            qoute.UserId = userId;
            _qoutesDbContext.Qoutes.Add(qoute);
            _qoutesDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT: api/Qoutes/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Qoute qoute)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Qoute entity = _qoutesDbContext.Qoutes.Find(id);
            if (entity == null)
            {
                return NotFound();
            }
            if (userId != entity.UserId)
            {
                return BadRequest(); 
            }
            entity.Author = qoute.Author;
            entity.Description = qoute.Description;
            entity.Title = qoute.Title;
            entity.Type = qoute.Type;
            entity.CreatedAt = qoute.CreatedAt;
            entity.UserId = userId;
            _qoutesDbContext.SaveChanges();
            return Ok(entity);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Qoute entity = _qoutesDbContext.Qoutes.Find(id);
            if (entity == null)
            {
                return NotFound();
            }
            if (userId != entity.UserId)
            {
                return BadRequest();
            }
            _qoutesDbContext.Qoutes.Remove(entity);
            _qoutesDbContext.SaveChanges();
            return NoContent();
        }

        [HttpGet("[action]")]
        
        public IActionResult PagingQoute(int pageNumber = 1, int pageSize = 3)
        {
            var qoutes = _qoutesDbContext.Qoutes;
            return Ok(qoutes.Skip((pageNumber - 1) * pageSize).Take(pageSize));
        }

        [HttpGet("[action]")]
        public IActionResult SearchQouteTypes(string type)
        {
            var qoutes = _qoutesDbContext.Qoutes.Where(q => q.Type.StartsWith(type));
            return Ok(qoutes); 
        }

        [HttpGet("[action]")]
        public IActionResult MyQoutes()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var qoutes = _qoutesDbContext.Qoutes.Where(q => q.UserId == userId); 

            return Ok(qoutes); 
        }
    }
}
