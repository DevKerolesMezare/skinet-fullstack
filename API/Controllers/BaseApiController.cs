using API.RequestHelper;
using Core.Entities;
using Core.Interfases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

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

    protected async Task<ActionResult> CreatePageResult<T, TDto>(IGenericRepository<T> repo,
    ISpecification<T> spec, int pageIndex, int pageSize, Func<T, TDto> toDto) where T
     : BaseEntity, IDtoConvertible
    {
        var itmes = await repo.ListAsync(spec);
        var count = await repo.CountAsync(spec);

        var dotItmes = itmes.Select(toDto).ToList();

        var pagination = new Pagination<TDto>(pageIndex, pageSize, count, dotItmes);

        return Ok(pagination);
    }
}
