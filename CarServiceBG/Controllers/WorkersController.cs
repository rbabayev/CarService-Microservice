using CarService.Business.Abstract;
using CarService.Entities.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public WorkersController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        // GET: api/Workers
        [HttpGet("GetAllWorkers")]
        public async Task<IActionResult> GetAllWorkers()
        {
            var workers = await _workerService.GetAllWorkersAsync();
            return Ok(workers);
        }

        // GET: api/Workers/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkerById(Guid id)
        {
            var workers = await _workerService.GetWorkerByIdAsync(id);
            if (workers == null)
                return NotFound(new { Message = "Worker not found" });

            return Ok(workers);
        }

        // POST: api/Workers
        [HttpPost]
        public async Task<IActionResult> CreateWorker([FromBody] Worker workers)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdWorker = await _workerService.CreateWorkerAsync(workers);
            return CreatedAtAction(nameof(GetWorkerById), new { id = createdWorker.Id }, createdWorker);
        }

        // PUT: api/Workers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorker(Guid id, [FromBody] Worker workers)
        {
            if (id != workers.Id)
                return BadRequest(new { Message = "ID mismatch" });

            var existingWorker = await _workerService.GetWorkerByIdAsync(id);
            if (existingWorker == null)
                return NotFound(new { Message = "Worker not found" });

            await _workerService.UpdateWorkerAsync(id, workers);
            return NoContent();
        }

        // DELETE: api/Workers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(Guid id)
        {
            var workers = await _workerService.GetWorkerByIdAsync(id);
            if (workers == null)
                return NotFound(new { Message = "Worker not found" });

            await _workerService.DeleteWorkerAsync(id);
            return NoContent();
        }

        // Get Workers by Category
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetWorkersByCategory(string category)
        {
            var workers = await _workerService.GetWorkersByCategoryAsync(category);
            if (workers == null || !workers.Any())
                return NotFound(new { Message = "No Workers found in this category" });

            return Ok(workers);
        }

        // Search Workers by Name
        [HttpGet("search")]
        public async Task<IActionResult> SearchWorkers([FromQuery] string query)
        {
            var workers = await _workerService.SearchWorkersAsync(query);
            if (workers == null || !workers.Any())
                return NotFound(new { Message = "No Workers found matching your query" });

            return Ok(workers);
        }
    }
}
