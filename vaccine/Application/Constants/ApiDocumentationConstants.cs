using Microsoft.OpenApi.Models;

namespace vaccine.Application.Constants;

public static class ApiDocumentationConstants
{
    public static OpenApiTag UserTag = new OpenApiTag()
    {
        Name = "APIs | 1) Utilizadores",
        Description = """
                      Essa secção tem como objetivo agrupar todas as ações
                      relacionadas ao utilizador no sistema, incluindo
                      registo e autenticação.
                      """
    };
    
    public static OpenApiTag ManagerVaccineTag = new OpenApiTag()
    {
        Name = "APIs | 2) Gestão de Vacinas",
        Description = """
                      Essa secção tem como objetivo agrupar todas as ações,
                      que possam ser realizadas relacionadas a vacinas no sistema.
                      """
    };
    
    public static OpenApiTag[] Tags =
    [
        UserTag,
        ManagerVaccineTag,
    ];
}