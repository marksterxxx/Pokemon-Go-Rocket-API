﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AllEnum;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;

namespace PokemonGo.RocketAPI.Logic
{
    public class Logic
    {
        private readonly Client _client;
        private readonly ISettings _clientSettings;
        private readonly Inventory _inventory;

        public Logic(ISettings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
            _inventory = new Inventory(_client);
        }

        public async Task Execute()
        {
            Logger.Write($"Starting Execute on login server: {_clientSettings.AuthType}", LogLevel.Info);

            while (true)
            {
                try
                {
                    if (_clientSettings.AuthType == AuthType.Ptc)
                        await _client.DoPtcLogin(_clientSettings.PtcUsername, _clientSettings.PtcPassword);
                    else if (_clientSettings.AuthType == AuthType.Google)
                        await _client.DoGoogleLogin();

                    await PostLoginExecute();
                }
                catch (AccessTokenExpiredException)
                {
                    Logger.Write($"Access token expired", LogLevel.Info);
                }
                await Task.Delay(10000);
            }
        }

        public async Task PostLoginExecute()
        {
            while (true)
            {
                try
                {
                    await _client.SetServer();
                  
                    var inventory = await _client.GetInventory();
                    var playerStats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData).FirstOrDefault(i => i.PlayerStats != null);
                    //   await EvolveAllPokemonWithEnoughCandy();
<<<<<<< HEAD

                    await myRoutine();


=======
                    //
                    var profile = await _client.GetProfile();
                    if (profile.Profile != null)
                    {
                        Logger.Write($"Playing Profile: {profile.Profile.Username},  {profile.Profile.Team} Team, {profile.Profile.Currency}", LogLevel.Info);
                    }
                    //
                    await TransferDuplicatePokemon(true);
                    await RecycleItems();
<<<<<<< HEAD
                    await RepeatAction(5, async () => await ExecuteFarmingPokestopsAndPokemons(_client));

                    /*
                * Example calls below
                *
                var profile = await _client.GetProfile();
                var settings = await _client.GetSettings();
                var mapObjects = await _client.GetMapObjects();
                var inventory = await _client.GetInventory();
                var pokemons = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon).Where(p => p != null && p?.PokemonId > 0);
                */



=======
                    await ExecuteFarmingPokestopsAndPokemons();

                    /*
            * Example calls below
            *
            var profile = await _client.GetProfile();
            var settings = await _client.GetSettings();
            var mapObjects = await _client.GetMapObjects();
            var inventory = await _client.GetInventory();
            var pokemons = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon).Where(p => p != null && p?.PokemonId > 0);
            */
>>>>>>> origin/master
                }
                catch (AccessTokenExpiredException)
                {
                    throw;
<<<<<<< HEAD
=======
>>>>>>> refs/remotes/FeroxRev/master
>>>>>>> origin/master
                }
                    await Task.Delay(10000);
            }
        }
        public async Task myRoutine()
        {
            Logger.Write($"Doing routine...");
            await DisplayPlayerLevelInTitle();
            await TransferDuplicatePokemon();
            await RecycleItems();
            await RepeatAction(1, ExecuteFarmingPokestopsAndPokemons);
         
            await Task.Delay(2000);
            await myRoutine();
        }


        public static float Perfect(PokemonData poke)
         {
             return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
         }

    public async Task RepeatAction(int repeat, Func<Task> action)
        {
            Logger.Write($"Farming area for {repeat} times");
            for (int i = 0; i < repeat; i++)
                await action();
        }

        private async Task DisplayPlayerLevelInTitle()
         {

            var profile = await _client.GetProfile();
            
            if (profile.Profile != null)
            {
                Logger.Write($"Playing Profile: {profile.Profile.Username},  {profile.Profile.Team} Team, {profile.Profile.Currency.FirstOrDefault()}", LogLevel.Info);
            }
            var pname = profile.Profile != null ? profile.Profile.Username : "x";
            var pdust = "x";

            var stats = await _inventory.GetPlayerStats();
             PlayerStats stat = stats.FirstOrDefault();
             if (stat != null)
             {
                 System.Console.Title = string.Format("Player {0:0} level {1:0} - ({2:0} / {3:0}) - Stardust: {4:0}",
                     pname,
                     +stat.Level,
                      +(stat.Experience - stat.PrevLevelXp),
                     +(stat.NextLevelXp - stat.PrevLevelXp),
                     pdust
                     );
             }
             await Task.Delay(5000);
         }

private async Task ExecuteFarmingPokestopsAndPokemons()
        {
            var mapObjects = await _client.GetMapObjects();

            var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());
          //  pokeStops = Navigation.generatePath(pokeStops);
            foreach (var pokeStop in pokeStops)
            {
                var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLat, _client.CurrentLng, pokeStop.Latitude, pokeStop.Longitude);
                var update = await _client.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude, _clientSettings.DefaultAltitude);
                var fortInfo = await _client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await _client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Logger.Write($"Using Pokestop: {fortInfo.Name} in {Math.Round(distance)}m distance");
                Logger.Write($"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: { fortSearch.GemsAwarded}, Eggs: {fortSearch.PokemonDataEgg} Items: {StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded)}", LogLevel.Info);
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> origin/master

                if (fortSearch.ExperienceAwarded > 0)
                                    {
                    //await DisplayPlayerLevelInTitle();
                                    }
                await Task.Delay(1000);
                await ExecuteCatchAllNearbyPokemons();
<<<<<<< HEAD

               await Task.Delay(1000);
            
=======
                await TransferDuplicatePokemon(true);
>>>>>>> refs/remotes/FeroxRev/master
>>>>>>> origin/master
            }
        }

        private async Task ExecuteCatchAllNearbyPokemons()
        {
            var mapObjects = await _client.GetMapObjects();

            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);

            foreach (var pokemon in pokemons)
            {
                var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLat, _client.CurrentLng, pokemon.Latitude, pokemon.Longitude);
                if (distance > 100)
                    await Task.Delay(15000);
                else
                    await Task.Delay(500);

                await _client.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude, _clientSettings.DefaultAltitude);

                var encounter = await _client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                await CatchEncounter(encounter, pokemon);
            }
            await Task.Delay(5000);
        }

        private async Task CatchEncounter(EncounterResponse encounter, MapPokemon pokemon)
        {

            CatchPokemonResponse caughtPokemonResponse;
            do
            {
                if (encounter?.CaptureProbability.CaptureProbability_.First() < 0.35)
                {
<<<<<<< HEAD

                    //Throw berry is we can
                    await UseBerry(pokemon.EncounterId, pokemon.SpawnpointId);
                }

=======
<<<<<<< HEAD
                    if (encounterPokemonResponse?.CaptureProbability.CaptureProbability_.First() < 0.4 && pokemonCP > 400)
                    {
                        //Throw berry is we can
                        await UseBerry(pokemon.EncounterId, pokemon.SpawnpointId);
                    }

                    caughtPokemonResponse = await client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, pokeball);
                    await Task.Delay(2000);
=======
                    //Throw berry is we can
                    await UseBerry(pokemon.EncounterId, pokemon.SpawnpointId);
>>>>>>> refs/remotes/FeroxRev/master
                }

<<<<<<< HEAD
                Logger.Write(caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess ? $"We caught a {pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {pokeball}" : $"{pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} got away while using a {pokeball}..", LogLevel.Info);
                await Task.Delay(5000);
=======
                var pokeball = await GetBestBall(encounter?.WildPokemon);
>>>>>>> origin/master

                var pokeball = await GetBestBall(encounter?.WildPokemon);
                var distance = Navigation.DistanceBetween2Coordinates(_client.CurrentLat, _client.CurrentLng, pokemon.Latitude, pokemon.Longitude);
                caughtPokemonResponse = await _client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, pokeball);
                Logger.Write(caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess ? $"We caught a {pokemon.PokemonId} with CP {encounter?.WildPokemon?.PokemonData?.Cp} ({Perfect(encounter?.WildPokemon?.PokemonData).ToString("0.00")}% perfect) and CaptureProbability: {encounter?.CaptureProbability.CaptureProbability_.First()} using a {pokeball} in {Math.Round(distance)}m distance" : $"{pokemon.PokemonId} with CP {encounter?.WildPokemon?.PokemonData?.Cp} CaptureProbability: {encounter?.CaptureProbability.CaptureProbability_.First()} in {Math.Round(distance)}m distance {caughtPokemonResponse.Status} while using a {pokeball}..", LogLevel.Info);

                await Task.Delay(2000);
<<<<<<< HEAD
=======
>>>>>>> refs/remotes/FeroxRev/master
>>>>>>> origin/master
            }
            while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape) ;
            if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                            {
               // await DisplayPlayerLevelInTitle();
                            }
        }

        private async Task EvolveAllPokemonWithEnoughCandy()
        {
            var pokemonToEvolve = await _inventory.GetPokemonToEvolve();
            int pokemonEvolved = 0;
            foreach (var pokemon in pokemonToEvolve)
            {
                var evolvePokemonOutProto = await _client.EvolvePokemon((ulong)pokemon.Id);

                if (evolvePokemonOutProto.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
                {
                    Logger.Write($"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp", LogLevel.Info); Logger.Write($"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp", LogLevel.Info);
                    pokemonEvolved += 1;
                                    }
                else
                    Logger.Write($"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", LogLevel.Info);

                await Task.Delay(3000);
            }
            if (pokemonEvolved > 0)
                            {
               await DisplayPlayerLevelInTitle();
                           }
        }

        private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false, float keepPerfectPokemonLimit = 85.0f)
        {
<<<<<<< HEAD

            var duplicatePokemons = await _inventory.GetDuplicatePokemonToTransfer(_clientSettings.PokemonOfEachToKeep);
=======
<<<<<<< HEAD
            var duplicatePokemons = await _inventory.GetDuplicatePokemonToTransfer(_clientSettings.PokemonOfEachToKeep);
=======
            var duplicatePokemons = await _inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve);
>>>>>>> refs/remotes/FeroxRev/master
>>>>>>> origin/master

            Logger.Write($"Transfer pokemon: ", LogLevel.Info);
            foreach (var duplicatePokemon in duplicatePokemons)
            {
               
                if (Perfect(duplicatePokemon) >= keepPerfectPokemonLimit)
                {
                    Logger.Write($"Skip {duplicatePokemon.PokemonId} {duplicatePokemon.Cp} CP with {Perfect(duplicatePokemon)} perfectness", LogLevel.Warning);
                    continue;
                }
               
                var transfer = await _client.TransferPokemon(duplicatePokemon.Id);
                Logger.Write($"Transfer {duplicatePokemon.PokemonId} {duplicatePokemon.Cp} CP with {Perfect(duplicatePokemon)} perfectness", LogLevel.Info);
                await Task.Delay(500);
            }
        }

        private async Task RecycleItems()
        {
            var items = await _inventory.GetItemsToRecycle(_clientSettings);

            foreach (var item in items)
            {
                var transfer = await _client.RecycleItem((AllEnum.ItemId)item.Item_, item.Count);
                Logger.Write($"Recycled {item.Count}x {(AllEnum.ItemId)item.Item_}", LogLevel.Info);
                await Task.Delay(500);
            }
        }

        private async Task<MiscEnums.Item> GetBestBall(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var pokeBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_POKE_BALL);
            var greatBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_GREAT_BALL);
            var ultraBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_ULTRA_BALL);
            var masterBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_MASTER_BALL);

            if (masterBallsCount > 0 && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_MASTER_BALL;
            else if (ultraBallsCount > 0 && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            else if (greatBallsCount > 0 && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (ultraBallsCount > 0 && pokemonCp >= 600)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            else if (greatBallsCount > 0 && pokemonCp >= 600)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (greatBallsCount > 0 && pokemonCp >= 350)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (pokeBallsCount > 0)
                return MiscEnums.Item.ITEM_POKE_BALL;
            if (greatBallsCount > 0)
                return MiscEnums.Item.ITEM_GREAT_BALL;
            if (ultraBallsCount > 0)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            if (masterBallsCount > 0)
                return MiscEnums.Item.ITEM_MASTER_BALL;

            return MiscEnums.Item.ITEM_POKE_BALL;
        }

        public async Task UseBerry(ulong encounterId, string spawnPointId)
        {
            var inventoryBalls = await _inventory.GetItems();
            var berries = inventoryBalls.Where(p => (ItemId) p.Item_ == ItemId.ItemRazzBerry);
            var berry = berries.FirstOrDefault();

            if (berry == null)
                return;
            
            var useRaspberry = await _client.UseCaptureItem(encounterId, AllEnum.ItemId.ItemRazzBerry, spawnPointId);
            Logger.Write($"Use Rasperry. Remaining: {berry.Count}", LogLevel.Info);
            await Task.Delay(3000);
        }
    }
}
