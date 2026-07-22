using API.RequestHelper;
using Core.Entities;
using Core.Interfases;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected async Task<ActionResult> CreatePageResult<T>(IGenericRepository<T> repo,
    ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
    {
        var itmes = await repo.ListAsync(spec);
        var count = await repo.CountAsync(spec);

        var pagination = new Pagination<T>(pageIndex, pageSize, count, itmes);

        return Ok(pagination);
    }

}
