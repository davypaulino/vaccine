using vaccine.Application.Configurations;
using vaccine.Application.Constants;
using vaccine.Application.Filters;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;

namespace vaccine.Endpoints;

public static class UserEndpoints
{
    private const string CLASSNAME = nameof(VaccineEndpoints);
    private static readonly string[] _tags =
    [
        ApiDocumentationConstants.UserTag.Name
    ];

    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var apiVersion = 1;
        var group = app
            .MapGroup($"/api/v{apiVersion}/users")
            .WithTags(_tags)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOrder(10);

        group.MapPost("/register", RegisterUser)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<RegisterRequest>(StatusCodes.Status201Created)
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>()
            .WithSummary("Registar um novo utilizador")
            .WithDescription("""
                             Cria um novo utilizador no sistema.

                             Este endpoint é responsável por:
                             - Validar os dados de registo
                             - Verificar se o utilizador já existe
                             - Criar o utilizador com as permissões padrão
                             - Retornar o identificador do utilizador criado

                             Erros de validação ou regras de negócio inválidas
                             resultarão em um erro 400 (Bad Request).
                             """);

        group.MapPost("/authenticate", AuthenticateUser)
            .ProducesValidationProblem()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .AddEndpointFilter<ValidationFilter<AuthenticateRequest>>()
            .WithSummary("Autenticar utilizador")
            .WithDescription("""
                             Autentica um utilizador no sistema através de credenciais válidas.

                             Este endpoint é responsável por:
                             - Validar os dados de autenticação
                             - Verificar se o utilizador existe
                             - Validar a palavra-passe
                             - Gerar e devolver um token JWT válido
                             - Definir as permissões (roles) do utilizador

                             Caso as credenciais sejam inválidas, será retornado
                             um erro 401 (Unauthorized).
                             """);
        
        return group;
    }

    private static async Task<IResult> AuthenticateUser(
        AuthenticateRequest request,
        IRequestInfo requestInfo,
        IAuthenticationService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
        if (response.Success)
        {
            return Results.Ok(response.Data);
        }
        
        return Results.Problem(
            type: ProblemDetailTypes.Validation,
            title: "Falha na authenticação",
            detail: response.Error,
            statusCode: StatusCodes.Status401Unauthorized,
            extensions: new Dictionary<string, object?>
            {
                ["correlationId"] = requestInfo.CorrelationId
            });
    }

    private static async Task<IResult> RegisterUser(
        RegisterRequest request,
        IRequestInfo requestInfo,
        IAuthenticationService authService,
        CancellationToken cancellation)
    {
        var response = await authService.RegisterAsync(request, cancellation);
        if (response.Success)
        {
            return Results.Created($"/api/v1/users/{response.Data.userId}", response.Data);
        }
        
        return Results.Problem(
            type: ProblemDetailTypes.BadRequest,
            title: "Fail to register User",
            detail: response.Error,
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?>
            {
                ["correlationId"] = requestInfo.CorrelationId
            });
    }
}