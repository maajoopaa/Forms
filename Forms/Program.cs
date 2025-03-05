using Forms.DataAccess;
using Forms.Handlers;
using Forms.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/SignIn");
builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddTransient<IUserService,UserService>();

builder.Services.AddTransient<ITemplateService, TemplateService>();

builder.Services.AddTransient<IFormService, FormService>();

builder.Services.AddTransient<ICommentService, CommentService>();

builder.Services.AddTransient<ILikeService,LikeService>();

builder.Services.AddTransient<IQuestionService,QuestionService>();

builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, ApiTokenAuthenticationHandler>("ApiToken",null);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("FormsConnection");
builder.Services.AddDbContext<FormsDbContext>(opt => opt.UseLazyLoadingProxies().UseNpgsql(connectionString,
    b => b.MigrationsAssembly("Forms")));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  
app.UseAuthorization(); 

app.UseMiddleware<BlockedUserMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
