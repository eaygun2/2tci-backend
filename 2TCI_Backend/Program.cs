using ApplicationCore.DomainServices.Interfaces;
using ApplicationCore.DomainServices.Services.Int;
using ApplicationCore.Entities;
using ApplicationCore.Entities.DataTransferObjects;
using Infrastructure.Data;
using Infrastructure.Implementations;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Database context
builder.Services.AddDbContext<CarInspectionsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'sqlConnection' not found.")));

// Add Models Configurations for the model service to make it generic 
builder.Services.Configure<ModelSettingsBase>(builder.Configuration.GetSection("ModelSettings"));

// Add services to the container.
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddSingleton<IModelService<ModelInputDto, DetectionModelOutput>, ModelService<ModelInputDto, DetectionModelOutput>>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build services to the container
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseCors(opt => opt.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
