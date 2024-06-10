using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Packaging;
using SchoolServer.API.Models;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Auth;
using SchoolServer.Application.Services;
using SchoolServer.Core.Models;
using SchoolServer.Infrastructure.Authentification;
using System.Text.Json;
namespace SchoolServer.API.Controllers;

[Authorize]
[ApiController]
[Route("api/test")]
public class TestController : Controller
{
    private readonly TestsService testsService;
    private readonly IJWTProvider jWTProvider;
    private readonly ResourceServices resourceServices;
    private readonly ILogger<TestController> logger;
    private readonly JWTOptions jWTOptions;
    public TestController(TestsService testsService, IOptions<JWTOptions> jWTOptions, IJWTProvider jWTProvider, ResourceServices resourceServices, ILogger<TestController> logger)
    {
        this.testsService = testsService;
        this.jWTProvider = jWTProvider;
        this.resourceServices = resourceServices;
        this.logger = logger;
        this.jWTOptions = jWTOptions.Value;
    }
    [HttpGet("get_all_tests")]
    public async Task<IActionResult> GetAllTests()
    {
        var testsM = await testsService.GetAllTests();
        var result = testsM.Select(s => new
        {
            Id = s.Id,
            Title = s.Title,
            ContentUrl = s.ContentUrl,
        });
        return Ok(result);

    }


    [HttpDelete("delete_test/{id:int}")]
    [HasPermission(Core.Enums.Permission.DeleteTest)]
    public async Task<IActionResult> DeleteTest(int id)
    {
        await testsService.DeleteTest(id);
        return Ok();
    }

    [HttpDelete("delete_attempt/{id:int}")]
    [HasPermission(Core.Enums.Permission.DeleteAttempt)]
    public async Task<IActionResult> DeleteAttempt(int id)
    {
        await testsService.DeleteAttempt(id);
        return Ok();
    }


    [HttpPost("create")]
    [HasPermission(Core.Enums.Permission.CreateTest)]
    public async Task<IActionResult> CreateTest([FromBody] CreateTestModel testModel)

    {
        Test test = new Test()
        {
            Title = testModel.Title,
            AnswersFile = testModel.AnswersFile,
            ContentUrl = testModel.ContentUrl,
        };
        try
        {
            await testsService.CreateTest(test);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (TestAlreadyExistException)
        {
            return Conflict("Test already exist in database");
        }
        catch(Exception ex) 
        { 
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }


    }
    [HttpPost("check/{testId:int}")]
    public async Task<IActionResult> CheckTest([FromBody] List<AnswerModel> answers, int testId)
    {
        Test test;
        try
        {
            test = await testsService.GetTestById(testId);
        }
        catch (TestNotFoundException)
        {

            return BadRequest("test id not exist");
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        string answersJson = "";
        FileStream? answersStream = null;
        StreamReader? answersStreamReader = null;
        try
        {
            answersStream = resourceServices.GetResource("TestsAnswers", test.AnswersFile);
            answersStreamReader = new StreamReader(answersStream);
            answersJson = await answersStreamReader.ReadToEndAsync();
        }
        catch (FileNotFoundException)
        {
            logger.LogError($"{test.AnswersFile} not exist in TestsAnswers dirrectory");
            return StatusCode(StatusCodes.Status500InternalServerError, "answers file not find");
        }
        finally
        {
            answersStream?.Dispose();
            answersStreamReader?.Dispose();
        }

        List<TestQuestion>? testQuestions = null!;
        try
        {
            testQuestions = JsonSerializer.Deserialize<List<TestQuestion>>(answersJson);
        }
        catch (JsonException)
        {
            logger.LogError($"{test.AnswersFile} not valid json file");
            return StatusCode(StatusCodes.Status500InternalServerError, "answers file is not valid");
        }


        string token = Request.Cookies[jWTOptions.JWTCookieName] ?? string.Empty;
        string username = jWTProvider.GetUsernameFromToken(token);
        List<TestAnswer> userAnswers = new List<TestAnswer>();
        foreach (var answer in answers)
        {
            userAnswers.Add(new TestAnswer()
            {
                AnswerText = answer.AnswerValue,
                QuestionId = answer.QuestionId
            });
        }

        var result = testsService.CheckTest(userAnswers, testQuestions);
        try
        {
            await testsService.UploadTestResult(result, username, testId);
        }catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok(new
        {
            pointsScored = result.PointsScored,
            totalPoints = result.TotalPoints
        });
    }
}
