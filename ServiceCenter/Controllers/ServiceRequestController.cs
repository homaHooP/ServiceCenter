using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceCenter.data;
using ServiceCenter.Messaging;
using ServiceCenter.Models;

namespace ServiceCenter.Controllers
{
    public class ServiceRequestController(AppDbContext _db, ServiceRequestQueue _queue) : Controller
    {
        [HttpGet("/")]
        [HttpGet("requests")]
        public async Task<IActionResult> Index()
        {
            var requests = await _db.ServiceRequests.OrderBy(r => r.CreatedAt).ToListAsync();
            return View(requests);
        }

        [HttpGet("requests/{id}")]
        public async Task<IActionResult> Details([FromRoute] Guid id)
        {
            var request = await _db.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpPost("requests/{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id)
        {
            var request = await _db.ServiceRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.Status == ServiceRequestStatus.Closed)
            {
                return BadRequest("Cannot update status of a closed request.");
            }
            else if (request.Status == ServiceRequestStatus.Unresolved)
            {
                request.Status = ServiceRequestStatus.InProgress;
            }
            else if (request.Status == ServiceRequestStatus.InProgress)
            {
                request.Status = ServiceRequestStatus.Closed;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = request.Id });
        }

        [HttpGet("requests/create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost("requests/create")]
        public async Task<IActionResult> Create([Bind(nameof(ServiceRequest.RequesterName), nameof(ServiceRequest.Title), nameof(ServiceRequest.Description), nameof(ServiceRequest.Priority))]
            ServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            request.Id = Guid.NewGuid();
            request.Status = ServiceRequestStatus.Unresolved;
            request.CreatedAt = DateTime.UtcNow;

            _db.ServiceRequests.Add(request);
            await _db.SaveChangesAsync();
            await _queue.PublishCreatedAsync(request);
            return RedirectToAction(nameof(Details), new { id = request.Id });
        }
    }
}
