using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : Controller
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;

        public FoodController(IFoodRepository foodRepository, IMapper mapper)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodDto>))]
        public IActionResult GetFoods()
        {
            var foods = _mapper.Map<List<FoodDto>>(_foodRepository.GetActiveFoods());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(foods);
        }

        [HttpGet("{foodId}")]
        [ProducesResponseType(200, Type = typeof(FoodDto))]
        [ProducesResponseType(400)]
        public IActionResult GetFood(int foodId)
        {
            if (!_foodRepository.FoodExists(foodId))
                return NotFound();

            var food = _mapper.Map<FoodDto>(_foodRepository.GetFood(foodId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(food);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateFood([FromBody] FoodDto foodCreate)
        {
            if (foodCreate == null)
                return BadRequest(ModelState);

            var existingFood = _foodRepository.GetFoodByName(foodCreate.Name);
            if (existingFood != null)
            {
                ModelState.AddModelError("", "Food already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foodMap = _mapper.Map<Food>(foodCreate);
            

            if (!_foodRepository.CreateFood(foodMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{foodId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFood(int foodId, [FromBody] FoodDto updatedFood)
        {
            if (updatedFood == null)
                return BadRequest(ModelState);

            if (foodId != updatedFood.Id)
                return BadRequest(ModelState);

            if (!_foodRepository.FoodExists(foodId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var foodMap = _mapper.Map<Food>(updatedFood);

            if (!_foodRepository.UpdateFood(foodMap))
            {
                ModelState.AddModelError("", "Something went wrong updating food");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("soft/{foodId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult SoftDeleteFood(int foodId)
        {
            if (!_foodRepository.FoodExists(foodId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_foodRepository.SoftDeleteFood(foodId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpGet("{foodId}/pokemons")]
        public IActionResult GetPokemonsOfFood(int foodId)
        {
            var pokemons = _foodRepository.GetPokemonsOfFood(foodId);

            var mapped = _mapper.Map<List<PokemonDto>>(pokemons);

            return Ok(mapped);
        }

        [HttpGet("{foodId}/foodtype")]
        public IActionResult GetFoodTypeOfFood(int foodId)
        {
            var foodType = _foodRepository.GetFoodTypeOfFood(foodId);

            var mapped = _mapper.Map<FoodTypeDto>(foodType);

            return Ok(mapped);
        }
    }
}
