using Microsoft.AspNetCore.Mvc;
using ristorante_backend.Models;
using ristorante_backend.Repositories;

namespace ristorante_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private CategoryRepository _categoryRepository { get; set; }

        public CategoryController(CategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? name)
        {
            try
            {
                if (name == null)
                {
                    return Ok(await this._categoryRepository.GetAllCategories());
                }

                return Ok(await this._categoryRepository.GetCategoriesByName(name));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                Category category = await this._categoryRepository.GetCategory(id);

                if (category == null)
                {
                    return NotFound();
                }

                return Ok(category);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category newCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                return Ok(await this._categoryRepository.CreateCategory(newCategory));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                int affectedRows = await this._categoryRepository.UpdateCategory(id, updatedCategory);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return Ok(affectedRows);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int affectedRows = await this._categoryRepository.DeleteCategory(id);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return Ok(affectedRows);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
