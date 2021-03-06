﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllEnum;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.RocketAPI.Logic
{
    public class Inventory
    {
        private readonly Client _client;

        public Inventory(Client client)
        {
            _client = client;
        }
        
        public async Task<IEnumerable<PokemonData>> GetPokemons()
        {
            var inventory = await _client.GetInventory();
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);
        }
        
        public async Task<IEnumerable<PokemonFamily>> GetPokemonFamilies()
        {
            var inventory = await _client.GetInventory();
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            var templates = await _client.GetItemTemplates();
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        } 


        public async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer(int pokemonOfEachToKeep, bool keepPokemonsThatCanEvolve = false)
        {
            var myPokemon = await GetPokemons();

            var pokemonList = myPokemon.Where(p => p.DeployedFortId == 0).ToList(); //Don't evolve pokemon in gyms
            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 2).ToList();

                var myPokemonSettings = await GetPokemonSettings();
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = await GetPokemonFamilies();
                var pokemonFamilies = myPokemonFamilies.ToArray();

                foreach (var pokemon in pokemonsThatCanBeTransfered)
                {
                    var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.Key);
                    var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);
                    if (settings.CandyToEvolve == 0)
                        continue;

                    var amountToSkip = (familyCandy.Candy + settings.CandyToEvolve - 1)/settings.CandyToEvolve + 2;

                    results.AddRange(pokemonList.Where(x => x.PokemonId == pokemon.Key && x.Favorite == 0)
                        .OrderByDescending(x => x.Cp)
                        .ThenBy(n => n.StaminaMax)
                        .Skip(amountToSkip)
                        .ToList());

                }

                return results;
            }
            //var pokemonOfEachToKeep = 2;
            var whitelist = new List<PokemonId>() { PokemonId.Pikachu,PokemonId.Mewtwo}; //whitelist
            return pokemonList.Where(p => !whitelist.Contains(p.PokemonId))
                .GroupBy(p => p.PokemonId)
                .Where(x => x.Count() > 1)
                .SelectMany(p => p.Where(x => x.Favorite == 0).OrderByDescending(x => x.Cp).ThenBy(n => n.StaminaMax).Skip(pokemonOfEachToKeep).ToList());
        }


        public async Task<IEnumerable<PokemonData>> GetPokemonToEvolve()
        {
            var myPokemons = await GetPokemons();
            var pokemons = myPokemons.Where(p => p.DeployedFortId == 0).ToList(); //Don't evolve pokemon in gyms

            var myPokemonSettings = await GetPokemonSettings();
            var pokemonSettings = myPokemonSettings.ToList();

            var myPokemonFamilies = await GetPokemonFamilies();
            var pokemonFamilies = myPokemonFamilies.ToArray();
            
            var pokemonToEvolve = new List<PokemonData>();
            foreach (var pokemon in pokemons)
            {
                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                //Don't evolve if we can't evolve it
                if (settings.EvolutionIds.Count == 0)
                    continue;

                var pokemonCandyNeededAlready = pokemonToEvolve.Count(p => pokemonSettings.Single(x => x.PokemonId == p.PokemonId).FamilyId == settings.FamilyId) * settings.CandyToEvolve;
                if (familyCandy.Candy - pokemonCandyNeededAlready > settings.CandyToEvolve)
                    pokemonToEvolve.Add(pokemon);
            }

            return pokemonToEvolve;
        }

       

        public async Task<IEnumerable<PlayerStats>> GetPlayerStats()
         {
             var inventory = await _client.GetInventory();
             return inventory.InventoryDelta.InventoryItems
                 .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
         }


    public async Task<IEnumerable<Item>> GetItems()
        {
            var inventory = await _client.GetInventory();
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public async Task<int> GetItemAmountByType(MiscEnums.Item type)
        {
            var pokeballs = await GetItems();
            return pokeballs.FirstOrDefault(i => (MiscEnums.Item)i.Item_ == type)?.Count ?? 0;
        }

        public async Task<IEnumerable<Item>> GetItemsToRecycle(ISettings settings)
        {
            var myItems = await GetItems();

            return myItems
                .Where(x => settings.itemRecycleFilter.Any(f => f.Key == ((ItemId)x.Item_) && x.Count > f.Value))
                .Select(x => new Item { Item_ = x.Item_, Count = x.Count - settings.itemRecycleFilter.Single(f => f.Key == (AllEnum.ItemId)x.Item_).Value, Unseen = x.Unseen });
        }
    }
}
