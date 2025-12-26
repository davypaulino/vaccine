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
    
    public static OpenApiTag VaccinationTag = new OpenApiTag()
    {
        Name = "APIs | 3) Vacinação",
        Description = """
                      Esta seção agrupa todos os endpoints responsáveis
                      pelo gerenciamento de vacinas aplicadas no sistema,
                      incluindo as operações de criação, modificação e exclusão
                      de registros de vacinação.
                      """
    };
    
    public static OpenApiTag PersonTag = new OpenApiTag()
    {
        Name = "APIs | 4) Pessoal",
        Description = """
                      Esta seção tem como objetivo agrupar todas as ações
                      relacionadas ao gerenciamento de pessoas no sistema,
                      incluindo operações de criação, edição, exclusão
                      e listagem paginada de pessoas, bem como a consulta
                      das vacinações associadas a uma pessoa específica.
                      """
    };
    
    public static OpenApiTag[] Tags =
    [
        UserTag,
        ManagerVaccineTag,
        VaccinationTag,
        PersonTag
    ];
}