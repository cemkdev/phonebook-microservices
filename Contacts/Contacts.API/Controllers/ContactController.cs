using Contacts.API.Dto;
using Contacts.API.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController(IContactService _contactService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactDto createContactDto, CancellationToken cancellationToken)
        {
            var id = await _contactService.CreateAsync(createContactDto, cancellationToken);
            return Ok(id);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var contact = await _contactService.GetAsync(id, cancellationToken);
            return contact is not null ? Ok(contact) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
            => Ok(await _contactService.ListAsync(cancellationToken));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var removed = await _contactService.DeleteAsync(id, cancellationToken);
            return removed ? Ok() : NotFound();
        }
    }
}
