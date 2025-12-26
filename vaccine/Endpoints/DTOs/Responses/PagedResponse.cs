namespace vaccine.Endpoints.DTOs.Responses;

public class PagedResponse<T>
{
    /// <summary>
    /// Número da página atual
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Quantidade de itens por página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens disponíveis
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Dados da página atual
    /// </summary>
    public IReadOnlyCollection<T> Data { get; set; }

    public PagedResponse() { }

    public PagedResponse(IReadOnlyCollection<T> data, int totalCount, int page, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
