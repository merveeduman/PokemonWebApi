﻿namespace PokemonReviewApp.Models
{
    public class PokemonCategory : BaseEntity
    {
        public int PokemonId { get; set; }
        public int CategoryId { get; set; }
        public Pokemon Pokemon { get; set; }
        public Category Category { get; set; }

    }
}
