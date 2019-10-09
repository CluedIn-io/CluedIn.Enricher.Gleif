using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Messages.Processing;
using CluedIn.Core.Processing;
using CluedIn.Core.Serialization;
using CluedIn.Core.Workflows;
using CluedIn.ExternalSearch;
using CluedIn.ExternalSearch.Providers.Gleif;
using CluedIn.Testing.Base.Context;
using CluedIn.Testing.Base.Processing.Actors;
using Moq;
using Xunit;

namespace CluedIn.Tests.Integration.ExternalSearch
{
    public class GleifTests : IDisposable
    {
        private TestContext testContext;

        [Fact(Skip = "GitHub Issue 829 - ref https://github.com/CluedIn-io/CluedIn/issues/829")]
        public void Test()
        {
            // Arrange
            this.testContext = new TestContext();
            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesLeiCode, "213800552QI1A89ZCO39");

            IEntityMetadata entityMetadata = new EntityMetadataPart()
            {
                Name = "Sitecore",
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            var externalSearchProvider = new Mock<GleifExternalSearchProvider>(MockBehavior.Loose);
            var clues = new List<CompressedClue>();

            externalSearchProvider.CallBase = true;

            this.testContext.ProcessingHub.Setup(h => h.SendCommand(It.IsAny<ProcessClueCommand>())).Callback<IProcessingCommand>(c => clues.Add(((ProcessClueCommand)c).Clue));

            this.testContext.Container.Register(Component.For<IExternalSearchProvider>().UsingFactoryMethod(() => externalSearchProvider.Object));

            var context = this.testContext.Context.ToProcessingContext();
            var command = new ExternalSearchCommand();
            var actor = new ExternalSearchProcessingAccessor(context.ApplicationContext);
            var workflow = new Mock<Workflow>(MockBehavior.Loose, context, new EmptyWorkflowTemplate<ExternalSearchCommand>());

            workflow.CallBase = true;

            command.With(context);
            command.OrganizationId = context.Organization.Id;
            command.EntityMetaData = entityMetadata;
            command.Workflow = workflow.Object;
            context.Workflow = command.Workflow;

            // Act
            var result = actor.ProcessWorkflowStep(context, command);
            Assert.Equal(WorkflowStepResult.Repeat.SaveResult, result.SaveResult);

            result = actor.ProcessWorkflowStep(context, command);
            Assert.Equal(WorkflowStepResult.Success.SaveResult, result.SaveResult);
            context.Workflow.AddStepResult(result);

            context.Workflow.ProcessStepResult(context, command);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);

            Assert.True(clues.Count > 0);
        }

        [Fact(Skip = "GitHub Issue 829 - ref https://github.com/CluedIn-io/CluedIn/issues/829")]
        public void Test2()
        {
            // Arrange
            this.testContext = new TestContext();
            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesLeiCode, "300300KDIZ11PV2GH547");

            IEntityMetadata entityMetadata = new EntityMetadataPart()
            {
                Name = "全国金融标准化技术委员会",
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            var externalSearchProvider = new Mock<GleifExternalSearchProvider>(MockBehavior.Loose);
            var clues = new List<CompressedClue>();

            externalSearchProvider.CallBase = true;

            this.testContext.ProcessingHub.Setup(h => h.SendCommand(It.IsAny<ProcessClueCommand>())).Callback<IProcessingCommand>(c => clues.Add(((ProcessClueCommand)c).Clue));

            this.testContext.Container.Register(Component.For<IExternalSearchProvider>().UsingFactoryMethod(() => externalSearchProvider.Object));

            var context = this.testContext.Context.ToProcessingContext();
            var command = new ExternalSearchCommand();
            var actor = new ExternalSearchProcessingAccessor(context.ApplicationContext);
            var workflow = new Mock<Workflow>(MockBehavior.Loose, context, new EmptyWorkflowTemplate<ExternalSearchCommand>());

            workflow.CallBase = true;

            command.With(context);
            command.OrganizationId = context.Organization.Id;
            command.EntityMetaData = entityMetadata;
            command.Workflow = workflow.Object;
            context.Workflow = command.Workflow;

            // Act
            var result = actor.ProcessWorkflowStep(context, command);
            Assert.Equal(WorkflowStepResult.Repeat.SaveResult, result.SaveResult);

            result = actor.ProcessWorkflowStep(context, command);
            Assert.Equal(WorkflowStepResult.Success.SaveResult, result.SaveResult);
            context.Workflow.AddStepResult(result);

            context.Workflow.ProcessStepResult(context, command);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);

            Assert.True(clues.Count > 0);
        }

        [Fact]
        public void TestCompanyWithInvalidLeiCode()
        {
            // Arrange
            this.testContext = new TestContext();
            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.CodesLeiCode, "549300TL5406I");

            IEntityMetadata entityMetadata = new EntityMetadataPart()
            {
                Name = "Saxo Bank",
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            var externalSearchProvider = new Mock<GleifExternalSearchProvider>(MockBehavior.Loose);
            var clues = new List<CompressedClue>();

            externalSearchProvider.CallBase = true;

            this.testContext.ProcessingHub.Setup(h => h.SendCommand(It.IsAny<ProcessClueCommand>())).Callback<IProcessingCommand>(c => clues.Add(((ProcessClueCommand)c).Clue));

            this.testContext.Container.Register(Component.For<IExternalSearchProvider>().UsingFactoryMethod(() => externalSearchProvider.Object));

            var context = this.testContext.Context.ToProcessingContext();
            var command = new ExternalSearchCommand();
            var actor = new ExternalSearchProcessingAccessor(context.ApplicationContext);
            var workflow = new Mock<Workflow>(MockBehavior.Loose, context, new EmptyWorkflowTemplate<ExternalSearchCommand>());

            workflow.CallBase = true;

            command.With(context);
            command.OrganizationId = context.Organization.Id;
            command.EntityMetaData = entityMetadata;
            command.Workflow = workflow.Object;
            context.Workflow = command.Workflow;

            // Act
            var result = actor.ProcessWorkflowStep(context, command);
            Assert.Equal(WorkflowStepResult.Ignored.SaveResult, result.SaveResult);

            result = actor.ProcessWorkflowStep(context, command);
            context.Workflow.AddStepResult(result);
            context.Workflow.ProcessStepResult(context, command);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.Never);

            Assert.True(clues.Count == 0);
        }

        public void Dispose()
        {
            this.testContext?.Dispose();
        }
    }
}
