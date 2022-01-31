using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}",Name = "GetById")]    
        public IActionResult GetById(int id)
        {            
            var celestial = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();            
            if(celestial == null)
            {
                return NotFound();
            }
            else
            {
                celestial.Satellites = new List<Models.CelestialObject>();
                celestial.Satellites
                    .AddRange(_context.CelestialObjects.Where(s => s.OrbitedObjectId == celestial.Id));
                return new OkObjectResult(celestial);
            }
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestials = _context.CelestialObjects.Where(c => c.Name == name.Trim()).ToList();
            if (celestials == null || celestials.Count ==0)
            {
                return NotFound();
            }
            else
            {
                foreach(var celestial in celestials)
                {
                    celestial.Satellites = new List<Models.CelestialObject>();
                    celestial.Satellites
                        .AddRange(_context.CelestialObjects.Where(s => s.OrbitedObjectId == celestial.Id));
                }                               
                return new OkObjectResult(celestials);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestials = _context.CelestialObjects.ToList();
            if (celestials == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var celestial in celestials)
                {
                    celestial.Satellites = new List<Models.CelestialObject>();
                    celestial.Satellites
                        .AddRange(_context.CelestialObjects.Where(s => s.OrbitedObjectId == celestial.Id));
                }

                return new OkObjectResult(celestials);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
