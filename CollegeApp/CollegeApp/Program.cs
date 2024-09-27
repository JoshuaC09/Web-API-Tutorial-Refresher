using CollegeApp.Configurations;
using CollegeApp.Data;
using CollegeApp.Data.Repository;
using CollegeApp.MyLogging.Implementation;
using CollegeApp.MyLogging.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

//Getting JWTSecret from appsettings.json
//Console.WriteLine(builder.Configuration.GetValue<string>("JWTSecret"));

var jwtKeyForGoolge = builder.Configuration["Jwt:KeyForGoogle"];
var jwtKeyForMicrosoft = builder.Configuration["Jwt:KeyForMicrosoft"];
var jwtKeyForLocalUser = builder.Configuration["Jwt:KeyForLocal"];

var googleAudience = builder.Configuration["Jwt:GoogleAudience"];
var microsoftAudience = builder.Configuration["Jwt:MicrosoftAudience"];
var localAudience = builder.Configuration["Jwt:LocalAudience"];

var googleIssuer = builder.Configuration["Jwt:GoogleIssuer"];
var microsofIssuer = builder.Configuration["Jwt:MicrosoftIssuer"];
var LocalIssuer = builder.Configuration["Jwt:LocalIssuer"];


var jwtIssuer = builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrEmpty(jwtKeyForGoolge) || string.IsNullOrEmpty(jwtIssuer)
       || string.IsNullOrEmpty(jwtKeyForMicrosoft) || string.IsNullOrEmpty(jwtKeyForLocalUser))
{
    throw new InvalidOperationException("JWT Key or Issuer is not configured properly.");
}

var keyGoogle = Encoding.ASCII.GetBytes(jwtKeyForGoolge);
var keyMicrosoft = Encoding.ASCII.GetBytes(jwtKeyForMicrosoft);
var keyLocalUser = Encoding.ASCII.GetBytes(jwtKeyForLocalUser);



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("LoginForGoogleUsers",options =>
{
    //options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = googleAudience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = googleIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(keyGoogle)
    };
})
.AddJwtBearer("LoginForMicrosoftUsers", options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidAudience = microsoftAudience,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = microsofIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(keyMicrosoft)
     };
 })
.AddJwtBearer("LoginForLocalUsers", options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidAudience = localAudience,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = LocalIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(keyLocalUser)
     };
 });

builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: Bearer eyashd21dfasdfasdk5jfhak5sdfh",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "OAuth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string> {}
        }
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
