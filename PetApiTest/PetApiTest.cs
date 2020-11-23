using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PetApi;
using Xunit;

namespace PetApiTest
{
    public class PetApiTest
    {
        [Fact]
        public async Task Should_Add_Pet_When_Add_PetAsync()
        {
            // given
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            Pet pet = new Pet("Max", "dog", "black", 5000);
            string request = JsonConvert.SerializeObject(pet);
            StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");

            // when
            var response = await client.PostAsync("PetStore/addNewPet", requestBody);
            // then
            response.EnsureSuccessStatusCode();
            var requestString = await response.Content.ReadAsStringAsync();
            Pet actualPet = JsonConvert.DeserializeObject<Pet>(requestString);
            Assert.Equal(pet, actualPet);
        }

        [Fact]
        public async Task Should_Return_PetList_When_Get_All_PetsAsync()
        {
            //given
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            await client.DeleteAsync("petStore/clear");
            Pet pet = new Pet("Meow", "cat", "white", 3000);
            string request = JsonConvert.SerializeObject(pet);
            StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");
            await client.PostAsync("petStore/addNewPet", requestBody);

            //when
            var response = await client.GetAsync("petStore/pets");
            var responseString = await response.Content.ReadAsStringAsync();
            List<Pet> actualPets = JsonConvert.DeserializeObject<List<Pet>>(responseString);

            //then
            response.EnsureSuccessStatusCode();
            Assert.Equal(new List<Pet>() { pet }, actualPets);
        }

        [Fact]
        public async Task Should_Return_Correct_Pet_When_Get_Pet_By_NameAsync()
        {
            //given
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            await client.DeleteAsync("petStore/clear");
            List<Pet> pets = new List<Pet>() { new Pet("Meow", "cat", "white", 3000), new Pet("Meow", "cat", "white", 3000), new Pet("Pink", "cat", "black", 3000) };
            foreach (var pet in pets)
            {
                string request = JsonConvert.SerializeObject(pet);
                StringContent requestBody = new StringContent(request, Encoding.UTF8, "application/json");
                await client.PostAsync("petStore/addNewPet", requestBody);
            }

            //when
            var response = await client.GetAsync($"petStore/getPetByName/{pets[0].Name}");
            var responseString = await response.Content.ReadAsStringAsync();
            Pet actualPet = JsonConvert.DeserializeObject<Pet>(responseString);

            //then
            response.EnsureSuccessStatusCode();
            Assert.Equal(pets[0], actualPet);
        }
    }
}
