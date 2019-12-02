FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /webapi
COPY * ./
RUN dotnet restore "webapi.csproj"
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "webapi.dll"]