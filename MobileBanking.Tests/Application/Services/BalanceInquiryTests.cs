using FluentAssertions;
using Moq;
using MobileBanking.Application.Models;
using MobileBanking.Application.Services;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Models.RequestModels;
using MobileBanking.Data.Repositories;
using Xunit;

namespace MobileBanking.Tests.Application.Services;

public class BalanceInquiryTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<IAccountValidation> _mockAccountValidation;
    private readonly BalanceInquiry _balanceInquiry;

    public BalanceInquiryTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockAccountValidation = new Mock<IAccountValidation>();
        _balanceInquiry = new BalanceInquiry(_mockAccountRepository.Object, _mockAccountValidation.Object);
    }

    [Fact]
    public async Task GetBalance_WithValidAccount_ShouldReturnAccountDetail()
    {
        // Arrange
        var request = new BalanceInquiryModel { accountNumber = "123456", branchId = "01" };
        var accountDetail = new AccountDetailDTO
        {
            SavingName = "Savings",
            mainbookno = "030",
            acno = "030.01",
            AccountHolder = "John Doe",
            accountno = "123456",
            MemName = "John Doe",
            Balance = 1000.50m,
            MinBal = 100.00m,
            InterestRate = 5.5m,
            Gamt = 0,
            Lamt = 0,
            Disabled = false
        };

        _mockAccountRepository.Setup(x => x.GetAccountDetails(request.accountNumber))
            .ReturnsAsync(new List<AccountDetailDTO> { accountDetail });

        // Act
        var result = await _balanceInquiry.GetBalance(request);

        // Assert
        result.Should().NotBeNull();
        result.SavingName.Should().Be("Savings");
        result.Balance.Should().Be(1000.50m);
        result.MinBal.Should().Be(100.00m);
        result.AccountHolder.Should().Be("John Doe");
        
        _mockAccountValidation.Verify(x => x.InvalidAccount(request.accountNumber), Times.Once);
        _mockAccountValidation.Verify(x => x.AccountCountValidation(It.IsAny<List<AccountDetailDTO>>(), request.accountNumber), Times.Once);
    }

    [Fact]
    public async Task GetAccountList_WithValidQuery_ShouldReturnAccountList()
    {
        // Arrange
        var query = new AllDetailsQueryModel { MemberNo = "M001", AccountNumber = "123456" };
        var accountDetails = new List<AccountFullDetalDTO>
        {
            new()
            {
                MemberId = "M001",
                MemberName = "John Doe",
                Address = "Test Address",
                MobileNumber = "9841234567",
                AccountNumber = "123456",
                AccountType = "Savings",
                SavingType = "Normal",
                LedgerBalance = 1000m,
                AvailableBalance = 900m,
                MinBal = 100m
            }
        };

        _mockAccountRepository.Setup(x => x.GetAccountCount(It.IsAny<AccountDetailPaged>()))
            .ReturnsAsync(2);
        _mockAccountRepository.Setup(x => x.AllAccountFullDetails(It.IsAny<AccountDetailPaged>()))
            .ReturnsAsync(accountDetails);

        // Act
        var result = await _balanceInquiry.GetAccountList(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().MemberId.Should().Be("M001");
        result.First().MemberName.Should().Be("John Doe");
        result.First().AccountNumber.Should().Be("123456");
    }

    [Fact]
    public async Task GetAccountList_WithLargeAccountCount_ShouldMakeMultipleParallelCalls()
    {
        // Arrange
        var query = new AllDetailsQueryModel { MemberNo = "M001" };
        var accountDetails = new List<AccountFullDetalDTO>
        {
            new()
            {
                MemberId = "M001",
                MemberName = "John Doe",
                Address = "Test Address",
                MobileNumber = "9841234567",
                AccountNumber = "123456",
                AccountType = "Savings",
                SavingType = "Normal",
                LedgerBalance = 1000m,
                AvailableBalance = 900m,
                MinBal = 100m
            }
        };

        _mockAccountRepository.Setup(x => x.GetAccountCount(It.IsAny<AccountDetailPaged>()))
            .ReturnsAsync(6); // This should create 3 parallel tasks (6/2 = 3)
        _mockAccountRepository.Setup(x => x.AllAccountFullDetails(It.IsAny<AccountDetailPaged>()))
            .ReturnsAsync(accountDetails);

        // Act
        var result = await _balanceInquiry.GetAccountList(query);

        // Assert
        result.Should().NotBeNull();
        _mockAccountRepository.Verify(x => x.AllAccountFullDetails(It.IsAny<AccountDetailPaged>()), Times.Exactly(3));
    }
}