# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia só os .csproj primeiro: o restore fica em cache e só roda de novo
# quando alguma dependência muda, não a cada alteração de código.
COPY Imigrei.Domain/Imigrei.Domain.csproj                          Imigrei.Domain/
COPY Imigrei.Application/Imigrei.Application.csproj         Imigrei.Application/
COPY Imigrei.Infrastructure/Imigrei.Infrastructure.csproj   Imigrei.Infrastructure/
COPY Imigrei.API/Imigrei.API.csproj                         Imigrei.API/
RUN dotnet restore Imigrei.API/Imigrei.API.csproj

COPY . .
RUN dotnet publish Imigrei.API/Imigrei.API.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Pasta dos documentos, já pertencente ao usuário não-root "app" da imagem,
# para que o volume montado nasça com a permissão certa.
RUN mkdir -p /app/storage/imigrantes && chown -R app:app /app/storage

COPY --from=build /app/publish .

USER app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    Storage__ImigrantesPath=/app/storage/imigrantes
EXPOSE 8080

ENTRYPOINT ["dotnet", "Imigrei.API.dll"]
