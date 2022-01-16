using CarWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarWarehouse.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> _logger;
        List<WareHouseModel> warehouses = new List<WareHouseModel>();

        public CarController(ILogger<CarController> logger)
        {
            _logger = logger;
            //json file read and convert an object
            using (StreamReader r = new StreamReader("Files//warehouses.json"))
            {
                string json = r.ReadToEnd();
                warehouses = JsonConvert.DeserializeObject<List<WareHouseModel>>(json);
            }
        }

        /// <summary>
        /// Listing all warehouse's cars by filtering with licence statu
        /// </summary>
        /// <param name="isLicenced">default licence statu is true</param>
        /// <returns>Name of car list</returns>
        [HttpGet]
        [Route("list")]
        public IActionResult List(bool isLicenced = true)
        {
            //getting vehicles information with licence statu and to order date_added value by ascending
            return Ok(warehouses.SelectMany(a => a.cars.vehicles.Where(a => a.licensed == isLicenced).ToList()).OrderBy(a => a.date_added).ToList());
        }

        /// <summary>
        /// Getting car all details with car ids
        /// </summary>
        /// <param name="id">unique car id</param>
        /// <returns>Car details</returns>
        [HttpGet]
        [Route("get/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                //vehicle id is not sending return bad request
                if (id == 0) return BadRequest("Warehouse info or vehicle info cannot be null");
                //searching vehicle value by warehouse id and vehicle id
                var vehicle = warehouses.Select(a =>
                {
                    var car = a.cars.vehicles.FirstOrDefault(x => x._id == id);
                    if (car != null)
                    {
                        return new WareHouseModel { _id = a._id, name = a.name, location = a.location, cars = new Car { location = a.cars.location, vehicles = new List<Vehicle> { car } } };
                    }
                    return null;
                }).Where(a => a != null).FirstOrDefault();

                //the vehicle is exist return vehicle info with warehouses info
                if (vehicle != null)
                    return Ok(vehicle);
                else return NotFound(); //the vehicle is not exist return not found
            }
            catch (System.Exception ex)
            {
                //if there is error inside of try area,program return and not found with excetion message.
                return NotFound(ex.Message);
            }
        }
    }
}
