using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var celestialObject = _context.CelestialObjects.FirstOrDefault(o => o.Id == id);
            if (celestialObject == null) return NotFound();

            var celestialObjects = _context.CelestialObjects.Where(o => o.OrbitedObjectId == id).ToList();
            celestialObject.Satellites = new List<CelestialObject>();
            celestialObject.Satellites.AddRange(celestialObjects);

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(o => o.Name == name);
            if (!celestialObjects.Any()) return NotFound();

            foreach (var obj in celestialObjects)
            {
                var sameObj = _context.CelestialObjects.Where(o => o.OrbitedObjectId == obj.Id).ToList();
                obj.Satellites = new List<CelestialObject>();
                obj.Satellites.AddRange(sameObj);
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var obj in celestialObjects)
            {
                obj.Satellites = new List<CelestialObject>
                {
                    obj
                };
            }

            return NotFound(celestialObjects);
        }
    }
}
