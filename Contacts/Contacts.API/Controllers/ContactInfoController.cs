using Contacts.API.Dto;
using Contacts.API.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactInfoController(IContactInfoService _contactInfoService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddContactInfoDto addContactInfoDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(addContactInfoDto.Content))
                return BadRequest("Content is required.");

            var result = await _contactInfoService.AddAsync(addContactInfoDto, cancellationToken);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{infoId:guid}")]
        public async Task<IActionResult> Remove(Guid infoId, CancellationToken cancellationToken)
        {
            var removed = await _contactInfoService.RemoveAsync(infoId, cancellationToken);
            return removed ? Ok() : NotFound();
        }
    }
}
