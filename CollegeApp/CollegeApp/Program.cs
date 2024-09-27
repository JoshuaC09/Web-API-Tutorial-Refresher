using CollegeApp.Configurations;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
using CollegeApp.MyLogging.Implementation;
using CollegeApp.MyLogging.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddScoped<IMyLogger, LogToDB>();
//builder.Services.AddSingleton<IMyLogger, LogToServerMemory>();
builder.Services.AddTransient<IMyLogger, LogToFile>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddDbContext<CollegeDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeContext")));

builder.Services.AddControllers().AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperConfig));


builder.Services.AddCors(options =>
{
    //options.AddDefaultPolicy(policy =>
    //{
    //    //Allow Any Origins
    //    //policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

    //    //Allow specific origins
    //    policy.AllowAnyOrigin()
    //          .AllowAnyHeader()
    //          .AllowAnyMethod();
    //});

    options.AddPolicy("AllowAll",policy =>
    {
        //Allow Any Origins
        //policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

        //Allow specific origins
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });



    options.AddPolicy("AllowOnlyLocalHost", policy =>
    {
        //Allow specific origins
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowOnlyGoolge", policy =>
    {
      
        //Allow specific origins
        policy.WithOrigins("http://google.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    options.AddPolicy("AllowOnlyMicrosoft", policy =>
    {
        
        //Allow specific origins
        policy.WithOrigins("http://outlook.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
