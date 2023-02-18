using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models.Options;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Added configs
builder.Services.Configure<ProgressAppInstanceOptions>(builder.Configuration.GetSection(AppsettingsSections.AppliactionInstances));
builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(AppsettingsSections.ApplicationSettings));
builder.Services.Configure<PlatformOptions>(builder.Configuration.GetSection(PlatformOptions.AppSettingsSection));

string? connectionType = builder?.Configuration?[$"{AppsettingsSections.ApplicationSettings}:{AppsettingsSections.DataStorageSection}:{nameof(ApplicationOptions.CurrentDataStorage)}"];

switch (connectionType)
{
    case PossibleDataStorage.CSV:
        Console.WriteLine("CSV Data storage selected");
        builder?.Services?.AddSingleton<IDataAccess, CSVDataAccess>();
        break;

    case PossibleDataStorage.MySQL:
        Console.WriteLine("!! MySQL specified this is not implemented !!");
        throw new NotImplementedException();
        break;

    case PossibleDataStorage.MsSQL:
        Console.WriteLine("!! MsSQL specified this is not implemented !!");
        throw new NotImplementedException();
        break;

    default:
        builder?.Services?.AddSingleton<IDataAccess, CSVDataAccess>();        
        Console.WriteLine("!! No connection specified !!");
        break;
}

var app = builder?.Build();

// Configure the HTTP request pipeline.
if (app is not null)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

