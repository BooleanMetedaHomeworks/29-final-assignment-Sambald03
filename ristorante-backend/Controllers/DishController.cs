using Microsoft.AspNetCore.Mvc;
using ristorante_backend.Models;
using ristorante_backend.Repositories;

namespace ristorante_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DishController : Controller
    {
        private DishRepository _dishRepository { get; set; }

        public DishController(DishRepository dishRepository)
        {
            this._dishRepository = dishRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? name)
        {
            try
            {
                if (name == null)
                {
                    return Ok(await this._dishRepository.GetAllDishes());
                }

                return Ok(await this._dishRepository.GetDishesByName(name));
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
                Dish dish = await this._dishRepository.GetDishById(id);

                if (dish == null)
                {
                    return NotFound();
                }

                return Ok(dish);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Dish newDish)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                return Ok(await this._dishRepository.InsertDish(newDish));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Dish updatedDish)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                int affectedRows = await this._dishRepository.UpdateDish(id, updatedDish);

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
                int affectedRows = await this._dishRepository.DeleteDish(id);

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
