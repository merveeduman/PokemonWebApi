using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodTypeController : ControllerBase
    {
        private readonly IFoodTypeRepository _foodTypeRepository;
        private readonly IMapper _mapper;

        public FoodTypeController(IFoodTypeRepository foodTypeRepository, IMapper mapper)
        {
            _foodTypeRepository = foodTypeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FoodTypeDto>))]
        public IActionResult GetFoodTypes()
        {
            var foodTypes = _foodTypeRepository.GetFoodTypes();
            var foodTypeDtos = _mapper.Map<List<FoodTypeDto>>(foodTypes);
            return Ok(foodTypeDtos);
        }

        [HttpGet("{foodTypeId}")]
        [ProducesResponseType(200, Type = typeof(FoodTypeDto))]
        [ProducesResponseType(404)]
        public IActionResult GetFoodType(int foodTypeId)
        {
            if (!_foodTypeRepository.FoodTypeExists(foodTypeId))
                return NotFound();

            var foodType = _foodTypeRepository.GetFoodType(foodTypeId);
            var foodTypeDto = _mapper.Map<FoodTypeDto>(foodType);

            return Ok(foodTypeDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        
        public IActionResult CreateFoodType([FromBody] FoodTypeDto foodTypeCreate)
        {
            if (foodTypeCreate == null)
                return BadRequest(ModelState);

            var foodTypeMap = _mapper.Map<FoodType>(foodTypeCreate);

            if (!_foodTypeRepository.CreateFoodType(foodTypeMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return StatusCode(201, true);
        }

        [HttpPut("{foodTypeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFoodType(int foodTypeId, [FromBody] FoodTypeDto updatedFoodType)
        {
            if (updatedFoodType == null)
                return BadRequest(ModelState);

            if (!_foodTypeRepository.FoodTypeExists(foodTypeId))
                return NotFound();

            var foodTypeEntity = _foodTypeRepository.GetFoodType(foodTypeId);

            // DTO'daki değerleri Entity'e aktar
            _mapper.Map(updatedFoodType, foodTypeEntity);

            if (!_foodTypeRepository.UpdateFoodType(foodTypeEntity))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return NoContent(); // 204: Başarılı ama içerik yok
        }


        [HttpDelete("soft/{foodTypeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult SoftDeleteFoodType(int foodTypeId)
        {
            if (!_foodTypeRepository.FoodTypeExists(foodTypeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_foodTypeRepository.SoftDeleteFoodType(foodTypeId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
