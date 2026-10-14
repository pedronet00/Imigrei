using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Imigrei.API.OpenApi;

/// <summary>
/// Declara o esquema Bearer no documento OpenAPI para que o Scalar ofereça
/// o campo de token nas requisições.
/// </summary>
public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Informe apenas o token; o prefixo 'Bearer' é adicionado automaticamente.",
        };

        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>(),
            }
        ];

        return Task.CompletedTask;
    }
}
