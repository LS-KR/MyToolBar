﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media;

/*
 Weather Api
 see as : https://dev.qweather.com/docs/
 */

namespace MyToolBar.Func
{
    public static class WeatherDataCache
    {
        public static DateTime UpdateTime { get;set; }
        public static WeatherApi.WeatherNow CurrentWeather{ get; set; }
        public static WeatherApi.City CurrentCity { get; set; }
        public static List<WeatherApi.WeatherDay> DailyForecast { get; set; }
        public static List<WeatherApi.AirData> DailyAirForecast { get; set; }
    }
    public static class WeatherApi
    {
        public class City
        {
            public string province, city, area,id;
        }
        public class WeatherNow
        {
            public string status,link,feel;
            public string windDir, windScale, humidity, vis;
            public int code,temp;
            public City city;
            //...more
        }
        public class WeatherDay
        {
            public string status_day, status_night;
            public int code_day, code_night;
            public int temp_max, temp_min;
        }
        public class AirData
        {
            public int aqi, level;
            public string desc;
        }
        private static string key =ApiKeys.Weather.key,
            lang="en",
            host= ApiKeys.Weather.host;
        public static async Task<City> GetPositionByIpAsync()
        {
            string data = await HttpHelper.Get("https://ip.useragentinfo.com/json",false);
            JsonNode obj= JsonNode.Parse(data);
            if (obj!=null& obj["code"].ToString() == "200")
            {
                return new City()
                {
                    province = obj["province"].ToString(),
                    city= obj["city"].ToString(),
                    area= obj["area"].ToString()
                };
            }
            return null;
        }
        public static async Task<bool> SearchCityAsync(this City city)
        {
            string c1 = city.city, c2 = city.area;
            if (string.IsNullOrEmpty(city.area))
            {
                c1 = city.province; c2 = city.city;
            }

            string url = $"https://geoapi.qweather.com/v2/city/lookup?location={HttpUtility.UrlEncode(c2)}&adm={HttpUtility.UrlEncode(c1)}&key={key}&lang={lang}";
            string data=await HttpHelper.Get(url);
            var obj= JsonNode.Parse(data);
            if (obj != null && obj["code"].ToString() == "200")
            {
                JsonArray cities = obj["location"].AsArray();
                if (cities.Count() > 0)
                {
                    var ci = cities[0];
                    city.area = ci["name"].ToString();
                    city.city = ci["adm2"].ToString();
                    city.id = ci["id"].ToString();
                    return true;
                }
            }
            return false;
        }

        public static async Task<WeatherNow> GetCurrentWeather(this City city)
        {
            string url = $"https://{host}/v7/weather/now?location={city.id}&key={key}&lang={lang}";
            string data= await HttpHelper.Get(url);
            var obj= JsonNode.Parse(data);
            if (obj != null & obj["code"].ToString() == "200")
            {
                var now = obj["now"];
                return new WeatherNow()
                {
                    temp = int.Parse(now["temp"].ToString()),
                    code = int.Parse(now["icon"].ToString()),
                    status = now["text"].ToString(),
                    city = city,
                    link = obj["fxLink"].ToString(),
                    feel = now["feelsLike"].ToString(),
                    humidity = now["humidity"].ToString(),
                    windDir= now["windDir"].ToString(),
                    windScale= now["windScale"].ToString(),
                    vis= now["vis"].ToString()
                };
            }
            return null;
        }
        public static string GetIcon(int code)
        {
            return $"https://a.hecdn.net/img/common/icon/202106d/{code}.png";
        }
        public static async Task<List<AirData>> GetAirAsync(this City city)
        {
            string url = $"https://{host}/v7/air/5d?location={city.id}&key={key}&lang={lang}";
            string data = await HttpHelper.Get(url);
            var obj = JsonNode.Parse(data);
            if (obj != null & obj["code"].ToString() == "200")
            {
                List<AirData> list = new();
                var daily = obj["daily"].AsArray();
                foreach(var i in daily)
                {
                    list.Add(new AirData()
                    {
                        aqi = int.Parse(i["aqi"].ToString()),
                        level = int.Parse(i["level"].ToString()),
                        desc = i["category"].ToString()
                    });
                }
                return list;
            }
            return null;
        }
        public static Color GetAirLevelColor(int level) => level switch
        {
            1=>Color.FromArgb(255,0,228,0),
            2=>Color.FromArgb(255,255,255,0),
            3=>Color.FromArgb(255,255,126,0),
            4=>Color.FromArgb(255,255,0,0),
            5=>Color.FromArgb(255,153,0,76),
            6=>Color.FromArgb(255,126,0,35)
        };
        public static async Task<List<WeatherDay>> GetForecastAsync(this City city)
        {
            string url = $"https://{host}/v7/weather/7d?location={city.id}&key={key}&lang={lang}";
            string data = await HttpHelper.Get(url);
            var obj = JsonNode.Parse(data);
            if (obj != null & obj["code"].ToString() == "200")
            {
                List<WeatherDay> list = new();
                var daily = obj["daily"].AsArray();
                foreach (var i in daily)
                {
                    list.Add(new WeatherDay()
                    {
                        status_day = i["textDay"].ToString(),
                        status_night= i["textNight"].ToString(),
                        code_day= int.Parse(i["iconDay"].ToString()),
                        code_night= int.Parse(i["iconNight"].ToString()),
                        temp_max = int.Parse(i["tempMax"].ToString()),
                        temp_min = int.Parse(i["tempMin"].ToString()),
                    });
                }
                return list;
            }
            return null;
        }
    }
}