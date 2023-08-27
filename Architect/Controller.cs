/// <summary>
/// Controller:
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class DanhSachHoSoController : ControllerBase
{
    private readonly ISeedService _service;
    public DanhSachHoSoController(ISeedService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetBySearch(DanhSachHoSoBySearchRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResultApi("NguoiLapId is required"));
        }
        try
        {
            var result = await _service.DanhSachHoSoService.GetBySearch(request);
            return Ok(new ResultApi(result));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResultApi(ex));
        }
    }
    
    // ......
}


/// Generic Controller:
[Route("api/[controller]")]
    [ApiController]
    public class GenericController<TKey, TEntity, TContext> : ControllerBase
        where TEntity : class
        where TContext : DbContext
    {
        internal IGenericService<TEntity, TContext> _service;
        public GenericController(IGenericService<TEntity, TContext> service)
        {
            this._service = service;
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public virtual IActionResult GetAll()
        {
            try
            {
                var result = new { Data = _service.GetList() };
                return Ok(new ResultApi(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultApi(ex));
            }
        }
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public virtual IActionResult GetPagination(int pageIndex = 0, int pageSize = 0)
        {
            try
            {
                var result = new { TotalRow = _service.GetList().Count, Data = _service.GetList(PageIndex: pageIndex, PageSize: pageSize) };
                return Ok(new ResultApi(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultApi(ex));
            }
        }
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public virtual IActionResult GetById(TKey id)
        {
            try
            {
                object data = new();
                if (id != null)
                    data = _service.GetByID(id);
                var result = new { Data = data };
                return Ok(new ResultApi(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultApi(ex));
            }
        }


        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public virtual IActionResult Create(TEntity entity)
        {
            try
            {
                _service.Insert(entity);
                return Ok(new ResultApi(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultApi(ex));
            }
        }
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public virtual IActionResult CreateRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _service.InsertRange(entities);
                return Ok(new ResultApi(entities));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultApi(ex));
            }
        }
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public virtual IActionResult Update(TEntity entity)
        {
            try
            {
                _service.Update(entity);
                return Ok(new ResultApi(entity));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultApi(ex));
            }
        }
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public virtual IActionResult HardDelete(TKey? id)
        {
            try
            {
                if (id != null)
                    _service.TryDelete(id);
                return Ok(new ResultApi(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultApi(ex));
            }
        }
    }