using System.Text.Json;
using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.RefereesController_test
{
    public class GetRefereesForRefereeGroup_test : AppForSEII4SqliteUT
    {
        // The action method returns anonymous objects, so the test converts them
        // to this record (through JSON) to be able to compare them with Assert.Equal.
        public record RefereeResult(string Id, string? Name, string? Surname, int YearsRefereeing, int Rating);

        public GetRefereesForRefereeGroup_test()
        {
            var football = new Sport("Football", 11, 3, "11 vs 11 association football", "Rules");
            var basketball = new Sport("Basketball", 5, 2, "5 vs 5 basketball", "Rules");
            _context.Sports.AddRange(football, basketball);
            _context.SaveChanges();

            var referees = new List<Referee>()
            {
                new Referee(5, 3, football.Id) { Id = "2", Name = "Ana", Surname = "Torres" },
                new Referee(4, 1, football.Id) { Id = "3", Name = "Luis", Surname = "Perez" },
                // this referee has 0 years refereeing so it shouldn't be returned
                new Referee(3, 0, football.Id) { Id = "4", Name = "Marta", Surname = "Ruiz" },
            };
            _context.Referee.AddRange(referees);
            _context.SaveChanges();
        }

        private static List<RefereeResult> ToRefereeResults(object? value)
        {
            string json = JsonSerializer.Serialize(value);
            return JsonSerializer.Deserialize<List<RefereeResult>>(json)!.OrderBy(r => r.Id).ToList();
        }

        public static IEnumerable<object?[]> TestCasesFor_GetRefereesForRefereeGroup()
        {
            var refereeResults = new List<RefereeResult>()
            {
                new RefereeResult("2", "Ana", "Torres", 3, 5),
                new RefereeResult("3", "Luis", "Perez", 1, 4),
            };

            var TC1WithoutFilter = new List<RefereeResult> { refereeResults[0], refereeResults[1] };
            var TC2MinYears2 = new List<RefereeResult> { refereeResults[0] };
            var TC3MinYears1 = new List<RefereeResult> { refereeResults[0], refereeResults[1] };

            var allTests = new List<object?[]>
            {
                new object?[] { 1, null, TC1WithoutFilter },
                new object?[] { 1, 2, TC2MinYears2 },
                // boundary: the filter is "higher or equal", so a referee with exactly 1 year is included
                new object?[] { 1, 1, TC3MinYears1 },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(TestCasesFor_GetRefereesForRefereeGroup))]
        public async Task GetRefereesForRefereeGroup_Ok_test(int sportId, int? minYearsRefereeing, List<RefereeResult> expectedReferees)
        {
            // Arrange
            var mock = new Mock<ILogger<RefereesController>>();
            ILogger<RefereesController> logger = mock.Object;
            var controller = new RefereesController(_context, logger);

            // Act
            var result = await controller.GetRefereesForRefereeGroup(sportId, minYearsRefereeing);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualReferees = ToRefereeResults(okResult.Value);
            Assert.Equal(expectedReferees, actualReferees);
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [InlineData(99, null, "The sport does not exist")]
        [InlineData(2, null, "There are no referees available for this sport")] // basketball has no referees
        [InlineData(1, 10, "There are no referees available for this sport")]   // no football referee has 10 or more years
        public async Task GetRefereesForRefereeGroup_Error_test(int sportId, int? minYearsRefereeing, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<RefereesController>>();
            ILogger<RefereesController> logger = mock.Object;
            var controller = new RefereesController(_context, logger);

            // Act
            var result = await controller.GetRefereesForRefereeGroup(sportId, minYearsRefereeing);

            // Assert
            // we check that the response type is BadRequest and that the error message is the expected one
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorExpected, badRequestResult.Value);
        }
    }
}