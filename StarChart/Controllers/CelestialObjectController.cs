using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;
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
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestial)
        {
            _context.CelestialObjects.Add(celestial);
            _context.SaveChanges();
            var newId = celestial.Id;           
            return CreatedAtRoute("GetById", new { id = newId}, celestial);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject input)
        {
            var celestialToUpdate = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();

            if(celestialToUpdate==null)
            {
                return NotFound();
            }
            else
            {
                celestialToUpdate.Name = input.Name;
                celestialToUpdate.OrbitalPeriod = input.OrbitalPeriod;
                celestialToUpdate.OrbitedObjectId = input.OrbitedObjectId;

                _context.CelestialObjects.Update(celestialToUpdate);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id,string name)
        {
            var celestialToUpdate = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();

            if (celestialToUpdate == null)
            {
                return NotFound();
            }
            else
            {
                celestialToUpdate.Name = name;
                _context.CelestialObjects.Update(celestialToUpdate);
                _context.SaveChanges();
                return NoContent();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var matchedOnId = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();            
            var matchedOnOrbit = _context
                .CelestialObjects
                .Where(c => c.Satellites.Any(s => s.OrbitedObjectId == id)).ToList();

            if (matchedOnId == null && matchedOnOrbit != null && matchedOnOrbit.Count == 0)
                return NotFound();

            if(matchedOnId !=null)
                _context.CelestialObjects.Remove(matchedOnId);
            if(matchedOnOrbit != null && matchedOnOrbit.Count > 0)
                _context.CelestialObjects.RemoveRange(matchedOnOrbit);

            _context.SaveChanges();
            return NoContent();
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
