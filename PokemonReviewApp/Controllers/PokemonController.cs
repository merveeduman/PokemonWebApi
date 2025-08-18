using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository,
            IReviewRepository reviewRepository,
            IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetActivePokemons());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(rating);
        }

        [HttpPost]
[ProducesResponseType(204)]
[ProducesResponseType(400)]
public IActionResult CreatePokemon(
    [FromQuery] int ownerId,
    [FromQuery] int catId,
    [FromQuery] int foodId, // Yeni eklendi
    [FromBody] PokemonDto pokemonCreate)
{
    if (pokemonCreate == null)
        return BadRequest(ModelState);

    var pokemons = _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate);

    if (pokemons != null)


    {
        ModelState.AddModelError("", "Pokemon already exists");
        return StatusCode(422, ModelState);
    }

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

    // Yeni: Food bağlantısı da yapılacaksa repository methodu güncellenmeli
    if (!_pokemonRepository.CreatePokemon(ownerId, catId, foodId, pokemonMap))
    {
        ModelState.AddModelError("", "Something went wrong while saving");
        return StatusCode(500, ModelState);
    }

    return Ok("Successfully created");
}


        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId,
            [FromQuery] int ownerId, [FromQuery] int catId,
            [FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (pokeId != updatedPokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /* [HttpDelete("{pokeId}")]
         [ProducesResponseType(400)]
         [ProducesResponseType(204)]
         [ProducesResponseType(404)]
         public IActionResult DeletePokemon(int pokeId)
         {
             if (!_pokemonRepository.PokemonExists(pokeId))
             {
                 return NotFound();
             }

             var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);
             var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

             if (!ModelState.IsValid)
                 return BadRequest(ModelState);

             if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
             {
                 ModelState.AddModelError("", "Something went wrong when deleting reviews");
             }

             if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
             {
                 ModelState.AddModelError("", "Something went wrong deleting owner");
             }

             return NoContent();
         }*/
        [HttpDelete("soft/{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult SoftDeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_pokemonRepository.SoftDeletePokemon(pokeId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        [HttpGet("{pokemonId}/owners")]
        public IActionResult GetOwnersOfPokemon(int pokemonId)
        {
            // Repository katmanından, verilen 'pokemonId'ye ait olan Owner listesini çekiyoruz.
            var owners = _pokemonRepository.GetOwnersOfAPokemon(pokemonId);

            // Çekilen Owner verilerini DTO'ya (OwnerDto) dönüştürüyoruz.
            // Amaç: Response'da yalnızca ihtiyaç duyulan alanları dönmek ve veri sızıntısını engellemek.
            var mapped = _mapper.Map<List<OwnerDto>>(owners);
            return Ok(mapped);
        }

        [HttpGet("{pokemonId}/categories")]
        public IActionResult GetCategoriesOfPokemon(int pokemonId)
        {
            
            var categories = _pokemonRepository.GetCategoriesOfPokemon(pokemonId);

            
            var mapped = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(mapped);
        }

        [HttpGet("{pokemonId}/foods")]
        public IActionResult GetFoodsOfPokemon(int pokemonId)
        {

            var foods = _pokemonRepository.GetFoodsOfPokemon(pokemonId);


            var mapped = _mapper.Map<List<FoodDto>>(foods);
            return Ok(mapped);
        }


    }
}