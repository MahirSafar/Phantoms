# ============================================================
#  Stage 1 – Base runtime image
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# ============================================================
#  Stage 2 – SDK / build + restore
#  Copy every .csproj first so Docker can cache the restore
#  layer independently of source changes.
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# --- Core layer ---
COPY src/Core/Phantoms.Domain/Phantoms.Domain.csproj            src/Core/Phantoms.Domain/
COPY src/Core/Phantoms.Application/Phantoms.Application.csproj  src/Core/Phantoms.Application/

# --- Infrastructure layer ---
COPY src/Infrastructure/Phantoms.Persistence/Phantoms.Persistence.csproj     src/Infrastructure/Phantoms.Persistence/
COPY src/Infrastructure/Phantoms.Infrastructure/Phantoms.Infrastructure.csproj src/Infrastructure/Phantoms.Infrastructure/

# --- Presentation layer ---
COPY src/Presentation/Phantoms.API/Phantoms.API.csproj           src/Presentation/Phantoms.API/

# Restore all projects (uses the cached layer when .csproj files haven't changed)
RUN dotnet restore src/Presentation/Phantoms.API/Phantoms.API.csproj

# --- Copy full source ---
COPY src/ src/

# ============================================================
#  Stage 3 – Publish (Release)
# ============================================================
FROM build AS publish
RUN dotnet publish src/Presentation/Phantoms.API/Phantoms.API.csproj \
	--configuration Release \
	--no-restore \
	--output /app/publish \
	/p:UseAppHost=false

# ============================================================
#  Stage 4 – Final image (runtime only, no SDK)
# ============================================================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Run as non-root for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Phantoms.API.dll"]
