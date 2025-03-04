using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ristorante_backend.Models;
using ristorante_backend.Repositories;

namespace ristorante_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : Controller
    {
        private MenuRepository _menuRepository { get; set; }

        public MenuController(MenuRepository menuRepository)
        {
            this._menuRepository = menuRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? name)
        {
            try
            {
                if (name == null)
                {
                    return Ok(await this._menuRepository.GetAllMenus());
                }

                return Ok(await this._menuRepository.GetMenusByName(name));
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
                Menu menu = await this._menuRepository.GetMenu(id);

                if (menu == null)
                {
                    return NotFound();
                }

                return Ok(menu);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Menu newMenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                return Ok(await this._menuRepository.CreateMenu(newMenu));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Menu updatedMenu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                int affectedRows = await this._menuRepository.UpdateMenu(id, updatedMenu);

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
                int affectedRows = await this._menuRepository.DeleteMenu(id);

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
