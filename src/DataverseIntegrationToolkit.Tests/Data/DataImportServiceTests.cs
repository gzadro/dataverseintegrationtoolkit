using DataverseIntegrationToolkit.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace DataverseIntegrationToolkit.Tests.Data
{
    [TestClass]
    public class DataImportServiceTests
    {
        private IOrganizationService _organizationService;
        private ILogger<DataImportService> _logger;
        private DataImportService _dataImportService;

        [TestInitialize]
        public void Intialize()
        {
            _organizationService = Substitute.For<IOrganizationService>();
            _logger = Substitute.For<ILogger<DataImportService>>();
            _dataImportService = new DataImportService(_organizationService, _logger);
        }

        [TestMethod]
        public void Add_Should_AddToData()
        {
            var record = new Entity("account") { Id = Guid.NewGuid() };
            var record2 = new Entity("account") { Id = Guid.NewGuid() };

            _dataImportService.Add(record);
            _dataImportService.Add(record2);

            _dataImportService.GetCurrentItemsCount().ShouldBe(2);
        }

        [TestMethod]
        public void Add_Null_Should_ThrowException()
        {
            Should.Throw<ArgumentNullException>(() => _dataImportService.Add(null));
        }

        [TestMethod]
        public void Update_Should_AddToData()
        {
            var record = new Entity("contact") { Id = Guid.NewGuid() };
            var record2 = new Entity("contact") { Id = Guid.NewGuid() };

            _dataImportService.Update(record);
            _dataImportService.Update(record2);

            _dataImportService.GetCurrentItemsCount().ShouldBe(2);
        }

        [TestMethod]
        public void Update_Null_Should_ThrowException()
        {
            Should.Throw<ArgumentNullException>(() => _dataImportService.Update(null));
        }

        [TestMethod]
        public void Delete_Should_AddToData()
        {
            var record = new Entity("account") { Id = Guid.NewGuid() };
            var record2 = new Entity("account") { Id = Guid.NewGuid() };
            var record3 = new Entity("account") { Id = Guid.NewGuid() };

            _dataImportService.Delete(record);
            _dataImportService.Delete(record2);
            _dataImportService.Delete(record3);

            _dataImportService.GetCurrentItemsCount().ShouldBe(3);
        }

        [TestMethod]
        public void Delete_Null_Should_ThrowException()
        {
            Should.Throw<ArgumentNullException>(() => _dataImportService.Delete(null));
        }

        [TestMethod]
        public void Associate_Should_AddToData()
        {
            var fromRecord = new Entity("account") { Id = Guid.NewGuid() };
            var toRecord = new Entity("contact") { Id = Guid.NewGuid() };
            var relationship = new Relationship("new_account_contact");

            _dataImportService.Associate(fromRecord, toRecord, relationship);

            _dataImportService.GetCurrentItemsCount().ShouldBe(1);
        }

        [TestMethod]
        public void Associate_Null_Should_ThrowException()
        {
            Should.Throw<ArgumentNullException>(() => _dataImportService.Associate(null, null, null));
        }

        [TestMethod]
        public void Disassociate_Should_AddToData()
        {
            var fromRecord = new Entity("account") { Id = Guid.NewGuid() };
            var toRecord = new Entity("contact") { Id = Guid.NewGuid() };
            var relationship = new Relationship("new_account_contact");

            _dataImportService.Disassociate(fromRecord, toRecord, relationship);

            _dataImportService.GetCurrentItemsCount().ShouldBe(1);
        }

        [TestMethod]
        public void Disassociate_Null_Should_ThrowException()
        {
            Should.Throw<ArgumentNullException>(() => _dataImportService.Disassociate(null, null, null));
        }

        [TestMethod]
        public void ClearAll_Should_Clear_Data_List()
        {
            var record = new Entity("account") { Id = Guid.NewGuid() };
            _dataImportService.Add(record);

            _dataImportService.ClearAll();

            _dataImportService.GetCurrentItemsCount().ShouldBe(0);
        }

        [TestMethod]
        public void Commit_ClearList_Should_Succeeded()
        {
            var record = new Entity("account") { Id = Guid.NewGuid() };
            _dataImportService.Add(record);

            _dataImportService.ClearAll();

            _dataImportService.Commit().ShouldBe(RequestProcessingResult.Succeeded);
        }

        [TestMethod]
        public void Commit_Should_Call_OrganizationService_Execute_And_Should_Succeeded()
        {
            var record = new Entity("account") { Id = Guid.NewGuid() };
            _dataImportService.Add(record);

            var result = _dataImportService.Commit();

            _organizationService.Received(1).Execute(Arg.Any<ExecuteTransactionRequest>());
            result.ShouldBe(RequestProcessingResult.Succeeded);
        }

        [TestMethod]
        public void Commit_Should_Return_Failed_When_Exception_Occurs()
        {
            var record = new Entity("account") { Id = Guid.NewGuid() };
            _dataImportService.Add(record);

            _organizationService
                .When(x => x.Execute(Arg.Any<ExecuteTransactionRequest>()))
                .Do(x => throw new Exception("Service Failure"));

            var result = _dataImportService.Commit();

            result.ShouldBe(RequestProcessingResult.Failed);
        }

        [TestMethod]
        public void Commit_Should_Call_OrganizationService_Execute_WithCreateRequest_When_AddRecord_IsUsed()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var record = new Entity("account") { Id = entityId };
            _dataImportService.Add(record);

            // Act
            var result = _dataImportService.Commit();

            // Assert
            _organizationService.Received(1).Execute(Arg.Is<ExecuteTransactionRequest>(request =>
                request.Requests.Any(r =>
                    IsCreateRequestForEntityId(r, entityId)
                )
            ));

            result.ShouldBe(RequestProcessingResult.Succeeded);
        }

        [TestMethod]
        public void Commit_Should_Call_OrganizationService_Execute_WithCreateRequest_When_UpdateRecord_IsUsed()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var record = new Entity("account") { Id = entityId };
            _dataImportService.Update(record);

            // Act
            var result = _dataImportService.Commit();

            // Assert
            _organizationService.Received(1).Execute(Arg.Is<ExecuteTransactionRequest>(request =>
                request.Requests.Any(r =>
                    IsUpdateRequestForEntityId(r, entityId)
                )
            ));

            result.ShouldBe(RequestProcessingResult.Succeeded);
        }


        [TestMethod]
        public void Commit_Should_Call_OrganizationService_Execute_WithCreateRequest_When_DeleteRecord_IsUsed()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var record = new Entity("account") { Id = entityId };
            _dataImportService.Delete(record);

            // Act
            var result = _dataImportService.Commit();

            // Assert
            _organizationService.Received(1).Execute(Arg.Is<ExecuteTransactionRequest>(request =>
                request.Requests.Any(r =>
                    IsDeleteRequestForEntityId(r, entityId)
                )
            ));

            result.ShouldBe(RequestProcessingResult.Succeeded);
        }

        [TestMethod]
        public void Commit_Should_Call_OrganizationService_Execute_WithCreateRequest_When_AssociateRecord_IsUsed()
        {
            var entityIdFrom = Guid.NewGuid();
            var entityIdTo = Guid.NewGuid();

            var fromRecord = new Entity("account") { Id = entityIdFrom };
            var toRecord = new Entity("contact") { Id = entityIdTo };
            var relationship = new Relationship("new_account_contact");

            _dataImportService.Associate(fromRecord, toRecord, relationship);

            // Act
            var result = _dataImportService.Commit();

            // Assert
            _organizationService.Received(1).Execute(Arg.Is<ExecuteTransactionRequest>(request =>
                request.Requests.Any(r =>
                    IsAssociateRequestForEntityId(r, entityIdFrom, entityIdTo)
                )
            ));

            result.ShouldBe(RequestProcessingResult.Succeeded);
        }

        [TestMethod]
        public void Commit_Should_Call_OrganizationService_Execute_WithCreateRequest_When_DisassociateRecord_IsUsed()
        {
            // Arrange
            var entityIdFrom = Guid.NewGuid();
            var entityIdTo = Guid.NewGuid();

            var fromRecord = new Entity("account") { Id = entityIdFrom };
            var toRecord = new Entity("contact") { Id = entityIdTo };
            var relationship = new Relationship("new_account_contact");

            _dataImportService.Disassociate(fromRecord, toRecord, relationship);

            // Act
            var result = _dataImportService.Commit();

            // Assert
            _organizationService.Received(1).Execute(Arg.Is<ExecuteTransactionRequest>(request =>
                request.Requests.Any(r =>
                    IsDisassociateRequestForEntityId(r, entityIdFrom, entityIdTo)
                )
            ));

            result.ShouldBe(RequestProcessingResult.Succeeded);
        }

        [TestMethod]
        public void Commit_Should_Call_OrganizationService_Execute_WithCreateRequest_MultipleTimes_When_MoreThan100RequestsAreUsed()
        {
            for(var i=0; i< 1001; i++)
            {
                var entityId = Guid.NewGuid();
                var record = new Entity("account") { Id = entityId };

                _dataImportService.Delete(record);
            }

            var result = _dataImportService.Commit();

            _organizationService.Received(2).Execute(Arg.Any<ExecuteTransactionRequest>());

            result.ShouldBe(RequestProcessingResult.Succeeded);
        }

        private bool IsCreateRequestForEntityId(OrganizationRequest request, Guid entityId)
        {
            if (request is CreateRequest createRequest &&
                createRequest.Parameters.ContainsKey("Target") &&
                createRequest.Parameters["Target"] is Entity entity)
            {
                return entity.Id == entityId;
            }

            return false;
        }

        private bool IsUpdateRequestForEntityId(OrganizationRequest request, Guid entityId)
        {
            if (request is UpdateRequest updateRequest &&
                updateRequest.Parameters.ContainsKey("Target") &&
                updateRequest.Parameters["Target"] is Entity entity)
            {
                return entity.Id == entityId;
            }

            return false;
        }

        private bool IsDeleteRequestForEntityId(OrganizationRequest request, Guid entityId)
        {
            if (request is DeleteRequest deleteRequest &&
                deleteRequest.Parameters.ContainsKey("Target") &&
                deleteRequest.Parameters["Target"] is Entity entity)
            {
                return entity.Id == entityId;
            }

            return false;
        }

        private bool IsAssociateRequestForEntityId(OrganizationRequest request, Guid entityIdFrom, Guid entityIdTo)
        {
            if (request is AssociateRequest associateRequest &&
                associateRequest.Parameters.ContainsKey("Target") &&
                associateRequest.Parameters.ContainsKey("Relationship") &&
                associateRequest.Parameters["Target"] is EntityReference entity &&
                associateRequest.Parameters["RelatedEntities"] is EntityReferenceCollection relationships)
            {
                return entity.Id == entityIdFrom && relationships[0].Id == entityIdTo & associateRequest.RelatedEntities[0].Id == entityIdTo;

            }

            return false;
        }

        private bool IsDisassociateRequestForEntityId(OrganizationRequest request, Guid entityIdFrom, Guid entityIdTo)
        {
            if (request is DisassociateRequest disassociateRequest &&
                disassociateRequest.Parameters.ContainsKey("Target") &&
                disassociateRequest.Parameters.ContainsKey("Relationship") &&
                disassociateRequest.Parameters["Target"] is EntityReference entity &&
                disassociateRequest.Parameters["RelatedEntities"] is EntityReferenceCollection relationships)
            {
                return entity.Id == entityIdFrom && relationships[0].Id == entityIdTo && disassociateRequest.RelatedEntities[0].Id == entityIdTo;

            }

            return false;
        }
    }
}
