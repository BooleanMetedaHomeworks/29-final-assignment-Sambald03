﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ristorante_frontend.Models;
using Newtonsoft.Json;

namespace ristorante_frontend.Services
{
    /*
    public enum ApiServiceResultType
    {
        Success,
        Error
    }
    */

    public static class ApiService
    {
        public const string API_URL = "https://localhost:7205";

        public static async Task<ApiServiceResult<List<Dish>>> GetDishes()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var httpResult = await client.GetAsync($"{API_URL}/Dish");
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<Dish>>(resultBody);
                return new ApiServiceResult<List<Dish>>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<List<Dish>>(e);
            }
        }

        public static async Task<ApiServiceResult<int>> CreateDish(Dish dish)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                var httpResult = await httpClient.PostAsync($"{API_URL}/Dish", JsonContent.Create(dish));
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<int>(resultBody);
                return new ApiServiceResult<int>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<int>(e);
            }
        }

        public static async Task<ApiServiceResult<int>> UpdateDish(Dish dish)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                var httpResult = await httpClient.PutAsync($"{API_URL}/Dish/{dish.Id}", JsonContent.Create(dish));
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<int>(resultBody);
                return new ApiServiceResult<int>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<int>(e);
            }
        }

        public static async Task<ApiServiceResult<int>> DeleteDish(int id)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                var httpResult = await httpClient.DeleteAsync($"{API_URL}/Dish/{id}");
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<int>(resultBody);
                return new ApiServiceResult<int>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<int>(e);
            }
        }

        /*
        public static async Task<ApiServiceResult<List<Categoria>>> GetCategorie()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var httpResult = await client.GetAsync($"{API_URL}/Categoria");
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<Categoria>>(resultBody);
                return new ApiServiceResult<List<Categoria>>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<List<Categoria>>(e);
            }
        }
        */
    }
}
