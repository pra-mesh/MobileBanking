using FluentAssertions;
using Moq;
using MobileBanking.Data.Services;
using MobileBanking.Data.Services.Connection;
using Xunit;

namespace MobileBanking.Tests.Data.Services;

public class UnitOfWorkTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _unitOfWork = new UnitOfWork(_mockSqlDataAccess.Object);
    }

    [Fact]
    public void Begin_ShouldCallStartTransaction()
    {
        // Act
        _unitOfWork.Begin();

        // Assert
        _mockSqlDataAccess.Verify(x => x.StartTransaction(), Times.Once);
    }

    [Fact]
    public void Commit_ShouldCallCommitTransaction()
    {
        // Act
        _unitOfWork.Commit();

        // Assert
        _mockSqlDataAccess.Verify(x => x.CommitTransaction(), Times.Once);
    }

    [Fact]
    public void RollBack_ShouldCallRollBackTransaction()
    {
        // Act
        _unitOfWork.RollBack();

        // Assert
        _mockSqlDataAccess.Verify(x => x.RollBackTransaction(), Times.Once);
    }

    [Fact]
    public void Dispose_ShouldCallSqlDataAccessDispose()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        _mockSqlDataAccess.Verify(x => x.Dispose(), Times.Once);
    }
}