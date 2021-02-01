using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null) return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(o => o.Name == name).ToList();
            if (!celestialObjects.Any()) return NotFound();

            foreach (var obj in celestialObjects)
            {
                obj.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == obj.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var obj in celestialObjects)
            {
                obj.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == obj.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            try
            {
                _context.CelestialObjects.Add(celestialObject);
                _context.SaveChanges();
                return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create celestial object");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject cObject)
        {
            try
            {
                var celestialObject = _context.CelestialObjects.Find(id);
                if (celestialObject is null) return NotFound();

                celestialObject.Name = cObject.Name;
                celestialObject.OrbitalPeriod = cObject.OrbitalPeriod;
                celestialObject.OrbitedObjectId = cObject.OrbitedObjectId;

                _context.CelestialObjects.Update(celestialObject);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update celestial object");
            }
            
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            try
            {
                var celestialObject = _context.CelestialObjects.Find(id);
                if (celestialObject is null) return NotFound();

                celestialObject.Name = name;
                _context.CelestialObjects.Update(celestialObject);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to rename celestial object");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var celestialObjects = _context.CelestialObjects.Where(o => o.Id == id || o.OrbitedObjectId == id).ToList();
                if (!celestialObjects.Any()) return NotFound();

                _context.CelestialObjects.RemoveRange(celestialObjects);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete celestial objects");
            }
        }
    }
}
