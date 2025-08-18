using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonFoodController : ControllerBase
    {
        private readonly IPokemonFoodRepository _pokemonFoodRepository;
        private readonly IMapper _mapper;

        public PokemonFoodController(IPokemonFoodRepository pokemonFoodRepository, IMapper mapper)
        {
            _pokemonFoodRepository = pokemonFoodRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetPokemonFoods()
        {
            var pokemonFoods = _pokemonFoodRepository.GetPokemonFoods();
            var pokemonFoodDtos = _mapper.Map<List<PokemonFoodDto>>(pokemonFoods);
            return Ok(pokemonFoodDtos);
        }

        [HttpGet("{pokemonId}/{foodId}")]
        public IActionResult GetPokemonFood(int pokemonId, int foodId)
        {
            if (!_pokemonFoodRepository.PokemonFoodExists(pokemonId, foodId))
                return NotFound();

            var pokemonFood = _pokemonFoodRepository.GetPokemonFood(pokemonId, foodId);
            var pokemonFoodDto = _mapper.Map<PokemonFoodDto>(pokemonFood);
            return Ok(pokemonFoodDto);
        }

        [HttpPost]
        public IActionResult CreatePokemonFood([FromBody] PokemonFoodDtoPost pokemonFoodCreate)
        {
            if (pokemonFoodCreate == null)
                return BadRequest(ModelState);

            var pokemonFood = _mapper.Map<PokemonFood>(pokemonFoodCreate);

            // FK kontrolü burada da olabilir veya repository'de yapılabilir.
            bool pokemonExists = _pokemonFoodRepository.PokemonExists(pokemonFood.PokemonId);
            bool foodExists = _pokemonFoodRepository.FoodExists(pokemonFood.FoodId);

            if (!pokemonExists || !foodExists)
                return NotFound("PokemonId veya FoodId bulunamadı.");

            // CreatePokemonFood true dönerse başarılı
            bool created = _pokemonFoodRepository.CreatePokemonFood(pokemonFood);

            if (!created)
            {
                // Burada duplicate kayıt ise Conflict, yoksa 500 olabilir. 
                // Ama biz repository'de 409 ve 404 ayrımı yapamadığımız için genelde Conflict dönebiliriz.
                return Conflict("Bu PokemonFood kaydı zaten mevcut.");
            }

            return Ok("Başarıyla eklendi.");
        }




        [HttpPut("{pokemonId}/{foodId}")]
        public IActionResult UpdatePokemonFood(int pokemonId, int foodId, [FromBody] PokemonFoodDto updatedPokemonFood)
        {
            if (updatedPokemonFood == null || pokemonId != updatedPokemonFood.PokemonId || foodId != updatedPokemonFood.FoodId)
                return BadRequest(ModelState);

            if (!_pokemonFoodRepository.PokemonFoodExists(pokemonId, foodId))
                return NotFound();

            var pokemonFoodEntity = _pokemonFoodRepository.GetPokemonFood(pokemonId, foodId);
            _mapper.Map(updatedPokemonFood, pokemonFoodEntity);

            if (!_pokemonFoodRepository.UpdatePokemonFood(pokemonFoodEntity))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{pokemonId}/{foodId}")]
        public IActionResult DeletePokemonFood(int pokemonId, int foodId)
        {
            if (!_pokemonFoodRepository.PokemonFoodExists(pokemonId, foodId))
                return NotFound();

            var pokemonFood = _pokemonFoodRepository.GetPokemonFood(pokemonId, foodId);

            if (!_pokemonFoodRepository.DeletePokemonFood(pokemonFood))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("soft/{pokemonId}/{foodId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult SoftDeletePokemonFood(int pokemonId, int foodId)
        {
            // İlgili PokemonFood kaydının var olup olmadığını kontrol et
            if (!_pokemonFoodRepository.PokemonFoodExists(pokemonId, foodId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Soft delete işlemi başarılı mı diye kontrol et
            if (!_pokemonFoodRepository.SoftDeletePokemonFood(pokemonId, foodId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
