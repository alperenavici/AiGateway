// using AiGateway.Core.Interfaces;
// using AiGateway.Service;
// using Microsoft.Extensions.DependencyInjection;
// using Moq;
// using Xunit;
//
// namespace AiGateway.Tests;
//
// public class AskAiCommandHandlerTests
// {
//     [Fact]
//     public async Task Handle_GecerliOpenAiIstegi_BasariylaCevapDonmeli()
//     {
//         var prompt = "Bana bir fıkra anlat";
//         var expectedResponse = "Mock OpenAI Cevabı: Komik bir fıkra.";
//         var command = new AskAiCommand(prompt, "Openai");
//
//         var mockStrategy = new Mock<ILanguageModelStrategy>();
//         
//         mockStrategy
//             .Setup(s => s.GenerateResponseAsync(prompt, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(expectedResponse);
//
//         var mockServiceProvider = new Mock<IKeyedServiceProvider>();
//         
//         mockServiceProvider
//             .Setup(sp => sp.GetKeyedService(typeof(ILanguageModelStrategy), "Openai"))
//             .Returns(mockStrategy.Object);
//
//         var handler = new AskAiCommandHandler(mockServiceProvider.Object);
//
//       
//         var result = await handler.Handle(command, CancellationToken.None);
//
//         
//         Assert.Equal(expectedResponse, result);
//         
//         mockStrategy.Verify(s => s.GenerateResponseAsync(prompt, It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task Handle_BilinmeyenModelTipiIstendiginde_ArgumentExceptionFirlatmali()
//     {
//         
//         var command = new AskAiCommand("Merhaba", "HackerModel"); 
//
//         var mockServiceProvider = new Mock<IKeyedServiceProvider>();
//         
//         mockServiceProvider
//             .Setup(sp => sp.GetKeyedService(typeof(ILanguageModelStrategy), "HackerModel"))
//             .Returns((ILanguageModelStrategy)null!);
//
//         var handler = new AskAiCommandHandler(mockServiceProvider.Object);
//
//         
//         var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
//             handler.Handle(command, CancellationToken.None));
//
//         Assert.Contains("HackerModel adında bir AI modeli bulunamadı", exception.Message);
//     }
//     
// }